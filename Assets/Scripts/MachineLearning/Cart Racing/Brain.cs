using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using MachineLearning.Perceptron;
using UnityEngine;

namespace MachineLearning.Cart_Racing
{
    public class Brain : MonoBehaviour
    {
        private Network _network;
        private const int TESTING_ITERATIONS = 4096;

        private double _sumSquareError = 0;
        private bool _trainingDone = false;
        private float _trainingProgress = 0;
        private double _lastSumSquareError = 1;

        public float translation;
        public float rotation;
        
        private Transform _transform;
        private Vector3 _position;
        private Vector3 _right;
        private Vector3 _forward;

        [SerializeField] private bool loadFile;

        private void Start()
        {
            _transform = transform;
            _position = _transform.position;
            _right = _transform.right;
            _forward = _transform.forward;
            
            _network = new Network(5, 2, 1, 10, 0.5);
            if (loadFile) LoadWeightsFromFile();
            else StartCoroutine(LoadTrainingSet());
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

                    string currentWeights = _network.GetWeights();
                    
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        _position = _transform.position;
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

                            outputs.Add(TheAshenWolf.RepetitiveStatics.Map((float)doubleData[5], -1, 1, 0, 1));
                            outputs.Add(TheAshenWolf.RepetitiveStatics.Map((float)doubleData[6], -1, 1, 0, 1));

                            List<double> calculatedOutputs = _network.Train(inputs, outputs);
                            thisError = (Mathf.Pow((float) (outputs[0] - calculatedOutputs[0]), 2) +
                                         Mathf.Pow((float) (outputs[1] - calculatedOutputs[1]), 2)) / 2f;
                        }

                        _sumSquareError += thisError;

                    }

                    _trainingProgress = epoch / (float) TESTING_ITERATIONS;
                    _sumSquareError /= lineCount;

                    if (_lastSumSquareError < _sumSquareError)
                    {
                        _network.LoadWeightsFromString(currentWeights);
                        _network.learningRate = Mathf.Clamp((float) _network.learningRate - 0.001f, 0.1f, 0.9f);
                    }
                    else
                    {
                        _network.learningRate = Mathf.Clamp((float) _network.learningRate + 0.001f, 0.1f, 0.9f);
                        _lastSumSquareError = _sumSquareError;
                    }

                    if (epoch % 4 == 0) yield return null;
                }
            }

            _trainingDone = true;
            _trainingProgress = 1;
            SaveWeightsToFile();
        }

        private void Update()
        {
            _position = _transform.position;
            _right = _transform.right;
            _forward = _transform.forward;
            
            Debug.DrawRay(_position, _forward * CartEngine.VISIBLE_DISTANCE, Color.red);
            Debug.DrawRay(_position, -_right * CartEngine.VISIBLE_DISTANCE, Color.red);
            Debug.DrawRay(_position, _right * CartEngine.VISIBLE_DISTANCE, Color.red);
            Debug.DrawRay(_position, (_forward + _right) * CartEngine.VISIBLE_DISTANCE, Color.red);
            Debug.DrawRay(_position, (_forward - _right) * CartEngine.VISIBLE_DISTANCE, Color.red);
            
            
            if (!_trainingDone) return;

            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();

            float frontDistance = 0; 
            float leftDistance = 0;
            float rightDistance = 0;
            float frontLeftDistance = 0;
            float frontRightDistance = 0;

            if (Physics.Raycast(_position, _forward, out RaycastHit hit, CartEngine.VISIBLE_DISTANCE))
                frontDistance = 1 - CartEngine.Round(hit.distance / CartEngine.VISIBLE_DISTANCE);
            if (Physics.Raycast(_position, -_right, out hit, CartEngine.VISIBLE_DISTANCE))
                leftDistance = 1 - CartEngine.Round(hit.distance / CartEngine.VISIBLE_DISTANCE);
            if (Physics.Raycast(_position, _right, out hit, CartEngine.VISIBLE_DISTANCE))
                rightDistance = 1 - CartEngine.Round(hit.distance / CartEngine.VISIBLE_DISTANCE);
            if (Physics.Raycast(_position, _forward - _right, out hit, CartEngine.VISIBLE_DISTANCE))
                frontLeftDistance = 1 - CartEngine.Round(hit.distance / CartEngine.VISIBLE_DISTANCE);
            if (Physics.Raycast(_position, _forward + _right, out hit, CartEngine.VISIBLE_DISTANCE))
                frontRightDistance = 1 - CartEngine.Round(hit.distance / CartEngine.VISIBLE_DISTANCE);
            
            inputs.AddRange(new double[] {frontDistance, leftDistance, rightDistance, frontLeftDistance, frontRightDistance});
            outputs.AddRange(new double[] {0,0});
            List<double> calculatedOutputs = _network.Execute(inputs, outputs);
            
            float translationInput = TheAshenWolf.RepetitiveStatics.Map((float) calculatedOutputs[0], 0, 1, -1, 1);
            float rotationInput = TheAshenWolf.RepetitiveStatics.Map((float) calculatedOutputs[1], 0, 1, -1, 1);
            
            translation = translationInput * CartEngine.SPEED * Time.deltaTime;
            rotation = rotationInput * CartEngine.ROTATION_SPEED * Time.deltaTime;
            
            _transform.Translate(0,0,translation);
            _transform.Rotate(0, rotation, 0);
        }

        private void SaveWeightsToFile()
        {
            string path = Application.dataPath + "/weightsKart.txt";
            if (File.Exists(path))
            {
                StreamReader streamReader = File.OpenText(path);
                string line = streamReader.ReadLine();
                if (Convert.ToDouble(line) < _sumSquareError) return;
                streamReader.Close();
            }
            
            StreamWriter streamWriter = File.CreateText(path);
            streamWriter.WriteLine(_sumSquareError);
            streamWriter.WriteLine(_network.GetWeights());
            streamWriter.Close();
        }

        private void LoadWeightsFromFile()
        {
            string path = Application.dataPath + "/weightsKart.txt";
            StreamReader streamReader = File.OpenText(path);

            if (File.Exists(path))
            {
                _sumSquareError = Convert.ToDouble(streamReader.ReadLine());
                string line = streamReader.ReadLine();
                _network.LoadWeightsFromString(line);
                _trainingDone = true;
                streamReader.Close();
            }
            else
            {
                throw new Exception("File does not exist.");
            }
        }
    }
}