namespace Intersection.Applications.GraphVisualizer
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public interface IBatchDrawable
    {
        void Draw(SpriteBatch batch, GameTime time);
    }
}