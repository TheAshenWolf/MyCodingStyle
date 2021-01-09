using System;

namespace MachineLearning.Perceptron
{
    [Serializable]
    public struct TrainingSet
    {
        public double[] inputs;
        public double output;
    }
}