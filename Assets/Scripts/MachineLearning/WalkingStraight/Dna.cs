using System.Collections.Generic;
using UnityEngine;

namespace MachineLearning.WalkingStraight
{
    public class Dna
    {
        private readonly List<int> _genes = new List<int>();
        private readonly int _dnaLength;
        private readonly int _maxValues;

        public Dna(int length, int values)
        {
            _dnaLength = length;
            _maxValues = values;
            SetRandom();
        }

        private void SetRandom()
        {
            _genes.Clear();
            for (int i = 0; i < _dnaLength; i++)
            {
                _genes.Add(Random.Range(0, _maxValues));
            }
        }

        public void Combine(Dna dna1, Dna dna2)
        {
            for (int i = 0; i < _dnaLength; i++)
            {
                if (i < _dnaLength / 2f)
                {
                    int gene = dna1._genes[i];
                    _genes[i] = gene;
                }
                else
                {
                    int gene = dna2._genes[i];
                    _genes[i] = gene;
                }
            }
        }

        public void Mutate()
        {
            _genes[Random.Range(0, _dnaLength)] = Random.Range(0, _maxValues);
        }

        public int GetGene(int index)
        {
            return _genes[index];
        }
    }
}
