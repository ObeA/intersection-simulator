namespace Intersection.Applications.Broker
{
    using System;
    using System.Threading.Tasks;
    using MQTTnet;
    using MQTTnet.Diagnostics;
    using MQTTnet.Server;

    class Program
    {
        static async Task Main(string[] args)
        {
            var logger = new MqttNetLogger();
            logger.LogMessagePublished += (s, e) => Console.WriteLine(e.TraceMessage.ToString());
            var server = new MqttFactory().CreateMqttServer(logger);
            var options = new MqttServerOptionsBuilder()
                .WithDefaultEndpointPort(6969);
            
            await server.StartAsync(options.Build());
            logger.Publish(MqttNetLogLevel.Info, "Program", "Press any key to shutdown the server.", null, null);
            Console.ReadLine();
            await server.StopAsync();
        }
    }
}