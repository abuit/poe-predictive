using POEStash;
using System;
using System.Collections.Generic;
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
            lastCycleResult = new TrainingCycleResult("0", new BeltNetwork(), new List<Belt>());
            Task.Factory.StartNew(StartTrainingInternal);
        }

        public BeltNetwork BeltNetwork
        {
            get
            {
                return lastCycleResult.BeltNetwork;
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

                Console.WriteLine($"Accuracy of current belt network with {lastCycleResult.LoadedBelts.Count} belts: {lastCycleResult.BeltNetwork.DetermineAccuracy(lastCycleResult.LoadedBelts)} %");
            }
        }

        private async Task<TrainingCycleResult> PerformCycle()
        {
            //Make a new list of belts.
            var loadedBelts = new List<Belt>(lastCycleResult.LoadedBelts);
            var knownBeltsCount = loadedBelts.Count;
            var changeId = lastCycleResult.NextChangeId;

            Console.WriteLine($"Loading for {changeId}...");

            var stash = POEStash.POEStash.CreateAPIStash(changeId);
            StashCollection c = await stash.GetStashes();

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

            Console.WriteLine($"Number of belts loaded: {loadedBelts.Count}. This cycle there were {loadedBelts.Count - knownBeltsCount} new belts added.");
            
            //We create a new network here to learn from the new dataset. Is it worth learning on the existing (cloned) network?
            BeltNetwork beltNetwork = new BeltNetwork();
            beltNetwork.LearnFromBelts(loadedBelts.ToArray());

            return new TrainingCycleResult(c.NextChangeID, beltNetwork, loadedBelts);
        }
    }

    struct TrainingCycleResult
    {
        public string NextChangeId;

        //Add more networks here
        public BeltNetwork BeltNetwork;
        public List<Belt> LoadedBelts;

        public TrainingCycleResult(string nextChangeId, BeltNetwork beltNetwork, List<Belt> loadedBelts)
        {
            NextChangeId = nextChangeId;
            BeltNetwork = beltNetwork;
            LoadedBelts = loadedBelts;
        }
    }
}
