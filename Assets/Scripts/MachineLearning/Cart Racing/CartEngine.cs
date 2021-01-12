using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MachineLearning.Cart_Racing
{
    public class CartEngine : MonoBehaviour
    {
        public float speed = 50.0f;
        public float rotationSpeed = 100.0f;
        public float visibleDistance = 200;
        private List<string> _collectedTrainingData = new List<string>();
        private StreamWriter _trainingDataFile;

        private Transform _transform;
        private Vector3 _position;
        private Vector3 _right;
        private Vector3 _forward;

        private void Start()
        {
            _transform = transform;
            _position = _transform.position;
            _right = _transform.right;
            _forward = _transform.forward;

            string path = Application.dataPath + "/trainingData.txt";
            _trainingDataFile = File.CreateText(path);
        }

        private void OnApplicationQuit()
        {
            foreach (string data in _collectedTrainingData)
            {
                _trainingDataFile.WriteLine(data);
            }

            _trainingDataFile.Close();
        }

        private void Update()
        {
            _position = _transform.position;
            float translationInput = Input.GetAxis("Vertical");
            float rotationInput = Input.GetAxis("Horizontal");

            float translation = Time.deltaTime * speed * translationInput;
            float rotation = Time.deltaTime * rotationSpeed * rotationInput;

            _transform.Translate(0, 0, translation);
            _transform.Rotate(0, rotation, 0);

            Debug.DrawRay(_position, _forward * visibleDistance, Color.red);
            Debug.DrawRay(_position, -_right * visibleDistance, Color.red);
            Debug.DrawRay(_position, _right * visibleDistance, Color.red);
            Debug.DrawRay(_position, (_forward + _right) * visibleDistance, Color.red);
            Debug.DrawRay(_position, (_forward - _right) * visibleDistance, Color.red);

            float frontDistance = 0;
            float leftDistance = 0;
            float rightDistance = 0;
            float frontLeftDistance = 0;
            float frontRightDistance = 0;

            if (Physics.Raycast(_position, _forward, out RaycastHit hit, visibleDistance))
                frontDistance = 1 - Round(hit.distance / visibleDistance);
            if (Physics.Raycast(_position, -_right, out hit, visibleDistance))
                leftDistance = 1 - Round(hit.distance / visibleDistance);
            if (Physics.Raycast(_position, _right, out hit, visibleDistance))
                rightDistance = 1 - Round(hit.distance / visibleDistance);
            if (Physics.Raycast(_position, _forward + _right, out hit, visibleDistance))
                frontLeftDistance = 1 - Round(hit.distance / visibleDistance);
            if (Physics.Raycast(_position, _forward - _right, out hit, visibleDistance))
                frontRightDistance = 1 - Round(hit.distance / visibleDistance);

            string values = frontDistance + "," + leftDistance + "," + rightDistance + "," + frontLeftDistance + "," +
                            frontRightDistance + "," + Round(translationInput) + "," + Round(rotationInput);

            if (!_collectedTrainingData.Contains(values)) _collectedTrainingData.Add(values);
        }

        private static float Round(float x)
        {
            return (float) Math.Round(x, MidpointRounding.AwayFromZero) / 2f;
        }
    }
}