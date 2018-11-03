using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMaze
{
    /// <summary>
    /// An object of this class displays a visible grid for the map editor.
    /// </summary>
    public class VisibleGrid
    {
        private Texture2D tx2Node; // Texture used to display the square nodes on the screen.

        // Loads the images and sprite fonts needed for showing information.
        public void LoadContent(ContentManager Content)
        {
            tx2Node = Content.Load<Texture2D>("Sprites/Node");
        }

        // Draws the outline of the grid onto the screen.
        public void Draw(SpriteBatch spriteBatch, GridLayer Grid)
        {
            DrawNodes(spriteBatch, Grid);
        }

        // Visually draws the outline of nodes of the grid layer onto the screen using a texture.
        public void DrawNodes(SpriteBatch spriteBatch, GridLayer Grid)
        {
            for (int intRow = 0; intRow < Grid.Nodes.GetLength(1); intRow++)
            {
                for (int intCol = 0; intCol < Grid.Nodes.GetLength(0); intCol++)
                {
                    spriteBatch.Draw(tx2Node, Grid.Nodes[intCol, intRow].Tile, Color.White);
                }
            }
        }
    }
}
