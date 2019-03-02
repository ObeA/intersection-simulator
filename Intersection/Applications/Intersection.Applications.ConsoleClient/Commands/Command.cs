namespace Intersection.Applications.ConsoleClient.Commands
{
    using System.Threading.Tasks;

    public abstract class Command
    {
        public abstract Task ExecuteAsync(string[] args);
    }
}