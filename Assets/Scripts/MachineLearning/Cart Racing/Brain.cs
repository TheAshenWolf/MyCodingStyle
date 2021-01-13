using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MachineLearning.Perceptron;
using UnityEngine;

namespace MachineLearning.Cart_Racing
{
    public class Brain : MonoBehaviour
    {
        private Network _network;
        private const int TESTING_ITERATIONS = 1024;

        private double _sumSquareError = 0;
        private bool _trainingDone = false;
        private float _trainingProgress = 0;
        private double _lastSumSquareError = 1;

        public float translation;
        public float rotation;


        private void Start()
        {
            _network = new Network(5, 2, 1, 10, 0.5);
            StartCoroutine(LoadTrainingSet());
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(25, 25, 250, 30), "SSE: " + _lastSumSquareError);
            GUI.Label(new Rect(25, 40, 250, 30), "Alpha: " + _network.learningRate);
            GUI.Label(new Rect(25, 55, 250, 30), "Trained: " + _trainingProgress);
        }

        private IEnumerator LoadTrainingSet()
        {
            string path = Application.dataPath + "/trainingData.txt";
            if (File.Exists(path))
            {
                int lineCount = File.ReadAllLines(path).Length;
                StreamReader streamReader = File.OpenText(path);
                List<double> inputs = new List<double>();
                List<double> outputs = new List<double>();

                for (int epoch = 0; epoch < TESTING_ITERATIONS; epoch++)
                {
                    _sumSquareError = 0;
                    streamReader.BaseStream.Position = 0;
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] data = line.Split(',');
                        float thisError = 0;

                        double[] doubleData = data.Select(Convert.ToDouble).ToArray();

                        if (!Mathf.Approximately((float) Convert.ToDouble(data[5]), 0) &&
                            !Mathf.Approximately((float) Convert.ToDouble(data[6]), 0))
                        {
                            inputs.Clear();
                            outputs.Clear();
                            inputs.AddRange(new[]
                            {
                                doubleData[0],
                                doubleData[1],
                                doubleData[2],
                                doubleData[3],
                                doubleData[4]
                            });

                            outputs.Add(TheAshenWolf.RepetitiveStatics.Map((float)doubleData[5], 0, 1, -1, 1));
                            outputs.Add(TheAshenWolf.RepetitiveStatics.Map((float)doubleData[6], 0, 1, -1, 1));

                            List<double> calculatedOutputs = _network.Train(inputs, outputs);
                            thisError = (Mathf.Pow((float) (outputs[0] - calculatedOutputs[0]), 2) +
                                         Mathf.Pow((float) (outputs[1] - calculatedOutputs[1]), 2)) / 2f;
                        }

                        _sumSquareError += thisError;

                    }

                    _trainingProgress = epoch / (float) TESTING_ITERATIONS;
                    _sumSquareError /= lineCount;
                    _lastSumSquareError = _sumSquareError;

                    yield return null;
                }
            }

            _trainingDone = true;
        }
    }
}