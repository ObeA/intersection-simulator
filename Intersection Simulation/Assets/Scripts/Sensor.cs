using UnityEngine;

public class Sensor : MonoBehaviour
{
    public CommunicationsManager communicationsManager;
    public string topic;

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

    private async void OnTriggerEnter(Collider other)
    {
        if (communicationsManager == null || !communicationsManager.IsInitialized || topic == null) return;
        
        Debug.Log($"Sent sensor was high on {Topic}");
        await communicationsManager.Client.PublishAsync(Topic, "1");
    }
    
    private async void OnTriggerExit(Collider other)
    {
        if (communicationsManager == null || !communicationsManager.IsInitialized || topic == null) return;
        
        Debug.Log($"Sent sensor was low on {Topic}");
        await communicationsManager.Client.PublishAsync(Topic, "0");
    }
}
