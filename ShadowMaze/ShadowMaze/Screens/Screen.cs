using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMaze
{
    /// <summary>
    /// This class will hold all the methods for initializing, loading content for,
    /// unloading content for, and updating screens. The majority of the methods in this class
    /// will call methods from other classes to carry out the tasks to be done on screen.
    /// </summary>
    public abstract class Screen
    {
        public virtual void Initialize() { }

        protected ContentManager content; // Manages content specifically for the individual screen.

        // Instantiates the content manager for the screen.
        public virtual void LoadContent(ContentManager Content)
        {
            content = new ContentManager(Content.ServiceProvider, "Content");
        }

        // Unloads all the content used for the individual screen.
        public virtual void UnloadContent()
        {
            content.Unload();
        }
        

        // This method will be overriden in the spefific screen class.
        public virtual void Update(GameTime gameTime) { }

        //This method will be overriden in the specific screen class.
        public virtual void Draw(SpriteBatch spriteBatch) { }
    }
}
