namespace Intersection.Applications.GraphVisualizer
{
    using Graphing;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class GraphVisualizer : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Graph _graph;

        public GraphVisualizer()
        {
            _graph = new Graph(this);
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            
            var a = new SpawnpointNode(new Vector2(10, 10));
            var b = new EndpointNode(new Vector2(10, 20));
            a.AddNeighbour(b);
            _graph.AddNodes(a);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _graph.Draw(gameTime);

            base.Draw(gameTime);
        }
    }
}