/*Original Code: http://wiki.unity3d.com/index.php?title=MouseOrbitZoom*/
/*Modified by Penny de Byl on 8 Aug 2017. 
  to Zoom with scroll, orbit with ALT and Pan with Q
*/

// TheAshenWolf: This script was written by Penny de Byl and should be ignored in case of "code examination". It is just used to make my life easier within the game.

using UnityEngine;

namespace VoxelWorld
{
    public class CameraController : MonoBehaviour
    {
        public Transform target;
        public Vector3 targetOffset;
        public float distance = 5.0f;
        public float maxDistance = 100;
        public float minDistance = .6f;
        public float xSpeed = 200.0f;
        public float ySpeed = 200.0f;
        public int yMinLimit = -80;
        public int yMaxLimit = 80;
        public int zoomRate = 40;
        public float panSpeed = 0.3f;
        public float zoomDampening = 5.0f;
 
        private float _xDeg;
        private float _yDeg;
        private float _currentDistance;
        private float _desiredDistance;
        private Quaternion _currentRotation;
        private Quaternion _desiredRotation;
        private Quaternion _rotation;
        private Vector3 _position;

        private void Start() { Init(); }

        private void Init()
        {
            GameObject go = new GameObject("Fake Cam Target");
            Transform localTransform = transform;
            Vector3 position = localTransform.position;
            Quaternion rotation = localTransform.rotation;
            
            go.transform.position = position + (localTransform.forward * distance);
            target = go.transform;
 
            distance = Vector3.Distance(position, target.position);
            _currentDistance = distance;
            _desiredDistance = distance;
            
            _position = position;
            
            _rotation = rotation;
            _currentRotation = rotation;
            _desiredRotation = rotation;
 
            _xDeg = Vector3.Angle(Vector3.right, localTransform.right );
            _yDeg = Vector3.Angle(Vector3.up, transform.up );
        }
 
        /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled. 
     */
        private void LateUpdate()
        {
            // If Control and Alt and Middle button? ZOOM!
            if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
            {
                _desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate*0.125f * Mathf.Abs(_desiredDistance);
            }
            // If middle mouse and left alt are selected? ORBIT
            else if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt))
            {
                _xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                _yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
 
                ////////OrbitAngle
 
                //Clamp the vertical axis for the orbit
                _yDeg = ClampAngle(_yDeg, yMinLimit, yMaxLimit);
                // set camera rotation 
                _desiredRotation = Quaternion.Euler(_yDeg, _xDeg, 0);
                _currentRotation = transform.rotation;
 
                _rotation = Quaternion.Lerp(_currentRotation, _desiredRotation, Time.deltaTime * zoomDampening);
                transform.rotation = _rotation;
            }
            else if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.Q))
            {
                //grab the rotation of the camera so we can move in a pseudo local XY space
                target.rotation = transform.rotation;
                target.Translate(Vector3.right * (-Input.GetAxis("Mouse X") * panSpeed));
                target.Translate(transform.up * (-Input.GetAxis("Mouse Y") * panSpeed), Space.World);
            }
 
            ////////Orbit Position
            
            _desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(_desiredDistance);
            _desiredDistance = Mathf.Clamp(_desiredDistance, minDistance, maxDistance);
            _currentDistance = Mathf.Lerp(_currentDistance, _desiredDistance, Time.deltaTime * zoomDampening);
 
            // calculate position based on the new currentDistance 
            _position = target.position - (_rotation * Vector3.forward * _currentDistance + targetOffset);
            transform.position = _position;
        }
 
        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}