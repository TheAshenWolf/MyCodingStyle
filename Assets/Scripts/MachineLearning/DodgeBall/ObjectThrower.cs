using System;
using UnityEngine;

namespace MachineLearning.DodgeBall
{
    public class ObjectThrower : MonoBehaviour
    {
        [SerializeField] private GameObject spherePrefab;
        [SerializeField] private GameObject cubePrefab;

        [SerializeField] private Material greenMaterial;
        [SerializeField] private Material redMaterial;

        private Perceptron _perceptron;

        private void Start()
        {
            _perceptron = GetComponent<Perceptron>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ThrowObject(spherePrefab, redMaterial);
                _perceptron.SendInput(0, 0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                ThrowObject(spherePrefab, greenMaterial);
                _perceptron.SendInput(0, 1, 0);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                ThrowObject(cubePrefab, redMaterial);
                _perceptron.SendInput(1, 0, 0);
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                ThrowObject(cubePrefab, greenMaterial);
                _perceptron.SendInput(1, 1, 0);
            }
        }

        private void ThrowObject(GameObject prefab, Material material)
        {
            if (Camera.main != null)
            {
                Transform cameraTransform = Camera.main.transform;
                GameObject item = Instantiate(prefab, cameraTransform.position, cameraTransform.rotation);
                item.GetComponent<Renderer>().material = material;
                item.GetComponent<Rigidbody>().AddForce(0, 0, 500);
            }
        }
    }
}