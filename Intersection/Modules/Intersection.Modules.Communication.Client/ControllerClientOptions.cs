using MQTTnet.Protocol;

namespace Intersection.Modules.Communication.Client
{
    using System;

    public class ControllerClientOptions
    {
        public string Identifier { get; set; }
        public TimeSpan ReconnectDelay { get; set; } = TimeSpan.FromSeconds(5);
        public string BrokerAddress { get; set; }
        public int BrokerPort { get; set; } = 7001;
        public bool UseTls { get; set; }
        public bool EnableConsoleLogging { get; set; }
        
        public string LastWillTopic { get; set; }
        public MqttQualityOfServiceLevel QualityOfService { get; set; }
    }
}