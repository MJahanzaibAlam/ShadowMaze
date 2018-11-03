using System;
using System.Linq;

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ShadowMaze
{
    /// <summary>
    /// This class will hold the properties and initialize, load content for, unload content for
    /// and update the screen when the user is actually playing a level in the game.
    /// </summary>
    class GameScreen : Screen
    {
        #region Variables
        private SpriteFont fontGame; // Stores a font used to draw information onto the bottom of the screen.
        private GridLayer layerGameGrid; // Forms a grid of nodes, used for collisions and pathfinding AI.
        private Player[] player; // Stores the array of the 4 players that are controlled by users.
        private ShadowCopy[] shadowCopy; // Stores the array of initial shadow copies.
        private AddShadowCopy[] addShadowCopy; // Stores the array of additional shadow copies.
        private Keys[,] InputKeys; // Stores the sets of keys to be used for each player.
        private string strMapName; // Stores the name of the map which is currently loaded.
        private int intProgressionTime; // Stores the time that has passed since the start of the game in seconds.
        private int intTimeSinceFrame; // Stores the time since the last frame to add to the progression time.
        private Texture2D tx2ShadowCopy; // Stores the texture of shadow copies to spawn them.
        #endregion
        
        // Instantiates objects and initializes the game grid (creates the grid of nodes).
        // Provides the input keys for each player and sets every entity's
        // starting position.
        public override void Initialize()
        {
            base.Initialize();
            layerGameGrid = new GridLayer(true);
            player = new Player[4];
            shadowCopy = new ShadowCopy[4];
            switch (ScreenManager.Instance.Difficulty)
            {
                case "easy":
                    addShadowCopy = new AddShadowCopy[0];
                    break;
                case "intermediate":
                    addShadowCopy = new AddShadowCopy[2];
                    break;
                case "hard":
                    addShadowCopy = new AddShadowCopy[4];
                    break;
                case "impossible":
                    addShadowCopy = new AddShadowCopy[6];
                    break;
            }
            InputKeys = new Keys[4, 4];

            // four players are instantiated and each has their starting node set by the InitPos function.
            for (int intPlayerIndex = 0; intPlayerIndex <= 3; intPlayerIndex++)
            {
                player[intPlayerIndex] = new Player();
                player[intPlayerIndex].Initialize(InitPos(intPlayerIndex, layerGameGrid.Nodes));
            }
            // Sets the colour of players.
            player[0].Colour = Color.Blue;
            player[1].Colour = Color.ForestGreen;
            player[2].Colour = Color.Gold;
            player[3].Colour = Color.Red;

            // four shadow copies are instantiated and each has their starting node set by the InitPos function.
            for (int intCopyIndex = 0; intCopyIndex <= 3; intCopyIndex++)
            {
                shadowCopy[intCopyIndex] = new ShadowCopy();
                shadowCopy[intCopyIndex].Initialize(InitCopyPos(intCopyIndex, layerGameGrid.Nodes));
            }

            #region setKeys
            InputKeys[0, 0] = Keys.D;
            InputKeys[0, 1] = Keys.S;
            InputKeys[0, 2] = Keys.A;
            InputKeys[0, 3] = Keys.W;

            InputKeys[1, 0] = Keys.H;
            InputKeys[1, 1] = Keys.G;
            InputKeys[1, 2] = Keys.F;
            InputKeys[1, 3] = Keys.T;

            InputKeys[2, 0] = Keys.L;
            InputKeys[2, 1] = Keys.K;
            InputKeys[2, 2] = Keys.J;
            InputKeys[2, 3] = Keys.I;

            InputKeys[3, 0] = Keys.Right;
            InputKeys[3, 1] = Keys.Down;
            InputKeys[3, 2] = Keys.Left;
            InputKeys[3, 3] = Keys.Up;
            #endregion
        }

        // Sets the name of the map to load as the passed in parameter.
        public GameScreen(string strMapToLoad)
        {
            strMapName = strMapToLoad;
        }

        // Sets the nodes to use as starting positions for each player. This is done mainly
        // by using the nodes' array length for the right or the top, dividing it by 2 for the middle
        // or using 0 for the left or bottom (for the index of the node).
        private Node InitPos(int intEntityIndex, Node[,] Nodes)
        {
            switch (intEntityIndex)
            {
                case 0:
                    return Nodes[Nodes.GetLength(0) - 1, (Nodes.GetLength(1) / 2) - 1]; // Middle right.
                case 1:
                    return Nodes[Nodes.GetLength(0) / 2, 0]; // Top middle.
                case 2:
                    return Nodes[0, (Nodes.GetLength(1) / 2) - 1]; // Middle left.
                case 3:
                    return Nodes[Nodes.GetLength(0) / 2, Nodes.GetLength(1) - 2]; // Bottom middle.
                default:
                    return Nodes[Nodes.GetLength(0) - 1, Nodes.GetLength(1) / 2 - 1]; // Middle right.
            }
        }

        // Sets shadow copies initial nodes near players.
        private Node InitCopyPos(int intEntityIndex, Node[,] Nodes)
        {
            switch (intEntityIndex)
            {
                case 0:
                    return Nodes[Nodes.GetLength(0) - 2, (Nodes.GetLength(1) / 2) - 1]; // Middle right.
                case 1:
                    return Nodes[Nodes.GetLength(0) / 2, 1]; // Top middle.
                case 2:
                    return Nodes[1, (Nodes.GetLength(1) / 2) - 1]; // Middle left.
                case 3:
                    return Nodes[Nodes.GetLength(0) / 2, Nodes.GetLength(1) - 3]; // Bottom middle.
                default:
                    return Nodes[Nodes.GetLength(0) - 2, Nodes.GetLength(1) / 2 - 1]; // Middle right.
            }
        }

        // Loads all of the textures needed for the map. Also calls the LoadContent method for each
        // player and shadow copy to load each individual sprite image.
        public override void LoadContent(ContentManager Content)
        {
            base.LoadContent(Content);
            fontGame = content.Load<SpriteFont>("SpriteFonts/fontGame");

            // Loads the sprite for each player.
            for (int intPlayerIndex = 0; intPlayerIndex <= 3; intPlayerIndex++)
            {
                player[intPlayerIndex].LoadContent(Content, "Sprites/PlayerSprite");
            }

            // Loads the sprite for each initial copy.
            foreach (ShadowCopy shadowcopy in shadowCopy)
                shadowcopy.LoadContent(Content, "Sprites/ShadowCopySprite");

            tx2ShadowCopy = shadowCopy[0].EntityImage; // Stores the texture of shadow copies.

            // Loads the map to play on.
            LoadMap(layerGameGrid);
        }

        // Calls the screen method of UnloadContent to unload any content used for this screen.
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        // Checks for key presses and processes changes accordingly each update cycle. This deals
        // with a number of things such as AI calculation updates, player movement, player interaction,
        // lamps, door movement and also to detect if the user has pressed escape to show the pause menu.
        public override void Update(GameTime gameTime)
        {
            if (ScreenManager.Instance.SingleKeyPress(Keys.Escape))
            {
                ScreenManager.Instance.PushTranslucentScreen(new PauseScreen());
            }

            for (int intPlayerIndex = 0; intPlayerIndex <= 3; intPlayerIndex++)
            {
                if (player[intPlayerIndex].Dead != true)
                {
                    Keys[] playerKeys = new Keys[4];
                    playerKeys[0] = InputKeys[intPlayerIndex, 0];
                    playerKeys[1] = InputKeys[intPlayerIndex, 1];
                    playerKeys[2] = InputKeys[intPlayerIndex, 2];
                    playerKeys[3] = InputKeys[intPlayerIndex, 3];
                    player[intPlayerIndex].Update(gameTime, playerKeys, layerGameGrid, addShadowCopy, shadowCopy, tx2ShadowCopy);
                }
            }

            if (intProgressionTime > 5) // After 5 seconds, spawn shadow copies.
            // Updates each initial copy.
            foreach (ShadowCopy shadowcopy in shadowCopy)
                    if (shadowcopy != null) shadowcopy.Update(gameTime, player, layerGameGrid.Nodes);

            // Updates each additional copy if it has been instantiated.
            foreach (AddShadowCopy shadowcopy in addShadowCopy)
                if (shadowcopy != null) shadowcopy.Update(gameTime, player, layerGameGrid);

            // Every 20 seconds after the start of the game, respawn initial shadow copies if they were erased.
            if (intProgressionTime == 20 || intProgressionTime == 40 || intProgressionTime == 60 || intProgressionTime == 80)
                if (shadowCopy.Count(sc => sc == null) > 0) // Checks if the number of null shadow copies is greater than 0.
                {
                    int intRespawnIndex = Array.IndexOf(shadowCopy, null); // Stores the index of a null item in the array to respawn a shadow copy.
                    shadowCopy[intRespawnIndex] = new ShadowCopy(); // Respawn a shadow copy at its default spawn point.
                    shadowCopy[intRespawnIndex].Initialize(InitCopyPos(intRespawnIndex, layerGameGrid.Nodes));
                    shadowCopy[intRespawnIndex].EntityImage = tx2ShadowCopy;
                    shadowCopy[intRespawnIndex].Animation.AnimationImage = tx2ShadowCopy;
                }

            layerGameGrid.CheckNodes(player, shadowCopy, addShadowCopy);

            intTimeSinceFrame += gameTime.ElapsedGameTime.Milliseconds; // Adds the number of milliseconds passed each frame
            if (intTimeSinceFrame >= 1000)  // until a second has passed.
            {
                intProgressionTime += 1; // Then a second is added to the progression time and the time since frame is reset.
                intTimeSinceFrame = 0;
            }
            // After 100 seconds, or if all the players have died, end the game.
            if (intProgressionTime == 100 || (player[0].Dead && player[1].Dead && player[2].Dead && player[3].Dead))
                EndGame(player);
        }

        // Draws all textures for players, shadow copies and map objects to the screen.
        public override void Draw(SpriteBatch spriteBatch)
        {
            layerGameGrid.DrawNodes(spriteBatch);

            int intHealthSpacing = 20; // Stores a value to space out player health text at the bottom of the screen.
            for (int intPlayerIndex = 0; intPlayerIndex <= 3; intPlayerIndex++)
            {
                if (player[intPlayerIndex].Dead != true)
                {
                    player[intPlayerIndex].Draw(spriteBatch);
                    spriteBatch.DrawString(fontGame, "Player" + intPlayerIndex + " : " + player[intPlayerIndex].Health, new Vector2(intHealthSpacing, 550), player[intPlayerIndex].Colour);
                    intHealthSpacing += 200;
                }
            }
            // Display the time remaining by subtracting the time progressed from the length of the game.
            spriteBatch.DrawString(fontGame, "Time Left: " + (100 - intProgressionTime), new Vector2(900, 550), Color.White);

            if (intProgressionTime > 5) // After 5 seconds, spawn shadow copies.
                foreach (ShadowCopy shadowcopy in shadowCopy)
                    if (shadowcopy != null) shadowcopy.Draw(spriteBatch); // Draws each initial shadow copy.

            // Draws each shadow copy which has been instantiated.
            foreach (AddShadowCopy shadowcopy in addShadowCopy)
                if (shadowcopy != null) shadowcopy.Draw(spriteBatch);
        }

        // Decides how many stars were achieved for completing the level.
        public void EndGame(Player[] player)
        {
            int intStarScore = 0; // Stores how many stars were achieved.
            for (int intPlayerIndex = 0; intPlayerIndex <= 3; intPlayerIndex++) // For each player,
            {
                if (player[intPlayerIndex].Dead != true) // If the player wasn't dead.
                {
                    if (player[intPlayerIndex].Health >= 750) // If it had more than 750 health,
                    {
                        intStarScore = 3; // 3 stars were achieved. Quit the loop.
                        break;
                    }
                    else if (player[intPlayerIndex].Health >= 500) // If it had more than 500 health,
                    {
                        intStarScore = 2; // 2 stars were achieved. Continue looping incase another player had 750 health.
                    }
                    else if (intStarScore != 2) // If a previous player didn't have 2 as a score,
                        intStarScore = 1; // 1 star was achieved. Continue looping incase another had more health.
                }
            }
            ScreenManager.MessageBox(new IntPtr(0), "You scored: " + intStarScore + " Stars", "Notice", 0);
            // SEARCH FOR THE MAP AND SAVE IT'S SCORE!
            int intMapIndex = ScreenManager.Instance.MapData.FindIndex(map => map.MapName == strMapName);

            if (ScreenManager.Instance.MapData[intMapIndex].StarScore < intStarScore)
                ScreenManager.Instance.MapData[intMapIndex].StarScore = intStarScore;
            ScreenManager.Instance.SaveSettingsAndData();
            ScreenManager.Instance.PopScreen(); // Shows how many stars were scored and exits to menu.
        }


        // Loads a map into the grid.
        public void LoadMap(GridLayer grid)
        {
            string[,] strMap = new string[grid.Nodes.GetLength(0), grid.Nodes.GetLength(1)]; // Stores each node's texture value.
            string[] strAllLines = null; // Stores the lines to load in for the map's texture names.
            // Stores the path to the map file to load in.
            string strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ShadowMaze\" + strMapName + ".csv";

            if (File.Exists(strFilePath))
            {
                for (int intError = 0; intError < 3; intError++)
                    try
                {
                    strAllLines = File.ReadAllLines(strFilePath); // All the lines of the map file are loaded in to a single string array.
                        break; // If the lines were read successfully, break out of this exception catching loop.
                }
                catch (IOException) // If the user has the map file open preventing it from being accessed, give the user 3 warnings to close it. 
                {
                    switch (intError)
                    {
                        case 0: // First warning.
                            ScreenManager.MessageBox(new IntPtr(0), "Attempting to load map. Another program is using the map file in Documents/ShadowMaze, please close it and click ok.", "Error", 0);
                            break;
                        case 1: // Second warning.
                            ScreenManager.MessageBox(new IntPtr(0), "If the map file is not closed, the map will not be loaded. Please close it and click ok.", "Error", 0);
                            break;
                        case 2: // Third warning.
                            ScreenManager.MessageBox(new IntPtr(0), "Map could not be loaded as the file is still in use. Close the file and try again if you wish to play.", "Error", 0);
                            ScreenManager.Instance.PopScreen();
                            return; // Exit the method if the lines couldn't be read.
                    }
                }
            }
            else
            { // If the file does not exist, display an error message.
                ScreenManager.MessageBox(new IntPtr(0), "The map file doesn't exist. Please do not tamper with game files. It is recommended that you delete this map from this menu to avoid future problems.", "Error", 0);
                ScreenManager.Instance.PopScreen();
                return;
            }
            // The two dimensional map array is filled by splitting each individual line in the allLines array.
            // The individual values correspond to the texture to be used for the node at that position.
            for (int intRow = 0; intRow < strAllLines.GetLength(0); intRow++)
            {
                string[] line = strAllLines[intRow].Split(',');
                for (int intCol = 0; intCol < line.GetLength(0) - 1; intCol++)
                {
                    strMap[intCol, intRow] = line[intCol];
                }
            }
            SetTextures(strMap, grid);
        }
        // Loops through the nodes on the map and set each node's texture according to the map string array.
        public void SetTextures(string[,] map, GridLayer grid)
        {
            for (int intRow = 0; intRow < map.GetLength(1); intRow++)
            {
                for (int intCol = 0; intCol < map.GetLength(0); intCol++)
                {
                    // Based on the value in the element of the map array, set the node's texture.
                    try
                    {
                        grid.Nodes[intCol, intRow].Texture = content.Load<Texture2D>("Sprites/" + map[intCol, intRow]);
                        grid.Nodes[intCol, intRow].Texture.Name = map[intCol, intRow];
                    }
                    catch (ContentLoadException) // If the map file's contents were incorrect, use grass as a default texture.
                    {
                        grid.Nodes[intCol, intRow].Texture = content.Load<Texture2D>("Sprites/grs");
                        grid.Nodes[intCol, intRow].Texture.Name = "grs";
                    }

                    // If the texture was grass, fruit, blade, lamp or light beam, make the node traversable.
                    if (map[intCol, intRow] == "grs" || map[intCol, intRow] == "drk" || map[intCol, intRow] == "bld" ||
                        map[intCol, intRow] == "lbm" || map[intCol, intRow] == "frt" || grid.Nodes[intCol, intRow].Texture.Name == "grs")
                        grid.Nodes[intCol, intRow].Traversable = true;
                    else grid.Nodes[intCol, intRow].Traversable = false;
                }
            }
        }
    }
}
