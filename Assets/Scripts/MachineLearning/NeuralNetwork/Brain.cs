using System.Collections.Generic;
using MachineLearning.Perceptron;
using UnityEngine;

namespace MachineLearning.NeuralNetwork
{
    public class Brain : MonoBehaviour
    {
        private const int TESTING_ITERATIONS = 1024;
        private Network _network;
        private double _sumSquareError = 0;

        private void Start()
        {
            _network = new Network(2, 1, 1, 2, 0.8);

            for (int iteration = 0; iteration < TESTING_ITERATIONS; iteration++)
            {
                _sumSquareError = 0;
                /*result = Train(1, 1, 0);
                sumSquareError += Mathf.Pow((float) result[0] - 0, 2);
                result = Train(1, 0, 1);
                sumSquareError += Mathf.Pow((float) result[0] - 1, 2);
                result = Train(0, 1, 1);
                sumSquareError += Mathf.Pow((float) result[0] - 1, 2);
                result = Train(1, 1, 0);
                sumSquareError += Mathf.Pow((float) result[0] - 0, 2);*/

                _sumSquareError = RunTwoToOneLearning(TrainingResources.xor);
            }

            Debug.Log("SumSquareError: <b>" + _sumSquareError + "</b>");

            List<double> result = Execute(1, 1, 0);
            Debug.Log("1, 1: " + result[0]);
            result = Execute(1, 0, 1);
            Debug.Log("1, 0: " + result[0]);
            result = Execute(0, 1, 1);
            Debug.Log("0, 1: " + result[0]);
            result = Execute(0, 0, 0);
            Debug.Log("0, 0: " + result[0]);
        }

        private double RunTwoToOneLearning(TrainingSet[] sets)
        {
            double error = 0;
            
            foreach (TrainingSet set in sets)
            {
                List<double> result = Train(set.inputs[0], set.inputs[1], set.output);
                error =+ Mathf.Pow((float) result[0] - (float)set.output, 2);
            }

            return error;
        }

        private List<double> Train(double input1, double input2, double output)
        {
            return RunThroughNetwork(input1, input2, output, true);
        }

        private List<double> Execute(double input1, double input2, double output)
        {
            return RunThroughNetwork(input1, input2, output, false);
        }

        private List<double> RunThroughNetwork(double input1, double input2, double output, bool updateWeights)
        {
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();
            
            inputs.Add(input1);
            inputs.Add(input2);
            outputs.Add(output);

            return _network.Run(inputs, outputs, updateWeights);
        }
    }
}