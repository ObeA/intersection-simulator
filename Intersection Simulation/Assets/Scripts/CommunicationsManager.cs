using System;
using System.Threading.Tasks;
using Intersection.Modules.Communication.Client;
using MQTTnet.Protocol;
using UnityEngine;

public class CommunicationsManager : MonoBehaviour
{
    public string ip;
    public int port;
    public int teamId;

    public ControllerClient Client { get; private set; }

    public bool IsInitialized => Client != null;

    // Start is called before the first frame update
    async void Start()
    {
        var options = new ControllerClientOptions
        {
            BrokerAddress = ip,
            BrokerPort = port,
            EnableConsoleLogging = true,
            Identifier = "gruppo-4",
            ReconnectDelay = TimeSpan.FromSeconds(5),
            QualityOfService = MqttQualityOfServiceLevel.AtLeastOnce,
            LastWillTopic = $"{teamId}/features/lifecycle/simulator/ondisconnect"
        };
        
        Client = new ControllerClient(options);
        Debug.Log("Connecting to broker");
        await Client.StartAsync();

        var publishOnConnectTask = Client.PublishAsync($"{teamId}/features/lifecycle/simulator/onconnect", string.Empty);
        var subscribeOnConnectTask = Client.SubscribeAsync(
            $"{teamId}/features/lifecycle/controller/onconnect", 
            message => Debug.Log("Received on connect from controller")
        );
        var subscribeOnDisconnectTask = Client.SubscribeAsync(
            $"{teamId}/features/lifecycle/controller/ondisconnect", 
            message => Debug.Log("Received on disconnect from controller")
        );

        await Task.WhenAll(publishOnConnectTask, subscribeOnConnectTask, subscribeOnDisconnectTask);
        
        Debug.Log("Connected");
    }
}
