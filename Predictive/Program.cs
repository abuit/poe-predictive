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
                        if (i.TypeLine.IndexOf("Belt", 0, StringComparison.OrdinalIgnoreCase) == -1)
                        {
                            continue;
                        }

                        //No uniques for now
                        if (i.FrameType != FrameType.Magic && i.FrameType != FrameType.Rare)
                            continue;

                        //Ignore non-chaos priced items for now
                        if (i.Price.IsEmpty() || i.Price.CurrencyType != CurrencyType.ChaosOrb)
                            continue;

                        Belt b = new Belt(i.Corrupted, i.ImplicitMods, i.ExplicitMods)
                        {
                            CalibrationPrice = i.Price.Value
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

            //Find the maximum belt price. This way the price can be normalized for for the networks' output.
            Belt.MaxChaosPrice = loadedBelts.Max(b => (int)b.CalibrationPrice);

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

            Console.ReadKey();
        }
    }
}
