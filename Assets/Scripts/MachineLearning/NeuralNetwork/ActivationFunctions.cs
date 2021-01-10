using System;
// ReSharper disable UnusedMember.Global

namespace MachineLearning.NeuralNetwork
{
    public static class ActivationFunctions
    {
        public static double BinaryStep(double value)
        {
            return value < 0 ? 0 : 1;
        }

        public static double Sigmoid(double value)
        {
            double k = Math.Exp(value);
            return k / (1.0f + k);
        }
    }
}