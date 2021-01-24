using UnityEngine;

namespace MachineLearning.QLearnFlappy
{
    public class BirdState : MonoBehaviour
    {
        public bool isDropped = false;

        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log("collision");
            if (other.gameObject.CompareTag("Dead"))
            {
                isDropped = true;
            }
        }
    }
}
