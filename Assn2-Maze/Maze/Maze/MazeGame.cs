using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Maze
{
    public class MazeGame : Game
    {
        private GraphicsDeviceManager m_graphics;
        private SpriteBatch m_spriteBatch;

        // all textures (floor tiles, circles, etc)
        // floor tiles
        private Texture2D m_texNone;
        private Texture2D m_texU;
        private Texture2D m_texL;
        private Texture2D m_texR;
        private Texture2D m_texB;
        private Texture2D m_texUL;
        private Texture2D m_texUR;
        private Texture2D m_texUB;
        private Texture2D m_texLR;
        private Texture2D m_texLB;
        private Texture2D m_texRB;
        private Texture2D m_texULR;
        private Texture2D m_texULB;
        private Texture2D m_texURB;
        private Texture2D m_texLRB;
        private Texture2D m_texULRB;
        
        // overlay sprites
        private Texture2D m_texBlueCircle;
        private Texture2D m_texGreenCircle;
        private Texture2D m_texPinkCircle;
        private Texture2D m_texSmallDot;
        private Texture2D m_texTransparentGreenCircle;
        
        
        public MazeGame()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            m_graphics.PreferredBackBufferWidth = 1200;
            m_graphics.PreferredBackBufferHeight = 800;
            
            m_graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // all textures (floor tiles, circles, etc)
            // floor tiles
            m_texNone = this.Content.Load<Texture2D>("FloorTiles/none");
            m_texU = this.Content.Load<Texture2D>("FloorTiles/U");
            m_texL = this.Content.Load<Texture2D>("FloorTiles/L");
            m_texR = this.Content.Load<Texture2D>("FloorTiles/R");
            m_texB = this.Content.Load<Texture2D>("FloorTiles/B");
            m_texUL = this.Content.Load<Texture2D>("FloorTiles/UL");
            m_texUR = this.Content.Load<Texture2D>("FloorTiles/UR");
            m_texUB = this.Content.Load<Texture2D>("FloorTiles/UB");
            m_texLR = this.Content.Load<Texture2D>("FloorTiles/LR");
            m_texLB = this.Content.Load<Texture2D>("FloorTiles/LB");
            m_texRB = this.Content.Load<Texture2D>("FloorTiles/RB");
            m_texULR = this.Content.Load<Texture2D>("FloorTiles/ULR");
            m_texULB = this.Content.Load<Texture2D>("FloorTiles/ULB");
            m_texURB = this.Content.Load<Texture2D>("FloorTiles/URB");
            m_texLRB = this.Content.Load<Texture2D>("FloorTiles/LRB");
            m_texULRB = this.Content.Load<Texture2D>("FloorTiles/ULRB");
        
            // overlay sprites
            m_texBlueCircle = this.Content.Load<Texture2D>("OverlaySprites/BlueCircle");
            m_texGreenCircle = this.Content.Load<Texture2D>("OverlaySprites/GreenCircle");
            m_texPinkCircle = this.Content.Load<Texture2D>("OverlaySprites/PinkCircle");
            m_texSmallDot = this.Content.Load<Texture2D>("OverlaySprites/SmallDot");
            m_texTransparentGreenCircle = this.Content.Load<Texture2D>("OverlaySprites/TransparentGreenCircle"); 
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
