using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineInternal;
using UnityStandardAssets.Vehicles.Ball;
using Random = UnityEngine.Random;

namespace MachineLearning.PlatformBalancing
{
    public class Memory
    {
        public List<double> states;
        public double reward;

        public Memory(double platformXRotation, double ballZPosition, double ballXVelocity, double reward)
        {
            states = new List<double> {platformXRotation, ballZPosition, ballXVelocity};
            this.reward = reward;
        }
    }

    public class PlatformBrain : MonoBehaviour
    {
        public GameObject ball;

        private QNetwork _qNetwork;

        private float _reward = 0f;
        private List<Memory> _memories = new List<Memory>();
        private const int MEMORY_CAPACITY = 10000;

        private const float DISCOUNT = 0.99f;
        private float _exploreRate = 100f;
        private const float MAX_EXPLORE_RATE = 100f;
        private const float MIN_EXPLORE_RATE = 0.01f;
        private const float EXPLORE_DECAY = 0.001f;

        private Vector3 _ballStartPosition;
        private int _failCount = 0;
        private const float TILT_SPEED = 0.5f;

        private float _timer = 0;
        private float _maxBalanceTime = 0;

        private GUIStyle _guiStyle = new GUIStyle();


        private void Start()
        {
            _qNetwork = new QNetwork(3, 2, 1, 6, 0.2f);
            _ballStartPosition = ball.transform.position;
            Time.timeScale = 5f;

            _guiStyle.fontSize = 12;
            _guiStyle.normal.textColor = Color.white;
        }


        private void OnGUI()
        {
            GUI.BeginGroup(new Rect(10, 10, 300, 150));
            GUI.Box(new Rect(0, 0, 150, 150), "Stats", _guiStyle);
            GUI.Label(new Rect(10, 25, 500, 30), "Fails: " + _failCount, _guiStyle);
            GUI.Label(new Rect(10, 40, 500, 30), "Randomness chance: " + _exploreRate, _guiStyle);
            GUI.Label(new Rect(10, 55, 500, 30), "Last Best Balance: " + _maxBalanceTime, _guiStyle);
            GUI.Label(new Rect(10, 70, 500, 30), "This balance: " + _timer, _guiStyle);
            GUI.EndGroup();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) ResetBall();
        }

        private void FixedUpdate()
        {
            _timer += Time.deltaTime;
            List<double> states = new List<double>();
            List<double> qualities = new List<double>();

            states.Add(this.transform.rotation.x);
            states.Add(ball.transform.position.z);
            states.Add(ball.GetComponent<Rigidbody>().angularVelocity.x);

            qualities = SoftMax(_qNetwork.CalcOutput(states));
            double maxQuality = qualities.Max();
            int maxQualityIndex = qualities.ToList().IndexOf(maxQuality);
            _exploreRate = Mathf.Clamp(_exploreRate - EXPLORE_DECAY, MIN_EXPLORE_RATE, MAX_EXPLORE_RATE);

            if (Random.Range(0, 100) < _exploreRate)
            {
                maxQualityIndex = Random.Range(0, 2);
            }

            transform.Rotate(Vector3.right,
                (maxQualityIndex == 0 ? TILT_SPEED : -TILT_SPEED) * (float) qualities[maxQualityIndex]);

            _reward = ball.GetComponent<BallState>().isDropped ? -1 : 0.1f;
            
            Memory lastMemory = new Memory(transform.rotation.x, ball.transform.position.z, ball.GetComponent<Rigidbody>().angularVelocity.x, _reward);

            if (_memories.Count > MEMORY_CAPACITY)
            {
                _memories.RemoveAt(0);
            }
            
            _memories.Add(lastMemory);

            if (ball.GetComponent<BallState>().isDropped)
            {
                for (int i = _memories.Count - 1; i >= 0; i--)
                {
                    List<double> outputsOld = new List<double>();
                    List<double> outputsNew = new List<double>();
                    outputsOld = SoftMax(_qNetwork.CalcOutput(_memories[i].states));

                    double maxOldQuality = outputsOld.Max();
                    int action = outputsOld.ToList().IndexOf(maxOldQuality);

                    double feedback;
                    if (i == _memories.Count - 1 || _memories[i].reward == -1) feedback = _memories[i].reward;
                    else
                    {
                        outputsNew = SoftMax(_qNetwork.CalcOutput(_memories[i + 1].states));
                        maxQuality = outputsNew.Max();
                        feedback = _memories[i].reward + DISCOUNT * maxQuality;
                    }

                    outputsOld[action] = feedback;
                    _qNetwork.Train(_memories[i].states, outputsOld);
                }

                if (_timer > _maxBalanceTime)
                {
                    _maxBalanceTime = _timer;
                }

                _timer = 0;

                ball.GetComponent<BallState>().isDropped = false;
                transform.rotation = Quaternion.identity;
                ResetBall();
                _memories.Clear();
                _failCount++;
            }
        }

        private void ResetBall()
        {
            ball.transform.position = _ballStartPosition;
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        private static List<double> SoftMax(List<double> values)
        {
            double max = values.Max();

            float scale = values.Sum(t => Mathf.Exp((float) (t - max)));

            return values.Select(t => Mathf.Exp((float) (t - max)) / scale).Select(dummy => (double) dummy).ToList();
        }
    }
}