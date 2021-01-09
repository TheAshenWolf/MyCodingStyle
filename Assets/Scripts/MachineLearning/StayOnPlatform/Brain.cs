using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

namespace MachineLearning.StayOnPlatform
{
    public class Brain : MonoBehaviour
    {
        private const int DNA_LENGTH = 6;


        public Transform start;
        public Transform end;
        public float timeAlive;
        public float timeWalking;
        public Dna dna;
        public GameObject eyes;
        [SerializeField, ReadOnly] private bool _alive = true;
        private bool _seeGround = true;
        private float _previousDistance;
        private float _currentDistance;
        public float farthestDistance = 0;
        public PopulationManager populationManager;
        private int _backtrackingStepsLeft = 0;
        

        
        // Fitness color
        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private Color notFitColor = new Color(1, 0, 0, 1);
        [SerializeField] private Color fullFitColor = new Color(0, 1, 0, 1);
        [SerializeField] private Color bestCandidateColor = new Color(0,0,1,1);
        public float topFitness;
        public float averageParentFitness = 0;
        


        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Dead"))
            {
                farthestDistance = -1;
                _alive = false;
                gameObject.SetActive(false);
                gameObject.name = "Dead!";
                populationManager.died++;
            } 
        }

        private Color GetFitnessColor(float fitness)
        {
            if (Mathf.Approximately(fitness, 2f))
            {
                return bestCandidateColor;
            }

            Color fitnessColor = notFitColor * (1 - fitness) + (fullFitColor * fitness);

            return fitnessColor;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Finish"))
            {
                farthestDistance = topFitness;
                _alive = false;
                gameObject.SetActive(false);
                gameObject.name = "Passed!";
                populationManager.reachedGoal++;
            }
        }

        public void Init()
        {
            bodyRenderer.material.color = GetFitnessColor(averageParentFitness);
            dna = new Dna(DNA_LENGTH, new List<int>() {3, 3, 3, 3, 45, 5});
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
            Vector3 endPos = new Vector3(end.position.x, 0, end.position.z);
            _currentDistance = Vector3.Distance(startPos, endPos) - Vector3.Distance(myPos, endPos);
            farthestDistance = Mathf.Max(farthestDistance, _currentDistance);

            float turn = 0;
            float movement = 0;


            MovementType gene;
            if (_seeGround)
            {
                if (_previousDistance > _currentDistance || _backtrackingStepsLeft != 0)
                {
                    gene = (MovementType) dna.GetGene(0);
                    _backtrackingStepsLeft = Math.Max(0, _backtrackingStepsLeft - 1);
                }
                else
                {
                    gene = (MovementType) dna.GetGene(1);
                    _backtrackingStepsLeft = dna.GetGene(5);
                }
            }
            else
            {
                if (_previousDistance > _currentDistance || _backtrackingStepsLeft != 0)
                {
                    gene = (MovementType) dna.GetGene(2);
                    _backtrackingStepsLeft = Math.Max(0, _backtrackingStepsLeft - 1);
                }
                else
                {
                    gene = (MovementType) dna.GetGene(3);
                    _backtrackingStepsLeft = dna.GetGene(5);
                }
            }

            switch (gene)
            {
                case MovementType.MoveForward:
                    movement = 1;
                    timeWalking += Time.deltaTime;
                    break;
                case MovementType.TurnLeft:
                    turn = -dna.GetGene(4);
                    break;
                case MovementType.TurnRight:
                    turn = dna.GetGene(4);
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