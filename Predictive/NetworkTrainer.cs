using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using POEStash.Model;

namespace Predictive
{
    using POEStash;
    using POEStash.Currency;

    public class NetworkTrainer
    {
        //Update if need be.
        //Only supports stuff without integral armour/evasion/energy shield or damage.
        public static readonly ItemType[] SupportedItemTypes = new ItemType[] { ItemType.Belt, ItemType.Jewel, ItemType.Amulet, ItemType.Ring, ItemType.Quiver };

        private TrainingCycleResult lastCycleResult;
        private readonly ConversionTable conversionTable;

        private int LastReceivedSnapshotId = -1;

        public NetworkTrainer(ConversionTable table)
        {
            this.conversionTable = table;
            this.lastCycleResult = new TrainingCycleResult(SupportedItemTypes);
            POEStash.OnStashUpdated += POEStash_OnStashUpdated;
        }

        public void StartTraining()
        {
            POEStash.Start();
        }

        public ItemNetwork GetItemNetwork(ItemType type)
        {
            return lastCycleResult.Networks[type];
        }

        Task<TrainingCycleResult> ContinuationEndpoint;

        private async void POEStash_OnStashUpdated(object sender, SnapShotEventArgs e)
        {
            //Save the last received snapshot ID
            LastReceivedSnapshotId = e.Snapshot.Id;

            TrainingCycleResult result;
            if (ContinuationEndpoint == null)
            {
                ContinuationEndpoint = Task.Run(() => PerformCycle(e.Snapshot));
                result = await ContinuationEndpoint;
            }
            else {
                //A task is already running, continue with the new task.
                ContinuationEndpoint = ContinuationEndpoint.ContinueWith((p) => PerformCycle(e.Snapshot));
                result = await ContinuationEndpoint;
            }

            //Check for aborted
            if (result == TrainingCycleResult.ABORTED)
            {
                return;
            }

            lastCycleResult = result;
        }

        private TrainingCycleResult PerformCycle(Snapshot snapshot)
        {
            //Cancel right away when a more recent snapshot has been provided
            if (snapshot.Id < LastReceivedSnapshotId)
            {
                Console.WriteLine($"({snapshot.Id}) Evaluation of snapshot was aborted since a more recent snapshot is present.");
                return TrainingCycleResult.ABORTED;
            }

            Console.WriteLine($"({snapshot.Id}) Evaluating snapshot...");

            //Initialize a new dictionary to hold the loaded items
            Dictionary<ItemType, List<ParsedItem>> loadedItems = new Dictionary<ItemType, List<ParsedItem>>();
            foreach (ItemType type in SupportedItemTypes)
            {
                loadedItems[type] = new List<ParsedItem>();
            }

            foreach (JsonPOEStash s in snapshot)
            {
                foreach (JsonPOEItem i in s.Items)
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
                Console.WriteLine($"({snapshot.Id}) Number of {type} items loaded: {loadedItems[type].Count}. This cycle there were {loadedItems[type].Count - lastCycleResult.Networks[type].CalibrationItemsCount} new items added.");
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

                Console.WriteLine($"({snapshot.Id}) Training the {type} network...");
                network.LearnFromItems();

                networks[type] = network;
            }

            Console.WriteLine($"({snapshot.Id}) Training of snapshot {snapshot.Id} was done.");

            //Print the info of this run
            foreach (ItemType type in SupportedItemTypes)
            {
                Console.WriteLine($"({snapshot.Id}) Accuracy of {type} network with {networks[type].CalibrationItemsCount} items: {(int)networks[type].DetermineAccuracy()} %");
            }
            Console.WriteLine(string.Empty);

            return new TrainingCycleResult(networks);
        }
    }

    class TrainingCycleResult
    {
        public static TrainingCycleResult ABORTED = new TrainingCycleResult(true);

        //Add more networks here
        public Dictionary<ItemType, ItemNetwork> Networks;
        public readonly bool Aborted;

        private TrainingCycleResult(bool aborted)
        {
            Networks = null;
            Aborted = aborted;
        }

        //Initialize the first training cycle result
        public TrainingCycleResult(ItemType[] supportedTypes)
        {
            Aborted = false;
            Networks = new Dictionary<ItemType, ItemNetwork>();
            foreach(ItemType type in supportedTypes)
            {
                Networks[type] = new ItemNetwork(new ParsedItem[0], new KnownAffix[0], new KnownAffix[0]);
            }
        }

        //Initialize a completed training cycle result
        public TrainingCycleResult(Dictionary<ItemType, ItemNetwork> networks)
        {
            Aborted = false;
            Networks = networks;
        }
    }
}
