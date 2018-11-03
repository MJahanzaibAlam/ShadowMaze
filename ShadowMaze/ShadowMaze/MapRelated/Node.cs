using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMaze
{
    public class Node
    {
        private Node nodeParent; // Stores the parent of this node for pathfinding.
        private Texture2D tx2Tile; // Stores the texture of the tile.
        private Rectangle rectTile; // Stores the rectangle surrounding the tile.
        private bool boolTraversable; // Stores whether the node can be walked over.
        private int intF; // Stores the f score of the node for pathfinding (how efficient the route is).
        private int intG; // Stores the g score of the node for pathfinding (cost of moving to this adjacent node).
        private int intH; // Stores the h score of the node for pathfinding (estimated cost of moving from this node to the target node).
        private Color colourBright = Color.Gray; // Stores the colour of the node when drawn (dark or light based on lighting/visibility).

        public bool Traversable
        {
            get { return boolTraversable; }
            set { boolTraversable = value; }
        }

        public Color Colour
        {
            get { return colourBright; }
            set { colourBright = value; }
        }

        public Node Parent
        {
            get { return nodeParent; }
            set { nodeParent = value; }
        }

        public int F
        {
            get { return intF; }
            set { intF = value; }
        }

        public int G
        {
            get { return intG; }
            set { intG = value; }
        }

        public int H
        {
            get { return intH; }
            set { intH = value; }
        }

        // Calculates distance cost from start node to an adjacent node.
        public void CalculateG(Node nodeTarget)
        { // The g-score is cumulative so the target node's score is also added.
            // If the node is diagonal to the current node, its cost of movement is higher.
            if (TileX != nodeTarget.TileX && TileY != nodeTarget.TileY)
            {
                intG = nodeTarget.G + 14;
            }
            else intG = nodeTarget.G + 10;
        }
        
        // Calculates H cost using the diagonal shortcut method.
        public void CalculateH(Node nodeTarget)
        {
            int intXDiff = Math.Abs(TileX - nodeTarget.TileX);
            int intYDiff = Math.Abs(TileY - nodeTarget.TileY);
            
            if (intXDiff > intYDiff)
                intH = 14 * intYDiff + 10 * (intXDiff - intYDiff);
            else intH = 14 * intXDiff + 10 * (intYDiff - intXDiff);
        }

        // Calculates f cost by adding g and h costs.
        public void CalculateF()
        {
            intF = intG + intH; // F = G + H
        }

        public Texture2D Texture
        {
            get { return tx2Tile; }
            set { tx2Tile = value; }
        }

        public Rectangle Tile
        {
            get { return rectTile; }
        }

        public int TileWidth
        {
            get { return rectTile.Width; }
            set { rectTile.Width = value; }
        }

        public int TileHeight
        {
            get { return rectTile.Height; }
            set { rectTile.Height = value; }
        }

        public int TileX
        {
            get { return rectTile.X; }
            set { rectTile.X = value; }
        }

        public int TileY
        {
            get { return rectTile.Y; }
            set { rectTile.Y = value; }
        }

        // Takes a rectangle as a parameter and checks if the node's tile contains it.
        public bool Contains(Rectangle rect)
        {
            return rectTile.Contains(rect);
        }
    }
}