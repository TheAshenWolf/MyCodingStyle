using System.Collections.Generic;
using UnityEngine;

namespace MachineLearning.NeuralNetwork
{
    public class Neuron
    {
        public int amountOfInputs;
        public double bias;
        public double output;
        public double errorGradient;
        public List<double> weights = new List<double>();
        public List<double> inputs = new List<double>();

        public Neuron(int amountOfInputs)
        {
            bias = Random.Range(-1f, 1f);
            this.amountOfInputs = amountOfInputs;

            for (int i = 0; i < amountOfInputs; i++)
            {
                weights.Add(Random.Range(-1f, 1f));
            }
        }
    }
}
