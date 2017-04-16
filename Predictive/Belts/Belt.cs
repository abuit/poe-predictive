﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Predictive
{
    public class Belt
    {
        //The maximum price the neural network will be able to suggest.
        private readonly static int MaxSupportedChaosPrice = 250;

        //The price to calibrate this belt with.
        public double? CalibrationPrice;

        //The actual predicted price of the belt.
        public double? CalculatedPrice;

        public bool Corrupted { get; set; }

        private string[] implicitMods;
        private string[] explicitMods;

        public Belt()
        {
            Corrupted = false;
            implicitMods = new string[0];
            explicitMods = new string[0];
        }

        public Belt(bool corrupted, string[] implicitMods, string[] explicitMods)
        {
            this.implicitMods = implicitMods;
            this.explicitMods = explicitMods;
        }

        public void AddImplicit(string impl)
        {
            var old = implicitMods;
            implicitMods = new string[old.Length + 1];
            old.CopyTo(implicitMods, 0);
            implicitMods[implicitMods.Length - 1] = impl;
        }

        public void AddExplicit(string expl)
        {
            var old = explicitMods;
            explicitMods = new string[old.Length + 1];
            old.CopyTo(explicitMods, 0);
            explicitMods[explicitMods.Length - 1] = expl;
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
        /// The input vector size for a belt. Includes an item for each property.
        /// </summary>
        public int BeltInputVectorSize
        {
            get
            {
                return 1 + BeltImplicits.Count() + BeltExplicits.Count();
            }
        }

        /// <summary>
        /// Creates the input vector for this belt.
        /// Ensure this input vector matches the amount of neurons in the first layer of the network.
        /// </summary>
        public double[] CreateInputVector()
        {
            double[] inputArray = new double[BeltInputVectorSize];
            inputArray[0] = Corrupted ? 0 : 1;

            int indx = 1;
            
            HashSet<string> matchedImplicits = new HashSet<string>();
            foreach (var possibleImplcit in BeltImplicits.All())
            {
                foreach (string impl in implicitMods.Where(e => !matchedImplicits.Contains(e)))
                {
                    if (AffixUtils.TryMatch(impl, possibleImplcit.Pattern, possibleImplcit.MinValue, possibleImplcit.MaxValue, out double result))
                    {
                        matchedImplicits.Add(impl);
                        inputArray[indx] = result;
                        break;
                    }
                }
                indx++;
            }

            HashSet<string> matchedExplicits = new HashSet<string>();
            foreach (var possibleExplicit in BeltExplicits.All())
            {
                foreach (string expl in explicitMods.Where(e => !matchedExplicits.Contains(e)))
                {
                    if (AffixUtils.TryMatch(expl, possibleExplicit.Pattern, possibleExplicit.MinValue, possibleExplicit.MaxValue, out double result))
                    {
                        matchedExplicits.Add(expl);
                        inputArray[indx] = result;
                        break;
                    }
                }
                indx++;
            }

            //Diagnostics:
            foreach (string unmatched in implicitMods.Where(e => !matchedImplicits.Contains(e)))
            {
                System.Console.WriteLine($"Implicit was not recognised: {unmatched}");
            }

            foreach (string unmatched in explicitMods.Where(e => !matchedExplicits.Contains(e)))
            {
                System.Console.WriteLine($"Explicit was not recognised: {unmatched}");
            }

            return inputArray;
        }
    }

    struct BeltImplicits
    {
        const string CAPTURE = "CAPTURE";
        private static readonly string
            //Default
            ChainBelt = Regex.Escape($"+{CAPTURE} to maximum Energy Shield").Replace(CAPTURE, @"(\d*)"),
            RusticSash = Regex.Escape($"{CAPTURE}% increased Physical Damage").Replace(CAPTURE, @"(\d*)"),
            HeavyBelt = Regex.Escape($"+{CAPTURE} to Strength").Replace(CAPTURE, @"(\d*)"),
            LeatherBelt = Regex.Escape($"+{CAPTURE} to maximum Life").Replace(CAPTURE, @"(\d*)"),
            ClothBelt = Regex.Escape($"{CAPTURE}% increased Stun and Block Recovery").Replace(CAPTURE, @"(\d*)"),
            StuddedBelt = Regex.Escape($"{CAPTURE}% increased Stun Duration on Enemies").Replace(CAPTURE, @"(\d*)"),
            VanguardBelt = Regex.Escape($"+{CAPTURE} to Armour and Evasion Rating").Replace(CAPTURE, @"(\d*)"),
            CrystalBelt = Regex.Escape($"+{CAPTURE} to maximum Energy Shield").Replace(CAPTURE, @"(\d*)"),
            //Corruptions
            ChaosResist = Regex.Escape($"+{CAPTURE}% to Chaos Resistance").Replace(CAPTURE, @"(\d*)"),
            AvoidShock = Regex.Escape($"{CAPTURE}% chance to Avoid being Shocked").Replace(CAPTURE, @"(\d*)"),
            RadiusAoE = Regex.Escape($"{CAPTURE}% increased Radius of Area Skills").Replace(CAPTURE, @"(\d*)"),
            SkillDuration = Regex.Escape($"{CAPTURE}% increased Skill Effect Duration").Replace(CAPTURE, @"(\d*)"),
            AddTrap = Regex.Escape($"Can set up to {CAPTURE} additional trap").Replace(CAPTURE, @"(\d*)"),
            Endurance = Regex.Escape($"+{CAPTURE} Maximum Endurance Charge").Replace(CAPTURE, @"(\d*)"),
            Purity = Regex.Escape($"Grants level {CAPTURE} Purity of Elements Skill").Replace(CAPTURE, @"(\d*)"),
            Clarity = Regex.Escape($"Grants level {CAPTURE} Clarity Skill").Replace(CAPTURE, @"(\d*)"),
            Vitality = Regex.Escape($"Grants level {CAPTURE} Vitality Skill").Replace(CAPTURE, @"(\d*)");

        public string Pattern;
        public double MinValue;
        public double MaxValue;

        public BeltImplicits(string pattern, double minValue, double maxValue)
        {
            this.Pattern = pattern;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        public static IEnumerable<BeltExplicits> All()
        {
            yield return new BeltExplicits(ChainBelt, 9, 20);
            yield return new BeltExplicits(RusticSash, 12, 24);
            yield return new BeltExplicits(HeavyBelt, 25, 35);
            yield return new BeltExplicits(LeatherBelt, 25, 40);
            yield return new BeltExplicits(ClothBelt, 15, 25);
            yield return new BeltExplicits(StuddedBelt, 20, 30);
            yield return new BeltExplicits(VanguardBelt, 260, 320);
            yield return new BeltExplicits(CrystalBelt, 60, 80);
            yield return new BeltExplicits(ChaosResist, 2, 4);
            yield return new BeltExplicits(AvoidShock, 10, 20);
            yield return new BeltExplicits(RadiusAoE, 4, 6);
            yield return new BeltExplicits(SkillDuration, 5, 8);
            yield return new BeltExplicits(AddTrap, 1, 1);
            yield return new BeltExplicits(Endurance, 1, 1);
            yield return new BeltExplicits(Purity, 15, 15);
            yield return new BeltExplicits(Clarity, 4, 16);
            yield return new BeltExplicits(Vitality, 15, 15);
        }

        public static int Count()
        {
            return All().Count();
        }
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

        public static int Count()
        {
            return All().Count();
        }
    }
}
