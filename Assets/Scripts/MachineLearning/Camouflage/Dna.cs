using UnityEngine;

namespace MachineLearning.Camouflage
{
    public class Dna : MonoBehaviour
    {

        // color gene
        public Gene gene = new Gene()
        {
            r = 1,
            g = 1,
            b = 1,
            sizeFactor = 1
        };
        public float lifeLength = 999f;
        private SpriteRenderer _spriteRenderer;
        private Collider2D _collider2D;


        private void OnMouseDown()
        {
            lifeLength = PopulationManager.elapsed;
            _spriteRenderer.enabled = false;
            _collider2D.enabled = false;
        }

        public void SetColor()
        {
            _spriteRenderer.color = new Color(gene.r, gene.g, gene.b, 1);
        }

        public void SetSize()
        {
            transform.localScale *= gene.sizeFactor;
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
