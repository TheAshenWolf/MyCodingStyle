using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MachineLearning.Perceptron;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MachineLearning.DodgeBall
{
    public class Perceptron : MonoBehaviour
    {
        [SerializeField] private List<TrainingSet> trainingSets = new List<TrainingSet>();

        private readonly double[] _weights = {0, 0};
        private double _bias;
        private double _totalError;
        private bool _dataLoaded;

        public GameObject ethan;
        private static readonly int Crouch = Animator.StringToHash("Crouch");

        private void Start()
        {
            _dataLoaded = LoadWeights();
            if (!_dataLoaded) InitializeWeights();
        }

        private bool LoadWeights()
        {
            string path = Application.dataPath + "/weights.txt";
            if (File.Exists(path))
            {
                StreamReader streamReader = File.OpenText(path);
                string line = streamReader.ReadLine();
                if (line == null) return false;
                string[] import = line.Split(',');
                _weights[0] = Convert.ToDouble(import[0]);
                _weights[1] = Convert.ToDouble(import[1]);
                _bias = Convert.ToDouble(import[2]);

                Debug.Log("Loaded.");
                return true;
            }

            return false;
        }

        private void SaveWeights()
        {
            string path = Application.dataPath + "/weights.txt";
            StreamWriter streamWriter = File.CreateText(path);
            streamWriter.WriteLine(_weights[0] + "," + _weights[1] + "," + _bias);
            streamWriter.Close();
        }

        private void InitializeWeights()
        {
            for (int i = 0; i < _weights.Length; i++)
            {
                _weights[i] = Random.Range(-1.0f, 1.0f);
            }

            _bias = Random.Range(-1.0f, 1.0f);
        }

        public void SendInput(double shape, double color, double duck)
        {
            double result = CalculateOutput(shape, color);

            if (Mathf.Approximately((float) result, 1))
            {
                ethan.GetComponent<Animator>().SetTrigger(Crouch);
                ethan.GetComponent<Rigidbody>().isKinematic = false;
            }
            else
            {
                ethan.GetComponent<Rigidbody>().isKinematic = true;
            }

            TrainingSet set = new TrainingSet()
            {
                inputs = new[] {shape, color},
                output = duck
            };

            trainingSets.Add(set);
            Train();
        }

        private void Train() // Not the ChooChoo one.
        {
            if (_dataLoaded) return;
            
            _totalError = 0;
            for (int set = 0; set < trainingSets.Count; set++)
            {
                UpdateWeights(set);
                Debug.Log("Weight 1: <b>" + _weights[0] + "</b>; Weight 2: <b>" + _weights[1] + "</b>; Bias: <b>" +
                          _bias + "</b>");
            }
        }

        private double ActivationFunction(double dotProduct)
        {
            return dotProduct > 0 ? 1 : 0;
        }

        private double DotProduct(double[] weights, double[] inputs)
        {
            if (weights == null || inputs == null) return -1;
            if (weights.Length != inputs.Length) return -1;

            double dotProduct = weights.Select((t, i) => t * inputs[i]).Sum();

            dotProduct += _bias;

            return dotProduct;
        }

        private double CalculateOutput(int set) // Training calculation
        {
            return ActivationFunction(DotProduct(_weights, trainingSets[set].inputs));
        }

        private double CalculateOutput(double input1, double input2) // Execution calculation
        {
            double[] inputs = new double[] {input1, input2};
            return ActivationFunction(DotProduct(_weights, inputs));
        }

        private void UpdateWeights(int set)
        {
            double error = trainingSets[set].output - CalculateOutput(set);
            _totalError += Mathf.Abs((float) error);
            for (int i = 0; i < _weights.Length; i++)
            {
                _weights[i] = _weights[i] + error * trainingSets[set].inputs[i];
            }

            _bias += error;
        }

        private void OnApplicationQuit()
        {
            SaveWeights();
            Debug.Log("Saved data.");
        }
    }
}