using UnityEngine;

namespace MachineLearning.Pong
{
	public class MoveBall : MonoBehaviour {
		private Vector3 _ballStartPosition;
		private Rigidbody2D _localRigidbody2D;
		private const float SPEED = 400;
		public AudioSource blip;
		public AudioSource blop;

		// Use this for initialization
		private void Start () {
			_localRigidbody2D = GetComponent<Rigidbody2D>();
			_ballStartPosition = transform.position;
			ResetBall();
		}

		private void OnCollisionEnter2D(Collision2D col)
		{
			if(col.gameObject.CompareTag("backwall"))
				blop.Play();
			else
				blip.Play();
		}

		public void ResetBall()
		{
			transform.position = _ballStartPosition;
			_localRigidbody2D.velocity = Vector3.zero;
			Vector3 dir = new Vector3(Random.Range(100,300),Random.Range(-100,100),0).normalized;
			_localRigidbody2D.AddForce(dir*SPEED);
		}
	
		// Update is called once per frame
		private void Update () {

			if(Input.GetKeyDown(KeyCode.Space))
			{
				ResetBall();
			}
		}
	}
}
