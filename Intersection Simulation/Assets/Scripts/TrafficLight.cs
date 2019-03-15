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
    public string Topic => ParentLogicGroup != null 
        ? ParentLogicGroup.Topic + topic 
        : topic;

    private TrafficLightState _state;
    private bool _isSubscribed;

    private async void Start()
    {
        await EnsureSubscribeRegisteredAsync();
    }

    private async void Update()
    {
        await EnsureSubscribeRegisteredAsync();
        
        blockingObject.enabled = _state != TrafficLightState.Green;
        indicatorObject.material.color = GetColorByState(_state);
    }

    private async Task EnsureSubscribeRegisteredAsync()
    {
        if (_isSubscribed || communicationsManager == null || !communicationsManager.IsInitialized || topic == null) return;
        
        await communicationsManager.Client.SubscribeAsync(Topic,
            state =>
            {
                Debug.Log($"Received state {state} for {Topic}");
                _state = (TrafficLightState) int.Parse(state);
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
            default:
                return Color.red;
        }
    }

    public enum TrafficLightState
    {
        Red = 0,
        Transitioning = 1,
        Green = 2
    }
}