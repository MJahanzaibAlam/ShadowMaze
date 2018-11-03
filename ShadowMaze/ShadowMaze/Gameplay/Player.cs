using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ShadowMaze
{
    /// <summary>
    /// This class will contain all the methods for players including user input and movement.
    /// </summary>
    public class Player : Entity
    {
        private int intHealth; // Stores how much health the player has.
        private bool boolDead; // Stores whether or not the player has died (0 health).
        private string strItem; // Stores the player's currently equipped item.

        public string Item
        {
            get { return strItem; }
            set { strItem = value; }
        }

        public int Health
        {
            get { return intHealth; }
            set { intHealth = value; }
        }

        public bool Dead
        {
            get { return boolDead; }
        }

        // Calls the entity Initialization method to set the entity's current position based on the
        // node that has been passed into the initialize method. The speed and health of the player is set here.
        public override void Initialize(Node Node)
        {
            base.Initialize(Node);
            Speed = 100f;
            intHealth = 1000;
            boolDead = false;
        }

        // Calls the entity LoadContent method to load the sprite image for drawing using the sprite
        // path that is provided when this method is called from the game screen.
        public override void LoadContent(ContentManager Content, string strSpritePath)
        {
            base.LoadContent(Content, strSpritePath);
        }

        // Unloads all the content that was used for this player.
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        // Calls the player movement method, checks if the player has died and calls the process
        // item method if the player is holding an item.
        public void Update(GameTime gameTime, Keys[] InputKeys, GridLayer grid, AddShadowCopy[] addShadowCopies, ShadowCopy[] shadowCopies, Texture2D tx2ShadowCopy)
        {
            Distance = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            PlayerMovement(InputKeys, grid);
            RectEntityX = (int)EntityPositionX;
            RectEntityY = (int)EntityPositionY;

            if (intHealth <= 0) // If the player's health has reached or dropped below zero.
                boolDead = true; // The player has died.

            if (strItem != null) // If the player is holding an item, process it.
                ProcessItem(grid, addShadowCopies, shadowCopies, tx2ShadowCopy);
            Animation.Update(gameTime);
        }

        // Calls the entity draw method to draw the player's animation image.
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        // Processes whatever power-up/item the player has equipped.
        public void ProcessItem(GridLayer grid, AddShadowCopy[] addShadowCopies, ShadowCopy[] shadowCopies, Texture2D tx2ShadowCopy)
        {
            switch (strItem)
            {
                case "drk": // If the node contained a dark area, spawn an additional shadow copy.
                    {
                        if (addShadowCopies.Contains(null))
                        {
                            int intNewCopyIndex;
                            intNewCopyIndex = Array.IndexOf(addShadowCopies, null);
                            addShadowCopies[intNewCopyIndex] = new AddShadowCopy();
                            addShadowCopies[intNewCopyIndex].EntityImage = tx2ShadowCopy;
                            addShadowCopies[intNewCopyIndex].Initialize(EntityNode);
                        }
                    }
                    strItem = null;
                    break;
                case "bld":
                    EntityNode.Colour = Color.Red; // Make the colour of the node the player is in red, to show a blade is equipped.
                    for (int intCopyIndex = 0; intCopyIndex < shadowCopies.GetLength(0); intCopyIndex++)
                        if (shadowCopies[intCopyIndex] != null && EntityNode.Tile.Intersects(shadowCopies[intCopyIndex].EntityNode.Tile))
                        { // If a shadow copy exists and it intersects the player, remove the copy.
                            shadowCopies[intCopyIndex] = null;
                            strItem = null; // Remove the player's item after use.
                        }
                    for (int intCopyIndex = 0; intCopyIndex < addShadowCopies.GetLength(0); intCopyIndex++)
                        if (addShadowCopies[intCopyIndex] != null && EntityNode.Tile.Intersects(addShadowCopies[intCopyIndex].EntityNode.Tile))
                        { // If a shadow copy exists and it intersects the player, remove the copy.
                            addShadowCopies[intCopyIndex] = null;
                            strItem = null; // Remove the player's item after use.
                        }
                    // if a shadow copy is in the same node as a player with a blade. Remove the shadow copy.
                    break;
                case "lbm": // Casts a light ray where the player is facing from the centre of the player.
                    LightRay lightBeam = new LightRay(EntityPosition + new Vector2(RectEntityWidth / 2, RectEntityHeight / 2), EntityDirection, 200, grid);
                    BeamKillCopy(grid, lightBeam, addShadowCopies, shadowCopies); // If the beam intersects a shadow copy, remove it. 
                    break;
                case "frt":
                    intHealth = 1000; // If the player has a fruit, reset the player's health to 100.
                    strItem = null;
                    break;
            }
        }

        // Checks if a shadow copy is within the range of a light beam and removes it if it is.
        public void BeamKillCopy(GridLayer grid, LightRay lightBeam, AddShadowCopy[] addShadowCopies, ShadowCopy[] shadowCopies)
        { // Check each point in the beam, if the point is in the same node as a copy, remove the shadow copy.
            foreach (Point point in lightBeam.Points)
            {
                for (int intCopyIndex = 0; intCopyIndex < shadowCopies.GetLength(0); intCopyIndex++)
                    if (shadowCopies[intCopyIndex] != null && shadowCopies[intCopyIndex].RectEntity.Intersects(grid.GetNode(point).Tile))
                    { // Only check and remove the copy if it actually exists.
                        shadowCopies[intCopyIndex] = null;
                        strItem = null; // Removes the player's item after use.
                    }
                for (int intCopyIndex = 0; intCopyIndex < addShadowCopies.GetLength(0); intCopyIndex++)
                    if (addShadowCopies[intCopyIndex] != null && addShadowCopies[intCopyIndex].RectEntity.Intersects(grid.GetNode(point).Tile))
                    { // Only check and remove the copy if it actually exists.
                        addShadowCopies[intCopyIndex] = null;
                        strItem = null; // Removes the player's item after use.
                    }
                grid.GetNode(point).Colour = Color.White; // Makes any nodes in the beam brighter.
            }
        }

        // Based on the Input keys provided and the distance calculated in the update
        // method, the position of the player is changed. The direction to move in is calculated
        // by direction vector = (cos(angle), sin(angle)). Animation is updated accordingly.
        public void PlayerMovement(Keys[] InputKeys, GridLayer grid)
        {
            Vector2 v2NewPos; // Stores the position to which the player is being moved.
            float rotation = 0;
            Animation.Active = false;
            for (int intKeyIndex = 0; intKeyIndex < 4; intKeyIndex++)
            { // Loops through each key in the key array, checks if it is pressed and moves the player in the direction
                // corresponding to the key. See game screen initialize method for full keys array.
                if (ScreenManager.Instance.KeyState.IsKeyDown(InputKeys[intKeyIndex]))
                {
                    EntityRotation = rotation;
                    EntityDirection = new Vector2((float)Math.Cos(EntityRotation), (float)Math.Sin(EntityRotation));
                    EntityDirection.Normalize();
                    // Stores the position to which the player is going to be moved.
                    v2NewPos = EntityPosition + (EntityDirection * Distance);
                    if (LegalPosition(v2NewPos, grid))
                        EntityPosition = v2NewPos;
                    TmpCurrentFrameY = intKeyIndex;
                    // Sprite sheet order of rows must be the same as the order of the keys for movement.
                    // for this to work.
                    Animation.Active = true;
                }
                rotation += (float)Math.PI / 2;
            }
            TmpCurrentFrameX = Animation.CurrentFrame.X;
            Animation.Position = EntityPosition;
            Animation.CurrentFrame = TmpCurrentFrame;
        }

        // Checks if a point is in a traversable node.
        private bool LegalPosition(Vector2 v2NewPos, GridLayer grid)
        {

            if (grid.GetNode(ToPoint(v2NewPos)).Traversable && grid.GetNode(ToPoint(v2NewPos + new Vector2(RectEntityWidth, 0))).Traversable
                && grid.GetNode(ToPoint(v2NewPos + new Vector2(0, RectEntityHeight))).Traversable && grid.GetNode(ToPoint(v2NewPos + new Vector2(RectEntityWidth, RectEntityHeight))).Traversable)
                return true;
            return false;
        }

        // Converts a vector2 position to a point.
        private Point ToPoint(Vector2 v2Position)
        {
            return new Point((int)v2Position.X, (int)v2Position.Y);
        }
    }
}