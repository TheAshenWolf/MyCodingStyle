using System;
using UnityEngine;

namespace MachineLearning.PlatformBalancing
{
    public class BallState : MonoBehaviour
    {
        public bool isDropped = false;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Dead"))
            {
                isDropped = true;
            }
        }
    }
}
