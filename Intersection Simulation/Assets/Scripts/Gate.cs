using System;
using System.Threading.Tasks;
using Intersection;
using UnityEngine;

public class Gate : CommunicationMonoBehaviour
{
    public GameObject indicatorObject;
    
    private bool _isSubscribed;
    private GateState _state = GateState.Closed;
    public GateState State => _state;

    private async void Start()
    {
        Debug.Log($"Subscribing to {Topic}");
        await EnsureSubscribeRegisteredAsync();
    }

    private async void Update()
    {
        await EnsureSubscribeRegisteredAsync();
    }

    private async Task EnsureSubscribeRegisteredAsync()
    {
        if (_isSubscribed || !communicationsManager.IsInitialized || topic == null) return;
        
        await communicationsManager.Client.SubscribeAsync(Topic,
            state =>
            {
                Debug.Log($"Received state {state} for {Topic}");
                _state = (GateState) int.Parse(state);
            });
        _isSubscribed = true;
    }

    public enum GateState
    {
        Open = 0,
        Closed = 1 
    }
}
