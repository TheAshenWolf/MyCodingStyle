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
        public int died = 0;
        public int reachedGoal = 0;
        private readonly List<GameObject> _population = new List<GameObject>();
        public static float elapsed = 0f;
        public float trialTime = 30f;
        private int _generation = 0;
        private float _topFitness = 9999f;

        private float _maxDistance;

        [SerializeField] private Transform start;
        [SerializeField] private Transform end;


        private void OnGUI()
        {
            GUIStyle guiStyle = new GUIStyle
            {
                fontSize = 25,
                normal =
                {
                    textColor = Color.white
                }
            };


            GUI.Label(new Rect(10, 10, 50, 10), "Generation: " + _generation, guiStyle);
            GUI.Label(new Rect(10, 40, 50, 10), $"Trial time: {elapsed:0.00}", guiStyle);
            GUI.Label(new Rect(10, 70, 50, 10), "Population: " + (int) _population.Count, guiStyle);
            GUI.Label(new Rect(10, 100, 50, 10), "Dead: " + died, guiStyle);
            GUI.Label(new Rect(10, 130, 50, 10), "Reached Goal: " + reachedGoal, guiStyle);
            GUI.Label(new Rect(10, 160, 50, 10), "------------------------", guiStyle);
            GUI.Label(new Rect(10, 190, 50, 10), "Red -> Green = fitness level", guiStyle);
            GUI.Label(new Rect(10, 220, 50, 10), "Blue = Both parents reached goal", guiStyle);
        }

        private void Start()
        {
            _maxDistance = Vector3.Distance(start.position, end.position);

            for (int i = 0; i < populationSize; i++)
            {
                Vector3 position = transform.position;
                Vector3 startingPosition = new Vector3(position.x + Random.Range(-0.5f, 0.5f), position.y,
                    position.z + Random.Range(-0.5f, 0.5f));
                GameObject bot = Instantiate(botPrefab, startingPosition, this.transform.rotation);

                Brain botBrain = bot.GetComponent<Brain>();

                botBrain.averageParentFitness = 0;
                botBrain.Init();
                botBrain.start = start;
                botBrain.end = end;
                botBrain.topFitness = _topFitness;
                botBrain.populationManager = this;
                _population.Add(bot);
            }
        }


        private GameObject Breed(GameObject parent1, GameObject parent2)
        {
            Vector3 position = transform.position;
            Vector3 startingPosition = new Vector3(position.x + Random.Range(-0.5f, 0.5f), position.y,
                position.z + Random.Range(-0.5f, 0.5f));

            GameObject child = Instantiate(botPrefab, startingPosition, this.transform.rotation);
            Brain brain = child.GetComponent<Brain>();

            if (Random.Range(0, 100) == 50)
            {
                brain.averageParentFitness = 0;
                brain.Init();
                brain.start = start;
                brain.end = end;
                brain.dna.Mutate();
            }
            else
            {
                Brain parent1Brain = parent1.GetComponent<Brain>();
                Brain parent2Brain = parent2.GetComponent<Brain>();

                if (Mathf.Approximately(parent1Brain.farthestDistance, _topFitness) &&
                    Mathf.Approximately(parent2Brain.farthestDistance, _topFitness))
                {
                    brain.averageParentFitness = 2;
                }
                else
                {
                    brain.averageParentFitness = Mathf.Clamp01(
                        ((parent1Brain.farthestDistance + parent2Brain.farthestDistance) /
                         2f) / _maxDistance);
                }


                brain.Init();
                brain.start = start;
                brain.end = end;
                brain.dna.Combine(parent1Brain.dna, parent2Brain.dna);
            }

            brain.topFitness = _topFitness;
            brain.populationManager = this;

            return child;
        }

        private void BreedNewPopulation()
        {
            died = 0;
            reachedGoal = 0;
            
            List<GameObject> sortedList =
                _population.OrderBy(bot => bot.GetComponent<Brain>().farthestDistance).ToList();

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
            elapsed += Time.deltaTime;
            if (elapsed >= trialTime)
            {
                BreedNewPopulation();
                elapsed = 0;
            }
        }
    }
}