using System.Threading.Tasks;
using Intersection;
using UnityEngine;

public class Deck : CommunicationMonoBehaviour
{
    private DeckState _state = DeckState.Closed;
    private DeckState _newState = DeckState.Closed;
    private Animator _animator;
    private bool _isSubscribed;
    private static readonly int Open = Animator.StringToHash("Open");

    // Start is called before the first frame update
    private async void Start()
    {
        _newState = _state;
        _animator = GetComponentInParent<Animator>();
        
        Debug.Log($"Subscribing to {Topic}");
        await EnsureSubscribeRegisteredAsync();
    }

    // Update is called once per frame
    private async void Update()
    {
        await EnsureSubscribeRegisteredAsync();

        if (_state == _newState)
        {
            return;
        }
        
        _state = _newState;
        _animator.SetBool(Open, _state == DeckState.Open);
    }
    
    private async Task EnsureSubscribeRegisteredAsync()
    {
        if (_isSubscribed || !communicationsManager.IsInitialized || topic == null) return;
        
        await communicationsManager.Client.SubscribeAsync(Topic,
            state =>
            {
                Debug.Log($"Received state {state} for {Topic}");
                _newState = (DeckState) int.Parse(state);

            });
        _isSubscribed = true;
    }

    public enum DeckState
    {
        Open = 0,
        Closed = 1
    }
}
