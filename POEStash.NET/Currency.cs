using System;
using System.Globalization;

namespace POEStash
{
    public enum CurrencyType
    {
        Unknown,
        OrbOfAlteration,
        OrbOfFusing,
        OrbOfAlchemy,
        ChaosOrb,
        GemcuttersPrism,
        ExaltedOrb,
        ChromaticOrb,
        JewellersOrb,
        OrbOfChance,
        CartographersChisel,
        OrbOfScouring,
        BlessedOrb,
        OrbOfRegret,
        RegalOrb,
        DivineOrb,
        VaalOrb
    }

    public struct Currency : IEquatable<Currency>
    {
        private const string ALTERATION = "alt";
        private const string FUSING     = "fuse";
        private const string ALCHEMY    = "alch";
        private const string CHAOS      = "chaos";
        private const string GEMCUTTERS = "gcp";
        private const string EXALTED    = "exa";
        private const string CHROMATIC  = "chrom";
        private const string JEWELLERS  = "jew";
        private const string CHANCE     = "chance";
        private const string CHISEL     = "chisel";
        private const string SCOURING   = "scour";
        private const string BLESSED    = "blessed";
        private const string REGRET     = "regret";
        private const string REGAL      = "regal";
        private const string DIVINE     = "divine";
        private const string VAAL       = "vaal";

        public static Currency Empty
        {
            get
            {
                return new Currency(CurrencyType.Unknown, 0);
            }
        }

        public static Currency Parse(string currencyString)
        {
            CurrencyType type = CurrencyType.Unknown;
            float value = 0;

            if (currencyString == null || currencyString.Length < 8)
            {
                return new Currency(type, value);
            }

            int offset = -1;
            // Price
            if (currencyString.StartsWith("~b/o"))
            {
                offset = 5;
            }
            // Fixed Price
            else if (currencyString.StartsWith("~price"))
            {
                offset = 7;
            }

            if (offset < 0)
            {
                return Empty;
            }

            string valueString = currencyString.Substring(offset, currencyString.Length - offset);

            return ParseValueString(valueString);
        }

        private static Currency ParseValueString(string valueString)
        {
            CurrencyType type = CurrencyType.Unknown;
            float value = 0;

            int splitIndex = valueString.IndexOf(' ');
            if (splitIndex < 0)
            {
                return Empty;
            }

            string valString = valueString.Substring(0, splitIndex);

            float.TryParse(valString,NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value);

            string typeString = valueString.Substring(splitIndex + 1, valueString.Length - splitIndex - 1);
            switch (typeString)
            {
                case ALTERATION:
                    type = CurrencyType.OrbOfAlteration;
                    break;
                case FUSING:
                    type = CurrencyType.OrbOfFusing;
                    break;
                case ALCHEMY:
                    type = CurrencyType.OrbOfAlchemy;
                    break;
                case CHAOS:
                    type = CurrencyType.ChaosOrb;
                    break;
                case GEMCUTTERS:
                    type = CurrencyType.GemcuttersPrism;
                    break;
                case EXALTED:
                    type = CurrencyType.ExaltedOrb;
                    break;
                case CHROMATIC:
                    type = CurrencyType.ChromaticOrb;
                    break;
                case JEWELLERS:
                    type = CurrencyType.JewellersOrb;
                    break;
                case CHANCE:
                    type = CurrencyType.OrbOfChance;
                    break;
                case CHISEL:
                    type = CurrencyType.CartographersChisel;
                    break;
                case SCOURING:
                    type = CurrencyType.OrbOfScouring;
                    break;
                case BLESSED:
                    type = CurrencyType.BlessedOrb;
                    break;
                case REGRET:
                    type = CurrencyType.OrbOfRegret;
                    break;
                case REGAL:
                    type = CurrencyType.RegalOrb;
                    break;
                case DIVINE:
                    type = CurrencyType.DivineOrb;
                    break;
                case VAAL:
                    type = CurrencyType.VaalOrb;
                    break;
            }

            return new Currency(type, value);
        }

        public CurrencyType CurrencyType { get; private set; }
        public float Value { get; private set; }
        
        private Currency(CurrencyType type, float value)
        {
            this.CurrencyType = type;
            this.Value = value;
        }

        public override bool Equals(object obj)
        {
            return (obj is Currency)
                && this.CurrencyType.Equals(((Currency)obj).CurrencyType)
                && this.Value.Equals(((Currency)obj).Value);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + CurrencyType.GetHashCode();
            hash = hash * 23 + Value.GetHashCode();
            return hash;
        }

        public bool IsEmpty()
        {
            return this == Empty;
        }

        public static bool operator ==(Currency c1, Currency c2)
        {
            return
                c1.Value == c2.Value &&
                c1.CurrencyType == c2.CurrencyType;
        }

        public static bool operator !=(Currency c1, Currency c2)
        {
            return !(c1 == c2);
        }

        public bool Equals(Currency other)
        {
            return this == other;
        }

        public override string ToString()
        {
            return Value + " " + CurrencyType;
        }
    }
}
