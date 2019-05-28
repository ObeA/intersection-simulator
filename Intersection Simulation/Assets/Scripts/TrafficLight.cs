using System;
using Intersection;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class TrafficLight : CommunicationMonoBehaviour
{
    public Collider blockingObject;
    public MeshRenderer indicatorObject;

    private TrafficLightState _newState;
    private TrafficLightState _state;
    private bool _isSubscribed;
    private float _timeSinceGreen;

    public TrafficLightState State
    {
        get => _state;
        private set
        {
            if (_state == TrafficLightState.Green)
            {
                _timeSinceGreen = Time.time;
            }

            _state = value;
        }
    }

    public float TimeSinceGreen => _timeSinceGreen;

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
        if (_isSubscribed || !IsReady) return;
        
        await CommunicationsManager.Client.SubscribeAsync(Topic,
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