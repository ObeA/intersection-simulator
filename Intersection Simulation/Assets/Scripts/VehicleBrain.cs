using Intersection;
using PathCreation;
using UnityEngine;

public class VehicleBrain : MonoBehaviour
{
    public float lookAhead;
    
    private PathFollower _vehicle;
    private PathFollower Vehicle => _vehicle != null ? _vehicle : _vehicle = GetComponent<PathFollower>();

    private VehicleState _state = VehicleState.Cruising;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vehicle == null)
        {
            return; // TODO: remove this hack
        }

        if (HandleInDirection(transform.forward))
        {
            return;
        }
        
        var nextPoint = Vehicle.pathCreator.path.GetPointAtDistance(Vehicle.DistanceTravelled + lookAhead * 1.5f, EndOfPathInstruction.Stop);
        var currentPoint = Vehicle.transform.position;
        var curvatureVector = (nextPoint - currentPoint).normalized;
        var dot = Vector3.Dot(curvatureVector, transform.forward);
        
        if (HandleInDirection(Quaternion.Euler(0, 10, 0) * transform.forward) || HandleInDirection(curvatureVector))
        {
            return;
        }
        
        Debug.DrawRay(currentPoint, transform.forward * lookAhead * 1.5f, Color.white);
        Debug.DrawRay(currentPoint, Quaternion.Euler(0, 10, 0) * transform.forward * lookAhead * 1.5f, Color.cyan);
        Debug.DrawRay(currentPoint, curvatureVector * lookAhead * 1.5f, Color.blue);
        
        Vehicle.ChangeSpeed(Vehicle.topSpeed * dot, Vehicle.topSpeed);
        _state = VehicleState.Cruising;
    }

    private bool HandleInDirection(Vector3 direction)
    {
        if (Physics.Raycast(transform.position, direction, out var hitInfo, lookAhead * 1.5f, -5, QueryTriggerInteraction.Ignore))
        {
            Debug.DrawRay(transform.position, direction * hitInfo.distance, Color.yellow);

            var otherFollower = hitInfo.collider.gameObject.GetComponent<PathFollower>();
            if (otherFollower != null)
            {
                HandleHitWithOtherVehicle(otherFollower, hitInfo);
                return true;
            }

            var trafficLight = hitInfo.collider.gameObject.GetComponentInParent<TrafficLight>();
            if (trafficLight != null && HandleCollisionWithTrafficLight(trafficLight, hitInfo))
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

    private void HandleHitWithOtherVehicle(PathFollower follower, RaycastHit hitInfo)
    {
        if (hitInfo.distance < lookAhead * 0.5f && (follower.Acceleration <= 0 || Mathf.Approximately(follower.CurrentSpeed, 0)))
        {
            Vehicle.ForceSpeed(follower.CurrentSpeed);
        }
        else if (_vehicle.CurrentSpeed > follower.CurrentSpeed)
        {
            Vehicle.ChangeSpeed(follower.CurrentSpeed, -Vehicle.CurrentSpeed / (hitInfo.distance / Vehicle.CurrentSpeed));
        }
        else if (hitInfo.distance > lookAhead)
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

    private bool HandleCollisionWithTrafficLight(TrafficLight trafficLight, RaycastHit hitInfo)
    {
        switch (trafficLight.State)
        {
            case TrafficLight.TrafficLightState.Green:
                return false;
            case TrafficLight.TrafficLightState.Red:
            case TrafficLight.TrafficLightState.Transitioning when (Time.time - trafficLight.LastStateChange) > 2:
                if (hitInfo.distance < lookAhead * 0.5f)
                {
                    Vehicle.ForceSpeed(0);
                }
                else
                {
                    Vehicle.ChangeSpeed(0, -Vehicle.CurrentSpeed / (Mathf.Max(hitInfo.distance, lookAhead) / Vehicle.CurrentSpeed));
                }

                _state = VehicleState.Waiting;
                return true;
            case TrafficLight.TrafficLightState.Transitioning:
                Vehicle.ChangeSpeed(Vehicle.topSpeed * 1.1f, Vehicle.topSpeed * 2);
                return true;
        }

        return false;
    }

    private enum VehicleState
    {
        Cruising,
        Stopping,
        Tailing,
        Waiting
    }
}
