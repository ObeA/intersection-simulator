using Intersection;
using PathCreation;
using UnityEngine;

public class VehicleBrain : MonoBehaviour
{
    public float lookAhead;
    
    private PathFollower _vehicle;
    private PathFollower Vehicle => _vehicle != null ? _vehicle : _vehicle = GetComponent<PathFollower>();

    private VehicleState _state = VehicleState.Cruising;
    private Bounds _bounds;
    private float _distanceFromCenterToSide;
    private float _distanceFromCenterToFront;

    private float _farFactor = 1.5f;
    private float _closeFactor = 0.75f;
    private float _farLookAhead;
    private float _closeLookAhead;

    void Start()
    {
        _bounds = GetComponent<Collider>().bounds;
        _distanceFromCenterToSide = _bounds.size.y * 0.5f;
        _distanceFromCenterToFront = _bounds.size.z * 0.5f;

        _farLookAhead = lookAhead * _farFactor;
        _closeLookAhead = lookAhead * _closeFactor;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vehicle == null)
        {
            return; // TODO: remove this hack
        }

        var t = transform;
        var right = t.right;
        var left = -right;
        var forward = t.forward;
        var center = t.position;
        var front = center + _distanceFromCenterToFront * forward;
        
        var nextPoint = Vehicle.pathCreator.path.GetPointAtDistance(Vehicle.DistanceTravelled + _farLookAhead, EndOfPathInstruction.Stop);
        var curvatureVector = (nextPoint - front).normalized;
        var dot = Vector3.Dot(curvatureVector, transform.forward);
        
        var curveRotation = Quaternion.FromToRotation(forward, curvatureVector);
        var forwardSideScanDirection = curveRotation * curveRotation * forward;
        //var leftFront = front + _distanceFromCenterToSide * left;
        var rightFront = center + _distanceFromCenterToSide * right;
        
        if (HandleInDirection(rightFront, forwardSideScanDirection, _farLookAhead)
            || HandleInDirection(front, forwardSideScanDirection, _farLookAhead))
        {
            return;
        }
        
        //Debug.DrawRay(leftFront, forwardSideScanDirection, Color.green);
        Debug.DrawRay(rightFront, forwardSideScanDirection * _farLookAhead, Color.green);
        //Debug.DrawRay(front, transform.forward * _farLookAhead, Color.white);
        Debug.DrawRay(front, forwardSideScanDirection * _farLookAhead, Color.blue);
        
        Vehicle.ChangeSpeed(Mathf.Max(Vehicle.topSpeed * dot * dot, Vehicle.topSpeed * 0.1f), Vehicle.topSpeed);
        _state = VehicleState.Cruising;
    }

    private bool HandleInDirection(Vector3 position, Vector3 direction, float distance)
    {
        if (Physics.Raycast(position, direction, out var hitInfo, distance * _farFactor, -5, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(position, direction * hitInfo.distance, Color.yellow);

            var otherFollower = hitInfo.collider.gameObject.GetComponent<PathFollower>();
            if (hitInfo.collider.gameObject == gameObject)
            {
                return false;
            }
            
            if (otherFollower != null)
            {
                HandleHitWithOtherVehicle(otherFollower, hitInfo, distance);
                return true;
            }

            var trafficLight = hitInfo.collider.gameObject.GetComponentInParent<TrafficLight>();
            if (trafficLight != null && HandleCollisionWithTrafficLight(trafficLight, hitInfo, distance))
            {
                return true;
            }

            var gate = hitInfo.collider.gameObject.GetComponentInParent<Gate>();
            if (gate != null && HandleCollisionWithGate(gate, hitInfo, distance))
            {
                return true;
            }
        }
        
        return false;
    }

    private void HandleCloseCollision(RaycastHit hitInfo)
    {
        Vehicle.ForceSpeed(0);
        _state = VehicleState.Stopping;
    }

    private void HandleHitWithOtherVehicle(PathFollower follower, RaycastHit hitInfo, float distance)
    {
        if (hitInfo.distance < distance * 0.5f && (follower.Acceleration <= 0 || Mathf.Approximately(follower.CurrentSpeed, 0)))
        {
            Vehicle.ForceSpeed(follower.CurrentSpeed);
        }
        else if (_vehicle.CurrentSpeed > follower.CurrentSpeed)
        {
            Vehicle.ChangeSpeed(follower.CurrentSpeed, -Vehicle.CurrentSpeed / (hitInfo.distance / Vehicle.CurrentSpeed));
        }
        else if (hitInfo.distance > distance)
        {
            Vehicle.ChangeSpeed(Mathf.Min(follower.CurrentSpeed, Vehicle.topSpeed), Mathf.Min(follower.Acceleration, Vehicle.topSpeed));
        }
        else if (hitInfo.distance > 0.5f)
        {
            Vehicle.ChangeSpeed(Vehicle.CurrentSpeed / 2, Vehicle.CurrentSpeed);
        }
        else
        {
            Vehicle.ForceSpeed(0);
        }

        _state = VehicleState.Tailing;
    }

    private bool HandleCollisionWithTrafficLight(TrafficLight trafficLight, RaycastHit hitInfo, float distance)
    {
        switch (trafficLight.State)
        {
            case TrafficLight.TrafficLightState.Green:
                return false;
            case TrafficLight.TrafficLightState.Red:
            case TrafficLight.TrafficLightState.Transitioning when (Time.time - trafficLight.TimeSinceGreen) > 2:
                if (hitInfo.distance < distance * 0.5f)
                {
                    Vehicle.ForceSpeed(0);
                }
                else
                {
                    Vehicle.ChangeSpeed(0, -Vehicle.CurrentSpeed / (Mathf.Max(hitInfo.distance, distance) / Vehicle.CurrentSpeed));
                }

                _state = VehicleState.Waiting;
                return true;
            case TrafficLight.TrafficLightState.Transitioning:
                Vehicle.ChangeSpeed(Vehicle.topSpeed * 1.1f, Vehicle.topSpeed * 2);
                return true;
        }

        return false;
    }

    private bool HandleCollisionWithGate(Gate gate, RaycastHit hitInfo, float distance)
    {
        if (gate.State == Gate.GateState.Open)
        {
            return false;
        }
        
        if (hitInfo.distance < distance * 0.5f)
        {
            Vehicle.ForceSpeed(0);
        }
        else
        {
            Vehicle.ChangeSpeed(0, -Vehicle.CurrentSpeed / (Mathf.Max(hitInfo.distance, distance) / Vehicle.CurrentSpeed));
        }
        _state = VehicleState.Waiting;
        
        return true;
    }

    private class Eyes
    {
        public Vector3 left;
        public Vector3 right;

        public Eyes(Bounds bounds)
        {
            left = bounds.min + new Vector3(bounds.size.x, 0);
            right = bounds.max;
        }
    }

    private enum VehicleState
    {
        Cruising,
        Stopping,
        Tailing,
        Waiting
    }
}
