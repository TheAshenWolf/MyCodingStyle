using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MachineLearning.Perceptron
{
    public class Perceptron : MonoBehaviour
    {
        [SerializeField] private TrainingSet[] trainingSets;

        private readonly double[] _weights = {0, 0};
        private double _bias;
        private double _totalError;

        public SimpleGrapher simpleGrapher;

        private void Start()
        {
            DrawAllPoints();
            Train(16);
            simpleGrapher.DrawRay((float) (-(_bias / _weights[1]) / (_bias / _weights[0])),
                (float) (-_bias / _weights[1]), Color.cyan);
            
            /*Debug.Log("==========");
            Debug.Log("Test 0, 0 (0): " + CalculateOutput(0, 0));
            Debug.Log("Test 0, 1 (1): " + CalculateOutput(0, 1));
            Debug.Log("Test 1, 0 (1): " + CalculateOutput(1, 0));
            Debug.Log("Test 1, 1 (1): " + CalculateOutput(1, 1));*/

            RunGraphingTestCase(0.3, 0.9);
            RunGraphingTestCase(0.7, 0.2);
            RunGraphingTestCase(0.2, 0.4);
            RunGraphingTestCase(0, 0.6);
            RunGraphingTestCase(0.7, 0.1);
        }

        private void RunGraphingTestCase(double sharpness, double edibility)
        {
            simpleGrapher.DrawPoint((float) sharpness, (float) edibility,
                Mathf.Approximately((float) CalculateOutput(sharpness, edibility), 0) ? Color.magenta : Color.yellow);
        }

        private void InitializeWeights()
        {
            for (int i = 0; i < _weights.Length; i++)
            {
                _weights[i] = Random.Range(-1.0f, 1.0f);
            }

            _bias = Random.Range(-1.0f, 1.0f);
        }

        private void Train(int epochs)
        {
            InitializeWeights();

            for (int epoch = 0; epoch < epochs; epoch++)
            {
                Debug.Log("---------- <b>EPOCH " + epoch + "</b> ----------");
                _totalError = 0;
                for (int set = 0; set < trainingSets.Length; set++)
                {
                    UpdateWeights(set);
                    Debug.Log("Weight 1: <b>" + _weights[0] + "</b>; Weight 2: <b>" + _weights[1] + "</b>; Bias: <b>" +
                              _bias + "</b>");
                }

                Debug.Log("Total Error: <b>" + _totalError + "</b>");
            }
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
            double dotProduct = DotProduct(_weights, trainingSets[set].inputs);
            return dotProduct > 0 ? 1 : 0;
        }

        private double CalculateOutput(double input1, double input2) // Execution calculation
        {
            double[] inputs = new[] {input1, input2};
            double dotProduct = DotProduct(_weights, inputs);
            return dotProduct > 0 ? 1 : 0;
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

        private void DrawAllPoints()
        {
            for (int set = 0; set < trainingSets.Length; set++)
            {
                simpleGrapher.DrawPoint((float) trainingSets[set].inputs[0], (float) trainingSets[set].inputs[1],
                    Mathf.Approximately((float) trainingSets[set].output, 0) ? Color.red : Color.green);
            }
        }
    }
}