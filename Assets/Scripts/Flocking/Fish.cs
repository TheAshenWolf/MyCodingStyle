using UnityEngine;
using Random = UnityEngine.Random;

namespace Flocking
{
    public class Fish : MonoBehaviour
    {
        // Rules of Flocking:
        // Separation - avoid crowding neighbours (short range repulsion)
        // Alignment - steer towards average heading of neighbours
        // Cohesion - steer towards average position of neighbours (long range attraction)
        // Avoidance - steer away from physical obstacles


        // Public
        public FlockManager flockManager;
        public float speed;

        private void Start()
        {
            speed = Random.Range(flockManager.minimumSpeed, flockManager.maximumSpeed);
        }


        // Update is called once per frame
        private void Update()
        {
            if (!OutsideBoundsBehavior())
            {
                BehaveLikeFishDo();
            }
            MoveForward();
        }

        private void BehaveLikeFishDo()
        {
            SpeedSwitch();
            
            if (Random.Range(0, 100) >= 20) return; 
            GameObject[] fish = flockManager.fish;
            Vector3 center = Vector3.zero;
            Vector3 avoid = Vector3.zero;
            float groupSpeed = 0.01f;
            int groupSize = 0;

            foreach (GameObject individual in fish)
            {
                if (individual != gameObject)
                {
                    float neighborDistance = Vector3.Distance(individual.transform.position, transform.position);
                    if (neighborDistance <= flockManager.neighborDistance)
                    {
                        center += individual.transform.position;
                        groupSize++;

                        if (neighborDistance < flockManager.avoidanceDistance)
                        {
                            avoid = (avoid + (transform.position - individual.transform.position));
                        }

                        Fish anotherFish = individual.GetComponent<Fish>();
                        groupSpeed += anotherFish.speed;
                    }
                }
            }

            if (groupSize > 0)
            {
                center = (center / groupSize) + (flockManager.goalPosition - transform.position);
                speed = Mathf.Clamp(groupSpeed / groupSize, flockManager.minimumSpeed, flockManager.maximumSpeed);

                Vector3 direction = (center + avoid) - transform.position;

                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                        flockManager.rotationSpeed * Time.deltaTime);
                }
            }
        }

        private void MoveForward()
        {
            transform.Translate(0, 0, Time.deltaTime * speed);
        }

        private bool OutsideBoundsBehavior()
        {
            bool turning;
            Vector3 direction = Vector3.zero;
            
            Bounds bounds = new Bounds(flockManager.transform.position,
                Vector3.one * (flockManager.flockSpawnRadius * 2));
            
            if (!bounds.Contains(transform.position))
            {
                direction = flockManager.transform.position - transform.position;;
                turning = true;
            }
            else if (Physics.Raycast(transform.position, transform.forward * 2, out RaycastHit hit))
            {
                turning = true;
                direction = Vector3.Reflect(transform.forward, hit.normal);
            }
            else
            {
                turning = false;
            }
            
            // Debug.DrawRay(transform.position, transform.forward * 5, Color.red);

            if (turning)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                    flockManager.rotationSpeed * Time.deltaTime);
                return true;
            }

            return false;
        }

        private void SpeedSwitch()
        {
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(flockManager.minimumSpeed, flockManager.maximumSpeed);
            }
        }
    }
}