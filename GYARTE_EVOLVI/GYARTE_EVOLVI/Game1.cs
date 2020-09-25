using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GYARTE_EVOLVI
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            GameHelper.Game = this;
            this.IsFixedTimeStep = true;

            graphics.PreferredBackBufferHeight = 834;
            graphics.PreferredBackBufferWidth = 1536;
        }

        protected override void Initialize()
        {
            GameHelper.Graphics = graphics;
            GameHelper.GraphicsDevice = GraphicsDevice;

            GameHelper.Window = Window;

            GameHelper.TextureManager.StartLoad += this.TextureManager_StartLoad;
            GameHelper.TextureManager.Loaded += this.TextureManager_Loaded;

            base.Initialize();
        }

        private void TextureManager_Loaded(object sender, System.EventArgs e)
        {
            ScreenManager.AddScreen("SC_MENU", new ScreenMenu());
            ScreenManager.AddScreen("SC_EVO", new ScreenEvo());
            ScreenManager.AddScreen("SC_LOAD_EVO", new ScreenLoadEvo());

            ScreenManager.SetScreen("SC_MENU");
        }

        private void TextureManager_StartLoad(object sender, System.EventArgs e)
        {
            ScreenManager.SetScreen("SC_LOAD");
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Getting default font.
            GameHelper.Font = Content.Load<SpriteFont>("FONT");

            // Adding the loading screen.
            ScreenManager.AddScreen("SC_LOAD", new ScreenLoad());

            // Initiating the loading of all texture assets.
            GameHelper.TextureManager.Load(Content);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            InputManager.Start();

            GameHelper.GameTime = gameTime;

            ScreenManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GameHelper.SpriteBatch = spriteBatch;

            ScreenManager.Draw();

            InputManager.End();

            base.Draw(gameTime);
        }
    }
}
