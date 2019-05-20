using System.Threading.Tasks;
using Intersection;
using UnityEngine;

public class Deck : CommunicationMonoBehaviour
{
    private DeckState _state;
    private DeckState _newState;
    private Animator _animator;
    private bool _isSubscribed;
    
    // Start is called before the first frame update
    private async void Start()
    {
        _newState = _state;
        _animator = GetComponent<Animator>();
        await EnsureSubscribeRegisteredAsync();
    }

    // Update is called once per frame
    async void Update()
    {
        await EnsureSubscribeRegisteredAsync();

        if (_state == _newState)
        {
            return;
        }

        // _animator.Play( ? "Bridge open" : "Bridge close");
        transform.position += (_state == DeckState.Closed ? 1 : -1) * Vector3.up;
        _state = _newState;
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
