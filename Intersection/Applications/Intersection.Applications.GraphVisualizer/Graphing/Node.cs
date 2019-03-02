namespace Intersection.Applications.GraphVisualizer.Graphing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public abstract class Node : IBatchDrawable
    {
        public Guid Identifier { get; }
        public List<Node> Neighbours { get; set; }
        
        public Vector2 Position { get; set; }

        private Texture2D _texture;

        public Node(Guid identifier, Vector2 position)
        {
            Identifier = identifier;
            Position = position;
            Neighbours = new List<Node>();
        }

        public Node(Vector2 position) : this(Guid.NewGuid(), position)
        {
            
        }

        public void AddNeighbour(Node node)
        {
            Neighbours.Add(node);
        }

        public override bool Equals(object obj)
        {
            if (obj is Node other)
            {
                return Identifier == other.Identifier;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public virtual void Draw(SpriteBatch batch, GameTime time)
        {
            if (_texture == null)
            {
                _texture = new Texture2D(batch.GraphicsDevice, 2, 2, false, SurfaceFormat.Color);
                _texture.SetData(Enumerable.Repeat(Color.White, 2 * 2).ToArray());
            }

            batch.Draw(_texture, Position, Color.Red);
        }
    }
}