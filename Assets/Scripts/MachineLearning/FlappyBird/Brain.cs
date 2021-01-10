using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MachineLearning.FlappyBird
{
    public class Brain : MonoBehaviour
    {
        private const int DNA_LENGTH = 4;

        public Dna dna;
        public GameObject eyes;
        [SerializeField, ReadOnly] private bool alive = true;
        public PopulationManager populationManager;

        private bool _seeTopWall;
        private bool _seeBottomWall;
        public float distanceTravelled;
        [SerializeField] private new Rigidbody2D rigidbody;
        private float _altitude;


        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Dead") || other.gameObject.CompareTag("DownDead") || other.gameObject.CompareTag("TopDead"))
            {
                alive = false;
                GameObject localGameObject;
                (localGameObject = gameObject).SetActive(false);
                localGameObject.name = "Dead!";
                populationManager.died++;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Finish"))
            {
                alive = false;
                distanceTravelled *= 2;
                GameObject localGameObject;
                (localGameObject = gameObject).SetActive(false);
                localGameObject.name = "Passed!";
                populationManager.reachedGoal++;
            }
        }

        public void Init()
        {
            dna = new Dna(DNA_LENGTH, new List<int>() {2, 2, 2, 2});
            alive = true;
        }

        private void Update()
        {
            if (!alive) return;
            _altitude = transform.position.y;

            Vector3 position = eyes.transform.position;
            Vector3 right = eyes.transform.right;
            Debug.DrawRay(position, right * 2, Color.red);


            RaycastHit2D hit = (Physics2D.CircleCast(position, 1, right, 2));
            
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("TopDead"))
                {
                    _seeBottomWall = false;
                    _seeTopWall = true;
                }
                else if (hit.collider.gameObject.CompareTag("DownDead"))
                {
                    _seeTopWall = false;
                    _seeBottomWall = true;
                }
                else
                {
                    _seeBottomWall = false;
                    _seeTopWall = false;
                }
            }
            else
            {
                _seeBottomWall = false;
                _seeTopWall = false;
            }

            MovementType gene;
            if (_seeTopWall)
            {
                gene = (MovementType) dna.GetGene(0);
            }
            else if (_seeBottomWall)
            {
                gene = (MovementType) dna.GetGene(1);
            }
            else
            {
                if (_altitude < 5)
                {
                    gene = (MovementType) dna.GetGene(2);
                }
                else
                {
                    gene = (MovementType) dna.GetGene(3);
                }
                
            }

            switch (gene)
            {
                case MovementType.Thrust:
                    rigidbody.velocity = new Vector2(0,2);
                    break;
                case MovementType.Wait:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            distanceTravelled = transform.position.x - populationManager.transform.position.x;
            transform.Translate(Time.deltaTime, 0, 0);
        }
    }
}