using System;
using PathCreation;
using UnityEngine;

namespace Intersection
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public float topSpeed = 8;

        private float _targetSpeed;
        private float _previousSpeed;
        private float _currentSpeed;
        
        private float _distanceTravelled;
        private float _acceleration;
        
        public float CurrentSpeed => _currentSpeed;
        public float Acceleration => _acceleration;
        public float DistanceTravelled => _distanceTravelled;

        private void Start()
        {
            enabled = pathCreator != null;

            var t = transform;
            t.position = pathCreator.path.GetPointAtDistance(0);
            t.rotation = pathCreator.path.GetRotationAtDistance(0) * Quaternion.Euler(0, 0, 90);
        }

        private void FixedUpdate()
        {
            if (_distanceTravelled >= pathCreator.path.length)
            {
                Destroy(gameObject);
            }
            
            UpdateSpeed();

            _distanceTravelled += _currentSpeed * Time.fixedDeltaTime;
            
            var targetPosition = pathCreator.path.GetPointAtDistance(_distanceTravelled, EndOfPathInstruction.Stop);
            var targetPosition1 = pathCreator.path.GetPointAtDistance(Mathf.Min(_distanceTravelled + 0.1f, pathCreator.path.length), EndOfPathInstruction.Stop);
            var t = transform;
            t.LookAt(targetPosition1);
            t.position = targetPosition;
        }

        private void UpdateSpeed()
        {
            if (Mathf.Approximately(_currentSpeed, _targetSpeed))
            {
                return;
            }
            
            _currentSpeed = _currentSpeed + _acceleration * Time.fixedDeltaTime;
            if (_previousSpeed > _targetSpeed)
            {
                _currentSpeed = Mathf.Max(_currentSpeed, _targetSpeed);
            }
            else if (_previousSpeed < _targetSpeed)
            {
                _currentSpeed = Mathf.Min(_currentSpeed, _targetSpeed);
            }

            _currentSpeed = Mathf.Max(_currentSpeed, 0);

            //Debug.Log($"{name}: {prev} -> {_currentSpeed} (d:{_currentSpeed - prev} a:{_acceleration} a:{(_currentSpeed - prev) / Time.fixedDeltaTime} t:{Time.fixedDeltaTime})");
        }

        public void ChangeSpeed(float newSpeed, float acceleration)
        {
            acceleration = Mathf.Abs(acceleration);
            if ((Mathf.Approximately(newSpeed, _targetSpeed) && Mathf.Approximately(acceleration, _acceleration)) || acceleration < 0.001f)
            {
                return;
            }
            
            //Debug.Log($"{name}: Changed speed {_targetSpeed} -> {newSpeed} ({acceleration}m/s*s {(newSpeed - _targetSpeed) / acceleration} sec)");
            
            _previousSpeed = _currentSpeed;
            _acceleration = newSpeed > _currentSpeed ? acceleration : -acceleration;
            _targetSpeed = newSpeed;
        }

        public void ForceSpeed(float newSpeed)
        {
            _previousSpeed = _currentSpeed;
            _currentSpeed = _targetSpeed = newSpeed;
        }
    }
}