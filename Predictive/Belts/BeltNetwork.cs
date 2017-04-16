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
                BeltImplicits.Count() + BeltExplicits.Count(),
                //Hidden layers:
                10,
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
            for (int i = 0; i < 1000; i++)
                teacher.RunEpoch(beltInputArray, beltResultArray);
        }

        //Learns iteratively
        public void LearnFromBelt(Belt belt)
        {
            if (belt.CalibrationPrice == null)
                throw new ArgumentException("Calibration belt has no chaos price");

            double[] beltArray = belt.CreateInputVector();

            //Console.WriteLine("Before:" + PredictBelt(belt));
            //Console.WriteLine("Predicted:" + belt.CalibrationPrice);
            teacher.Run(beltArray, belt.CreateCalibrationOutputVector());
            //Console.WriteLine("After:" + PredictBelt(belt));
        }

        public double PredictBelt(Belt belt)
        {
            var result = network.Compute(belt.CreateInputVector());
            belt.ProcessOutputVector(result);
            return belt.CalculatedPrice.Value;
        }
    }
}
