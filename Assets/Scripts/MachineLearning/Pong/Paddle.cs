using System;
using System.Collections.Generic;
using UnityEngine;

namespace MachineLearning.Pong
{
    public class Paddle : MonoBehaviour
    {
        public GameObject paddle;
        public GameObject ball;
        private Rigidbody2D _ballRigidBody;
        private float _yVelocity;
        private const float MAX_Y = 17.4f;
        private const float MIN_Y = 8.8f;

        private const float MAX_SPEED = 5;
        // private float _amountSaved = 0;
        // private float _amountMissed = 0;

        private Network _network;

        private void Start()
        {
            _network = new Network(6, 1, 1, 4, 0.01);
            _ballRigidBody = ball.GetComponent<Rigidbody2D>();
        }

        private List<double> Run(double ballX, double ballY, double ballVelocityX, double ballVelocityY, double paddleX,
            double paddleY, double paddleVelocity, bool train = true)
        {
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();

            inputs.AddRange(new List<double> {ballX, ballY, ballVelocityX, ballVelocityY, paddleX, paddleY});
            outputs.Add(paddleVelocity);

            return train ? _network.Train(inputs, outputs) : _network.Execute(inputs, outputs);
        }

        private void Update()
        {
            Vector3 position = paddle.transform.position;
            float positionY = Mathf.Clamp(transform.position.y + (_yVelocity * Time.deltaTime * MAX_SPEED),
                MIN_Y, MAX_Y);

            position = new Vector3(position.x, positionY, position.z);

            paddle.transform.position = position;

            const int layerMask = 1 << 9;
            RaycastHit2D hit = Physics2D.Raycast(ball.transform.position, _ballRigidBody.velocity, 1000, layerMask);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("tops"))
                {
                    Vector3 reflection = Vector3.Reflect(_ballRigidBody.velocity, hit.normal);
                    hit = Physics2D.Raycast(hit.point, reflection, 1000, layerMask);
                }

                if (hit.collider != null && hit.collider.gameObject.CompareTag("backwall"))
                {
                    Vector3 paddlePosition = paddle.transform.position;
                    float differenceY = hit.point.y - paddlePosition.y;

                    Vector2 ballVelocity = _ballRigidBody.velocity;
                    Vector3 ballPosition = ball.transform.position;

                    List<double> output = Run(ballPosition.x,
                        ballPosition.y,
                        ballVelocity.x,
                        ballVelocity.y,
                        paddlePosition.x,
                        paddlePosition.y,
                        differenceY);

                    _yVelocity = (float) output[0];
                }
            }
            else
            {
                _yVelocity = 0;
            }
        }
    }
}