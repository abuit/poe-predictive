using AForge.Neuro;
using AForge.Neuro.Learning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Predictive
{
    public class BeltNetwork
    {
        private readonly BackPropagationLearning teacher;
        private readonly ActivationNetwork network;

        public BeltNetwork()
        {
            SigmoidFunction f = new SigmoidFunction(0.5);

            //Regression mode
            network = new ActivationNetwork(
                f,
                1 + BeltImplicits.Count() + BeltExplicits.Count(),
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
        public void LearnFromBelts(params Belt[] belts)
        {
            var beltInputArray = belts.Select(b => b.CreateInputVector()).ToArray();
            var beltResultArray = belts.Select(b => b.CreateCalibrationOutputVector()).ToArray();

            Console.WriteLine("Loading data into the network...");

            int trainingCycles = 10000;

            for (int i = 0; i < trainingCycles; i++)
            {
                teacher.RunEpoch(beltInputArray, beltResultArray);
            }
            Console.WriteLine("Data loaded!");
        }

        //Learns iteratively
        public void LearnFromBelt(Belt belt)
        {
            if (belt.CalibrationPrice == null)
                throw new ArgumentException("Calibration belt has no chaos price");

            double[] beltArray = belt.CreateInputVector();
            teacher.Run(beltArray, belt.CreateCalibrationOutputVector());
        }

        public double PredictBelt(Belt belt)
        {
            var result = network.Compute(belt.CreateInputVector());
            belt.ProcessOutputVector(result);
            return belt.CalculatedPrice.Value;
        }

        public double DetermineAccuracy(List<Belt> calibrationBelts)
        {
            double accuracy = 0;
            int hits = 0;
            foreach (Belt b in calibrationBelts)
            {
                if (b.CalibrationPrice == null)
                    continue;

                PredictBelt(b);

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

            return accuracy;
        }
    }
}
