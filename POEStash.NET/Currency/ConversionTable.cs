using System.Collections.Generic;

namespace POEStash
{
    public class ConversionTable
    {
        private Dictionary<CurrencyType, float> weights;

        public ConversionTable()
        {
            InitializeBaseWeights();
        }

        public ConversionTable(Dictionary<CurrencyType, float> rates)
        {
            InitializeBaseWeights();
            foreach (var pair in rates)
            {
                weights[pair.Key] = pair.Value;
            }
        }

        private void InitializeBaseWeights()
        {
            weights = new Dictionary<CurrencyType, float>
            {
                { CurrencyType.Unknown            , 0f },
                { CurrencyType.OrbOfAlteration    , 1f },
                { CurrencyType.OrbOfFusing        , 1f },
                { CurrencyType.OrbOfAlchemy       , 1f },
                { CurrencyType.ChaosOrb           , 1f },
                { CurrencyType.GemcuttersPrism    , 1f },
                { CurrencyType.ExaltedOrb         , 1f },
                { CurrencyType.JewellersOrb       , 1f },
                { CurrencyType.OrbOfChance        , 1f },
                { CurrencyType.CartographersChisel, 1f },
                { CurrencyType.OrbOfScouring      , 1f },
                { CurrencyType.BlessedOrb         , 1f },
                { CurrencyType.OrbOfRegret        , 1f },
                { CurrencyType.RegalOrb           , 1f },
                { CurrencyType.DivineOrb          , 1f },
                { CurrencyType.VaalOrb            , 1f },
            };
        }

        public Currency ConvertTo(Currency currency, CurrencyType type)
        {
            if (weights[type] == 0f)
            {
                return new Currency(type, 0f);
            }

            float newValue = currency.Value * (weights[currency.CurrencyType] / weights[type]);
            return new Currency(type, newValue);
        }
    }
}
