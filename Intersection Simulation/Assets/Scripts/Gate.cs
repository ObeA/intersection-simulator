using System.Threading.Tasks;
using Intersection;
using UnityEngine;

public class Gate : CommunicationMonoBehaviour
{
    public Animator beamAnimator;
    public GameObject blocker;
    
    private bool _isSubscribed;
    private GateState _newState;
    private GateState _state = GateState.Closed;
    private static readonly int Open = Animator.StringToHash("Open");
    public GateState State => _state;

    private async void Start()
    {
        Debug.Log($"Subscribing to {Topic}");
        await EnsureSubscribeRegisteredAsync();
    }

    private async void Update()
    {
        await EnsureSubscribeRegisteredAsync();

        if (_state == _newState)
        {
            return;
        }
        
        _state = _newState;
        beamAnimator.SetBool(Open, _state == GateState.Open);
        if (blocker != null)
        {
            blocker.SetActive(_state == GateState.Closed);
        }
    }

    private async Task EnsureSubscribeRegisteredAsync()
    {
        if (_isSubscribed || !communicationsManager.IsInitialized || topic == null) return;
        
        await communicationsManager.Client.SubscribeAsync(Topic,
            state =>
            {
                Debug.Log($"Received state {state} for {Topic}");
                _newState = (GateState) int.Parse(state);
            });
        _isSubscribed = true;
    }

    public enum GateState
    {
        Open = 0,
        Closed = 1 
    }
}
