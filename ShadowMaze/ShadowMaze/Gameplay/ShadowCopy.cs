using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMaze
{
    /// <summary>
    /// This class will contain all the algorithms/methods for shadow copy movement and other behaviour.
    /// </summary>
    public class ShadowCopy : Entity
    {
        private Path pathToPlayer; // Stores the path to the targetted player.
        private Vector2 v2Direction; // Stores the direction in which the shadow copy is facing.
        private int intTargetPlayerIndex; // Stores the index of the player to track.
        
        public Vector2 Direction
        {
            get { return v2Direction; }
            set { v2Direction = value; }
        }

        public Path PathToPlayer
        {
            get { return pathToPlayer; }
            set { pathToPlayer = value; }
        }

        public int TargetPlayer
        {
            get { return intTargetPlayerIndex; }
            set { intTargetPlayerIndex = value; }
        }

        // Calls the entity Initialization method to set the entity's current position based on the
        // node that has been passed into the initialize method. The speed of the copy and other properties are set here.
        public override void Initialize(Node Node)
        {
            base.Initialize(Node);

            switch (ScreenManager.Instance.Difficulty)
            {
                case "easy":
                    Speed = 45f;
                    break;
                case "intermediate":
                    Speed = 60f;
                    break;
                case "hard":
                    Speed = 75f;
                    break;
                case "impossible":
                    Speed = 90f;
                    break;
            }
            Colour = Color.Gray;
            ColourA = 20;
            // Shadow copies appear slightly transparent.
        }

        // Calls the entity LoadContent method to load the sprite image for drawing using the sprite
        // path that is provided when this method is called from the game screen.
        public override void LoadContent(ContentManager Content, string strSpritePath)
        {
            base.LoadContent(Content, strSpritePath);
        }

        // Unloads all the content that was used for this shadow copy.
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        // Calls AI methods for shadow copies to process pathfinding, movement and processes damage dealt to players.
        public void Update(GameTime gameTime, Player[] player, Node[,] gridNodes)
        {
            RectEntityX = (int)EntityPositionX;
            RectEntityY = (int)EntityPositionY;
            Distance = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds; // Sets distance to move according to speed.
            
            FindClosestPlayer(player);
            
            pathToPlayer = new Path(EntityNode, player[intTargetPlayerIndex].EntityNode, gridNodes);
            FollowPath(pathToPlayer.NodePath, player[intTargetPlayerIndex]); // Follow the path to the target player.

            if (EntityNode == player[intTargetPlayerIndex].EntityNode) // If a shadow copy is in the same node as the player..
            {
                player[intTargetPlayerIndex].Health -= 5; // decrease the player's health
                ScreenManager.Instance.SoundManager.PlaySound("Damage"); // and play the sound for damage.
            }

            // Move the shadow copy in the set direction by the calculated distance.
            EntityPosition += v2Direction * Distance;
            UpdateAnimation();
            Animation.Update(gameTime);
        }
        
        // Calls entity draw method to draw shadow copy animation image.
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        // Decides which animation sprite to display based on the direction the copy is facing.
        public void UpdateAnimation()
        {
            // The copy is facing up.
            if (EntityRotation >= -Math.PI / 4 && EntityRotation <= Math.PI / 4)
                TmpCurrentFrameY = 2;
            // The copy is facing right.
            else if (EntityRotation > Math.PI / 4 && EntityRotation <= Math.PI * 3 / 4)
                TmpCurrentFrameY = 1;
            // The copy is facing left.
            else if (EntityRotation < -Math.PI / 4 && EntityRotation >= -Math.PI * 3 / 4)
                TmpCurrentFrameY = 3;
            // The copy is facing down.
            else TmpCurrentFrameY = 0;

            Animation.Active = true;
            TmpCurrentFrameX = Animation.CurrentFrame.X;
            Animation.Position = EntityPosition;
            Animation.CurrentFrame = TmpCurrentFrame;
        }

        // Causes the shadow copy to follow the path to reach the player.
        public void FollowPath(List<Node> pathOfNodes, Player player)
        {
            if (pathOfNodes.Count > 1) // If the path isn't empty.
            {
                // Calculate the direction from the shadow copy to the next node it has to follow.
                v2Direction = new Vector2(pathOfNodes[pathOfNodes.Count - 2].TileX, pathOfNodes[pathOfNodes.Count - 2].TileY) - EntityPosition;
                EntityRotation = (float)Math.Atan2(v2Direction.X, -v2Direction.Y);
                v2Direction.Normalize();
            }
            else if (pathOfNodes != null) // If the player has been reached. Move directly towards the player.
            {
                v2Direction = player.EntityPosition - EntityPosition;
                EntityRotation = (float)Math.Atan2(v2Direction.X, -v2Direction.Y);
                v2Direction.Normalize();
            }
        }
        
        // Finds the index of the closest player by calculating the distance from the shadow copy to each player,
        // storing it in array and then using the index of the shortest distance in the distance array.
        public void FindClosestPlayer(Player[] player)
        {
            // Stores the distances to each player to decide which player is the closest.
            double[] dblDistances = new double[4];
            for (int intPlayerIndex = 0; intPlayerIndex <= 3; intPlayerIndex++)
            {
                if (!player[intPlayerIndex].Dead)
                    dblDistances[intPlayerIndex] = FindDistance(player[intPlayerIndex].EntityPosition, EntityPosition);
                else dblDistances[intPlayerIndex] = 10000; // If the player isn't on the map, make the distance negligable.
            }
            TargetPlayer = Array.IndexOf(dblDistances, dblDistances.Min());
        }
        
        // Takes two vector2 positions and calculates the distance between them via the formula
        // distance = sqrt[(x2-x)^2+(y2-y)^2]
        private double FindDistance(Vector2 pos1, Vector2 pos2)
        {
            return Math.Sqrt(Math.Pow((pos2.X - pos1.X), 2) + Math.Pow((pos2.Y - pos1.Y), 2));
        }

    }
}