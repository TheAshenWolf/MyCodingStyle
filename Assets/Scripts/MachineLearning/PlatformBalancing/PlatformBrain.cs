using System.Collections.Generic;
using UnityEngine;

namespace MachineLearning.PlatformBalancing
{
    public abstract class Memory
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

        private float _discount = 0.99f;
        private float _exploreRate = 100f;
        private float _maxExploreRate = 100f;
        private float _minExploreRate = 0.01f;
        private float _exploreDecay = 0.0001f;

        private Vector3 _ballStartPosition;
        private int _failCount = 0;
        private float _tiltSpeed = 0.5f;

        private float _timer = 0;
        private float _maxBalanceTime = 0;
        
        
        
    }
}