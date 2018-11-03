using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMaze
{
    /// <summary>
    /// This will be the title screen which is displayed at the start of the game,
    /// it is displayed to allow the user to be able to go to different parts of the game.
    /// </summary>
    class MenuScreen : Screen
    {
        #region Variables
        private SpriteFont fontMenu; // Stores the font used for text in the menu.
        private Texture2D tx2Background; // Stores the background image of the menu.
        private Texture2D tx2Help; // Stores the help image.
        private Vector2 v2Title; // Stores the coordinates of the top left of the title text.
        private Vector2 v2Help; // Stores the coordinates of the top left of the "press escape for a guide" text.
        private Button[] buttons; // Stores the buttons used for this screen.
        private bool boolHelp; // Stores whether or not the user has pressed escape to see the help image.
        #endregion

        // Instantiates buttons and sets their text.
        public override void Initialize()
        {
            base.Initialize();
            buttons = new Button[3];
            for (int intButtonIndex = 0; intButtonIndex < buttons.GetLength(0); intButtonIndex++)
                buttons[intButtonIndex] = new Button();

            buttons[0].Text = "Play";
            buttons[1].Text = "Map Editor";
            buttons[2].Text = "Settings";
        }

        // Loads all of the textures for buttons and sprite fonts for text. Then correctly
        // positions buttons on screen.
        public override void LoadContent(ContentManager Content)
        {
            base.LoadContent(Content);
            fontMenu = content.Load<SpriteFont>("SpriteFonts/fontMenu");
            tx2Background = content.Load<Texture2D>("Sprites/Background");
            tx2Help = content.Load<Texture2D>("Sprites/Help");

            // The centre of the screen is retrieved by dividing the screen dimensions by two.
            Vector2 v2CentreScreen = ScreenManager.Instance.ScreenDimensions / 2;
            v2Title = v2CentreScreen - new Vector2(fontMenu.MeasureString("SHADOW MAZE").X / 2, 280);
            v2Help = v2CentreScreen - new Vector2(fontMenu.MeasureString("Press escape for a guide").X / 2, -210);
            // The buttons' images/fonts are loaded and they are spaced out by 155 pixels from the centre.
            int intSpacing = -150;
            for (int intButtonIndex = 0; intButtonIndex < buttons.GetLength(0); intButtonIndex++)
            {
                buttons[intButtonIndex].Image = content.Load<Texture2D>("Sprites/Button");
                buttons[intButtonIndex].Font = fontMenu;
                buttons[intButtonIndex].SetButtonPosition();
                buttons[intButtonIndex].Position += new Vector2(0, intSpacing);
                buttons[intButtonIndex].SetTextPosition();
                intSpacing += 155;
            }
            
        }

        // Unloads all the content that was used for this screen.
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        // Will check for mouse movement/clicks over buttons.
        // Displays help image if escape is pressed.
        public override void Update(GameTime gameTime)
        {
            // If the escape button was pressed, allow the help guide to be drawn.
            if (ScreenManager.Instance.SingleKeyPress(Microsoft.Xna.Framework.Input.Keys.Escape))
            { // If the help guide was already displayed. Stop displaying it.
                if (!boolHelp) boolHelp = true;
                else boolHelp = false;
            }
            if (!boolHelp) // If the help guide isn't being shown, detect button clicks.
            DetectButtonPresses();
        }

        // Draws all textures, text etc. to screen.
        // Draws help guide if boolHelp is true.
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background and the rain onto the screen.
            spriteBatch.Draw(tx2Background, Vector2.Zero, Color.White);

            // Draws the title (SHADOW MAZE) onto the screen using the menu's font.
            spriteBatch.DrawString(fontMenu, "SHADOW MAZE", v2Title, Color.Black);
            spriteBatch.DrawString(fontMenu, "Press escape for a guide", v2Help, Color.LightGray);

            for (int intButtonIndex = 0; intButtonIndex < buttons.GetLength(0); intButtonIndex++)
                buttons[intButtonIndex].DrawButtonAndText(spriteBatch);
            
            if (boolHelp)
                spriteBatch.Draw(tx2Help, Vector2.Zero, Color.White);
        }

        // Detects button presses and handles them accordingly.
        private void DetectButtonPresses()
        {
            if (buttons[0].DetectButtonClick())
                ScreenManager.Instance.PushScreen(new MapSelectScreen());
            if (buttons[1].DetectButtonClick())
                ScreenManager.Instance.PushScreen(new MapEditor());
            if (buttons[2].DetectButtonClick())
                ScreenManager.Instance.PushScreen(new SettingScreen());
        }
    }
}