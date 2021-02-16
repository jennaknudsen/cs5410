using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Maze
{
    public class MazeGame : Game
    {
        private GraphicsDeviceManager m_graphics;
        private SpriteBatch m_spriteBatch;

        # region Assets
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
 
        # endregion

        private const int BOARD_SIZE_PIXELS = 600;

        private int boardSize;
        private int tileSizePixels;
        
        // underlying data structure to hold a Maze
        private Maze thisMaze;
 
        public MazeGame()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Add your initialization logic here
            m_graphics.PreferredBackBufferWidth = 1200;
            m_graphics.PreferredBackBufferHeight = 800;
            
            m_graphics.ApplyChanges();

            // initialize the maze
            // TODO: handle this somewhere else
            boardSize = 5;
            thisMaze = new Maze(boardSize);
            
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
            GraphicsDevice.Clear(Color.DarkGray);

            m_spriteBatch.Begin();
            
            // TODO: Add your drawing code here
            Rectangle position = new Rectangle(500, 500, 50, 50);
            Rectangle position2 = new Rectangle(550, 500, 50, 50);
            m_spriteBatch.Draw(m_texULB, position, Color.White);
            m_spriteBatch.Draw(m_texURB, position2, Color.White);
            
            m_spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
