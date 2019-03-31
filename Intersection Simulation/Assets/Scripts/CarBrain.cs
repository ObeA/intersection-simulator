using Intersection;
using UnityEngine;

public class CarBrain : MonoBehaviour
{
    public float lookAhead;
    
    private PathFollower _car;

    private VehicleState _state = VehicleState.Cruising;
    
    // Start is called before the first frame update
    void Start()
    {
        _car = GetComponent<PathFollower>();
        
        if (_car == null)
        {
            enabled = false;
            return;
        } 
        
        _car.ChangeSpeed(_car.topSpeed, _car.topSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out var hitInfo, lookAhead))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hitInfo.distance, Color.yellow);

            var follower = hitInfo.collider.gameObject.GetComponent<PathFollower>();
            if (follower != null)
            {
                HandleHitWithOtherVehicle(follower, hitInfo);
            }
            else if (_state != VehicleState.Stopping)
            {
                _car.ChangeSpeed(0, -_car.CurrentSpeed / (hitInfo.distance / _car.CurrentSpeed));
                _state = VehicleState.Stopping;
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * lookAhead, Color.white);
        }
    }

    private void HandleHitWithOtherVehicle(PathFollower follower, RaycastHit hitInfo)
    {
        if (_state != VehicleState.Decellerating && follower.CurrentSpeed < _car.CurrentSpeed)
        {
            _car.ChangeSpeed(follower.CurrentSpeed, -_car.CurrentSpeed / (hitInfo.distance / _car.CurrentSpeed));
            _state = VehicleState.Decellerating;
        } 
        else if (_state == VehicleState.Decellerating)
        {
            if (Mathf.Abs(follower.CurrentSpeed - _car.CurrentSpeed) < 0.1f)
            {
                _state = VehicleState.Tailing;
                if (Mathf.Approximately(follower.CurrentSpeed, 0))
                {
                    _car.ChangeSpeed(0, -_car.CurrentSpeed / (hitInfo.distance / _car.CurrentSpeed));
                }
                else
                {
                    _car.ChangeSpeed(follower.CurrentSpeed, follower.Acceleration);
                }
            }
        }
    }

    private enum VehicleState
    {
        Cruising,
        Stopping,
        Decellerating,
        Tailing
    }
}
