using System;
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
        private MovementType _lastMovement;
        public float farthestDistance = 0;


        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Dead"))
            {
                _alive = false;
                gameObject.SetActive(false);
            } 
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Finish"))
            {
                farthestDistance = 9999f;
                _alive = false;
                gameObject.SetActive(false);
            }
        }

        public void Init()
        {
            dna = new Dna(DNA_LENGTH, 3);
            timeAlive = 0;
            _alive = true;
        }

        private void Update()
        {
            if (!_alive) return;

            Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 10, Color.red);
            _seeGround = false;

            if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10, out RaycastHit hit))
            {
                if (hit.collider.gameObject.CompareTag("Platform") || hit.collider.gameObject.CompareTag("Player"))
                {
                    _seeGround = true;
                }
            }

            timeAlive = PopulationManager.elapsed;
            Vector3 myPos = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 startPos = new Vector3(start.position.x, 0, start.position.z);
            farthestDistance = Mathf.Max(farthestDistance, Vector3.Distance(myPos, startPos));

            float turn = 0;
            float movement = 0;


            MovementType gene;
            if (_seeGround)
            {
                gene = (MovementType) dna.GetGene(0);
            }
            else
            {
                switch (_lastMovement)
                {
                    case MovementType.TurnLeft:
                        gene = (MovementType) dna.GetGene(2);
                        break;
                    case MovementType.TurnRight:
                        gene = (MovementType) dna.GetGene(3);
                        break;
                    default:
                        gene = (MovementType) dna.GetGene(1);
                        break;
                }
            }

            switch (gene)
            {
                case MovementType.MoveForward:
                    movement = 1;
                    timeWalking += Time.deltaTime;
                    break;
                case MovementType.TurnLeft:
                    turn = -90;
                    _lastMovement = MovementType.TurnLeft;
                    break;
                case MovementType.TurnRight:
                    _lastMovement = MovementType.TurnRight;
                    turn = 90;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.transform.Translate(0, 0, movement * Time.deltaTime * 10f);
            this.transform.Rotate(0, turn, 0);
        }
    }
}