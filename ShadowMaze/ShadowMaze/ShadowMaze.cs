using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.IO;

namespace ShadowMaze
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class ShadowMaze : Game
    {
        GraphicsDeviceManager graphics; // Manages the game window size and other graphics related properties.
        SpriteBatch spriteBatch; // Manages the drawing of objects onto the screen.

        public ShadowMaze()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Create the folder for settings, progress and maps upon start up if it does not exist.
            System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ShadowMaze");

            string strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ShadowMaze\ShadowGenesis.csv";
            if (!File.Exists(strFilePath))
            {
                // Stores the default map for the game in-case it is missing from the game directory.
                string strDefaultMap = @"tre,grs,bld,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,tre,
tre,grs,stw,stw,stw,stw,stw,stw,tre,grs,grs,grs,tre,stw,stw,stw,stw,stw,stw,grs,tre,
grs,drk,stw,tre,tre,tre,grs,grs,grs,grs,drk,grs,grs,grs,grs,tre,tre,tre,stw,drk,grs,
grs,grs,stw,tre,lbm,grs,grs,frt,stw,stw,stw,stw,stw,frt,grs,grs,lbm,tre,stw,grs,bld,
grs,grs,tre,drk,grs,grs,grs,stw,wtr,stw,wtr,stw,wtr,stw,grs,grs,grs,drk,tre,grs,grs,
grs,grs,grs,grs,grs,grs,grs,stw,wtr,stw,wtr,stw,wtr,stw,grs,grs,grs,grs,grs,grs,grs,
grs,grs,tre,drk,grs,grs,grs,frt,stw,stw,stw,stw,stw,frt,grs,grs,grs,drk,tre,grs,grs,
bld,grs,stw,stw,stw,grs,grs,grs,grs,tre,tre,tre,grs,grs,grs,grs,stw,stw,stw,grs,grs,
grs,drk,stw,wtr,wtr,stw,lbm,grs,drk,grs,grs,grs,drk,grs,grs,stw,wtr,wtr,stw,drk,grs,
tre,grs,stw,stw,stw,stw,stw,stw,tre,grs,grs,grs,tre,stw,stw,stw,stw,stw,stw,grs,tre,
tre,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,grs,bld,grs,tre,
tre,tre,tre,tre,tre,tre,tre,tre,tre,tre,tre,tre,tre,tre,tre,tre,tre,tre,tre,tre,tre";
                File.WriteAllText(strFilePath, strDefaultMap); // Creates a default map file.
            }

            IsMouseVisible = true;

            #region ScreenManager.Instance
            ScreenManager.Instance.Initialize();
            ScreenManager.Instance.ScreenDimensions = new Vector2(1050, 600);
            graphics.PreferredBackBufferWidth = (int)ScreenManager.Instance.ScreenDimensions.X;
            graphics.PreferredBackBufferHeight = (int)ScreenManager.Instance.ScreenDimensions.Y;
            graphics.ApplyChanges();
            #endregion//Sets the screen dimensions and initializes the screen manager.
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            ScreenManager.Instance.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            ScreenManager.Instance.Update(gameTime);
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            ScreenManager.Instance.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
