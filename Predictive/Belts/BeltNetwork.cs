using AForge.Neuro;
using AForge.Neuro.Learning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Predictive
{
    class BeltNetwork
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
                5,
                // Regression mode: one output
                1
            );

            teacher = new BackPropagationLearning(network)
            {
                LearningRate = 1,
                Momentum = 0.2
            };
        }

        //Learns in an epoch
        public void LearnFromBelts(params Belt[] belts)
        {
            var beltInputArray = belts.Select(b => b.CreateInputVector()).ToArray();
            var beltResultArray = belts.Select(b => b.CreateCalibrationOutputVector()).ToArray();

            Console.Write("Loading data into the network");

            int trainingCycles = 10000;

            for (int i = 0; i < trainingCycles; i++)
            {
                if (i % (trainingCycles / 20) == 0)
                    Console.Write(".");
                teacher.RunEpoch(beltInputArray, beltResultArray);
            }
            Console.WriteLine();
            Console.WriteLine("Done!");
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
    }
}
