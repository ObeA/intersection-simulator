using UnityEngine;

public class Sensor : MonoBehaviour
{
    public CommunicationsManager communicationsManager;
    public string topic;

    private LogicGroup _logicGroup;
    public LogicGroup LogicGroup => _logicGroup != null ? _logicGroup : _logicGroup = GetComponentInParent<LogicGroup>();
    public string Topic => LogicGroup != null ? LogicGroup.topicRoot + topic : topic;

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
