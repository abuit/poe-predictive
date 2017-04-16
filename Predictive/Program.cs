using AForge.Neuro;
using POEStash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Predictive
{
    class Program
    {   
        //Set the starting point to the first change ID of the specific league.
        static readonly string startingPoint = "0";        

        public static void Main(string[] args)
        {
            //Based off of http://www.poeex.info/index/index/table/1
            var rates = new Dictionary<CurrencyType, float>()
            {
                { CurrencyType.ChromaticOrb, 1f/13f },
                { CurrencyType.OrbOfAlteration, 1f/13f },
                { CurrencyType.JewellersOrb, 1f/7f },
                { CurrencyType.OrbOfChance, 1f/5f },
                { CurrencyType.CartographersChisel, 1/4f },
                { CurrencyType.OrbOfFusing, 1f/2f },
                { CurrencyType.OrbOfAlchemy, 1f/3.5f },
                { CurrencyType.OrbOfScouring, 1f/1.8f },
                { CurrencyType.BlessedOrb, 1f/1.3f },
                { CurrencyType.OrbOfRegret, 1f },
                { CurrencyType.RegalOrb, 1.3f},
                { CurrencyType.GemcuttersPrism, 1.8f},
                { CurrencyType.DivineOrb, 17f},
                { CurrencyType.ExaltedOrb, 50f},
                { CurrencyType.VaalOrb, 1f},
            };
            ConversionTable conversionTable = new ConversionTable(rates);

            //Load belts
            BeltNetwork beltNetwork = new BeltNetwork();
            List<Belt> loadedBelts = new List<Belt>();

            string currentChangeId = startingPoint;
            while (true)
            {
                Console.WriteLine($"Loading for {currentChangeId}...");
                var stash = POEStash.POEStash.CreateAPIStash(currentChangeId);
                StashCollection c = stash.GetStashes().Result;
                foreach (Stash s in c.Stashes)
                {
                    foreach (Item i in s.Items)
                    {
                        //Only belts for now
                        if (i.ItemType != ItemType.Belt)
                        {
                            continue;
                        }

                        //No uniques for now
                        if (i.FrameType != FrameType.Magic && i.FrameType != FrameType.Rare)
                            continue;

                        //Ignore non-chaos priced items for now
                        if (i.Price.IsEmpty() || i.Price.CurrencyType == CurrencyType.Unknown)
                            continue;

                        //Convert if needed
                        float value;
                        if (i.Price.CurrencyType != CurrencyType.ChaosOrb)
                            value = conversionTable.ConvertTo(i.Price, CurrencyType.ChaosOrb).Value;
                        else
                            value = i.Price.Value;
                        
                        Belt b = new Belt(i.Corrupted, i.ImplicitMods, i.ExplicitMods)
                        {
                            CalibrationPrice = value
                        };
                        loadedBelts.Add(b);
                    }
                }

                Console.WriteLine("Number of belts loaded:" + loadedBelts.Count);
                if (string.IsNullOrEmpty(c.NextChangeID))
                {
                    break;
                }
                currentChangeId = c.NextChangeID;

                if (loadedBelts.Count > 1000)
                    break;
            }

            beltNetwork.LearnFromBelts(loadedBelts.ToArray());

            var testBelt = new Belt();
            testBelt.AddImplicit(@"+40 to maximum Life");
            testBelt.AddExplicit(@"+99 to maximum Life");
            testBelt.AddExplicit(@"+48% to Fire Resistance");
            testBelt.AddExplicit(@"+48% to Cold Resistance");
            testBelt.AddExplicit(@"+35% to Chaos Resistance");
            testBelt.AddExplicit(@"+460 to Armour");

            var testBelt2 = new Belt();
            testBelt2.AddImplicit(@"+3 to maximum Life");
            testBelt2.AddExplicit(@"+4 to maximum Life");
            testBelt2.AddExplicit(@"+13% to Fire Resistance");
            testBelt2.AddExplicit(@"+10% to Cold Resistance");
            testBelt2.AddExplicit(@"+5% to Chaos Resistance");
            testBelt2.AddExplicit(@"+3 to Armour");

            
            foreach (Belt b in loadedBelts)
            {
                beltNetwork.PredictBelt(b);
                Console.WriteLine($"Predicted price {(int)b.CalculatedPrice} chaos. Actual price: {(int)b.CalibrationPrice} chaos.");
            }

            beltNetwork.PredictBelt(testBelt);
            Console.WriteLine($"Triple resist maxed hp/armour belt: {(int)testBelt.CalculatedPrice} chaos!");

            beltNetwork.PredictBelt(testBelt2);
            Console.WriteLine($"Triple resist minimized hp/armour belt: {(int)testBelt2.CalculatedPrice} chaos!");

            double accuracy = 0;
            int hits = 0;
            foreach(Belt b in loadedBelts)
            {
                double provided = b.CalibrationPrice.Value;
                if (provided > Belt.MaxSupportedChaosPrice) provided = Belt.MaxSupportedChaosPrice;

                double predicted = b.CalculatedPrice.Value;

                double currentBeltAccuracy;
                if (provided == predicted)
                {
                    currentBeltAccuracy = 100;
                        
                }
                else if (provided < predicted)
                {
                    currentBeltAccuracy = 100 * provided / predicted;
                }
                else
                {
                    currentBeltAccuracy = 100 * predicted / provided;
                }

                accuracy = ((accuracy * hits) + currentBeltAccuracy) / (hits + 1);
                hits++;
            }

            Console.WriteLine($"Network accuracy: {accuracy}");
            Console.ReadKey();
        }
    }
}
