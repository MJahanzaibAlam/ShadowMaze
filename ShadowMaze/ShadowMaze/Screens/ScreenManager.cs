using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ShadowMaze
{
    /// <summary>
    /// This class will handle the screen stack. It will push/pop screens off of/onto
    /// the screen stack and call the load/unload methods for each screen. Only one
    /// Instance of the screen manager will be created (as only one is needed).
    /// </summary>
    public class ScreenManager
    {
        #region Variables
        private Vector2 v2ScreenDimensions; // Stores the dimensions of the game window.
        private Screen ActiveScreen; // Stores the screen which is currently shown.
        private Screen UnderlayScreen; // Stores the screen to be drawn behind a translucent screen.
        private Stack<Screen> stackScreens; // Stores the stack of screens.
        private ContentManager content; // Handles the loading/unloading of content for screens.
        private KeyboardState keyState; // Stores the status of each key on the keyboard.
        private KeyboardState prevKeyState; // Stores the keyState before the update cycle.
        private MouseState mouseState; // Stores the state of the keys on the mouse.
        private MouseState prevMouseState; // Stores the mouseState before the update cycle.
        private bool boolRainEnabled; // Stores whether or not background rain is enabled.
        private string strDifficulty; // Stores the difficulty setting of the game.
        private List<Map> listMapData; // Stores map names and stars achieved on maps.
        private static ScreenManager instance; // The single instance of the screen manager.
        private Texture2D tx2Rain; // Stores the texture for rain.
        private SoundManager soundManager; // Stores the sound manager which plays any required sound.
        #endregion

        #region SettingVariables
        // Creates a single Instance (singleton) of this class to handle all screens.
        public static ScreenManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ScreenManager();
                return instance;
            }
        }

        // Assigns a value to screen dimensions.
        public Vector2 ScreenDimensions 
        {
            get { return v2ScreenDimensions; }
            set { v2ScreenDimensions = value; }
        }

        public KeyboardState KeyState
        {
            get { return keyState; }
        }

        public MouseState MouseState
        {
            get { return mouseState; }
        }

        public Screen Active
        {
            get { return ActiveScreen; }
        }
        
        public bool RainEnabled
        {
            get { return boolRainEnabled; }
            set { boolRainEnabled = value; }
        }

        public string Difficulty
        {
            get { return strDifficulty; }
            set { strDifficulty = value; }
        }
        
        public List<Map> MapData
        {
            get { return listMapData; }
            set { listMapData = value; }
        }

        public SoundManager SoundManager
        {
            get { return soundManager; }
        }
        #endregion

        #region Methods
        // Pushes new screen onto stack and unloads content of previous screen.
        // Then sets the new screen as the active screen and loads its content.
        public void PushScreen(Screen screen)
        {
            stackScreens.Push(screen);
            ActiveScreen.UnloadContent();
            ActiveScreen = screen;
            ActiveScreen.Initialize();
            ActiveScreen.LoadContent(content);
        }
        
        // Effectively the same as PushScreen but sets an underlay screen as the previous active
        // screen to allow the previous screen to still be drawn in the background.
        public void PushTranslucentScreen(Screen screen)
        {
            UnderlayScreen = ActiveScreen;
            stackScreens.Push(screen);
            ActiveScreen = screen;
            ActiveScreen.Initialize();
            ActiveScreen.LoadContent(content);
        }

        // Pops the current screen off the stack, sets the previous screen as the active
        // screen and loads its contents. Used to go back to the previous screen.
        public void PopScreen()
        {
            ActiveScreen.UnloadContent();
            stackScreens.Pop();
            ActiveScreen = stackScreens.Peek();
            ActiveScreen.LoadContent(content);
        }

        // Clears the screen stack and places a menu screen on top.
        public void PopUntilMenu()
        {
            ActiveScreen.UnloadContent();
            UnderlayScreen.UnloadContent();
            UnderlayScreen = null;
            PushScreen(new MenuScreen());
            ActiveScreen = stackScreens.Peek();
            ActiveScreen.Initialize();
            ActiveScreen.LoadContent(content);
        }

        // Similar to pop screen but removes the underlay screen and does not reload the previous screen's content.
        public void PopTranslucentScreen()
        {
            ActiveScreen.UnloadContent();
            stackScreens.Pop();
            ActiveScreen = stackScreens.Peek();
            UnderlayScreen = null;
        }

        // Detects if a key has been intended to have been pressed a single time.
        public bool SingleKeyPress(Keys desiredKey)
        {
            if (keyState.IsKeyDown(desiredKey) && prevKeyState.IsKeyUp(desiredKey))
                return true;
            else return false;
        }

        // Detects if the mouse left button has been intended to have been clicked a single time.
        public bool SingleMouseClick()
        { // If the mouse left button wasn't pressed in the previous frame but it has now, the button was clicked.
            if (mouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton == ButtonState.Released)
                return true;
            else return false;
        }

        // Instantiates the screen stack and sound manager, and the new screen to be displayed is initialized.
        public void Initialize()
        {
            LoadSettingsAndData();
            stackScreens = new Stack<Screen>();
            ActiveScreen = new MenuScreen();
            ActiveScreen.Initialize();
            stackScreens.Push(ActiveScreen);
            soundManager = new SoundManager();
        }

        // Calls the current screen's load content method to load textures/sprites etc.
        public void LoadContent(ContentManager Content)
        {
            content = new ContentManager(Content.ServiceProvider, "Content");
            ActiveScreen.LoadContent(content);
            tx2Rain = Content.Load<Texture2D>("Sprites/Rain");
            soundManager.LoadContent(Content);
        }

        // Calls current screen's update method to make changes and updates the keyboard state.
        public void Update(GameTime gameTime)
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();
            prevMouseState = mouseState;
            mouseState = Mouse.GetState();
            ActiveScreen.Update(gameTime);
        }

        // Calls the current screen's draw method to display it. If an underlay screen is present i.e.
        // a pause menu has been brought up with a game screen being the underlaid screen,
        // the underlay screen will be drawn behind the active screen (translucency).
        public void Draw(SpriteBatch spriteBatch)
        {
            if (UnderlayScreen != null)
            {
                UnderlayScreen.Draw(spriteBatch);
            }
            ActiveScreen.Draw(spriteBatch);
            if (boolRainEnabled) // If rain is enabled, display it and play the rain sound.
            {
                spriteBatch.Draw(tx2Rain, Vector2.Zero, Color.Blue);
                soundManager.PlaySound("Rain");
            }
            else // Otherwise don't display rain and stop the rain sound.
            {
                soundManager.StopRain();
            }
        }

        // Loads in the settings and map data for the game.
        public void LoadSettingsAndData()
        {
            listMapData = new List<Map>();
            // Stores the path to the file containing the settings.
            string strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ShadowMaze\GameData.csv";

            string[] strAllLines = null;
            // Checks if the GameData file exists and then avoids crashing if the file is in use.
            // If the file is in use, 
            if (File.Exists(strFilePath))
                for (int intError = 0; intError < 3; intError++)
                try
                {
                    strAllLines = File.ReadAllLines(strFilePath); // Stores the lines of data file.
                    break;
                }
                catch (IOException) // If the file was in use while it was being loaded. Display an error in a message box.
                {
                    switch (intError)
                    {
                        case 0: // First warning.
                            MessageBox(new IntPtr(0), "Attempting to load data. Another program is using the GameData file in Documents/ShadowMaze, please close it and click ok.", "Error", 0);
                            break;
                        case 1: // Second warning.
                            MessageBox(new IntPtr(0), "If the GameData file is not closed, your settings and maps will not be loaded. Please close it and click ok.", "Error", 0);
                            break;
                        case 2: // Third warning.
                            MessageBox(new IntPtr(0), "GameData could not be loaded as the file is still in use. Close the file and restart the game to load your settings and avoid unsaved data.", "Error", 0);
                            break;
                    }
                }
            else  // If the file does not exist, create it and write in the default settings (rain is on, difficulty is normal).
                File.WriteAllText(strFilePath, @"TRUE,intermediate
ShadowGenesis,0");

            if (strAllLines!=null)
            {
                string[] strFirstLine = strAllLines[0].Split(','); // Splits the first line by the comma.
                if (strFirstLine[0] == "True") // If the first word was true, enable rain, otherwise disable it.
                    boolRainEnabled = true;
                else boolRainEnabled = false;
                strDifficulty = strFirstLine[1]; // Set the difficulty as whatever was stored in the file.
                
                // Loop through each additional line and add the map data to the 2D array.
                // The loop runs until the map index plus 1 (as line 0 is settings) is equal to the number of maps in the file.
                for (int intMapIndex = 0; intMapIndex + 1 != strAllLines.GetLength(0); intMapIndex++)
                { // For each row...
                    string[] strMapline = strAllLines[intMapIndex + 1].Split(',');
                    listMapData.Add(new Map(strMapline[0], Convert.ToInt32(strMapline[1]))); // Adds a map to the list.
                    listMapData[intMapIndex].MapName = strMapline[0]; // Store the map name in the list.
                    listMapData[intMapIndex].StarScore = Convert.ToInt32(strMapline[1]); // Store the score in the list.
                }
            }
            else
            { // Use default settings if the file couldn't be accessed or the file did not exist.
                boolRainEnabled = true;
                strDifficulty = "intermediate";
                listMapData.Add(new Map("ShadowGenesis", 0));
            }
        }

        // Saves the settings and data from the game into the data file.
        public void SaveSettingsAndData()
        {
            // Stores the path to the file containing the settings.
            string strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ShadowMaze\GameData.csv";

            //IOException ex = new IOException();
            // The first line to write is the rain satus and difficulty setting.
            string strLinesToWrite = boolRainEnabled.ToString() + "," + strDifficulty;

            // Loops through the remaining lines and adds them as map data (map name, score) separated by new lines ("/n").
            for (int intDataIndex = 0; intDataIndex < listMapData.Count; intDataIndex++)
            {
                strLinesToWrite += Environment.NewLine + listMapData[intDataIndex].MapName;
                strLinesToWrite += "," + listMapData[intDataIndex].StarScore + ",";
            }

            // Tries to save data to the GameData file while avoiding a crash if the user has the GameData file
            // open in another program. Three warnings are given if the file is in use before giving up saving data.
            for (int intError = 0; intError < 3; intError++)
                try
                {
                    File.WriteAllText(strFilePath, strLinesToWrite); // Writes the string containing the data to the file.
                    break;
                }
                catch (IOException)
                {
                    switch (intError)
                    {
                        case 0: // First warning.
                            MessageBox(new IntPtr(0), "Attempting to save data. Another program is using the GameData file in Documents/ShadowMaze, please close it and click ok.", "Error", 0);
                            break;
                        case 1: // Second warning.
                            MessageBox(new IntPtr(0), "If the GameData file is not closed, you risk losing game data. Please close it and click ok.", "Error", 0);
                            break;
                        case 2: // Third warning.
                            MessageBox(new IntPtr(0), "GameData could not be saved as the file is still in use. Please close the file before continuing playing to avoid further data loss.", "Error", 0);
                            break;
                    }
                }
        }

        // Allows a message box to be displayed during the game.
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, int options);
        #endregion
    }
}