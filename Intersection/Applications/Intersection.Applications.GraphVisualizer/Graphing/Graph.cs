namespace Intersection.Applications.GraphVisualizer.Graphing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class Graph : IDrawable
    {
        private readonly Game _game;
        public List<SpawnpointNode> Spawnpoints;

        public Graph(Game game)
        {
            _game = game;
            Spawnpoints = new List<SpawnpointNode>();
        }

        public void AddNode(SpawnpointNode node)
        {
            Spawnpoints.Add(node);
        }

        public void AddNodes(IEnumerable<SpawnpointNode> nodes)
        {
            Spawnpoints.AddRange(nodes);
        }
        
        public void AddNodes(params SpawnpointNode[] nodes)
        {
            Spawnpoints.AddRange(nodes);
        }

        public IEnumerable<Node> GetRoute(SpawnpointNode a, Node b)
        {
            // Some pathfinding here
            throw new NotImplementedException();
        }

        public void Draw(GameTime gameTime)
        {
            var thickness = 4;
            var texture = new Texture2D(_game.GraphicsDevice, 1, thickness, true, SurfaceFormat.Color);
            texture.SetData(Enumerable.Repeat(Color.White, 1 * thickness).ToArray());
            
            using (var batch = new SpriteBatch(_game.GraphicsDevice))
            {
                batch.Begin();

                foreach (var node in Spawnpoints)
                {
                    node.Draw(batch, gameTime);
                }

                batch.End();
            }
        }

        public int DrawOrder { get; }
        public bool Visible { get; }
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
    }
}