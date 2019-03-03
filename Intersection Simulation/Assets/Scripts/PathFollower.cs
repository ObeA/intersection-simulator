using System;
using System.Linq;
using PathCreation;
using UnityEngine;
using Random = System.Random;

namespace Intersection
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        public float scale = 1.0f;

        private float _speed;
        private float _distanceTravelled;
        private Collider _lastCollision;

        private void Start()
        {
            _speed = speed;
        }

        void Update()
        {
            if (pathCreator == null) return;
            if (_lastCollision == null || !_lastCollision.enabled)
            {
                _distanceTravelled += _speed * Time.deltaTime * scale;
                _lastCollision = null;
            }

            var targetPosition = pathCreator.path.GetPointAtDistance(_distanceTravelled, endOfPathInstruction);
            var targetRotation = pathCreator.path.GetRotationAtDistance(_distanceTravelled, endOfPathInstruction);

            var transform1 = transform;
            transform1.position = targetPosition;
            transform1.rotation = targetRotation;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.GetComponentInParent<TrafficLight>() == null) return;
            
            _lastCollision = other.collider;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            _speed = speed * (float) (new Random().NextDouble() * (1.5 - 0.5) + 0.5);
        }

        private void OnCollisionExit(Collision other)
        {
            _lastCollision = null;
        }
    }
}