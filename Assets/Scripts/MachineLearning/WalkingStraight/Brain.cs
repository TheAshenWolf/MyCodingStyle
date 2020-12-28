using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace MachineLearning.WalkingStraight
{
    [RequireComponent(typeof(ThirdPersonCharacter))]
    public class Brain : MonoBehaviour
    {
        public int dnaLength = 1;
        public float timeAlive;
        public Dna dna;

        private ThirdPersonCharacter _character;
        private Vector3 _move;
        private bool _jump;
        private bool _alive = true;


        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Dead"))
            {
                _alive = false;
            }
        }

        public void Init()
        {
            dna = new Dna(dnaLength, 6);
            _character = GetComponent<ThirdPersonCharacter>();
            timeAlive = 0;
            _alive = true;
        }

        private void FixedUpdate()
        {
            float alongX = 0;
            float alongZ = 0;
            bool crouch = false;

            MovementType movementType = (MovementType) dna.GetGene(0);
            switch (movementType) // This is a mess... but trust me, the tutorial was even worse.
            {
                case MovementType.Forward:
                    alongX = 1;
                    break;
                case MovementType.Back:
                    alongX = -1;
                    break;
                case MovementType.Left:
                    alongZ = -1;
                    break;
                case MovementType.Right:
                    alongZ = 1;
                    break;
                case MovementType.Jump:
                    _jump = true;
                    break;
                case MovementType.Crouch:
                    crouch = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _move = alongX * Vector3.forward + alongZ * Vector3.right;
            _character.Move(_move, crouch, _jump);
            _jump = false;
            if (_alive) timeAlive += Time.deltaTime;
        }
    }
}
