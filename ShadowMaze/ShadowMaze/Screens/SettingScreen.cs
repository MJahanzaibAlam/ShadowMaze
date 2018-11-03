using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ShadowMaze
{
    // Displays settings which can be changed (rain & thunder on/off, difficulty, reset progress)
    class SettingScreen : Screen
    {
        #region Variables
        private SpriteFont fontSettings; // Stores the font used for text in the settings menu.
        private Texture2D tx2Background; // Stores the background image of the game.
        private Vector2 v2Title; // Stores the coordinates of the top left of the title text.
        private Button[] buttons; // Stores the buttons used for this screen.
        private Dictionary<string, string> strDiffDesc; // Stores the description of the difficulty matching the difficulty.
        #endregion

        // Instantiates buttons and sets their text.
        public override void Initialize()
        {
            base.Initialize();
            buttons = new Button[4];
            for (int intButtonIndex = 0; intButtonIndex < buttons.GetLength(0); intButtonIndex++)
                buttons[intButtonIndex] = new Button();

            strDiffDesc = new Dictionary<string, string>();
            strDiffDesc["easy"] = "1-Helpless Shadows";
            strDiffDesc["intermediate"] = "2-Balanced Light";
            strDiffDesc["hard"] = "3-Flicker in darkness";
            strDiffDesc["impossible"] = "4-Dying Light";

            buttons[0].Text = "Toggle Rain";
            buttons[1].Text = "Difficulty: " + strDiffDesc[ScreenManager.Instance.Difficulty];
            buttons[2].Text = "Reset Progress";
            buttons[3].Text = "Back To Menu";
        }

        // Loads all of the textures for buttons and sprite fonts for text.
        public override void LoadContent(ContentManager Content)
        {
            base.LoadContent(Content);
            tx2Background = content.Load<Texture2D>("Sprites/Background");
            fontSettings = content.Load<SpriteFont>("SpriteFonts/FontSettings");

            // The centre of the screen is retrieved by dividing the screen dimensions by two.
            Vector2 v2CentreScreen = ScreenManager.Instance.ScreenDimensions / 2;
            v2Title = v2CentreScreen - new Vector2(fontSettings.MeasureString("Settings").X / 2, 280);

            // The buttons' images/fonts are loaded and they are 160 pixels from the centre and spaced out by 130 pixels.
            int intSpacing = -160;
            for (int intButtonIndex = 0; intButtonIndex < buttons.GetLength(0); intButtonIndex++)
            {
                buttons[intButtonIndex].Image = content.Load<Texture2D>("Sprites/Button");
                buttons[intButtonIndex].Font = fontSettings;
                buttons[intButtonIndex].SetButtonPosition();
                buttons[intButtonIndex].Position += new Vector2(0, intSpacing);
                buttons[intButtonIndex].SetTextPosition();
                intSpacing += 130;
            }

        }

        // Unloads all the content that was used for this screen.
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        // Will check for mouse movement/clicks over buttons.
        public override void Update(GameTime gameTime)
        {
            DetectButtonPresses();
        }

        // Draws all buttons and text to the screen.
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background and the rain onto the screen.
            spriteBatch.Draw(tx2Background, Vector2.Zero, Color.White);

            // Draws the title (Settings) onto the screen using the setting menu's font.
            spriteBatch.DrawString(fontSettings, "Settings", v2Title, Color.Black);

            for (int intButtonIndex = 0; intButtonIndex < buttons.GetLength(0); intButtonIndex++)
                buttons[intButtonIndex].DrawButtonAndText(spriteBatch);
        }

        // Detects if a button was clicked and handles this accordingly.
        private void DetectButtonPresses()
        {
            // Toggle rain based on current program settings.
            if (buttons[0].DetectButtonClick())
            {
                if (ScreenManager.Instance.RainEnabled)
                    ScreenManager.Instance.RainEnabled = false;
                else ScreenManager.Instance.RainEnabled = true;
                ScreenManager.Instance.SaveSettingsAndData();
            }

            // Increase difficulty (or reset to easy if on hardest) and display the difficulty in the button.
            if (buttons[1].DetectButtonClick())
            {
                switch (ScreenManager.Instance.Difficulty)
                {
                    case "easy":
                        ScreenManager.Instance.Difficulty = "intermediate";
                        break;
                    case "intermediate":
                        ScreenManager.Instance.Difficulty = "hard";
                        break;
                    case "hard":
                        ScreenManager.Instance.Difficulty = "impossible";
                        break;
                    case "impossible":
                        ScreenManager.Instance.Difficulty = "easy";
                        break;
                }
                buttons[1].Text = "Difficulty: " + strDiffDesc[ScreenManager.Instance.Difficulty];
                buttons[1].SetTextPosition(); // Update the button's text and the text's position once it has changed.
                ScreenManager.Instance.SaveSettingsAndData();
            }


            // Resets all map completion progress (all star scores set to 0).
            if (buttons[2].DetectButtonClick())
            {
                int intMsgResult = ScreenManager.MessageBox(new IntPtr(0), "This will reset the star scores for all your maps to 0, are you sure you wish to proceeed?", "Notice", 3);
                if (intMsgResult == 6)
                    // For each map in the map data list, reset its star score to 0.
                    foreach (Map map in ScreenManager.Instance.MapData)
                        map.StarScore = 0;

                ScreenManager.Instance.SaveSettingsAndData();
            }

            if (buttons[3].DetectButtonClick())
                // Returns to menu by popping the settings screen off of the stack.
                ScreenManager.Instance.PopScreen();
        }
    }
}
