using System.Collections.Generic;
using MachineLearning.Perceptron;
using UnityEngine;

namespace MachineLearning.Cart_Racing
{
    public class Brain : MonoBehaviour
    {
        private const int TESTING_ITERATIONS = 1024;
        private Network _network;
        private double _sumSquareError;

        private void Start()
        {
            _network = new Network(2, 1, 1, 2, 0.8);

            for (int iteration = 0; iteration < TESTING_ITERATIONS; iteration++)
            {
                _sumSquareError = 0;

                _sumSquareError = RunTwoToOneLearning(TrainingResources.xor);
            }

            Debug.Log("SumSquareError: <b>" + _sumSquareError + "</b>");

            List<double> result = _network.Execute(1, 1, 0);
            Debug.Log("1, 1: " + result[0]);
            result = _network.Execute(1, 0, 1);
            Debug.Log("1, 0: " + result[0]);
            result = _network.Execute(0, 1, 1);
            Debug.Log("0, 1: " + result[0]);
            result = _network.Execute(0, 0, 0);
            Debug.Log("0, 0: " + result[0]);
        }

        private double RunTwoToOneLearning(TrainingSet[] sets)
        {
            double error = 0;
            
            foreach (TrainingSet set in sets)
            {
                List<double> result = _network.Train(set.inputs[0], set.inputs[1], set.output);
                error =+ Mathf.Pow((float) result[0] - (float)set.output, 2);
            }

            return error;
        }
    }
}