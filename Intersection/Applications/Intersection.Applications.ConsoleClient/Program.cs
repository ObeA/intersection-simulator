namespace Intersection.Applications.ConsoleClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Commands;
    using Modules.Communication.Client;

    class Program
    {
        private static Dictionary<string, Type> Commands = new Dictionary<string, Type>
        {
            {"subscribe", typeof(SubscribeCommand)},
            {"publish", typeof(PublishCommand)}
        };

        private static bool Stopped;

        private static ControllerClientOptions DefaultOptions = new ControllerClientOptions
        {
            BrokerAddress = "localhost",
            BrokerPort = 6969,
            Identifier = "simulator",
            UseTls = false,
            EnableConsoleLogging = true
        };
        private static ControllerClient Client = new ControllerClient(DefaultOptions);
        
        static async Task Main(string[] args)
        {
            await Client.StartAsync();
            
            Console.CancelKeyPress += (sender, eventArgs) => {
                Stopped = true;
            };

            while (!Stopped)
            {
                await ReadAndExecuteCommandAsync();
            }
            
            await Client.StopAsync();
        }

        static async Task ReadAndExecuteCommandAsync()
        {
            var input = Console.ReadLine()?.Trim().Split(' ').Where(s => !string.IsNullOrEmpty(s)).ToList();
            if (input == null || input.Count == 0)
            {
                WriteResponse("Empty input");
                return;
            }

            if (input.Count < 1 || !Commands.ContainsKey(input[0]))
            {
                WriteResponse("Invalid command");
                return;
            }

            var command = (Command)Activator.CreateInstance(Commands[input[0]], Client);
            await command.ExecuteAsync(input.Skip(1).ToArray());
        }

        static void WriteResponse(string message)
        {
            Console.WriteLine($"# {message}");
        }
    }
}