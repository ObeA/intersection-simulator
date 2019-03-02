using System;

namespace Intersection.Applications.GraphVisualizer
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GraphVisualizer())
                game.Run();
        }
    }
}