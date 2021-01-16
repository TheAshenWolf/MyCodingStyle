using System.Collections.Generic;
using System.Linq;
using MachineLearning.NeuralNetwork;
using UnityEngine;

namespace MachineLearning.PlatformBalancing
{
	public class QNetwork{
		private readonly int _amountOfInputs;
		private readonly int _amountOfHiddenLayers;
		private readonly double _learningRate;
		private readonly List<NeuronLayer> _layers = new List<NeuronLayer>();

		public QNetwork(int amountOfInputs, int amountOfOutputs, int amountOfHiddenLayers, int neuronsPerHiddenLayer, double a)
		{
			_amountOfInputs = amountOfInputs;
			int amountOfOutputs1 = amountOfOutputs;
			_amountOfHiddenLayers = amountOfHiddenLayers;
			int neuronsPerHiddenLayer1 = neuronsPerHiddenLayer;
			_learningRate = a;

			if(_amountOfHiddenLayers > 0)
			{
				_layers.Add(new NeuronLayer(neuronsPerHiddenLayer1, _amountOfInputs));

				for(int i = 0; i < _amountOfHiddenLayers-1; i++)
				{
					_layers.Add(new NeuronLayer(neuronsPerHiddenLayer1, neuronsPerHiddenLayer1));
				}

				_layers.Add(new NeuronLayer(amountOfOutputs1, neuronsPerHiddenLayer1));
			}
			else
			{
				_layers.Add(new NeuronLayer(amountOfOutputs1, _amountOfInputs));
			}
		}

		public List<double> Train(List<double> inputValues, List<double> desiredOutput)
		{
			List<double> outputValues = CalcOutput(inputValues, desiredOutput);
			UpdateWeights(outputValues, desiredOutput);
			return outputValues;
		}

		private List<double> CalcOutput(List<double> inputValues, List<double> desiredOutput)
		{
			List<double> outputValues = new List<double>();
			int currentInput = 0;

			if(inputValues.Count != _amountOfInputs)
			{
				Debug.Log("ERROR: Number of Inputs must be " + _amountOfInputs);
				return outputValues;
			}

			List<double> inputs = new List<double>(inputValues);
			for(int i = 0; i < _amountOfHiddenLayers + 1; i++)
			{
				if(i > 0)
				{
					inputs = new List<double>(outputValues);
				}
				outputValues.Clear();

				for(int j = 0; j < _layers[i].amountOfNeurons; j++)
				{
					double dotProduct = 0;
					_layers[i].neurons[j].inputs.Clear();

					for(int k = 0; k < _layers[i].neurons[j].amountOfInputs; k++)
					{
						_layers[i].neurons[j].inputs.Add(inputs[currentInput]);
						dotProduct += _layers[i].neurons[j].weights[k] * inputs[currentInput];
						currentInput++;
					}

					dotProduct -= _layers[i].neurons[j].bias;

					_layers[i].neurons[j].output = i == _amountOfHiddenLayers ? ActivationFunctionO(dotProduct) : ActivationFunction(dotProduct);
					
					outputValues.Add(_layers[i].neurons[j].output);
					currentInput = 0;
				}
			}
			return outputValues;
		}

		public List<double> CalcOutput(List<double> inputValues)
		{
			List<double> outputValues = new List<double>();
			int currentInput = 0;

			if(inputValues.Count != _amountOfInputs)
			{
				Debug.LogError("Number of Inputs must be " + _amountOfInputs);
				return outputValues;
			}

			List<double> inputs = new List<double>(inputValues);
			for(int i = 0; i < _amountOfHiddenLayers + 1; i++)
			{
				if(i > 0)
				{
					inputs = new List<double>(outputValues);
				}
				outputValues.Clear();

				for(int j = 0; j < _layers[i].amountOfNeurons; j++)
				{
					double N = 0;
					_layers[i].neurons[j].inputs.Clear();

					for(int k = 0; k < _layers[i].neurons[j].amountOfInputs; k++)
					{
						_layers[i].neurons[j].inputs.Add(inputs[currentInput]);
						N += _layers[i].neurons[j].weights[k] * inputs[currentInput];
						currentInput++;
					}

					N -= _layers[i].neurons[j].bias;

					_layers[i].neurons[j].output = i == _amountOfHiddenLayers ? ActivationFunctionO(N) : ActivationFunction(N);
					
					outputValues.Add(_layers[i].neurons[j].output);
					currentInput = 0;
				}
			}
			return outputValues;
		}


		public string PrintWeights()
		{
			string weightStr = "";
			foreach (Neuron n in _layers.SelectMany(l => l.neurons))
			{
				weightStr = n.weights.Aggregate(weightStr, (current, w) => current + (w + ","));
				weightStr += n.bias + ",";
			}
			return weightStr;
		}

		public void LoadWeights(string weightStr)
		{
			if(weightStr == "") return;
			string[] weightValues = weightStr.Split(',');
			int w = 0;
			foreach (Neuron n in _layers.SelectMany(l => l.neurons))
			{
				for(int i = 0; i < n.weights.Count; i++)
				{
					n.weights[i] = System.Convert.ToDouble(weightValues[w]);
					w++;
				}
				n.bias = System.Convert.ToDouble(weightValues[w]);
				w++;
			}
		}

		private void UpdateWeights(List<double> outputs, List<double> desiredOutput)
		{
			for(int i = _amountOfHiddenLayers; i >= 0; i--)
			{
				for(int j = 0; j < _layers[i].amountOfNeurons; j++)
				{
					double error;
					if(i == _amountOfHiddenLayers)
					{
						error = desiredOutput[j] - outputs[j];
						_layers[i].neurons[j].errorGradient = outputs[j] * (1-outputs[j]) * error;
					}
					else
					{
						_layers[i].neurons[j].errorGradient = _layers[i].neurons[j].output * (1-_layers[i].neurons[j].output);
						double errorGradSum = 0;
						for(int p = 0; p < _layers[i+1].amountOfNeurons; p++)
						{
							errorGradSum += _layers[i+1].neurons[p].errorGradient * _layers[i+1].neurons[p].weights[j];
						}
						_layers[i].neurons[j].errorGradient *= errorGradSum;
					}	
					for(int k = 0; k < _layers[i].neurons[j].amountOfInputs; k++)
					{
						if(i == _amountOfHiddenLayers)
						{
							error = desiredOutput[j] - outputs[j];
							_layers[i].neurons[j].weights[k] += _learningRate * _layers[i].neurons[j].inputs[k] * error;
						}
						else
						{
							_layers[i].neurons[j].weights[k] += _learningRate * _layers[i].neurons[j].inputs[k] * _layers[i].neurons[j].errorGradient;
						}
					}
					_layers[i].neurons[j].bias += _learningRate * -1 * _layers[i].neurons[j].errorGradient;
				}

			}

		}


		private double ActivationFunction(double value)
		{
			return ActivationFunctions.TanH(value);
		}

		private double ActivationFunctionO(double value)
		{
			return ActivationFunctions.Sigmoid(value);
		}
	}
}
