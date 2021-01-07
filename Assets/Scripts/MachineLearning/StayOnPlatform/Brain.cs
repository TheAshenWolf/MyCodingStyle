using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace MachineLearning.StayOnPlatform
{
    public class Brain : MonoBehaviour
    {
        private const int DNA_LENGTH = 4;
        public Transform start;
        public float timeAlive;
        public float timeWalking;
        public Dna dna;
        public GameObject eyes;
        [SerializeField, ReadOnly] private bool _alive = true;
        private bool _seeGround = true;
        private float _previousDistance;
        private float _currentDistance;
        public float farthestDistance = 0;


        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Dead"))
            {
                farthestDistance = -1;
                _alive = false;
                gameObject.SetActive(false);
                gameObject.name = "Dead!";
            } 
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Finish"))
            {
                farthestDistance = 9999f;
                _alive = false;
                gameObject.SetActive(false);
                gameObject.name = "Passed!";
            }
        }

        public void Init()
        {
            dna = new Dna(DNA_LENGTH, new List<int>() {3, 3, 3, 3});
            timeAlive = 0;
            _alive = true;
        }

        private void Update()
        {
            if (!_alive) return;

            Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 2, Color.red);
            _seeGround = false;

            if (Physics.SphereCast(eyes.transform.position, 0.1f, eyes.transform.forward * 2, out RaycastHit hit))
            {
                if (hit.collider.gameObject.CompareTag("Platform") || hit.collider.gameObject.CompareTag("Player"))
                {
                    _seeGround = true;
                }
            }

            timeAlive = PopulationManager.elapsed;
            Vector3 myPos = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 startPos = new Vector3(start.position.x, 0, start.position.z);
            _currentDistance = Vector3.Distance(myPos, startPos);
            farthestDistance = Mathf.Max(farthestDistance, _currentDistance);

            float turn = 0;
            float movement = 0;


            MovementType gene;
            if (_seeGround)
            {
                if (_previousDistance > _currentDistance) gene = (MovementType) dna.GetGene(0);
                else gene = (MovementType) dna.GetGene(1);
            }
            else
            {
                if (_previousDistance > _currentDistance) gene = (MovementType) dna.GetGene(2);
                else gene = (MovementType) dna.GetGene(3);
            }

            switch (gene)
            {
                case MovementType.MoveForward:
                    movement = 1;
                    timeWalking += Time.deltaTime;
                    break;
                case MovementType.TurnLeft:
                    turn = -5;
                    break;
                case MovementType.TurnRight:
                    turn = 5;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _previousDistance = _currentDistance;

            this.transform.Translate(0, 0, movement * Time.deltaTime * 10f);
            this.transform.Rotate(0, turn, 0);
        }
    }
}