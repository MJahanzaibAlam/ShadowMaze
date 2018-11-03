using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMaze
{
    // Stores the methods and fields used for detecting button clicks and drawing a button to screen with text.
    public class Button
    {
        private Texture2D tx2Button; // Stores the image for the button.
        private Vector2 v2ButtonPos; // Stores the coordinates of the left corner of the button.
        private Vector2 v2TextPos; // Stores the position of the text within the button.
        private string strButtonText; // Stores the text of the button.
        private Color colourText; // Stores the colour of the text in the button.
        private SpriteFont fontButton; // Stores the font for the text in the button.
        
        public Vector2 Position
        {
            get { return v2ButtonPos; }
            set { v2ButtonPos = value; }
        }
        public Texture2D Image
        {
            get { return tx2Button; }
            set { tx2Button = value; }
        }
        
        public string Text
        {
            get { return strButtonText; }
            set { strButtonText = value; }
        }
        
        public SpriteFont Font
        {
            get { return fontButton; }
            set { fontButton = value; }
        }

        // Draws the button and the text within it using the set positions, colour, font and text.
        public void DrawButtonAndText(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(tx2Button, v2ButtonPos, Color.White);
            spriteBatch.DrawString(fontButton, strButtonText, v2TextPos, colourText);
        }
        
        // Positions the button relative to the centre of the screen.
        public void SetButtonPosition()
        {
            // The centre of the screen is retrieved by dividing the screen dimensions by two.
            Vector2 v2CentreScreen = ScreenManager.Instance.ScreenDimensions / 2;
            // The left corner of the button is set as the centre of the screen minus the buttons dimensions.
            v2ButtonPos = v2CentreScreen - new Vector2(tx2Button.Width / 2, tx2Button.Height / 2);
        }

        // For the button's text, sets the position as the button's left plus half the dimensions of the button (to centre) minus half of the string's dimensions (to align text).
        public void SetTextPosition()
        {
            v2TextPos = v2ButtonPos + new Vector2(tx2Button.Width / 2 - fontButton.MeasureString(strButtonText).X / 2, tx2Button.Height / 2 - fontButton.MeasureString(strButtonText).Y / 2);
        }

        // Checks if the mouse pointer is within the bounds of the texture of the button.
        private bool ButtonContainsMouse()
        {
            if (ScreenManager.Instance.MouseState.X > v2ButtonPos.X &&
                ScreenManager.Instance.MouseState.Y > v2ButtonPos.Y &&
                ScreenManager.Instance.MouseState.X < v2ButtonPos.X + tx2Button.Width &&
                ScreenManager.Instance.MouseState.Y < v2ButtonPos.Y + tx2Button.Height)
            {
                return true;
            }
            else return false;
        }

        // Checks whether the pointer is hovering over a mouse and if it has been clicked.
        public bool DetectButtonClick()
        {
            if (ButtonContainsMouse()) // If the button contains the pointer...
            {
                colourText = Color.White; // Make the text of the button white.
                if (ScreenManager.Instance.SingleMouseClick())
                {
                    ScreenManager.Instance.SoundManager.PlaySound("Button");
                    return true; // If the mouse is clicked, return true
                }
            }
            else colourText = Color.Black; // Otherwise, if the mouse hasn't been clicked, make the text black.
            return false;
        }
    }
}
