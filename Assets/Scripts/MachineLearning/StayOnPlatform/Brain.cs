using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace MachineLearning.StayOnPlatform
{
    public class Brain : MonoBehaviour
    {
        public int dnaLength = 2;
        public float timeAlive;
        public Dna dna;
        public GameObject eyes;
        private bool _alive = true;
        private bool _seeGround = true;


        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Dead"))
            {
                _alive = false;
                gameObject.SetActive(false);
            }
        }

        public void Init()
        {
            dna = new Dna(dnaLength, 6);
            timeAlive = 0;
            _alive = true;
        }

        private void Update()
        {
            if (!_alive) return;

            Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 10, Color.red, 10);
            _seeGround = false;

            RaycastHit hit;
            if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10, out hit))
            {
                if (hit.collider.gameObject.CompareTag("Platform"))
                {
                    _seeGround = true;
                }
            }

            timeAlive = PopulationManager.elapsed;

            float turn = 0;
            float movement = 0;

            switch (_seeGround ? (MovementType) dna.GetGene(0) : (MovementType) dna.GetGene(1))
            {
                case MovementType.MoveForward:
                    movement = 1;
                    break;
                case MovementType.TurnLeft:
                    turn = -90;
                    break;
                case MovementType.TurnRight:
                    turn = 90;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            this.transform.Translate(0, 0, movement * 0.1f);
            this.transform.Rotate(0, turn, 0);
        }
    }
}