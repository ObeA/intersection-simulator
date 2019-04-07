using System;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class TrafficLight : MonoBehaviour
{
    public CommunicationsManager communicationsManager;
    public string topic;
    public Collider blockingObject;
    public MeshRenderer indicatorObject;
    
    private LogicGroup _logicGroup;
    private bool _hasProbed = false;
    public LogicGroup ParentLogicGroup
    {
        get
        {
            if (_hasProbed)
            {
                return _logicGroup;
            }

            _hasProbed = true;
            return _logicGroup != null ? _logicGroup : _logicGroup = (transform.parent != null ? transform.parent.GetComponent<LogicGroup>() : null);
        }
    }
    public string Topic => ParentLogicGroup != null && ParentLogicGroup.Topic != null ? $"{ParentLogicGroup.Topic}{topic}" : topic;

    private TrafficLightState _newState;
    private TrafficLightState _state;
    private bool _isSubscribed;
    private float _lastStateChange;

    public TrafficLightState State
    {
        get => _state;
        private set
        {
            _lastStateChange = Time.time;
            _state = value;
        }
    }

    public float LastStateChange => _lastStateChange;

    private async void Start()
    {
        Debug.Log($"Subscribing to {Topic}");
        await EnsureSubscribeRegisteredAsync();
    }

    private async void Update()
    {
        await EnsureSubscribeRegisteredAsync();

        if (State != _newState)
        {
            State = _newState;
        }
        
        indicatorObject.material.color = GetColorByState(State);
    }

    private async Task EnsureSubscribeRegisteredAsync()
    {
        if (_isSubscribed || communicationsManager == null || !communicationsManager.IsInitialized || topic == null) return;
        
        await communicationsManager.Client.SubscribeAsync(Topic,
            state =>
            {
                Debug.Log($"Received state {state} for {Topic}");
                _newState = (TrafficLightState) int.Parse(state);
            });
        _isSubscribed = true;
    }

    private Color GetColorByState(TrafficLightState state)
    {
        switch (state)
        {
            case TrafficLightState.Transitioning:
                return new Color(1, 0.5f, 0);
            case TrafficLightState.Green:
                return Color.green;
            case TrafficLightState.Red:
                return Color.red;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public enum TrafficLightState
    {
        Red = 0,
        Transitioning = 1,
        Green = 2
    }
}