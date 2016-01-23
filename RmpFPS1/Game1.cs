using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using RmpFPS1.GameObjects;

namespace RmpFPS1
{
    public enum GameState
    {
        InGame,
        GameOver
    }
    public class Game1 : Game
    {

        float x = 0;
        float y = 0;
        public float time { get; protected set; }
        //public area
        public Camera camera { get; protected set; }
        //private area
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameObjectManager gameObjectManager;
        Texture2D crosshairTexture;
        BasicEffect effect;
        SpriteFont Arial;
        


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            this.IsMouseVisible = true;
        }

 
        protected override void Initialize()
        {
            camera = new Camera(this, new Vector3(0, 100, 0), new Vector3(0, 0, -1), 0.2f);
            Components.Add(camera);

            gameObjectManager = new GameObjectManager(this, camera);
            Components.Add(gameObjectManager);

            effect = new BasicEffect(GraphicsDevice);

            this.IsMouseVisible = false;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            Arial = Content.Load<SpriteFont>(@"SpriteFonts\Courier New");
        }

       
        protected override void UnloadContent()
        {
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            time = float.Parse(gameTime.ElapsedGameTime.TotalMilliseconds.ToString()) / 1000;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue); 
            
            base.Draw(gameTime);
            
        }


    }
}