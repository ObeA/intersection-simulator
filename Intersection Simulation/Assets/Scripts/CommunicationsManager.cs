using System;
using Intersection.Modules.Communication.Client;
using UnityEngine;

public class CommunicationsManager : MonoBehaviour
{
    public string ip;
    public int port;

    public ControllerClient Client { get; private set; }

    public bool IsInitialized => Client != null;

    // Start is called before the first frame update
    async void Start()
    {
        if (Application.isEditor) return;
        
        var options = new ControllerClientOptions
        {
            BrokerAddress = ip,
            BrokerPort = port,
            EnableConsoleLogging = true,
            Identifier = "gruppe-4",
            ReconnectDelay = TimeSpan.FromSeconds(5)
        };
        Client = new ControllerClient(options);
        Debug.Log("Connecting to broker");
        await Client.StartAsync();
        Debug.Log("Connected");
    }
}
