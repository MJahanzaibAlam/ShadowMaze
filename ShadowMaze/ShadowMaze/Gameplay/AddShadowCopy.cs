using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMaze
{
    // Methods to handle additional shadow copies (with line of sight and roaming/default paths).
    public class AddShadowCopy : ShadowCopy
    {
        private int intSight; // Stores how far the shadow copy can 'see'.
        private LightRay[] LRRays; // Stores the lines determining visibility for the shadow copy.
        private bool boolFound; // Stores whether or not a player has been found.
        private bool prevBoolFound; // Stores whether or not the player had been recently found.
        private Path defaultPath; // Stores the default path for the shadow copy to follow (roaming).
        private int intNodeIndex; // Stores the index of the node in the default path to follow next.
        private int intTrackTimer; // Stores how long a shadow copy has been following a player that has moved out of sight.

        // Sets the shadow copy's initial node as the node which is passed in. The shadow copy is passed in
        // to retrieve the shadow copy texture without having to load it in again.
        public override void Initialize(Node Node)
        {
            base.Initialize(Node);
            intSight = 70;
            RectEntityHeight = 35;
            RectEntityWidth = 33;
            // Height and width of the copy's rectangle is set according to the image loaded.
            EntityPosition = new Vector2(RectEntity.X + RectEntityWidth / 2, RectEntity.Y + RectEntityHeight / 2);
            // Entity's position is also set according to the image and current node.
        }

        // Unloads any content used for this shadow copy.
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        // Calls AI methods for the shadow copy to either roam the map or follow players that have entered field of vision.
        // Deals damage to players which are in contact.
        public void Update(GameTime gameTime, Player[] player, GridLayer grid)
        {
            RectEntityX = (int)EntityPositionX;
            RectEntityY = (int)EntityPositionY;
            Distance = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds; // Sets distance to move according to speed.
            FindClosestPlayer(player);

            CastLightRays(grid);

            // If the player is found or was found previously and it hasn't been 1000 seconds since they were found, follow the player.
            if (boolFound == true || (prevBoolFound == true && intTrackTimer < 500))
            {
                PathToPlayer = new Path(EntityNode, player[TargetPlayer].EntityNode, grid.Nodes);
                FollowPath(PathToPlayer.NodePath, player[TargetPlayer]);
            }

            if (InLineOfSight(grid, player[TargetPlayer])) // If a player is within line of sight.
            {
                prevBoolFound = true;
                intTrackTimer = 0; // It has been 0 seconds since they were found.
                defaultPath = null; // Reset the default path as the shadow copy will be moving away from the path whilst following player.
            }
            else if (prevBoolFound == true && intTrackTimer < 500)
            {
                intTrackTimer += gameTime.ElapsedGameTime.Milliseconds; // If player was previously found and it hasn't been 500 milliseconds, increase the tinmer.
            }
            else
            { // If the timer has reached 500 milliseconds, the player is no longer found and the shadow copy should roam.
                prevBoolFound = false;
                FollowPath(grid);
            }

            if (EntityNode == player[TargetPlayer].EntityNode) // If a shadow copy is in the same node as a player..
            {
                player[TargetPlayer].Health -= 5; // decrease the player's health.
                ScreenManager.Instance.SoundManager.PlaySound("Damage"); // and play the sound for damage.
            }

            UpdateAnimation();
            Animation.Update(gameTime);
            // Move the shadow copy in the set direction by the calculated distance.
            EntityPosition += Direction * Distance;
        }

        // Calls entity draw method to draw shadow copy animation image.
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        
        // Causes the shadow copy to follow a random path (to roam the map).
        private void FollowPath(GridLayer grid)
        {
            // If the path isn't null, follow the path.
            if (defaultPath != null)
            {
                // Calculate the direction from the shadow copy to the next node it has to follow.
                Direction = new Vector2(defaultPath.NodePath[intNodeIndex].TileX, defaultPath.NodePath[intNodeIndex].TileY) - EntityPosition;
                // Calculate the angle the shadow copy is moving so that light rays can be cast in the same direction.
                EntityRotation = (float)Math.Atan2(Direction.X, -Direction.Y);
                Direction = Vector2.Normalize(Direction);
                if (EntityNode == defaultPath.NodePath[intNodeIndex])
                    intNodeIndex--; // If the shadow copy has moved into the next node in the path. Move to the next one.
                if (intNodeIndex == -1) // If the last node in the path has been reached. Set the path to null.
                    defaultPath = null;
            }
            else // Otherwise create a new random path.
            {
                Random rnd = new Random(); // Creates a pseudo-random number generator.
                int intRndColumn = rnd.Next(0, 21);
                int intRndRow = rnd.Next(0, 12);
                // Creates a path to a random node on the map.
                defaultPath = new Path(EntityNode, grid.Nodes[intRndColumn, intRndRow], grid.Nodes);

                // If the generated path was to a non-traversable node or was empty, the path is set to null
                // so that it can be recalculated in the next update.
                if (defaultPath.NodePath.Count > 1 && grid.Nodes[intRndColumn, intRndRow].Traversable)
                    intNodeIndex = defaultPath.NodePath.Count - 1;
                else defaultPath = null;
            }
        }
        
        // Casts light rays in the direction that the shadow copy is facing.
        public void CastLightRays(GridLayer grid)
        {
            // Stores the directions in which light rays should be casted.
            Vector2[] v2Directions = new Vector2[3];
            // Directions to cast lines in are calculated: direction vector = cos(angle) - sin(angle)
            v2Directions[0] = new Vector2((float)Math.Sin(EntityRotation), -(float)Math.Cos(EntityRotation));
            v2Directions[0].Normalize(); // Cast a light ray in the direction that the copy is facing.
            v2Directions[1] = new Vector2((float)Math.Sin(EntityRotation - 0.5), -(float)Math.Cos(EntityRotation - 0.5));
            v2Directions[1].Normalize(); // Cast a light ray to the left of the first one. 
            v2Directions[2] = new Vector2((float)Math.Sin(EntityRotation + 0.5), -(float)Math.Cos(EntityRotation + 0.5));
            v2Directions[2].Normalize(); // Cast a light ray which is to the right of the first one.

            // Stores the centre of the shadow copy so that rays can be casted from there.
            Vector2 v2CentreOfCopy = EntityPosition + new Vector2(RectEntityWidth / 2, RectEntityHeight / 2);

            LRRays = new LightRay[3];
            // Create the light rays in the three directions using the constructor of the light ray class.
            for (int intRayIndex = 0; intRayIndex < 3; intRayIndex++)
                LRRays[intRayIndex] = new LightRay(v2CentreOfCopy, v2Directions[intRayIndex], intSight, grid);
        }

        // Raises the brightness of nodes which are visible and detects if a player is within field of vision.
        public bool InLineOfSight(GridLayer grid, Player player)
        {
            boolFound = false;
            foreach (LightRay line in LRRays)
            {
                foreach (Point point in line.Points)
                {
                    // Loop through each point in each line, make the node which the point is within visible.
                    grid.GetNode(point).Colour = Color.White;
                    // Then check if the node at the point intersects the closest player's rectangle (is within line of sight).
                    if (grid.GetNode(point).Tile.Intersects(player.RectEntity))
                        boolFound = true;
                }
            }
            return boolFound;
        }
    }
}