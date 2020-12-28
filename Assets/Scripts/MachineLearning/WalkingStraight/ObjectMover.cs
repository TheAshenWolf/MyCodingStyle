using System;
using UnityEngine;

namespace MachineLearning.WalkingStraight
{
    public class ObjectMover : MonoBehaviour
    {
        public float speed = 1;
        private Vector3 _initPos;

        private void Start()
        {
            _initPos = transform.position;
        }

        private void Update()
        {
            transform.position += Vector3.forward * (Time.deltaTime * speed);
        }

        public void Reset()
        {
            transform.position = _initPos;
        }
    }
}
