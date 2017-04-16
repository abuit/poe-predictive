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

                        Belt b = new Belt(i.ExplicitMods)
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

                if (loadedBelts.Count > 250)
                    break;
            }

            //Find the maximum belt price. This way the price can be normalized for for the networks' output.
            Belt.MaxChaosPrice = loadedBelts.Max(b => (int)b.CalibrationPrice);

            beltNetwork.LearnFromBelts(loadedBelts.ToArray());

            /*
            var testBelt = new Belt();
            testBelt.AddExplicit(@"+99 to maximum Life");
            testBelt.AddExplicit(@"+40% to Fire Resistance");
            testBelt.AddExplicit(@"+40% to Cold Resistance");
            testBelt.AddExplicit(@"+40% to Chaos Resistance");

            var result = beltNetwork.PredictBelt(testBelt);
            Console.WriteLine($"This belt ({nameof(testBelt)}) is probably worth around {result} chaos");

            testBelt = new Belt();
            testBelt.AddExplicit(@"+15 to maximum Energy Shield");
            testBelt.AddExplicit(@"+15% to Fire Resistance");
            testBelt.AddExplicit(@"+15% to Cold Resistance");
            testBelt.AddExplicit(@"+15% to Lightning Resistance");

            result = beltNetwork.PredictBelt(testBelt);
            Console.WriteLine($"This belt ({nameof(testBelt)}) is probably worth around {result} chaos");
            */

            foreach(Belt b in loadedBelts)
            {
                beltNetwork.PredictBelt(b);
                Console.WriteLine($"Predicted price {(int)b.CalculatedPrice} chaos. Actual price: {(int)b.CalibrationPrice} chaos.");
            }

            Console.ReadKey();
        }
    }
}
