namespace Intersection.Applications.ConsoleClient.Commands
{
    using System;
    using System.Threading.Tasks;
    using Modules.Communication.Client;

    public class SubscribeCommand : Command
    {
        private ControllerClient _client;
        
        public SubscribeCommand(ControllerClient client)
        {
            _client = client;
        }
        
        public override async Task ExecuteAsync(string[] args)
        {
            await _client.SubscribeAsync(args[0],
                message => Console.WriteLine($"[{DateTime.UtcNow}] Incoming message on {args[0]}: {message}"));
        }
    }
}