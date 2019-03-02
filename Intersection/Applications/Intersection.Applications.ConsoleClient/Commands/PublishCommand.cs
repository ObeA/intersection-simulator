namespace Intersection.Applications.ConsoleClient.Commands
{
    using System.Threading.Tasks;
    using Modules.Communication.Client;

    public class PublishCommand : Command
    {
        private ControllerClient _client;
        
        public PublishCommand(ControllerClient client)
        {
            _client = client;
        }
        
        public override async Task ExecuteAsync(string[] args)
        {
            await _client.PublishAsync(args[0], args[1]);
        }
    }
}