using POEStash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Predictive
{
    public class NetworkTrainer
    {
        private TrainingCycleResult lastCycleResult;
        private readonly ConversionTable conversionTable;


        public NetworkTrainer(ConversionTable table)
        {
            this.conversionTable = table;
        }

        public void StartTraining()
        {
            lastCycleResult = new TrainingCycleResult("45339225-47984329-44953841-51680241-48352319", new ItemNetwork(new Item[0], new KnownAffix[0], new KnownAffix[0]), new List<Item>());
            Task.Factory.StartNew(StartTrainingInternal);
        }

        public ItemNetwork BeltNetwork
        {
            get
            {
                return lastCycleResult.Network;
            }
        }

        //Fire and forget internal method
        private async void StartTrainingInternal()
        {
            while (lastCycleResult.NextChangeId != null)
            {
                //Store the result, then perform the next cycle.
                //The last cycle result will always contain an initialized network
                lastCycleResult = await PerformCycle();

                Console.WriteLine($"Accuracy of current belt network with {lastCycleResult.LoadedItems.Count} belts: {(int)lastCycleResult.Network.DetermineAccuracy()} %");
                Console.WriteLine(string.Empty);
            }
        }

        private async Task<TrainingCycleResult> PerformCycle()
        {
            //Make a new list of belts.
            var loadedItems = new List<Item>(lastCycleResult.LoadedItems);
            var knownItemsCount = loadedItems.Count;
            var changeId = lastCycleResult.NextChangeId;

            Console.WriteLine($"Loading for {changeId}...");

            var stash = POEStash.POEStash.CreateAPIStash(changeId);
            StashCollection c = await stash.GetStashes();

            foreach (Stash s in c.Stashes)
            {
                foreach (POEStash.Item i in s.Items)
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

                    Item b = new Item(i.Corrupted, i.ImplicitMods, i.ExplicitMods)
                    {
                        CalibrationPrice = value
                    };
                    loadedItems.Add(b);
                }
            }

            Console.WriteLine($"Number of items loaded: {loadedItems.Count}. This cycle there were {loadedItems.Count - knownItemsCount} new items added.");
            //We create a new network here to learn from the new dataset. Is it worth learning on the existing (cloned) network?

            Dictionary<string, KnownAffix> knownImplicits = new Dictionary<string, KnownAffix>();
            Dictionary<string, KnownAffix> knownExplicits = new Dictionary<string, KnownAffix>();
            foreach (Item i in loadedItems)
            {
                foreach (ParsedAffix a in i.ParsedImplicitMods)
                {
                    KnownAffix knownAffix;
                    if (!knownImplicits.TryGetValue(a.AffixCategory, out knownAffix))
                    {
                        knownAffix = new KnownAffix(a.AffixCategory, a.Value, a.Value);
                    }

                    knownAffix.UpdateWith(a);
                }

                foreach (ParsedAffix a in i.ParsedExplicitMods)
                {
                    KnownAffix knownAffix;
                    if (!knownExplicits.TryGetValue(a.AffixCategory, out knownAffix))
                    {
                        knownAffix = new KnownAffix(a.AffixCategory, a.Value, a.Value);
                    }

                    knownAffix.UpdateWith(a);
                }
            }

            ItemNetwork network = new ItemNetwork(loadedItems.ToArray(), 
                knownImplicits.Values.ToArray(), 
                knownExplicits.Values.ToArray());

            network.LearnFromItems();

            return new TrainingCycleResult(c.NextChangeID, network, loadedItems);
        }
    }

    struct TrainingCycleResult
    {
        public string NextChangeId;

        //Add more networks here
        public ItemNetwork Network;
        public List<Item> LoadedItems;

        public TrainingCycleResult(string nextChangeId, ItemNetwork network, List<Item> loadedItems)
        {
            NextChangeId = nextChangeId;
            Network = network;
            LoadedItems = loadedItems;
        }
    }
}
