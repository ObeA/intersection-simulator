using System;

namespace Intersection.Applications.TestServer
{
    using System.Threading.Tasks;
    using MQTTnet;
    using MQTTnet.Server;

    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new MqttFactory().CreateMqttServer();
            var options = new MqttServerOptionsBuilder()
                .WithDefaultEndpointPort(6969);
            RegisterLoggers(server);

            await server.StartAsync(options.Build());
            Console.ReadLine();
            await server.StopAsync();
        }

        static void RegisterLoggers(IMqttServer server)
        {
            server.ApplicationMessageReceived += (s, e) =>
                Console.WriteLine(
                    $"Message from {e.ClientId} on {e.ApplicationMessage.Topic}: {e.ApplicationMessage.ConvertPayloadToString()}");
            server.ClientConnected += (s, e) => Console.WriteLine($"Client {e.ClientId} connected");
            server.ClientSubscribedTopic += (s, e) =>
                Console.WriteLine($"Client {e.ClientId} subscribed to {e.TopicFilter.ToString()}");
            server.ClientUnsubscribedTopic += (s, e) => Console.WriteLine($"Client {e.ClientId} unsubscribed from {e.TopicFilter.ToString()}");
            server.ClientDisconnected += (s, e) => Console.WriteLine($"Client {e.ClientId} disconnected");
        }
    }
}