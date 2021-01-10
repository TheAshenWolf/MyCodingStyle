using System.Collections.Generic;

namespace MachineLearning.NeuralNetwork
{
    public class NeuronLayer
    {
        public int amountOfNeurons;
        public List<Neuron> neurons = new List<Neuron>();

        public NeuronLayer(int amountOfNeurons, int amountOfNeuronInputs)
        {
            this.amountOfNeurons = amountOfNeurons;
            for (int i = 0; i < amountOfNeurons; i++)
            {
                neurons.Add(new Neuron(amountOfNeuronInputs));
            }
        }
    }
}
