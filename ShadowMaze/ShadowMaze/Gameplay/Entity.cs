using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMaze
{
    /// <summary>
    /// This class will contain some of the base properties that will be needed for moving
    /// map objects i.e. players and shadow copies. This class will not have any objects
    /// instantiated from itself so it will be made abstract.
    /// </summary>
     public abstract class Entity
    {
        #region Variables
        private Texture2D tx2EntityImage; // Stores the texture to be used for the entity's sprite.
        private Vector2 v2EntityPosition; // Stores the current x and y coordinates of the entity.
        private Node nodeEntity; // Stores the node that the entity is occupying.
        private Rectangle rectEntity; // Stores the rectangle within which the entity is drawn.
        protected ContentManager content; // Manages the content (sprite image) for the individual entity.
        private float fltSpeed; // Stores the Speed for distance = Speed * time formula.
        private float fltDistance; // Stores the distance for the entity to move each update cycle.
        private Vector2 v2EntityDirection; // Stores the direction in which the entity is facing.
        private float fltEntityRotation; // Stores the rotation of the entity.
        private Animation animationEntity; // Stores the animation for the entity.
        private Vector2 v2TmpCurrentFrame; // Stores the current frame to display from the animation.
        private Color colourEntity; // Stores the colour of the entity.
        #endregion

        public Animation Animation
        {
            get { return animationEntity; }
            set { animationEntity = value; }
        }

        public Color Colour
        {
            get { return colourEntity; }
            set { colourEntity = value; }
        }

        public byte ColourA
        {
            get { return colourEntity.A; }
            set { colourEntity.A = value; }
        }

        public Vector2 TmpCurrentFrame
        {
            get { return v2TmpCurrentFrame; }
            set { v2TmpCurrentFrame = value; }
        }

        public float TmpCurrentFrameY
        {
            get { return v2TmpCurrentFrame.Y; }
            set { v2TmpCurrentFrame.Y = value; }
        }

        public float TmpCurrentFrameX
        {
            get { return v2TmpCurrentFrame.X; }
            set { v2TmpCurrentFrame.X = value; }
        }

        public Vector2 EntityDirection
        {
            get { return v2EntityDirection; }
            set { v2EntityDirection = value; }
        }

        public float EntityRotation
        {
            get { return fltEntityRotation; }
            set { fltEntityRotation = value; }
        }

        public Rectangle RectEntity
        {
            get { return rectEntity; }
            set { rectEntity = value; }
        }

        public int RectEntityWidth
        {
            get { return rectEntity.Width; }
            set { rectEntity.Width = value; }
        }

        public int RectEntityHeight
        {
            get { return rectEntity.Height; }
            set { rectEntity.Height = value; }
        }

        public int RectEntityX
        {
            get { return rectEntity.X; }
            set { rectEntity.X = value; }
        }

        public int RectEntityY
        {
            get { return rectEntity.Y; }
            set { rectEntity.Y = value; }
        }

        public Vector2 EntityPosition
        {
            get { return v2EntityPosition; }
            set { v2EntityPosition = value; }
        }

        public float EntityPositionX
        {
            get { return v2EntityPosition.X; }
            set { v2EntityPosition.X = value; }
        }

        public float EntityPositionY
        {
            get { return v2EntityPosition.Y; }
            set { v2EntityPosition.Y = value; }
        }

        public Node EntityNode
        {
            get { return nodeEntity; }
            set { nodeEntity = value; }
        }

        public Texture2D EntityImage
        {
            get { return tx2EntityImage; }
            set { tx2EntityImage = value; }
        }

        public float Speed
        {
            get { return fltSpeed; }
            set { fltSpeed = value; }
        }

        public float Distance
        {
            get { return fltDistance; }
            set { fltDistance = value; }
        }

        #region Methods
        // Sets the entity's initial node to be drawn into and sets the entity's position accordingly.
        // Initializes the entity's animation.
        public virtual void Initialize(Node Node)
        {
            nodeEntity = Node;
            rectEntity = Node.Tile;
            animationEntity = new Animation();
            if (EntityImage != null) // For shadow copies that are spawned afterwards and are given a pre-loaded texture.
            { // Set their animation image and rectangle based on their pre-loaded texture.
                Animation.AnimationImage = EntityImage;
                animationEntity.Initialize(EntityPosition, new Vector2(3, 4));
                rectEntity.Height = animationEntity.FrameHeight;
                rectEntity.Width = animationEntity.FrameWidth;
            }
            else animationEntity.Initialize(EntityPosition, new Vector2(3, 4));
            // Entity's position is also set according to the image and current node.
            v2EntityPosition = new Vector2(rectEntity.X + rectEntity.Width / 2, rectEntity.Y + rectEntity.Height / 2);
        }

        // Instantiates a content manager for the entity and loads the entity's sprite image.
        public virtual void LoadContent(ContentManager Content, string strSpritePath)
        {
            content = new ContentManager(Content.ServiceProvider, "Content");
            tx2EntityImage = Content.Load<Texture2D>(strSpritePath);
            animationEntity.AnimationImage = tx2EntityImage;
            // Height and width of the player's rectangle is set according to the image loaded for the player.
            rectEntity.Height = animationEntity.FrameHeight;
            rectEntity.Width = animationEntity.FrameWidth;
        }

        // Unloads the sprite image using the entity's own content manager.
        public virtual void UnloadContent()
        {
            content.Unload();
        }

        // Draws the entity using its animation at its position on the map.
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Animation.Draw(spriteBatch, colourEntity);
        }
        #endregion
    }
}
