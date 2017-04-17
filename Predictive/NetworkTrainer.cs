using POEStash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Predictive
{
    using POEStash;

    public class NetworkTrainer
    {
        //Update if need be.
        public static readonly ItemType[] SupportedItemTypes = new ItemType[] { ItemType.Belt, ItemType.Jewel };

        private TrainingCycleResult lastCycleResult;
        private readonly ConversionTable conversionTable;
        private readonly POEStash stash;

        public NetworkTrainer(ConversionTable table)
        {
            this.conversionTable = table;
            this.stash = POEStash.CreateAPIStash();
        }

        public void StartTraining()
        {
            lastCycleResult = new TrainingCycleResult("45339225-47984329-44953841-51680241-48352319", SupportedItemTypes);
            Task.Factory.StartNew(StartTrainingInternal);
        }

        public ItemNetwork GetItemNetwork(ItemType type)
        {
            return lastCycleResult.Networks[type];
        }

        //Fire and forget internal method
        private async void StartTrainingInternal()
        {
            while (lastCycleResult.NextChangeId != null)
            {
                //Store the result, then perform the next cycle.
                //The last cycle result will always contain an initialized network
                lastCycleResult = await PerformCycle();

                foreach (ItemType type in SupportedItemTypes)
                {
                    Console.WriteLine($"Accuracy of {type} network with {lastCycleResult.Networks[type].CalibrationItemsCount} items: {(int)lastCycleResult.Networks[type].DetermineAccuracy()} %");
                }
                Console.WriteLine(string.Empty);
            }
        }

        private async Task<TrainingCycleResult> PerformCycle()
        {
            var changeId = lastCycleResult.NextChangeId;

            //Load existing items (TODO - remove later)
            Dictionary<ItemType, List<ParsedItem>> loadedItems = new Dictionary<ItemType, List<ParsedItem>>();
            foreach (ItemType type in SupportedItemTypes)
            {
                loadedItems[type] = new List<ParsedItem>();
                loadedItems[type].AddRange(lastCycleResult.Networks[type].CalibrationItems);
            }

            Console.WriteLine($"Loading for {changeId}...");

            StashCollection c = await stash.GetStash(changeId);

            foreach (Stash s in c.Stashes)
            {
                foreach (Item i in s.Items)
                {
                    //Skip unsupported items
                    if (!SupportedItemTypes.Contains(i.ItemType))
                        continue;

                    //No uniques for now
                    if (i.FrameType != FrameType.Magic && i.FrameType != FrameType.Rare)
                        continue;

                    //Ignore non-chaos priced items for now
                    if (i.Price.IsEmpty() || i.Price.CurrencyType == CurrencyType.Unknown)
                        continue;

                    //Convert the value if needed
                    float value;
                    if (i.Price.CurrencyType != CurrencyType.ChaosOrb)
                        value = conversionTable.ConvertTo(i.Price, CurrencyType.ChaosOrb).Value;
                    else
                        value = i.Price.Value;

                    ParsedItem b = new ParsedItem(i.Corrupted, i.ImplicitMods, i.ExplicitMods)
                    {
                        CalibrationPrice = value
                    };

                    loadedItems[i.ItemType].Add(b);
                }
            }

            foreach (ItemType type in SupportedItemTypes)
            {
                Console.WriteLine($"Number of items loaded: {loadedItems[type].Count}. This cycle there were {loadedItems[type].Count - lastCycleResult.Networks[type].CalibrationItemsCount} new items added.");
            }

            Dictionary<ItemType, ItemNetwork> networks = new Dictionary<ItemType, ItemNetwork>();
            foreach (ItemType type in SupportedItemTypes)
            {
                var knownImplicits = new Dictionary<string, KnownAffix>();
                var knownExplicits = new Dictionary<string, KnownAffix>();
                foreach (ParsedItem i in loadedItems[type])
                {
                    foreach (ParsedAffix a in i.ParsedImplicitMods)
                    {
                        KnownAffix knownAffix;
                        if (!knownImplicits.TryGetValue(a.AffixCategory, out knownAffix))
                        {
                            knownAffix = new KnownAffix(a.AffixCategory, a.Value, a.Value);
                            knownImplicits.Add(knownAffix.AffixCategory, knownAffix);
                        }

                        knownAffix.UpdateWith(a);
                    }

                    foreach (ParsedAffix a in i.ParsedExplicitMods)
                    {
                        KnownAffix knownAffix;
                        if (!knownExplicits.TryGetValue(a.AffixCategory, out knownAffix))
                        {
                            knownAffix = new KnownAffix(a.AffixCategory, a.Value, a.Value);
                            knownExplicits.Add(knownAffix.AffixCategory, knownAffix);
                        }

                        knownAffix.UpdateWith(a);
                    }
                }

                ItemNetwork network = new ItemNetwork(
                    loadedItems[type].ToArray(),
                    knownImplicits.Values.ToArray(),
                    knownExplicits.Values.ToArray());

                network.LearnFromItems();

                networks[type] = network;
            }

            return new TrainingCycleResult(c.NextChangeID, networks);
        }
    }

    struct TrainingCycleResult
    {
        public string NextChangeId;

        //Add more networks here
        public Dictionary<ItemType, ItemNetwork> Networks;

        //Initialize the first training cycle result
        public TrainingCycleResult(string nextChangeId, ItemType[] supportedTypes)
        {
            NextChangeId = nextChangeId;
            Networks = new Dictionary<ItemType, ItemNetwork>();
            foreach(ItemType type in supportedTypes)
            {
                Networks[type] = new ItemNetwork(new ParsedItem[0], new KnownAffix[0], new KnownAffix[0]);
            }
        }

        //Initialize a completed training cycle result
        public TrainingCycleResult(string nextChangeId, Dictionary<ItemType, ItemNetwork> networks)
        {
            NextChangeId = nextChangeId;
            Networks = networks;
        }
    }
}
