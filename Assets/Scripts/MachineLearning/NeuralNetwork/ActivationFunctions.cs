using System;

// ReSharper disable UnusedMember.Global

namespace MachineLearning.NeuralNetwork
{
    public static class ActivationFunctions
    {
        // Useless for Neural network, great for a single perceptron.
        public static double BinaryStep(double value)
        {
            return value < 0 ? 0 : 1;
        }

        // If you need output between (0 , 1)
        public static double Sigmoid(double value)
        {
            double k = Math.Exp(value);
            return k / (1.0f + k);
        }

        // If you need output between (-1, 1)
        public static double TanH(double value)
        {
            return 2 * (Sigmoid(2 * value)) - 1;
        }

        // If you need output between (-Inf, +Inf)
        public static double LeakyReLu(double value)
        {
            return value < 0 ? 0.01 * value : value;
        }
        
        // If you need output between <0, +Inf)
        public static double ReLu(double value)
        {
            return value > 0 ? value : 0;
        }
    }
}