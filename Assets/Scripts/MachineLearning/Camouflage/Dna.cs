using UnityEngine;

namespace MachineLearning.Camouflage
{
    public class Dna : MonoBehaviour
    {
    
        // color gene
        public Color gene = Color.white;
        private bool _dead = false;
        public float lifeLength;
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider2D;


        private void OnMouseDown()
        {
            _dead = true;
            lifeLength = PopulationManager.elapsed;
            _spriteRenderer.enabled = false;
            _collider2D.enabled = false;
        }

        public void SetColor(Color color)
        {
            _spriteRenderer.color = color;
        }
        
        public void Initialize()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            Debug.Log(_spriteRenderer);
            _collider2D = GetComponent<Collider2D>();
            Debug.Log(_collider2D);
        }
    }
}
