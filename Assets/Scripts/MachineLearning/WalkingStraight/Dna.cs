using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dna
{
    private List<int> genes = new List<int>();
    private int dnaLength = 0;
    private int maxValues = 0;

    public Dna(int length, int values)
    {
        dnaLength = length;
        maxValues = values;
        SetRandom();
    }

    public void SetRandom()
    {
        genes.Clear();
        for (int i = 0; i < dnaLength; i++)
        {
            genes.Add(Random.Range(0, maxValues));
        }
    }

    public void SetInt(int index, int value)
    {
        genes[index] = value;
    }

    public void Combine(Dna dna1, Dna dna2)
    {
        for (int i = 0; i < dnaLength; i++)
        {
            if (i < dnaLength / 2f)
            {
                int gene = dna1.genes[i];
                genes[i] = gene;
            }
            else
            {
                int gene = dna2.genes[i];
                genes[i] = gene;
            }
        }
    }

    public void Mutate()
    {
        genes[Random.Range(0, dnaLength)] = Random.Range(0, maxValues);
    }

    public int GetGene(int index)
    {
        return genes[index];
    }
}
