namespace Intersection.Applications.GraphVisualizer.Graphing
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class SpawnpointNode : Node
    {
        public SpawnpointNode(Guid identifier, Vector2 position) : base(identifier, position)
        {
        }

        public SpawnpointNode(Vector2 position) : base(position)
        {
        }

        public override void Draw(SpriteBatch batch, GameTime time)
        {
            base.Draw(batch, time);

            foreach (var neighbour in Neighbours)
            {
                neighbour.Draw(batch, time);
            }
        }
    }
}