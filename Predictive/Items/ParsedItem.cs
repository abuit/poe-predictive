using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Predictive
{
    public class ParsedItem
    {
        //The maximum price the neural network will be able to suggest.
        public readonly static int MaxSupportedChaosPrice = 250;

        //The price to calibrate this belt with.
        public double? CalibrationPrice;

        //The actual predicted price of the belt.
        public double? CalculatedPrice;

        public readonly bool Corrupted;
        public readonly ParsedAffix[] ParsedImplicitMods;
        public readonly ParsedAffix[] ParsedExplicitMods;

        public ParsedItem(bool corrupted, string[] implicitMods, string[] explicitMods)
        {
            this.Corrupted = corrupted;

            ParsedImplicitMods = new ParsedAffix[implicitMods.Length];
            for (int i = 0; i < implicitMods.Length; i++)
            {
                ParsedImplicitMods[i] = new ParsedAffix(implicitMods[i]);
            }

            ParsedExplicitMods = new ParsedAffix[explicitMods.Length];
            for (int i = 0; i < explicitMods.Length; i++)
            {
                ParsedExplicitMods[i] = new ParsedAffix(explicitMods[i]);
            }
        }

        /// <summary>
        /// Processes the output vector to a readable price in Chaos.
        /// </summary>
        public void ProcessOutputVector(double[] result)
        {
            CalculatedPrice = result[0] * MaxSupportedChaosPrice;
        }

        /// <summary>
        /// Creates the desired output vector for this belt.
        /// Normalizes the value using the maximum supported chaos value.
        /// </summary>
        public double[] CreateCalibrationOutputVector()
        {
            var price = CalibrationPrice.Value;
            if (price > MaxSupportedChaosPrice)
                price = MaxSupportedChaosPrice;

            double normalized = price / MaxSupportedChaosPrice;
            return new double[] { normalized };
        }

        /// <summary>
        /// Creates the input vector for this belt.
        /// Ensure this input vector matches the amount of neurons in the first layer of the network.
        /// </summary>
        public double[] CreateInputVector(KnownAffix[] knownImplicits, KnownAffix[] knownExplicits)
        {
            double[] inputArray = new double[1 + knownImplicits.Length + knownExplicits.Length];
            inputArray[0] = Corrupted ? 0 : 1;

            int indx = 1;
            
            HashSet<ParsedAffix> matchedImplicits = new HashSet<ParsedAffix>();
            foreach (KnownAffix possibleImplcit in knownImplicits)
            {
                foreach (ParsedAffix impl in ParsedImplicitMods.Where(e => !matchedImplicits.Contains(e)))
                {
                    if (possibleImplcit.IsAffix(impl, out double normalizedValue))
                    {
                        matchedImplicits.Add(impl);
                        inputArray[indx] = normalizedValue;
                        break;
                    }
                }
                indx++;
            }

            HashSet<ParsedAffix> matchedExplicits = new HashSet<ParsedAffix>();
            foreach (KnownAffix possibleExplicit in knownExplicits)
            {
                foreach (ParsedAffix impl in ParsedExplicitMods.Where(e => !matchedImplicits.Contains(e)))
                {
                    if (possibleExplicit.IsAffix(impl, out double normalizedValue))
                    {
                        matchedImplicits.Add(impl);
                        inputArray[indx] = normalizedValue;
                        break;
                    }
                }
                indx++;
            }

            return inputArray;
        }

        public override string ToString()
        {
            return 
                string.Join(Environment.NewLine, ParsedImplicitMods.Select(e => e.OriginalAffix)) + Environment.NewLine +
                "------------" + Environment.NewLine +
                string.Join(Environment.NewLine, ParsedImplicitMods.Select(e => e.OriginalAffix)) + Environment.NewLine +
                (CalculatedPrice.HasValue ?  $"Predicted price:   {(int)CalculatedPrice} chaos" + Environment.NewLine : "") +
                (CalibrationPrice.HasValue ? $"Calibration price: {(int)CalibrationPrice} chaos" + Environment.NewLine : "");
        }
    }

    public class ParsedAffix
    {
        public static string Pattern = @"(?<affixStart>[^\d]*)(?<value>[\d]+)(?<affixEnd>[^\d]*)";
        public readonly string OriginalAffix;
        public readonly string AffixCategory;
        public readonly double Value;

        public ParsedAffix(string affix)
        {
            this.OriginalAffix = affix;

            Match m = Regex.Match(OriginalAffix, Pattern);
            if (m.Success)
            {
                AffixCategory = m.Groups["affixStart"].Value + " N " + m.Groups["affixEnd"].Value;
                Value = double.Parse(m.Groups["value"].Value);
                return;
            }
            else
            {
                AffixCategory = affix;
                Value = 1;
            }
        }
    }

    public class KnownAffix
    {
        public readonly string AffixCategory;
        public double MinValue { get; private set; }
        public double MaxValue { get; private set; }

        public KnownAffix(string affix, double minValue, double maxValue)
        {
            this.AffixCategory = affix;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        /// <summary>
        /// Update with information of an affix
        /// </summary>
        public void UpdateWith(ParsedAffix a)
        {
            if (!a.AffixCategory.Equals(this.AffixCategory))
                throw new ArgumentException($"Can't update {AffixCategory} with a value of {a.AffixCategory}");

            if (a.Value < MinValue)
                MinValue = a.Value;
            if (a.Value > MaxValue)
                MaxValue = a.Value;
        }

        public double Normalize(double value)
        {
            if (MinValue == MaxValue)
            {
                //Set it to 100% when there is no range.
                return 1.00;
            }
            else
            {
                //Reserve 5% for the fact that a value exists, even if the value is low
                var reservedWeight = 0.05;

                //Calculate where in the range between 0.05 and 1.00 the value is
                var fromMin = (value - MinValue);
                var weight = (fromMin / (MaxValue - MinValue)) * 0.95;

                return reservedWeight + weight;
            }
        }

        public bool IsAffix(ParsedAffix affix, out double normalizedWeight)
        {
            if (this.AffixCategory.Equals(affix.AffixCategory))
            {
                normalizedWeight = Normalize(affix.Value);
                return true;
            }
            normalizedWeight = default(double);
            return false;
        }
    }
}
