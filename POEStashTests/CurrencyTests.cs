using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using POEStash;
using POEStash.Currency;

namespace POEStashTests
{
    [TestClass]
    public class CurrencyTests
    {
        [TestMethod]
        public void TestConversionRateToZeroWeight()
        {
            CurrencyType wantedCurrency = CurrencyType.Unknown;
            Currency sourceCurrency = new Currency(CurrencyType.ChaosOrb, 1f);
            ConversionTable conversionTable = new ConversionTable();

            Currency convertedCurrency = conversionTable.ConvertTo(sourceCurrency, wantedCurrency);
            Assert.AreEqual(0, convertedCurrency.Value, 0);
        }

        [TestMethod]
        public void TestConversionRateFromUnitToMoreExpensive()
        {
            CurrencyType wantedCurrency = CurrencyType.ExaltedOrb;
            Currency sourceCurrency = new Currency(CurrencyType.ChaosOrb, 1f);
            ConversionTable conversionTable = new ConversionTable (
                new Dictionary<CurrencyType, float>
                {
                    { wantedCurrency, 10f }
                }
            );

            Currency convertedCurrency = conversionTable.ConvertTo(sourceCurrency, wantedCurrency);
            Assert.AreEqual(0.1f, convertedCurrency.Value, 0.001f);
        }

        [TestMethod]
        public void TestConversionRateFromUnitToCheaper()
        {
            CurrencyType wantedCurrency = CurrencyType.OrbOfAlchemy;
            Currency sourceCurrency = new Currency(CurrencyType.ChaosOrb, 1f);
            ConversionTable conversionTable = new ConversionTable(
                new Dictionary<CurrencyType, float>
                {
                    { wantedCurrency, 0.01f }
                }
            );

            Currency convertedCurrency = conversionTable.ConvertTo(sourceCurrency, wantedCurrency);
            Assert.AreEqual(100f, convertedCurrency.Value, 0.001f);
        }

        [TestMethod]
        public void TestConversionRateFromDecimalToMoreExpensive()
        {
            CurrencyType wantedCurrency = CurrencyType.ExaltedOrb;
            Currency sourceCurrency = new Currency(CurrencyType.ChaosOrb, 2.5f);
            ConversionTable conversionTable = new ConversionTable(
                new Dictionary<CurrencyType, float>
                {
                    { wantedCurrency, 10f }
                }
            );

            Currency convertedCurrency = conversionTable.ConvertTo(sourceCurrency, wantedCurrency);
            Assert.AreEqual(0.25f, convertedCurrency.Value, 0.001f);
        }

        [TestMethod]
        public void TestConversionRateFromDecimalToCheaper()
        {
            CurrencyType wantedCurrency = CurrencyType.OrbOfAlchemy;
            Currency sourceCurrency = new Currency(CurrencyType.ExaltedOrb, 5.5f);
            ConversionTable conversionTable = new ConversionTable(
                new Dictionary<CurrencyType, float>
                {
                    { wantedCurrency, 0.2f },
                }
            );

            Currency convertedCurrency = conversionTable.ConvertTo(sourceCurrency, wantedCurrency);
            Assert.AreEqual(27.5f, convertedCurrency.Value, 0.001f);
        }

        [TestMethod]
        public void TestFusingToExaltedOrbs()
        {
            CurrencyType wantedCurrency = CurrencyType.ExaltedOrb;
            Currency sourceCurrency = new Currency(CurrencyType.OrbOfFusing, 6f);
            ConversionTable conversionTable = new ConversionTable(
                new Dictionary<CurrencyType, float>
                {
                    { CurrencyType.ExaltedOrb, 50 },
                    { CurrencyType.OrbOfFusing, 1/12f }
                }
            );

            Currency convertedCurrency = conversionTable.ConvertTo(sourceCurrency, wantedCurrency);
            Assert.AreEqual(0.01f, convertedCurrency.Value, 0.001f);
        }

        [TestMethod]
        public void TestExaltedOrbsToFusing()
        {
            CurrencyType wantedCurrency = CurrencyType.OrbOfFusing;
            Currency sourceCurrency = new Currency(CurrencyType.ExaltedOrb, 1f);
            ConversionTable conversionTable = new ConversionTable(
                new Dictionary<CurrencyType, float>
                {
                    { CurrencyType.ExaltedOrb, 50 },
                    { CurrencyType.OrbOfFusing, 1f/12f }
                }
            );

            Currency convertedCurrency = conversionTable.ConvertTo(sourceCurrency, wantedCurrency);
            Assert.AreEqual(12 * 50f, convertedCurrency.Value, 0.001f);
        }
    }
}
