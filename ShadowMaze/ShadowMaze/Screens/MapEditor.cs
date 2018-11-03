using System;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ShadowMaze
{
    // Allows the user to create custom maps by selecting icons of objects and clicking areas on a grid to place them.
    class MapEditor : Screen
    {
        private GridLayer MapEditGrid; // Stores a grid which is 3/4 the size of the window.
        private string[,] strMap; // Stores the string array representing the map.
        private Texture2D[] tx2MapObjects; // Stores the textures for map objects.
        private Texture2D tx2Background; // Stores the background image.
        private int intSelectedTexture; // Stores the texture the user wants to place.
        private Vector2 v2ToolBoxPos; // Stores the position of the toolbox.
        private Vector2[,] v2ToolBoxItems; // Stores the positions of the objects in the toolbox.
        private SpriteFont fontEditor; // Stores the spritefont used for the map editor.
        private string strMapName; // Stores the name of the map to add or load.
        private KeyChecker keyChecker; // Checks the keyboard for input text.
        private Button[] buttons; // Stores the buttons used for this screen.
        private VisibleGrid visGrid; // Used to display a visible grid on the map editor.
        private Texture2D tx2Outline; // Stores a blank texture with an outline for outlining toolbox items.

        // Instantiates grid, key checker and arrays etc. Sets button text and positions.
        public override void Initialize()
        {
            base.Initialize();
            MapEditGrid = new GridLayer(false); // Creates a small grid for editing.
            visGrid = new VisibleGrid(); // Used to display the grid.
            strMap = new string[MapEditGrid.Columns, MapEditGrid.Rows]; // Stores the map to save to a file.

            keyChecker = new KeyChecker();
            v2ToolBoxPos = new Vector2(800, 100);
            v2ToolBoxItems = new Vector2[2, 4];
            
            // Sets the positions for the toolbox items by looping through each item and adding the spacing value.
            Vector2 v2Spacing = new Vector2(0, 0);
            for (int intColumn = 0; intColumn < v2ToolBoxItems.GetLength(0); intColumn++)
            {
                for (int intItemIndex = 0; intItemIndex < v2ToolBoxItems.GetLength(1); intItemIndex++)
                {
                    v2ToolBoxItems[intColumn, intItemIndex] = v2ToolBoxPos + v2Spacing;
                    v2Spacing.Y += 50;
                }
                v2Spacing.X += 50;
                v2Spacing.Y = 0;
            }
            
            buttons = new Button[3];
            for (int intButtonIndex = 0; intButtonIndex < buttons.GetLength(0); intButtonIndex++)
                buttons[intButtonIndex] = new Button();

            buttons[0].Text = "Open Map";
            buttons[1].Text = "Save Map";
            buttons[2].Text = "Back To Menu";

            buttons[0].Position = new Vector2(650, 400);
            buttons[1].Position = new Vector2(650, 500);
            buttons[2].Position = new Vector2(0, 500);

            strMapName = ""; // Sets the initial string as nothing so that it can be added to by the key checker.
        }
        
        // Loads all of the textures for buttons and objects and sprite fonts for text.
        // Also loads textures for nodes.
        public override void LoadContent(ContentManager Content)
        {
            base.LoadContent(Content);
            visGrid.LoadContent(Content);
            LoadTextures(); // Loads the textures for each tool box item.
            fontEditor = content.Load<SpriteFont>("SpriteFonts/fontSettings"); // Re-uses the settings font.
            tx2Background = content.Load<Texture2D>("Sprites/Background");
            tx2Outline = content.Load<Texture2D>("Sprites/Node");

            // The buttons' images/fonts are loaded.
            for (int intButtonIndex = 0; intButtonIndex < buttons.GetLength(0); intButtonIndex++)
            {
                buttons[intButtonIndex].Image = content.Load<Texture2D>("Sprites/ButtonShort");
                buttons[intButtonIndex].Font = fontEditor;
                buttons[intButtonIndex].SetTextPosition();
            }
            SetGrassTexture();
        }

        // Loads the texture for each individual toolbox item.
        public void LoadTextures()
        {
            // Stores the strings representing the map objects.
            string[] strObjects = new string[] { "grs", "wtr", "stw", "tre", "drk", "bld", "lbm", "frt" };
            tx2MapObjects = new Texture2D[strObjects.GetLength(0)];
            // Loops through the map object textures array loading each texture and settings its name.
            for (int intObjIndex = 0; intObjIndex < strObjects.GetLength(0); intObjIndex++)
            {
                tx2MapObjects[intObjIndex] = content.Load<Texture2D>("Sprites/" + strObjects[intObjIndex]);
                tx2MapObjects[intObjIndex].Name = strObjects[intObjIndex];
            }
        }

        // Sets all the nodes' textures as grass by default so the user can't save a map with null values.
        public void SetGrassTexture()
        {
            for (int intRow = 0; intRow < strMap.GetLength(1); intRow++)
            {
                for (int intCol = 0; intCol < strMap.GetLength(0); intCol++)
                {
                    strMap[intCol, intRow] = "grs";
                    MapEditGrid.Nodes[intCol, intRow].Texture = tx2MapObjects[0];
                }
            }
        }

        // Unloads all the content that was used for this screen.
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        // Will check for mouse movement/clicks over buttons and the map.
        public override void Update(GameTime gameTime)
        {
            keyChecker.Update(); // Take input from the user via the key checker class.
            strMapName = keyChecker.InputText; // Make the key checker's input text the map name.
            TextureSelection(); // Check which texture the user wishes to select.
            GridChange(); // Check if the user has tried to change the map.
            DetectButtonPresses(); // Process button clicks.
            ResetSpawnPoints(); // Resets the texture of spawn areas to grass.
            ResetBottomRow(); // Resets the texture of the bottom row to trees.
        }

        // Draws all buttons, text, toolbox items and the map to the screen.
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tx2Background, Vector2.Zero, Color.White);
            DrawTextureToolBox(spriteBatch);
            MapEditGrid.DrawNodes(spriteBatch);

            for (int intButtonIndex = 0; intButtonIndex < 3; intButtonIndex++)
                buttons[intButtonIndex].DrawButtonAndText(spriteBatch);

            spriteBatch.DrawString(fontEditor, strMapName, new Vector2(0, 450), Color.AliceBlue);
            spriteBatch.DrawString(fontEditor, "Use keyboard to enter map name", new Vector2(0, 400), Color.AliceBlue);
            spriteBatch.Draw(tx2Outline, v2ToolBoxItems[intSelectedTexture / 4, intSelectedTexture % 4], Color.Black);
            visGrid.Draw(spriteBatch, MapEditGrid);
        }

        // Draws the texture toolbox to screen.
        public void DrawTextureToolBox(SpriteBatch spriteBatch)
        {
            // Stores the index of the currently selected object in the toolbox.
            int intObjectNumber = 0;
            for (int intColumn = 0; intColumn < v2ToolBoxItems.GetLength(0); intColumn++)
            {
                for (int intItemIndex = 0; intItemIndex < v2ToolBoxItems.GetLength(1); intItemIndex++)
                {
                    spriteBatch.Draw(tx2MapObjects[intObjectNumber], v2ToolBoxItems[intColumn, intItemIndex], Color.White);
                    intObjectNumber++;
                }
            }
        }
        
        // Checks if the mouse pointer is within a tool box texture.
        private bool TextureContainsMouse(Vector2 v2TexturePos, Texture2D tx2Texture)
        {
            if (ScreenManager.Instance.MouseState.X > v2TexturePos.X &&
                ScreenManager.Instance.MouseState.Y > v2TexturePos.Y &&
                ScreenManager.Instance.MouseState.X < v2TexturePos.X + tx2Texture.Width &&
                ScreenManager.Instance.MouseState.Y < v2TexturePos.Y + tx2Texture.Height)
            {
                return true;
            }
            else return false;
        }

        // Checks if a texture has been clicked on. Sets the selected texture as the clicked texture.
        public void DetectTextureSelect(Vector2 v2TexturePos, Texture2D tx2Texture)
        {
            if (TextureContainsMouse(v2TexturePos, tx2Texture) && ScreenManager.Instance.SingleMouseClick())
            {
                intSelectedTexture = Array.IndexOf(tx2MapObjects, tx2Texture);
            }
        }
        
        // Detects if a user has tried to select an object texture by clicking on an item in the toolbox.
        public void TextureSelection()
        {
            int intObjectNumber = 0;
            for (int intColumn = 0; intColumn < v2ToolBoxItems.GetLength(0); intColumn++)
            {
                for (int intItemIndex = 0; intItemIndex < v2ToolBoxItems.GetLength(1); intItemIndex++)
                {
                    DetectTextureSelect(v2ToolBoxItems[intColumn, intItemIndex], tx2MapObjects[intObjectNumber]);
                    intObjectNumber++;
                }
            }
        }

        // Detects a mouse click on the grid meaning that the user wishes to set a texture.
        public void DetectGridClick(int intCol, int intRow)
        {
            if (MapEditGrid.Nodes[intCol, intRow].Tile.Contains(new Point(ScreenManager.Instance.MouseState.X, ScreenManager.Instance.MouseState.Y)) 
                && (ScreenManager.Instance.MouseState.LeftButton == ButtonState.Pressed))
                SetTexture(intCol, intRow);
        }

        // Loops through the grid and checks if the user has made a change.
        public void GridChange()
        {
            for (int intRow = 0; intRow < MapEditGrid.Rows; intRow++)
            {
                for (int intCol = 0; intCol < MapEditGrid.Columns; intCol++)
                {
                    DetectGridClick(intCol, intRow);
                }
            }
        }
        
        // Sets a nodes texture equal to the selected texture.
        public void SetTexture(int intCol, int intRow)
        {
            MapEditGrid.Nodes[intCol, intRow].Texture = tx2MapObjects[intSelectedTexture];
            strMap[intCol, intRow] = tx2MapObjects[intSelectedTexture].Name;
        }

        // Saves the map to a new file and adds the map data to the main map data array.
        public void SaveMap()
        {
            // Stores 6 if the map should be saved, otherwise, if the user clicked no
            // in the message box, stores a different number and the map is not saved.
            int intMsgResult = 6; 
            string strFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ShadowMaze\" + strMapName + ".csv";
            
            // Displays a message box asking if the user wants to overwrite an existing file.
            if (File.Exists(strFilePath))
                intMsgResult = ScreenManager.MessageBox(new IntPtr(0), "A map already exists with this name, overwrite it?", "Notice", 3);

            // If the user did not click yes in the message box or the message box was not shown due to the file
            // not previously existing, the pre-existing map is cleared.
            if (intMsgResult == 6)
            {
                for (int intError = 0; intError < 3; intError++)
                    try
                    {
                        File.WriteAllText(strFilePath, ""); // Creates a file or clears an existing file.
                        break; // If the map file could be cleared/crated, break out of this exception catching loop.
                    }
                    catch (IOException) // If the user has the pre-existing map file open preventing it from being overwritten, give the user 3 warnings to close it. 
                    {
                        switch (intError)
                        {
                            case 0: // First warning.
                                ScreenManager.MessageBox(new IntPtr(0), "Attempting to overwrite map. Another program is using the map file in Documents/ShadowMaze, please close it and click ok.", "Error", 0);
                                break;
                            case 1: // Second warning.
                                ScreenManager.MessageBox(new IntPtr(0), "If the map file is not closed, the map will not be saved. Please close it and click ok.", "Error", 0);
                                break;
                            case 2: // Third warning.
                                ScreenManager.MessageBox(new IntPtr(0), "Map could not be saved as the file is still in use. Close the file and try again if you wish to save it.", "Error", 0);
                                return; // If the map couldn't be created/cleared, exit the method.
                        }
                    }
                for (int intStrIndex = 0; intStrIndex < MapEditGrid.Nodes.GetLength(1); intStrIndex++)
                {
                    string strLineToWrite = ""; // Stores the next line to write to file.
                    for (int intLineIndex = 0; intLineIndex < MapEditGrid.Nodes.GetLength(0); intLineIndex++)
                    { // Loops through the map line by line and appends each individual line to the file.
                        strLineToWrite += MapEditGrid.Nodes[intLineIndex, intStrIndex].Texture.Name + ",";
                    }
                    for (int intError = 0; intError < 3; intError++)
                        try
                    {
                        File.AppendAllText(strFilePath, strLineToWrite + '\n'); // Add a line of map data to the map file.
                            break; // If the line could be added to the file, break out of this exception catching loop.
                    }
                    catch (IOException) // If the user has the newly created map file open preventing it from being added to with map data, give the user 3 warnings to close it. 
                    {
                        switch (intError)
                        {
                            case 0: // First warning.
                                ScreenManager.MessageBox(new IntPtr(0), "Attempting to save map. Another program is using the map file in Documents/ShadowMaze, please close it and click ok.", "Error", 0);
                                break;
                            case 1: // Second warning.
                                ScreenManager.MessageBox(new IntPtr(0), "If the map file is not closed, the map will not be saved. Please close it and click ok.", "Error", 0);
                                break;
                            case 2: // Third warning.
                                ScreenManager.MessageBox(new IntPtr(0), "Map could not be saved as the file is still in use. Close the file and try again if you wish to save it.", "Error", 0);
                                return; // If the line couldn't be added to the file, exit this method.
                        }
                    }
                }
                // Stores the map (name and score) which needs to be added to the map data list.
                Map mapToAdd = new Map(strMapName, 0);
                if (ScreenManager.Instance.MapData.Contains(mapToAdd)) // If the map already existed, remove it from the file.
                    ScreenManager.Instance.MapData.Remove(mapToAdd);
                ScreenManager.Instance.MapData.Add(new Map(strMapName, 0)); // Adds the map data to the main map data array with its name.
                ScreenManager.Instance.SaveSettingsAndData(); // Saves the main map data array to file.
                ScreenManager.MessageBox(new IntPtr(0), "Map saved!", "Notice", 0); // Displays a message box notifying the user that the map was saved.
            }
        }
        
        // Loads a map into the map editor based on the inputted map name.
        public void Openmap()
        {
            // Stores the lines of the map file to load into the grid.
            string[] strAllLines = null;
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
                            ScreenManager.MessageBox(new IntPtr(0), "Map could not be loaded as the file is still in use. Close the file and try again if you wish to edit it.", "Error", 0);
                            return; // Exit the method if the map couldn't be read from the file.
                    }
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
                SetTextures(); // Sets the textures for each node.
            }
            else ScreenManager.MessageBox(new IntPtr(0), "Map not found!", "Notice", 0); // If the file didn't exist, display an error message.
        }

        // Loops through the nodes on the map and set each node's texture according to the map string array.
        public void SetTextures()
        {
            for (int intRow = 0; intRow < strMap.GetLength(1); intRow++)
            {
                for (int intCol = 0; intCol < strMap.GetLength(0); intCol++)
                {
                    // Based on the value in the element of the map array, set the node's texture.
                    try
                    {
                        MapEditGrid.Nodes[intCol, intRow].Texture = content.Load<Texture2D>("Sprites/" + strMap[intCol, intRow]);
                    }
                    catch (ContentLoadException) // If the map file's contents were incorrect, use grass as a default texture.
                    {
                        MapEditGrid.Nodes[intCol, intRow].Texture = content.Load<Texture2D>("Sprites/grs");
                    }
                }
            }
        }

        // Sets the texture of the spawning locations to grass to ensure user does not block off spawning locations.
        private void ResetSpawnPoints()
        {
            MapEditGrid.Nodes[MapEditGrid.Nodes.GetLength(0) - 2, (MapEditGrid.Nodes.GetLength(1) / 2) - 1].Texture = tx2MapObjects[0]; // Middle right.
            MapEditGrid.Nodes[MapEditGrid.Nodes.GetLength(0) / 2, 0].Texture = tx2MapObjects[0]; // Top middle.
            MapEditGrid.Nodes[0, (MapEditGrid.Nodes.GetLength(1) / 2) - 1].Texture = tx2MapObjects[0]; // Middle left.
            MapEditGrid.Nodes[MapEditGrid.Nodes.GetLength(0) / 2, MapEditGrid.Nodes.GetLength(1) - 2].Texture = tx2MapObjects[0]; // Bottom middle.
            MapEditGrid.Nodes[MapEditGrid.Nodes.GetLength(0) - 1, MapEditGrid.Nodes.GetLength(1) / 2 - 1].Texture = tx2MapObjects[0]; // Middle right.
            MapEditGrid.Nodes[MapEditGrid.Nodes.GetLength(0) / 2, 1].Texture = tx2MapObjects[0];
            MapEditGrid.Nodes[1, (MapEditGrid.Nodes.GetLength(1) / 2) - 1].Texture = tx2MapObjects[0]; // Middle left.
            MapEditGrid.Nodes[MapEditGrid.Nodes.GetLength(0) / 2, MapEditGrid.Nodes.GetLength(1) - 3].Texture = tx2MapObjects[0]; // Bottom middle.
        }

        // Sets the textures of the nodes at the bottom of the map as trees to display information more clearly.
        private void ResetBottomRow()
        {
            int intRow = MapEditGrid.Nodes.GetLength(1) - 1;
            for (int intCol = 0; intCol < MapEditGrid.Nodes.GetLength(0); intCol++)
            {
                MapEditGrid.Nodes[intCol, intRow].Texture = tx2MapObjects[3]; // Sets the texture as a tree.
            }
        }

        // Detects if a button was pressed and handles this accordingly.
        private void DetectButtonPresses()
        {
            if (buttons[0].DetectButtonClick()) // Opens a map.
                Openmap();
            if (buttons[1].DetectButtonClick()) // Saves the map.
                SaveMap();
            if (buttons[2].DetectButtonClick())
                ScreenManager.Instance.PopScreen(); // Returns to the main menu.
        }
    }
}