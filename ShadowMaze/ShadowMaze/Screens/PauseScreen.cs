using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMaze
{
    class PauseScreen : Screen
    {
        #region Variables
        private SpriteFont fontPause; // Stores the font used for text in the pause screen.
        private Vector2 v2Title; // Stores the coordinates of the top left of the title text.
        private Button[] buttons; // Stores the buttons used for this screen.
        #endregion

        // Instantiates buttons and sets their text.
        public override void Initialize()
        {
            base.Initialize();
            buttons = new Button[2];
            for (int intButtonIndex = 0; intButtonIndex < buttons.GetLength(0); intButtonIndex++)
                buttons[intButtonIndex] = new Button();

            buttons[0].Text = "Resume";
            buttons[1].Text = "Exit To Menu";
        }

        // Loads textures for buttons, sprite fonts for text and then positions buttons.
        public override void LoadContent(ContentManager Content)
        {
            base.LoadContent(Content);
            fontPause = content.Load<SpriteFont>("SpriteFonts/fontPause");

            // The centre of the screen is retrieved by dividing the screen dimensions by two.
            Vector2 v2CentreScreen = ScreenManager.Instance.ScreenDimensions / 2;
            v2Title = v2CentreScreen - new Vector2(fontPause.MeasureString("GamePaused").X / 2, 280);

            // The buttons' images/fonts are loaded and they are spaced out by 150 pixels from the centre.
            int intSpacing = -150;
            for (int intButtonIndex = 0; intButtonIndex < buttons.GetLength(0); intButtonIndex++)
            {
                buttons[intButtonIndex].Image = content.Load<Texture2D>("Sprites/Button");
                buttons[intButtonIndex].Font = fontPause;
                buttons[intButtonIndex].SetButtonPosition();
                buttons[intButtonIndex].Position += new Vector2(0, intSpacing);
                buttons[intButtonIndex].SetTextPosition();
                intSpacing += 300;
            }
        }

        // Unloads all the content that was used for this screen.
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        // Calls detect button presses method to process clicks.
        public override void Update(GameTime gameTime)
        {
            DetectButtonPresses();
        }

        // Draws buttons and text to screen.
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draws "Game Paused" onto the screen using the menu's font.
            spriteBatch.DrawString(fontPause, "Game Paused", v2Title, Color.Black);

            for (int intButtonIndex = 0; intButtonIndex < buttons.GetLength(0); intButtonIndex++)
                buttons[intButtonIndex].DrawButtonAndText(spriteBatch);
        }

        // Detects if a button was clicked and handles this accordingly.
        private void DetectButtonPresses()
        {
            if (buttons[0].DetectButtonClick())
                ScreenManager.Instance.PopTranslucentScreen(); // Pops the pause screen off the stack to return to the game screen.
            if (buttons[1].DetectButtonClick())
                ScreenManager.Instance.PopUntilMenu(); // Pops all screens off of the stack and places a menu screen on top.
        }
    }
}