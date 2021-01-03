using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MachineLearning.StayOnPlatform
{
    public class PopulationManager : MonoBehaviour
    {
        public GameObject botPrefab;
        public int populationSize = 50;
        private List<GameObject> population = new List<GameObject>();
        public static float elapsed = 0f;
        public float trialTime = 10f;
        private int _generation = 0;

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
            GUI.Label(new Rect(10, 65, 100, 20), string.Format("Trial time: {0:0.00}",elapsed),  guiStyle);
            GUI.Label(new Rect(10, 120, 100, 20), "Population: " + (int) population.Count, guiStyle);
        }

        private void Start()
        {
            for (int i = 0; i < populationSize; i++)
            {
                Vector3 position = transform.position;
                Vector3 startingPosition = new Vector3(position.x + Random.Range(-2, 2), position.y,
                    position.z + Random.Range(-2, 2));
                GameObject bot = Instantiate(botPrefab, startingPosition, this.transform.rotation);
                bot.GetComponent<Brain>().Init();
                population.Add(bot);
            }
        }


        private GameObject Breed(GameObject parent1, GameObject parent2)
        {
            Vector3 position = transform.position;
            Vector3 startingPosition = new Vector3(position.x + Random.Range(-2, 2), position.y,
                position.z + Random.Range(-2, 2));

            GameObject child = Instantiate(botPrefab, startingPosition, this.transform.rotation);
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
            List<GameObject> sortedList = population.OrderBy(bot => bot.GetComponent<Brain>().timeAlive + bot.GetComponent<Brain>().timeWalking).ToList();

            population.Clear();
            for (int i = Mathf.FloorToInt(sortedList.Count / 2f) - 1; i < sortedList.Count - 1; i++)
            {
                Debug.Log("Breedin'");
                population.Add(Breed(sortedList[i], sortedList[i + 1]));
                population.Add(Breed(sortedList[i + 1], sortedList[i]));
            }

            foreach (GameObject bot in sortedList)
            {
                Destroy(bot);
            }

            _generation++;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            if (elapsed >= trialTime)
            {
                BreedNewPopulation();
                elapsed = 0;
            }
        }
    }
}