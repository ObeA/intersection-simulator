using System;
using System.Linq;
using System.Threading.Tasks;
using Intersection;
using UnityEngine;

public class SmartSensor : CommunicationMonoBehaviour
{
    public bool invert;
    public SensorGroupSettings[] settings;

    private int _activeUserCount = 0;
    private bool _lastSentStateIsHigh = false;

    private void Start()
    {
        if (settings.Length == 0)
        {
            Debug.LogWarning($"No inclusion / exclusion settings entered for smart sensor {name}");
        }
    }

    private async void OnTriggerEnter(Collider other)
    {
        var go = other.gameObject;
        if (!ShouldInclude(go)) return;
        
        _activeUserCount++;
        Debug.Log($"[ENTER] {go.name} {_activeUserCount}");
        await UpdateStateAsync();
    }

    private async void OnTriggerExit(Collider other)
    {
        var go = other.gameObject;
        if (!ShouldInclude(go)) return;
        
        _activeUserCount--;
        Debug.Log($"[EXIT] {go.name} {_activeUserCount}");
        await UpdateStateAsync();
    }

    private async Task UpdateStateAsync()
    {
        var isHigh = _activeUserCount > 0;
        if (!communicationsManager.IsInitialized || _lastSentStateIsHigh == isHigh) return;
        
        Debug.Log($"Updated sensor from {_lastSentStateIsHigh} to {isHigh}");
        await communicationsManager.Client.PublishAsync(Topic, isHigh ? "1" : "0");
        _lastSentStateIsHigh = isHigh;
    }

    private bool ShouldInclude(GameObject other)
    {
        var hit = settings.Length == 0 || settings.Any(type => type.type.CompareTag(other.tag) && type.include);
        return invert ? !hit : hit;
    }

    [Serializable]
    public class SensorGroupSettings
    {
        public GameObject type;
        public bool include;
    }
}
