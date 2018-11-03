using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMaze
{
    // Used to create and draw animations using sprite sheets.
    // Note: The methods in this class have been adapted from Coding-Made-Easy's (on Youtube) animation tutorial for XNA:
    // https://www.youtube.com/watch?v=D39zz1QWCiw
    // Credit-where-due: The spritesheet used for shadow copy and player animations was taken and edited from:
    // Curt - cjc83486 - http://opengameart.org/content/rpg-character
    public class Animation
    {
        private int intFrameCount; // Stores the number of frames (draw cycles) which have passed.
        private int intFrameSwitch; // Stores the number of frames after which the source rectangle to display should be changed.
        private bool boolActive; // Stores whether the animation should currently be played.
        private Vector2 v2Position; // Stores the position to draw the animation.
        private Vector2 v2CurrentFrame; // Stores the position of the current frame (rectangle) within the spritesheet.
        private Vector2 v2Frames; // Stores the number of frames in the sprite sheet so that they can be accessed.
        private Texture2D tx2Image; // Stores the image of the spritesheet.
        private Rectangle rectSource; // Stores the source rectangle (the part of the spritesheet to show).

        public bool Active
        {
            get { return boolActive; }
            set { boolActive = value; }
        }

        public Vector2 CurrentFrame
        {
            get { return v2CurrentFrame; }
            set { v2CurrentFrame = value; }
        }

        public Vector2 Position
        {
            get { return v2Position; }
            set { v2Position = value; }
        }

        public Texture2D AnimationImage
        {
            set { tx2Image = value; }
        }

        public int FrameWidth
        {
            get { return tx2Image.Width / (int)v2Frames.X; }
        }

        public int FrameHeight
        {
            get { return tx2Image.Height / (int)v2Frames.Y; }
        }

        // Sets the number of frames in the spritesheet according to the parameter.
        // Sets the initial position to draw the animation based on the v2Position parameter.
        // Sets frame switch as 150 meaning that after 150 draw cycles, the sprite being displayed will change.
        public void Initialize(Vector2 v2Position, Vector2 v2Frames)
        {
            boolActive = false;
            intFrameSwitch = 150;
            this.v2Position = v2Position;
            this.v2Frames = v2Frames;
        }

        // Checks if the frame count has reached the frame switch value so that the next sprite in the spritesheet
        // can be displayed.
        public void Update(GameTime gameTime)
        {
            if (boolActive) // Increment the frame count each update cycle.
                intFrameCount += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            else
                intFrameCount = 0;
            if (intFrameCount >= intFrameSwitch) // Check if the frame counter has exceeded the switch frame value to move
            { // to the next frame in the sprite sheet.
                intFrameCount = 0;
                v2CurrentFrame.X += FrameWidth;
                // -10 in the below if statement is because the current frame reaches 99 which does not exceed the width
                // of the image in the last sprite. Therefore, taking away 10 ensures that the current sprite is set as the
                // starting one after the last sprite. (This may not be needed with a different sprite sheet).
                if (v2CurrentFrame.X >= tx2Image.Width - 10) // Checks to see if the end of the spritesheet has been reached.
                    v2CurrentFrame.X = 0;
            } // Create the source rectangle around the selected sprite in the sprite sheet.
            rectSource = new Rectangle((int)v2CurrentFrame.X, (int)v2CurrentFrame.Y * FrameHeight, FrameWidth, FrameHeight);
        }

        // Draws the animation's current sprite from the spritesheet at the position specified by the parameter.
        public void Draw(SpriteBatch spriteBatch, Color colourEntity)
        {
            spriteBatch.Draw(tx2Image, v2Position, rectSource, colourEntity);
        }
    }
}
