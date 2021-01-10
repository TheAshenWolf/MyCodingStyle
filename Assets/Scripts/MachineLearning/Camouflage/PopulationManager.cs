using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MachineLearning.Camouflage
{
    public class PopulationManager : MonoBehaviour
    {
        public GameObject personPrefab;
        private const int POPULATION_SIZE = 10;
        private readonly List<GameObject> _population = new List<GameObject>();
        public static float elapsed;

        private const float TRIAL_TIME = 10;
        private int _generation;


        private void Start()
        {
            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                Vector3 position = new Vector3(Random.Range(-9, 9), Random.Range(-4.5f, 4.5f), 0);
                GameObject npc = Instantiate(personPrefab, position, Quaternion.identity);
                Dna dna = npc.GetComponent<Dna>();
                dna.Initialize();
                dna.gene.r = Random.Range(0f, 1f);
                dna.gene.g = Random.Range(0f, 1f);
                dna.gene.b = Random.Range(0f, 1f);
                dna.gene.sizeFactor = Random.Range(0.8f, 1.4f);
                _population.Add(npc);
                dna.SetColor();
                dna.SetSize();
            }
        }

        private void Update()
        {
            elapsed += Time.deltaTime;

            if (elapsed >= TRIAL_TIME)
            {
                BreedNewPopulation();
                elapsed = 0;
            }
        }

        private void BreedNewPopulation()
        {
            List<GameObject> sortedList = _population.OrderBy(person => person.GetComponent<Dna>().lifeLength).ToList();

            _population.Clear();

            for (int i = (int) (sortedList.Count / 2f) - 1; i < sortedList.Count - 1; i++)
            {
                _population.Add(Breed(sortedList[i], sortedList[i + 1]));
                _population.Add(Breed(sortedList[i + 1], sortedList[i]));
            }

            foreach (GameObject item in sortedList)
            {
                Destroy(item);
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