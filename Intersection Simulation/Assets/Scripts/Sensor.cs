using Intersection;
using UnityEngine;

public class Sensor : CommunicationMonoBehaviour
{
    private bool _isHigh = false;

    private async void OnTriggerEnter(Collider other)
    {
        if (!IsReady || _isHigh) return;
        
        _isHigh = true;
        Debug.Log($"Sent sensor was high on {Topic}");
        await CommunicationsManager.Client.PublishAsync(Topic, "1");
    }
    
    private async void OnTriggerExit(Collider other)
    {
        if (!IsReady || !_isHigh) return;

        _isHigh = false;
        Debug.Log($"Sent sensor was low on {Topic}");
        await CommunicationsManager.Client.PublishAsync(Topic, "0");
    }
}
