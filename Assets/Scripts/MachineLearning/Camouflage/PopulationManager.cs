using System;
using System.Collections.Generic;
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

        private float _trialTime = 15;
        private int _generation = 0;


        private void Start()
        {
            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                Vector3 position = new Vector3(Random.Range(-9, 9), Random.Range(-4.5f, 4.5f), 0);
                GameObject gameObject = Instantiate(personPrefab, position, Quaternion.identity);
                Dna dna = gameObject.GetComponent<Dna>();
                dna.Initialize();
                Color col = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                Debug.Log(col);
                dna.gene = col;
                _population.Add(gameObject);
                dna.SetColor(dna.gene);
            }
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
            
            GUI.Label(new Rect(10,10,100,20), "Generation: " + _generation, guiStyle );
            GUI.Label(new Rect(10,65,100,20), "Trial time: " + (int)elapsed, guiStyle );
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
            throw new NotImplementedException();
        }
    }
}
