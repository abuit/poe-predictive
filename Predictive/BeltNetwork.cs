using AForge.Neuro;
using AForge.Neuro.Learning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Predictive
{
    public class Belt
    {
        //The neural network will give 1 as output when it intends 500.
        public static int MaxChaosPrice = 100;

        public double? CalibrationPrice;
        public double? CalculatedPrice;

        //private string[] implicitMods
        private string[] explicits;

        public Belt()
        {
            explicits = new string[0];
        }

        public Belt(string[] explicits)
        {
            this.explicits = explicits;
        }

        public void AddExplicit(string expl)
        {
            var old = explicits;
            explicits = new string[old.Length + 1];
            old.CopyTo(explicits, 0);
            explicits[explicits.Length - 1] = expl;
        }

        public void ProcessOutputVector(double[] result)
        {
            CalculatedPrice = result[0] * MaxChaosPrice;
        }

        public double[] CreateCalibrationOutputVector()
        {
            if (CalibrationPrice.Value > MaxChaosPrice)
                CalibrationPrice = MaxChaosPrice;

            double normalized = CalibrationPrice.Value / MaxChaosPrice;
            return new double[] { normalized };
        }

        public double[] CreateInputVector()
        {
            double[] inputArray = new double[BeltExplicits.All().Count()];

            HashSet<string> matchedExplicits = new HashSet<string>();

            int indx = 0;
            foreach (var possibleExplicit in BeltExplicits.All())
            {
                foreach(string expl in explicits.Where(e => !matchedExplicits.Contains(e)))
                {
                    Match m = Regex.Match(expl, possibleExplicit.Pattern);
                    if (m.Success)
                    {
                        matchedExplicits.Add(expl);
                        if (double.TryParse(m.Groups[1].Value, out double result))
                        {
                            //Reserve 25% for the fact that a value exists.
                            double normalized = 0.25;

                            if (possibleExplicit.MinValue != possibleExplicit.MaxValue)
                            {
                                var fromMin = (result - possibleExplicit.MinValue);
                                normalized += (fromMin / (possibleExplicit.MaxValue - possibleExplicit.MinValue)) * 0.75;
                            }
                            else
                            {
                                //Set it to 100% when there is no range.
                                normalized = 1;
                            }

                            inputArray[indx] = normalized;
                        }
                        else
                        {
                            //Diagnostics:
                            System.Console.WriteLine($"Explicit matched but value could not be parsed to double: {m.Groups[0].Value} in {expl}.");
                        }
                        break;
                    }
                }
                indx++;
            }
            
            //Diagnostics:
            foreach (string unmatched in explicits.Where(e => !matchedExplicits.Contains(e)))
            {
                System.Console.WriteLine($"Explicit was not recognised: {unmatched}");
            }

            return inputArray;
        }
    }

    struct BeltImplicits
    {

    }

    struct BeltExplicits 
    {
        const string CAPTURE = "CAPTURE";
        private static readonly string
                Reflect = Regex.Escape($"Reflects {CAPTURE} Physical Damage to Melee Attackers").Replace(CAPTURE, @"(\d*)"),
                FlaskLifeRecoveryRate = Regex.Escape($"{CAPTURE}% increased Flask Life Recovery rate").Replace(CAPTURE, @"(\d*)"),
                FlaskManaRecoveryRate = Regex.Escape($"{CAPTURE}% increased Flask Mana Recovery rate").Replace(CAPTURE, @"(\d*)"),
                MaxES = Regex.Escape($"+{CAPTURE} to maximum Energy Shield").Replace(CAPTURE, @"(\d*)"),
                MaxLife = Regex.Escape($"+{CAPTURE} to maximum Life").Replace(CAPTURE, @"(\d*)"),
                MaxArmour = Regex.Escape($"+{CAPTURE} to Armour").Replace(CAPTURE, @"(\d*)"),
                MaxEWD = Regex.Escape($"{CAPTURE}% increased Elemental Damage with Weapons").Replace(CAPTURE, @"(\d*)"),
                FlaskChargesG = Regex.Escape($"{CAPTURE}% increased Flask Charges gained").Replace(CAPTURE, @"(\d*)"),
                FlaskChargesU = Regex.Escape($"{CAPTURE}% reduced Flask Charges used").Replace(CAPTURE, @"(\d*)"),
                FlaskEffectDuration = Regex.Escape($"{CAPTURE}% increased Flask effect duration").Replace(CAPTURE, @"(\d*)"),
                ChaosResist = Regex.Escape($"+{CAPTURE}% to Chaos Resistance").Replace(CAPTURE, @"(\d*)"),
                ColdResist = Regex.Escape($"+{CAPTURE}% to Cold Resistance").Replace(CAPTURE, @"(\d*)"),
                FireResist = Regex.Escape($"+{CAPTURE}% to Fire Resistance").Replace(CAPTURE, @"(\d*)"),
                LifeRegen = Regex.Escape($"{CAPTURE} Life Regenerated per second").Replace(CAPTURE, @"(\d*)"),
                LightingResist = Regex.Escape($"+{CAPTURE}% to Lightning Resistance").Replace(CAPTURE, @"(\d*)"),
                Strength = Regex.Escape($"+{CAPTURE} to Strength").Replace(CAPTURE, @"(\d*)"),
                StunDuration = Regex.Escape($"{CAPTURE}% increased Stun Duration on Enemies").Replace(CAPTURE, @"(\d*)"),
                StunBlockRecovery = Regex.Escape($"{CAPTURE}% increased Stun and Block Recovery").Replace(CAPTURE, @"(\d*)"),
                StunTreshold = Regex.Escape($"{CAPTURE}% reduced Enemy Stun Threshold").Replace(CAPTURE, @"(\d*)");

        public string Pattern;
        public double MinValue;
        public double MaxValue;

        public BeltExplicits(string pattern, double minValue, double maxValue)
        {
            this.Pattern = pattern;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        public static IEnumerable<BeltExplicits> All()
        {
            yield return new BeltExplicits(Reflect, 1, 10);
            yield return new BeltExplicits(FlaskLifeRecoveryRate, 10, 10);
            yield return new BeltExplicits(FlaskManaRecoveryRate, 10, 10);
            yield return new BeltExplicits(MaxES, 1, 51);
            yield return new BeltExplicits(MaxLife, 3, 99);
            yield return new BeltExplicits(MaxArmour, 3, 460);
            yield return new BeltExplicits(MaxEWD, 5, 42);
            yield return new BeltExplicits(FlaskChargesG, 10, 20);
            yield return new BeltExplicits(FlaskChargesU, 10, 20);
            yield return new BeltExplicits(FlaskEffectDuration, 10, 20);
            yield return new BeltExplicits(ChaosResist, 5, 35);
            yield return new BeltExplicits(ColdResist, 6, 48);
            yield return new BeltExplicits(FireResist, 6, 48);
            yield return new BeltExplicits(LifeRegen, 1, 7);
            yield return new BeltExplicits(LightingResist, 6, 48);
            yield return new BeltExplicits(Strength, 8, 55);
            yield return new BeltExplicits(StunDuration, 11, 35);
            yield return new BeltExplicits(StunBlockRecovery, 11, 28);
            yield return new BeltExplicits(StunTreshold, 5, 15);
        }
    }

    class BeltNetwork
    {
        private readonly BackPropagationLearning teacher;
        private readonly ActivationNetwork network;

        public BeltNetwork()
        {
            SigmoidFunction f = new SigmoidFunction(0.5);

            //Regression mode
            network = new ActivationNetwork(
                f,
                BeltExplicits.All().Count(), // Base & Affixes
                //Hidden layers:
                10,
                // Regression mode: one output
                1
            );

            teacher = new BackPropagationLearning(network);
            teacher.LearningRate = 1;
            teacher.Momentum = 0.2;
        }

        //Learns in an epoch
        public void LearnFromBelts(params Belt[] belts)
        {
            var beltInputArray = belts.Select(b => b.CreateInputVector()).ToArray();
            var beltResultArray = belts.Select(b => b.CreateCalibrationOutputVector()).ToArray();
            for (int i = 0; i < 1000; i++)
                teacher.RunEpoch(beltInputArray, beltResultArray);
        }

        //Learns iteratively
        public void LearnFromBelt(Belt belt)
        {
            if (belt.CalibrationPrice == null)
                throw new ArgumentException("Calibration belt has no chaos price");

            double[] beltArray = belt.CreateInputVector();

            //Console.WriteLine("Before:" + PredictBelt(belt));
            //Console.WriteLine("Predicted:" + belt.CalibrationPrice);
            teacher.Run(beltArray, belt.CreateCalibrationOutputVector());
            //Console.WriteLine("After:" + PredictBelt(belt));
        }

        public double PredictBelt(Belt belt)
        {
            var result = network.Compute(belt.CreateInputVector());
            belt.ProcessOutputVector(result);
            return belt.CalculatedPrice.Value;
        }
    }
}
