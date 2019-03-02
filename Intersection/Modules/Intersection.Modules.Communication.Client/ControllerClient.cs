namespace Intersection.Modules.Communication.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Diagnostics;
    using MQTTnet.Extensions.ManagedClient;

    public class ControllerClient : IDisposable
    {
        private IMqttNetLogger _logger;
        private IManagedMqttClient _client;
        private ControllerClientOptions _options;
        private Dictionary<string, List<Action<string>>> _subscribers;
        private Queue<(string Topic, string Message)> _pendingMessages;

        public ControllerClient(ControllerClientOptions options)
        {
            _options = options;
            _subscribers = new Dictionary<string, List<Action<string>>>();
            _pendingMessages = new Queue<(string Topic, string Message)>();
            if (_options.EnableConsoleLogging)
            {
                _logger = new MqttNetLogger();
                _logger.LogMessagePublished += (s, e) => Console.WriteLine(e.TraceMessage.ToString());
                _client = new MqttFactory().CreateManagedMqttClient(_logger);
            }
            else
            {
                _client = new MqttFactory().CreateManagedMqttClient();
            }

            _client.ApplicationMessageSkipped += (sender, args) => Console.WriteLine("Skipped");
            _client.ApplicationMessageReceived += (sender, args) => Console.WriteLine("Received");

            _client.ApplicationMessageReceived += OnMessageReceived;
            _client.ApplicationMessageProcessed += (sender, args) => Console.WriteLine($"Sent {args.HasSucceeded}/{args.HasFailed} {args.ApplicationMessage.ApplicationMessage.Topic}: {args.ApplicationMessage.ApplicationMessage.ConvertPayloadToString()}");
            _client.Connected += OnConnected;
        }

        private async void OnConnected(object sender, MqttClientConnectedEventArgs e)
        {
            var subscriberTasks = Task.WhenAll(_subscribers.Keys.Select(topic => _client.SubscribeAsync(topic)));
            var publisherTasks = Task.WhenAll(_pendingMessages.Select(kvp => _client.PublishAsync(kvp.Topic, kvp.Message)));

            await Task.WhenAll(subscriberTasks, publisherTasks);
            _pendingMessages.Clear();
        }

        private void OnMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            if (!_subscribers.ContainsKey(e.ApplicationMessage.Topic))
            {
                return;
            }

            foreach (var callback in _subscribers[e.ApplicationMessage.Topic])
            {
                callback(e.ApplicationMessage.ConvertPayloadToString());
            }
        }

        private ManagedMqttClientOptions BuildOptions()
        {
            var clientOptionsBuilder = new MqttClientOptionsBuilder()
                .WithClientId(_options.Identifier)
                .WithTcpServer(_options.BrokerAddress, _options.BrokerPort);
            if (_options.UseTls)
            {
                clientOptionsBuilder.WithTls();
            }

            return new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(_options.ReconnectDelay)
                .WithClientOptions(clientOptionsBuilder.Build())
                .Build();
        }

        public async Task SubscribeAsync(string topic, Action<string> callback)
        {
            if (_client.IsConnected)
            {
                await _client.SubscribeAsync(topic);
            }
            AddCallback(topic, callback);
            
            await EnsureStarted();
        }

        private async Task EnsureStarted()
        {
            if (!_client.IsStarted)
            {
                await StartAsync();
            }
        }

        private void AddCallback(string topic, Action<string> callback)
        {
            if (!_subscribers.ContainsKey(topic))
            {
                _subscribers[topic] = new List<Action<string>>();
            }
            
            _subscribers[topic].Add(callback);
        }

        public async Task PublishAsync(string topic, string message)
        {
            if (_client.IsConnected)
            {
                await _client.PublishAsync(topic, message);
            }
            else
            {
                _pendingMessages.Enqueue((topic, message));
            }
            
            await EnsureStarted();
        }

        public async Task StartAsync(CancellationToken token = default)
        {
            await _client.StartAsync(BuildOptions());
        }

        public async Task StopAsync(CancellationToken token = default)
        {
            await _client.StopAsync();
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}