namespace Intersection.Applications.GraphVisualizer.Graphing
{
    using System;
    using Microsoft.Xna.Framework;

    public class EndpointNode : Node
    {
        public EndpointNode(Guid identifier, Vector2 position) : base(identifier, position)
        {
        }

        public EndpointNode(Vector2 position) : base(position)
        {
        }
    }
}