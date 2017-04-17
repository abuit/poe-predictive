﻿using AForge.Neuro;
using AForge.Neuro.Learning;
using System;
using System.Linq;

namespace Predictive
{
    public class ItemNetwork
    {
        private readonly BackPropagationLearning teacher;
        private readonly ActivationNetwork network;

        private Item[] items;
        private readonly KnownAffix[] knownImplicits;
        private readonly KnownAffix[] knownExplicits;

        public ItemNetwork(Item[] items, KnownAffix[] knownImplicits, KnownAffix[] knownExplicits)
        {
            this.items = items;
            this.knownImplicits = knownImplicits;
            this.knownExplicits = knownExplicits;

            SigmoidFunction f = new SigmoidFunction(0.5);

            //Regression mode
            network = new ActivationNetwork(
                f,
                // Input layer. Corruption, implicits, explicits.
                1 + knownImplicits.Count() + knownImplicits.Count(),
                //Hidden layers:
                7,
                // Regression mode: one output
                1
            );

            teacher = new BackPropagationLearning(network)
            {
                LearningRate = 1,
                Momentum = 0.5
            };
        }

        //Learns in an epoch
        public void LearnFromItems()
        {
            var inputVectors = items.Select(b => b.CreateInputVector(knownImplicits, knownExplicits)).ToArray();
            var resultVectors = items.Select(b => b.CreateCalibrationOutputVector()).ToArray();

            Console.WriteLine("Loading data into the network...");

            int trainingCycles = 10000;

            for (int i = 0; i < trainingCycles; i++)
            {
                teacher.RunEpoch(inputVectors, resultVectors);
            }
            Console.WriteLine("Data loaded!");
        }

        public double PredictBelt(Item belt)
        {
            var result = network.Compute(belt.CreateInputVector(knownImplicits, knownExplicits));
            belt.ProcessOutputVector(result);
            return belt.CalculatedPrice.Value;
        }

        public double DetermineAccuracy()
        {
            double accuracy = 0;
            int hits = 0;
            foreach (Item i in items)
            {
                if (i.CalibrationPrice == null)
                    continue;

                PredictBelt(i);

                double provided = i.CalibrationPrice.Value;
                if (provided > Item.MaxSupportedChaosPrice) provided = Item.MaxSupportedChaosPrice;

                double predicted = i.CalculatedPrice.Value;

                double accuraccy;
                if (provided == predicted)
                {
                    accuraccy = 100;

                }
                else if (provided < predicted)
                {
                    accuraccy = 100 * provided / predicted;
                }
                else
                {
                    accuraccy = 100 * predicted / provided;
                }

                accuracy = ((accuracy * hits) + accuraccy) / (hits + 1);
                hits++;
            }

            return accuracy;
        }
    }
}