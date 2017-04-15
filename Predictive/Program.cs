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
        static BeltNetwork beltNetwork = new BeltNetwork();

        static void Main(string[] args)
        {
            List<Belt> loadedBelts = new List<Belt>();

            string currentChangeId = "0";
            while (true)
            {
                Console.WriteLine($"Loading for {currentChangeId}...");
                var stash = POEStash.POEStash.CreateAPIStash(currentChangeId);
                StashCollection c = stash.GetStashes().Result;
                foreach (Stash s in c.Stashes)
                {
                    foreach (Item i in s.Items)
                    {
                        //Check for the belt
                        if (i.TypeLine.IndexOf("Belt", 0, StringComparison.OrdinalIgnoreCase) == -1)
                        {
                            continue;
                        }

                        //No uniques
                        if (i.FrameType != FrameType.Magic && i.FrameType != FrameType.Rare)
                            continue;

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
