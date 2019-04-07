using System;
using System.Globalization;
using Intersection;
using UnityEngine;
using Task = System.Threading.Tasks.Task;

public class TimescaleFeatureManager : MonoBehaviour
{
    public CommunicationsManager communicationsManager;
    public string topic;
    public float value;
    public float step;

    // Start is called before the first frame update
    async void Start()
    {
        if (communicationsManager == null || !communicationsManager.IsInitialized) return;
        await ChangeSpeedAsync(value);
    }

    // Update is called once per frame
    async void Update()
    {
        var changed = false;
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            value += step;
            changed = true;
        }
        else if (Input.GetKey(KeyCode.KeypadMinus))
        {
            value -= step;
            changed = true;
        }

        if (changed)
        {
            value = (float)Math.Round(Math.Max(0.1, Math.Min(value, 5.0)), 2);
            await ChangeSpeedAsync(value);
        }
    }

    private async Task ChangeSpeedAsync(float newValue)
    {
        await communicationsManager.Client.PublishAsync(topic, newValue.ToString(CultureInfo.InvariantCulture));
        foreach (var pathFollower in FindObjectsOfType<PathFollower>())
        {
            throw new NotImplementedException();
        }
    }
}