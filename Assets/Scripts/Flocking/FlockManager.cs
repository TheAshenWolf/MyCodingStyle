using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Flocking
{
    public class FlockManager : MonoBehaviour
    {
        [Title("General")]
        [SerializeField] private GameObject fishPrefab;

        [Title("Flock Settings")]
        [SerializeField] public float flockSpawnRadius = 5;
        [SerializeField] private int amountOfFish = 25;
        [Range(0f, 100f)] public float minimumSpeed = 1;
        [Range(0f, 100f)] public float maximumSpeed = 3;
        [Range(.5f, 100f)] public float neighborDistance = 1;
        [Range(0f, 100f)] public float rotationSpeed = 1;
        [Range(0f, 50f)] public float avoidanceDistance = 1;
        public Vector3 goalPosition;
        
        [Title("Others")]
        [ReadOnly] public GameObject[] fish;


        private void Awake()
        {
            fish = new GameObject[amountOfFish];
            goalPosition = transform.position;
        }

        // Start is called before the first frame update
        private void Start()
        {
            SummonFish();
        }

        private void Update()
        {
            if (Random.Range(0, 100) < 1)
            {
                goalPosition = transform.position + GetRandomPointInSpawnRadius();
            }
            
        }


        private void SummonFish()
        {
            for (int i = 0; i < amountOfFish; i++)
            {
                Vector3 fishPosition = transform.position + GetRandomPointInSpawnRadius();
                
                GameObject fishObject = GameObject.Instantiate(fishPrefab, fishPosition, Quaternion.identity);
                fishObject.GetComponent<Fish>().flockManager = this;

                fish[i] = fishObject;

            }
        }

        private Vector3 GetRandomPointInSpawnRadius()
        {
            return new Vector3(Random.Range(-flockSpawnRadius, flockSpawnRadius),
                Random.Range(-flockSpawnRadius, flockSpawnRadius),
                Random.Range(-flockSpawnRadius, flockSpawnRadius));
        }
    }
}