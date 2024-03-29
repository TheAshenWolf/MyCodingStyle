﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MachineLearning.WalkingStraight
{
    public class PopulationManager : MonoBehaviour
    {
        public GameObject botPrefab;
        public int populationSize = 50;
        private readonly List<GameObject> _population = new List<GameObject>();
        private static float _elapsed;
        public float trialTime = 10f;
        private int _generation;

        [SerializeField] private ObjectMover objectMover;

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
            GUI.Label(new Rect(10, 65, 100, 20), "Trial time: " + (int) _elapsed, guiStyle);
            GUI.Label(new Rect(10, 120, 100, 20), "Population: " + _population.Count, guiStyle);
        }

        private void Start()
        {
            for (int i = 0; i < populationSize; i++)
            {
                Vector3 position = transform.position;
                Vector3 startingPosition = new Vector3(position.x + Random.Range(-2, 2), position.y,
                    position.z + Random.Range(-2, 2));
                GameObject bot = Instantiate(botPrefab, startingPosition, transform.rotation);
                bot.GetComponent<Brain>().Init();
                _population.Add(bot);
            }
        }


        private GameObject Breed(GameObject parent1, GameObject parent2)
        {
            Vector3 position = transform.position;
            Vector3 startingPosition = new Vector3(position.x + Random.Range(-2, 2), position.y,
                position.z + Random.Range(-2, 2));

            GameObject child = Instantiate(botPrefab, startingPosition, transform.rotation);
            Brain brain = child.GetComponent<Brain>();

            if (Random.Range(0, 100) == 50)
            {
                brain.Init();
                brain.dna.Mutate();
            }
            else
            {
                brain.Init();
                brain.dna.Combine(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);
            }

            return child;
        }

        private void BreedNewPopulation()
        {
            List<GameObject> sortedList = _population.OrderBy(bot => bot.GetComponent<Brain>().timeAlive).ToList();

            _population.Clear();
            for (int i = Mathf.FloorToInt(sortedList.Count / 2f) - 1; i < sortedList.Count - 1; i++)
            {
                Debug.Log("Breeding");
                _population.Add(Breed(sortedList[i], sortedList[i + 1]));
                _population.Add(Breed(sortedList[i + 1], sortedList[i]));
            }

            foreach (GameObject bot in sortedList)
            {
                Destroy(bot);
            }

            _generation++;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            if (_elapsed >= trialTime)
            {
                BreedNewPopulation();
                objectMover.Reset();
                _elapsed = 0;
            }
        }
    }
}