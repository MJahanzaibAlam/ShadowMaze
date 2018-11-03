using System;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMaze
{
    // Displays the available maps which the user can play on.
    class MapSelectScreen : Screen
    {
        #region Variables
        private SpriteFont fontSelection; // Stores the font used for text in the selection menu.
        private SpriteFont fontLargeText; // Stores a font to display larger text.
        private Texture2D tx2Background; // Stores the background image of the game.
        private Texture2D tx2Star; // Stores the image for a star.
        private Vector2 v2Star; // Stores the position of the star(s).
        private Vector2 v2Title; // Stores the coordinates of the top left of the title text.
        private Vector2 v2MapName; // Stores the coordinates for the map name to be displayed.
        private Button[] buttons; // Stores the buttons used for this screen.
        private string strMapName; // Stores the name of the currently selected map.
        private int intMapIndex; // Stores the index of the currently selected map.
        #endregion

        // Instantiates buttons and sets their text.
        public override void Initialize()
        {
            base.Initialize();
            buttons = new Button[5];
            for (int intButtonIndex = 0; intButtonIndex < buttons.GetLength(0); intButtonIndex++)
                buttons[intButtonIndex] = new Button();

            buttons[0].Text = "Play";
            buttons[1].Text = "Prev";
            buttons[2].Text = "Next";
            buttons[3].Text = "Back To Menu";
            buttons[4].Text = "Delete Map";

            intMapIndex = 0;
            // Retrieves the name of the map which is identified by the map index from the screen manager.
            strMapName = ScreenManager.Instance.MapData[intMapIndex].MapName;
        }

        // Loads all of the textures for buttons and sprite fonts for text.
        public override void LoadContent(ContentManager Content)
        {
            base.LoadContent(Content);
            tx2Background = content.Load<Texture2D>("Sprites/Background");
            fontSelection = content.Load<SpriteFont>("SpriteFonts/fontSelection");
            fontLargeText = content.Load<SpriteFont>("SpriteFonts/fontLarge");

            // The centre of the screen is retrieved by dividing the screen dimensions by two.
            Vector2 v2CentreScreen = ScreenManager.Instance.ScreenDimensions / 2;
            v2Title = v2CentreScreen - new Vector2(fontLargeText.MeasureString("Select A Map").X / 2, 280);
            v2Star = v2CentreScreen - new Vector2(150, 230);

            v2MapName = v2CentreScreen - new Vector2(fontLargeText.MeasureString(strMapName).X / 2, 150);

            buttons[0].Image = content.Load<Texture2D>("Sprites/Button");
            buttons[1].Image = content.Load<Texture2D>("Sprites/ButtonLeft");
            buttons[2].Image = content.Load<Texture2D>("Sprites/ButtonRight");
            buttons[3].Image = content.Load<Texture2D>("Sprites/ButtonShort");
            buttons[4].Image = content.Load<Texture2D>("Sprites/ButtonShort");
            tx2Star = content.Load<Texture2D>("Sprites/Star");

            buttons[0].SetButtonPosition(); // Play button is in centre of the screen.
            buttons[1].Position = v2CentreScreen - new Vector2(500, buttons[1].Image.Height/2); // Left of the screen.
            buttons[2].Position = v2CentreScreen + new Vector2(400, -buttons[2].Image.Height/2); // Right of the screen.
            buttons[3].SetButtonPosition();
            buttons[3].Position -= new Vector2(270, -200); // Below and to the left of the centre of the screen.
            buttons[4].SetButtonPosition();
            buttons[4].Position += new Vector2(270, 200); // Below and to the right of the centre of the screen.

            // Adjust the buttons' text appropriately.
            for (int intButtonIndex = 0; intButtonIndex < buttons.GetLength(0); intButtonIndex++)
            {
                buttons[intButtonIndex].Font = fontSelection;
                buttons[intButtonIndex].SetTextPosition();
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
            DetectButtonPresses(); // Changes the map index or starts the game or quits to menu.
            strMapName = ScreenManager.Instance.MapData[intMapIndex].MapName; // Updates the map name.

            // Gets the centre of the screen to update the text position of the map name (as the map name varies in size).
            Vector2 v2CentreScreen = ScreenManager.Instance.ScreenDimensions / 2;
            v2MapName = v2CentreScreen - new Vector2(fontLargeText.MeasureString(strMapName).X / 2, 150);
        }

        // Draws all buttons, text etc. to screen.
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the background onto the screen.
            spriteBatch.Draw(tx2Background, Vector2.Zero, Color.White);

            // Draws the title (Settings) onto the screen using the setting menu's font.
            spriteBatch.DrawString(fontLargeText, "Select A Map", v2Title, Color.Black);
            spriteBatch.DrawString(fontLargeText, strMapName, v2MapName, Color.AliceBlue);
            // Draws text displaying which map in the list is selected.
            spriteBatch.DrawString(fontLargeText, "Map " + (intMapIndex + 1) + " of " + 
                ScreenManager.Instance.MapData.Count, v2Title + new Vector2(30, 340), Color.AliceBlue);

            // Draws stars spaced out by 10 pixels (the stars at 50 pixels each, 10 + 50 = 60).
            for (int intStarIndex = 1; (intStarIndex <= ScreenManager.Instance.MapData[intMapIndex].StarScore) && intStarIndex <= 3; intStarIndex++)
                spriteBatch.Draw(tx2Star, v2Star + new Vector2(intStarIndex * 60, 0), Color.White);

            for (int intButtonIndex = 0; intButtonIndex < buttons.GetLength(0); intButtonIndex++)
                buttons[intButtonIndex].DrawButtonAndText(spriteBatch);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, int options);

        // Asks the user for confirmation via a message box and deletes the current map.
        private void DeleteMap()
        {
            int intMsgResult = 6;
            intMsgResult = MessageBox(new IntPtr(0), "Are you sure you want to delete this map?", "Confirmation", 3);

            // If the user clicked yes in the message box, the map is deleted.
            if (intMsgResult == 6)
            {
                for (int intError = 0; intError < 3; intError++)
                    try
                    {
                        File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ShadowMaze\" + strMapName + ".csv");
                        break;
                    }
                    catch (IOException)
                    {
                        switch (intError)
                        {
                            case 0: // First warning.
                                MessageBox(new IntPtr(0), "Attempting to delete map. Another program is using the map file in Documents/ShadowMaze, please close it and click ok.", "Error", 0);
                                break;
                            case 1: // Second warning.
                                MessageBox(new IntPtr(0), "If the map file is not closed, the map will not be deleted.", "Error", 0);
                                break;
                            case 2: // Third warning.
                                MessageBox(new IntPtr(0), "Map file could not be deleted as the file is in use.", "Error", 0);
                                return;
                        }
                    }
                ScreenManager.Instance.MapData.RemoveAt(intMapIndex);
                ScreenManager.Instance.MapData.TrimExcess();
                ScreenManager.Instance.SaveSettingsAndData();
                MessageBox(new IntPtr(0), "Map deleted!", "Notice", 0); // Displays a message box notifying the user that the map was saved.
                intMapIndex--; // Decrease the map index to account for the change.
            }
        }

        // Detects if a button was clicked and handles this accordingly.
        private void DetectButtonPresses()
        {
            // Start a game with the selected map.
            if (buttons[0].DetectButtonClick())
                ScreenManager.Instance.PushScreen(new GameScreen(strMapName));

            // Select the next map in the list.
            if (buttons[1].DetectButtonClick())
                if (intMapIndex != 0)
                    intMapIndex--;


            // Selects the previous map in the list.
            if (buttons[2].DetectButtonClick())
                if (intMapIndex != ScreenManager.Instance.MapData.Count - 1)
                    intMapIndex++;
            
            if (buttons[3].DetectButtonClick())
                // Returns to menu by popping the selection screen off of the stack.
                ScreenManager.Instance.PopScreen();

            if (buttons[4].DetectButtonClick())
                // Removes a map from the list, saves the list to the game data file and then deletes the map file.
                if (intMapIndex!= 0)
                DeleteMap();
        }
    }
}