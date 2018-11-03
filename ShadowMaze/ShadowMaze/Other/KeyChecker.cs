using Microsoft.Xna.Framework.Input;

namespace ShadowMaze
{
    // Allows input to be taken for typing text.
    class KeyChecker
    {
        private string strInputText = ""; // Stores the text which has been input.

        public string InputText
        {
            get { return strInputText; }
            set { strInputText = value; }
        }

        // Stores the keys in the alphabet and numeric keys.
        Keys[] keysAlphaNumeric = new Keys[] {
    Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
    Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
    Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
    Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
    Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
    Keys.Z, Keys.Back, Keys.Space, Keys.D0,
    Keys.D1, Keys.D2, Keys.D3, Keys.D4,
    Keys.D5, Keys.D6, Keys.D7, Keys.D8,
    Keys.D9};
        
        // Adds each individual single key pressed button to the inputted text.
        public void Update()
        {
            foreach (Keys key in keysAlphaNumeric)
            {
                if (ScreenManager.Instance.SingleKeyPress(key))
                {
                    AddKeyAsText(key);
                    break;
                }
            }
        }

        // Adds a character to the string if a key was pressed once.
        private void AddKeyAsText(Keys key)
        {
            string charToAdd = ""; // Stores the character to add to the input text.

            if (strInputText.Length >= 20 && key != Keys.Back)
                return;

            switch (key)
            {
                case Keys.A:
                    charToAdd += "a";
                    break;
                case Keys.B:
                    charToAdd += "b";
                    break;
                case Keys.C:
                    charToAdd += "c";
                    break;
                case Keys.D:
                    charToAdd += "d";
                    break;
                case Keys.E:
                    charToAdd += "e";
                    break;
                case Keys.F:
                    charToAdd += "f";
                    break;
                case Keys.G:
                    charToAdd += "g";
                    break;
                case Keys.H:
                    charToAdd += "h";
                    break;
                case Keys.I:
                    charToAdd += "i";
                    break;
                case Keys.J:
                    charToAdd += "j";
                    break;
                case Keys.K:
                    charToAdd += "k";
                    break;
                case Keys.L:
                    charToAdd += "l";
                    break;
                case Keys.M:
                    charToAdd += "m";
                    break;
                case Keys.N:
                    charToAdd += "n";
                    break;
                case Keys.O:
                    charToAdd += "o";
                    break;
                case Keys.P:
                    charToAdd += "p";
                    break;
                case Keys.Q:
                    charToAdd += "q";
                    break;
                case Keys.R:
                    charToAdd += "r";
                    break;
                case Keys.S:
                    charToAdd += "s";
                    break;
                case Keys.T:
                    charToAdd += "t";
                    break;
                case Keys.U:
                    charToAdd += "u";
                    break;
                case Keys.V:
                    charToAdd += "v";
                    break;
                case Keys.W:
                    charToAdd += "w";
                    break;
                case Keys.X:
                    charToAdd += "x";
                    break;
                case Keys.Y:
                    charToAdd += "y";
                    break;
                case Keys.Z:
                    charToAdd += "z";
                    break;
                case Keys.Space:
                    charToAdd += " ";
                    break;
                case Keys.Back: // Reduces the string length by one if the backspace button was pressed.
                    if (strInputText.Length != 0)
                        strInputText = strInputText.Remove(strInputText.Length - 1);
                    return;
                case Keys.D0:
                    charToAdd += "0";
                    break;
                case Keys.D1:
                    charToAdd += "1";
                    break;
                case Keys.D2:
                    charToAdd += "2";
                    break;
                case Keys.D3:
                    charToAdd += "3";
                    break;
                case Keys.D4:
                    charToAdd += "4";
                    break;
                case Keys.D5:
                    charToAdd += "5";
                    break;
                case Keys.D6:
                    charToAdd += "6";
                    break;
                case Keys.D7:
                    charToAdd += "7";
                    break;
                case Keys.D8:
                    charToAdd += "8";
                    break;
                case Keys.D9:
                    charToAdd += "9";
                    break;
            }
            if (ScreenManager.Instance.KeyState.IsKeyDown(Keys.RightShift) || ScreenManager.Instance.KeyState.IsKeyDown(Keys.LeftShift))
            {
                charToAdd = charToAdd.ToUpper(); // Captalises the key if the shift key was pressed. 
            }
            strInputText += charToAdd;
        }

    }
}
