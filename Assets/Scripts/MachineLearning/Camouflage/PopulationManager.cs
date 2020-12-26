using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MachineLearning.Camouflage
{
    public class PopulationManager : MonoBehaviour
    {
        public GameObject personPrefab;
        public const int POPULATION_SIZE = 10;
        private List<GameObject> _population = new List<GameObject>();
        public static float elapsed = 0f;

        private float _trialTime = 10;
        private int _generation = 0;


        private void Start()
        {
            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                Vector3 position = new Vector3(Random.Range(-9, 9), Random.Range(-4.5f, 4.5f), 0);
                GameObject gameObject = Instantiate(personPrefab, position, Quaternion.identity);
                Dna dna = gameObject.GetComponent<Dna>();
                dna.Initialize();
                dna.gene.r = Random.Range(0f, 1f);
                dna.gene.g = Random.Range(0f, 1f);
                dna.gene.b = Random.Range(0f, 1f);
                dna.gene.sizeFactor = Random.Range(0.8f, 1.4f);
                _population.Add(gameObject);
                dna.SetColor();
                dna.SetSize();
            }
        }

        private void Update()
        {
            elapsed += Time.deltaTime;

            if (elapsed >= _trialTime)
            {
                BreedNewPopulation();
                elapsed = 0;
            }
        }

        private void BreedNewPopulation()
        {
            List<GameObject> newPopulation = new List<GameObject>();

            List<GameObject> sortedList = _population.OrderBy(person => person.GetComponent<Dna>().lifeLength).ToList();

            _population.Clear();

            for (int i = (int) (sortedList.Count / 2f) - 1; i < sortedList.Count - 1; i++)
            {
                _population.Add(Breed(sortedList[i], sortedList[i + 1]));
                _population.Add(Breed(sortedList[i + 1], sortedList[i]));
            }

            for (int i = 0; i < sortedList.Count; i++)
            {
                Destroy(sortedList[i]);
            }

            _generation++;
        }

        GameObject Breed(GameObject parent1, GameObject parent2)
        {
            Vector3 position = new Vector3(Random.Range(-9, 9), Random.Range(-4.5f, 4.5f), 0);
            GameObject child = Instantiate(personPrefab, position, Quaternion.identity);
            Dna dna1 = parent1.GetComponent<Dna>();
            Dna dna2 = parent2.GetComponent<Dna>();

            Dna childDna = child.GetComponent<Dna>();
            childDna.Initialize();

            childDna.gene.r = Random.Range(0, 10) < 5 ? dna1.gene.r : dna2.gene.r + Random.Range(-.1f, .1f);
            childDna.gene.g = Random.Range(0, 10) < 5 ? dna1.gene.g : dna2.gene.g + Random.Range(-.1f, .1f);
            childDna.gene.b = Random.Range(0, 10) < 5 ? dna1.gene.b : dna2.gene.b + Random.Range(-.1f, .1f);

            float parentAvgSize = (dna1.gene.sizeFactor + dna2.gene.sizeFactor) / 2f;
            
            childDna.gene.sizeFactor = Random.Range(0, 10) < 5 ? dna1.gene.sizeFactor : dna2.gene.sizeFactor + Random.Range(-parentAvgSize, parentAvgSize) / 10f; 
            
            childDna.SetColor();
            childDna.SetSize();
            
            return child;
        }
        
        
        private void OnGUI()
        {
            GUIStyle guiStyle = new GUIStyle
            {
                fontSize = 50,
                normal =
                {
                    textColor = Color.white
                }
            };

            GUI.Label(new Rect(10, 10, 100, 20), "Generation: " + _generation, guiStyle);
            GUI.Label(new Rect(10, 65, 100, 20), "Trial time: " + (int) elapsed, guiStyle);
        }
    }
}