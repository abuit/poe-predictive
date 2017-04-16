using AForge.Neuro;
using POEStash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Predictive
{
    class Program
    {
        //Set the starting point to the first change ID of the specific league.
        static readonly string startingPoint = "0";

        static ConversionTable conversionTable;
        static BeltNetwork beltNetwork;

        static bool loadingHalted = false;

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
            conversionTable = new ConversionTable(rates);

            NetworkTrainer trainer = new NetworkTrainer(conversionTable);
            trainer.StartTraining();

            Console.WriteLine("The networks are training in the background. Press any key to evaluate belts with current data.");
            Console.ReadKey();

            var testBelt = new Belt();
            testBelt.AddImplicit(@"+40 to maximum Life");
            testBelt.AddExplicit(@"+99 to maximum Life");
            testBelt.AddExplicit(@"+48% to Fire Resistance");
            testBelt.AddExplicit(@"+48% to Cold Resistance");
            testBelt.AddExplicit(@"+48% to Lightning Resistance");
            testBelt.AddExplicit(@"+460 to Armour");


            var testBelt2 = new Belt();
            testBelt2.AddImplicit(@"+3 to maximum Life");
            testBelt2.AddExplicit(@"+4 to maximum Life");
            testBelt2.AddExplicit(@"+13% to Fire Resistance");
            testBelt2.AddExplicit(@"+10% to Cold Resistance");
            testBelt2.AddExplicit(@"+10% to Lightning Resistance");
            testBelt2.AddExplicit(@"+3 to Armour");
            
            trainer.BeltNetwork.PredictBelt(testBelt);
            Console.WriteLine(testBelt.ToString());

            trainer.BeltNetwork.PredictBelt(testBelt2);
            Console.WriteLine(testBelt2.ToString());

            Console.ReadKey();
        }
    }  
}
