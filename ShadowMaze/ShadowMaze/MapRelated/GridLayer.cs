using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMaze
{
    /// <summary>
    /// Objects of this class form a grid in the game window made of rectangles (nodes)
    /// that are checked by methods of the colissions class.
    /// </summary>
    public class GridLayer
    {
        private Node[,] nodes; // Stores all the nodes in a 2d array representing a grid.
        private int intNodeWidth; // Stores the width of each node.
        private int intNodeHeight; // Stores the height of each node.
        private int intColumns; // Stores how many columns of nodes there are.
        private int intRows; // Stores how many rows of nodes there are.

        public Node[,] Nodes
        {
            get { return nodes; }
            set { value = nodes; }
        }

        public int Columns
        {
            get { return intColumns; }
        }

        public int Rows
        {
            get { return intRows; }
        }

        // Creates a new grid. The size is dependent on the Boolean value which is passed in.
        public GridLayer(bool FullScreen)
        {
            intNodeWidth = 50;
            intNodeHeight = 50;
            // The number of rows/columns is calculated by dividng the dimensions of the
            // screen by the dimensions of each Node.

            intColumns = (int)ScreenManager.Instance.ScreenDimensions.X / intNodeWidth;
            intRows = (int)ScreenManager.Instance.ScreenDimensions.Y / intNodeHeight;

            if (!FullScreen) // If the boolean value was false, decrease tile size by 25%.
            {
                intNodeWidth = (int)(intNodeWidth / 1.5);
                intNodeHeight = (int)(intNodeHeight / 1.5);
            }

            nodes = new Node[intColumns, intRows];
            InitializeNodes();
        }

        // Visually draws the nodes of the grid layer onto the screen using textures.
        public void DrawNodes(SpriteBatch spriteBatch)
        {
            for (int intRow = 0; intRow < intRows; intRow++)
            {
                for (int intCol = 0; intCol < intColumns; intCol++)
                {
                    if (nodes[intCol, intRow].Texture != null) // Ensures the game does not try to draw a null texture (due to bad map file).
                    {
                        spriteBatch.Draw(nodes[intCol, intRow].Texture, nodes[intCol, intRow].Tile, nodes[intCol, intRow].Colour);
                    }
                    nodes[intCol, intRow].Colour = Color.Gray; // Resets the node's colour.
                }
            }
        }


        // Sets the dimensions and position of each node by looping through each
        // row and column of nodes.
        public void InitializeNodes()
        {
            for (int intRow = 0; intRow < intRows; intRow++)
            {
                for (int intCol = 0; intCol < intColumns; intCol++)
                {
                    nodes[intCol, intRow] = new Node();
                    nodes[intCol, intRow].TileWidth = intNodeWidth;
                    nodes[intCol, intRow].TileHeight = intNodeHeight;
                    nodes[intCol, intRow].TileX = intCol * intNodeWidth;
                    nodes[intCol, intRow].TileY = intRow * intNodeHeight;

                    // The above two lines mean that the position of the node
                    // will be set in multiples of its dimensions. This prevents
                    // having to alter the for loops to go up in 50s for example
                    // as this would intergere with the array indices.
                }
            }
        }

        // Loops through all the nodes on the map to set entity nodes, detect collisions
        // and item pick ups.
        public void CheckNodes(Player[] player, ShadowCopy[] shadowCopy, AddShadowCopy[] addShadowCopy)
        {
            for (int intRow = 0; intRow < intRows; intRow++)
            {
                for (int intCol = 0; intCol < intColumns; intCol++)
                {
                    // Loops through player array to set each player's current node and 
                    // check if a player has passed a non-traversable object.
                    for (int intPlayerIndex = 0; intPlayerIndex <= 3; intPlayerIndex++)
                    {
                        SetEntityPosition(nodes[intCol, intRow], player[intPlayerIndex]);
                        DetectBoundaryCollision(nodes[intCol, intRow], player[intPlayerIndex]);
                        DetectCollision(nodes[intCol, intRow], player[intPlayerIndex]);
                        DetectItemPickUp(nodes[intCol, intRow], player[intPlayerIndex]);
                    }
                    for (int intCopyIndex = 0; intCopyIndex < shadowCopy.GetLength(0); intCopyIndex++)
                    {
                        if (shadowCopy[intCopyIndex] != null)
                            SetEntityPosition(nodes[intCol, intRow], shadowCopy[intCopyIndex]);
                    }
                    for (int intCopyIndex = 0; intCopyIndex < addShadowCopy.GetLength(0); intCopyIndex++)
                    {
                        if (addShadowCopy[intCopyIndex] != null)
                            SetEntityPosition(nodes[intCol, intRow], addShadowCopy[intCopyIndex]);
                    }
                    ResetFGH(nodes[intCol, intRow]);
                }
            }
        }

        // Reset the f,g, and h scores of each node from pathfinding calculations.
        public void ResetFGH(Node node)
        {
            node.G = 0;
            node.H = 0;
            node.F = 0;
        }

        // Checks the current node for an additional shadow copy and sets it's entity node according to the centre of its image.
        public void SetEntityPosition(Node node, AddShadowCopy shadowCopy)
        {
            // Stores the centre of the shadow copy based on its rectangle.
            Vector2 v2CopyCentre = shadowCopy.EntityPosition + new Vector2(shadowCopy.RectEntity.Width / 2, shadowCopy.RectEntity.Height / 2);
            if (v2CopyCentre != null)
                if (node.Tile.Contains(new Point((int)v2CopyCentre.X, (int)v2CopyCentre.Y)))
                {
                    shadowCopy.EntityNode = node;
                }
        }

        // Checks the current node for a shadow copy and sets it's entity node according to the centre of its image.
        public void SetEntityPosition(Node node, ShadowCopy shadowCopy)
        {
            // Stores the centre of the shadow copy based on its rectangle.
            Vector2 v2CopyCentre = shadowCopy.EntityPosition + new Vector2(shadowCopy.RectEntity.Width / 2, shadowCopy.RectEntity.Height / 2);
            if (v2CopyCentre != null)
                if (node.Tile.Contains(new Point((int)v2CopyCentre.X, (int)v2CopyCentre.Y)))
                {
                    shadowCopy.EntityNode = node;
                }
        }

        // Checks the current node for a player and sets it's entity node according to the centre of its image.
        public void SetEntityPosition(Node node, Player player)
        {
            // Stores the centre of the player based on its rectangle.
            Vector2 v2PlayerCentre = player.EntityPosition + new Vector2(player.RectEntity.Width / 2, player.RectEntity.Height / 2);
            if (node.Tile.Contains(new Point((int)v2PlayerCentre.X, (int)v2PlayerCentre.Y)))
            {
                player.EntityNode = node;
            }
        }

        // Detects collisions between players and map boundaries and handles them accordingly.
        public void DetectBoundaryCollision(Node node, Player player)
        {
            if (player.EntityPosition.X + player.RectEntity.Width > ScreenManager.Instance.ScreenDimensions.X)
            {
                player.EntityPositionX -= player.Distance;
            } // If the player has moved past the right side of the map, move the player left.
            if (player.EntityPosition.X < 0)
            {
                player.EntityPositionX += player.Distance;
            } // If the player has moved past the left of the map, move the player right.
            if (player.EntityPosition.Y + player.RectEntity.Height > ScreenManager.Instance.ScreenDimensions.Y)
            {
                player.EntityPositionY -= player.Distance;
            } // If the player has moved below the map, move the player up.
            if (player.EntityPosition.Y < 0)
            {
                player.EntityPositionY += player.Distance;
            } // If the player has moved above the map, move the player down.
        }

        // Detects collisions between players and map objects and handles them accordingly.
        public void DetectCollision(Node node, Player player)
        {
            // If a player has moved into an entity node and the node is not traversable...
            if (node.Tile.Intersects(player.RectEntity) && !node.Traversable)
            {
                if (player.EntityPositionX >= node.TileX)
                    player.EntityPositionX += player.Distance;
                // If the player has moved left into the node, move the player right.
                if (player.EntityPositionX <= node.TileX + node.TileWidth)
                    player.EntityPositionX -= player.Distance;
                // If the player has moved right into the node, move the player left.
                if (player.EntityPositionY >= node.TileY)
                    player.EntityPositionY += player.Distance;
                // If the player has moved up into a node, move the player down.
                if (player.EntityPositionY <= node.TileY + node.TileHeight)
                    player.EntityPositionY -= player.Distance;
                // If the player has moved down into a node, move the player up.
            }
        }

        // Detects if a player has picked up a map item. Sets the player's item
        // as the item, plays a sound, and changes the node's texture to grass.
        public void DetectItemPickUp(Node node, Player player)
        {
            if (node == player.EntityNode && ((node.Texture.Name == "drk") || (node.Texture.Name == "bld")
                || (node.Texture.Name == "lbm") || (node.Texture.Name == "frt")))
            {
                if (node.Texture.Name == "frt" && player.Health == 1000)
                    return; // If the player has full health, don't pick up the fruit to restore health.
                // If the player is in the same node as the current node and it contains a item.
                player.Item = node.Texture.Name; // Set the player's item as the item.
                ScreenManager.Instance.SoundManager.PlaySound(node.Texture.Name);
                node.Texture = nodes[0, 5].Texture; // and make the texture of the node which had the item grass.
            }
        }

        // Returns the node which contains the passed in point.
        public Node GetNode(Point point)
        {
            for (int intRow = 0; intRow < intRows; intRow++)
            {
                for (int intCol = 0; intCol < intColumns; intCol++)
                {
                    if (nodes[intCol, intRow].Tile.Contains(point))
                        return nodes[intCol, intRow];
                }
            }
            return new Node();
        }
    }
}