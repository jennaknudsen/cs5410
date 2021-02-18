using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using static Maze.MazeSquare.Wall;

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
        private Texture2D m_texT;
        private Texture2D m_texL;
        private Texture2D m_texR;
        private Texture2D m_texB;
        private Texture2D m_texTL;
        private Texture2D m_texTR;
        private Texture2D m_texTB;
        private Texture2D m_texLR;
        private Texture2D m_texLB;
        private Texture2D m_texRB;
        private Texture2D m_texTLR;
        private Texture2D m_texTLB;
        private Texture2D m_texTRB;
        private Texture2D m_texLRB;
        private Texture2D m_texTLRB;

        // overlay sprites
        private Texture2D m_texBlueCircle;
        private Texture2D m_texGreenCircle;
        private Texture2D m_texPinkCircle;
        private Texture2D m_texSmallDot;
        private Texture2D m_texTransparentGreenCircle;
        private Texture2D m_texSuperTransparentGreenCircle;

        # endregion

        // consts that hold unchanging values
        private const int BOARD_SIZE_PIXELS = 600;
        private const int WINDOW_WIDTH = 1200;
        private readonly (int x, int y) TOP_LEFT_CORNER = (300, 100);
        private const int WINDDOW_HEIGHT = 800;

        // dimensions in tiles (5x5, 10x10, 15x15, 20x20)
        private int boardSize;
        // pixel width/height of each tile
        private int tileSizePixels;

        // underlying data structure to hold a Maze
        private Maze thisMaze;

        // whether or not to show certain UI elements
        private bool showBreadcrumbs = true;
        private bool showShortestPath = true;
        private bool showHint = true;

        // all buttons that need debouncing (aka all of them)
        private Debouncer leftDebouncer;
        private Debouncer rightDebouncer;
        private Debouncer upDebouncer;
        private Debouncer downDebouncer;
        private Debouncer hintDebouncer;
        private Debouncer shortestPathDebouncer;
        private Debouncer breadcrumbsDebouncer;

        public MazeGame()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Add your initialization logic here
            m_graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            m_graphics.PreferredBackBufferHeight = WINDDOW_HEIGHT;

            m_graphics.ApplyChanges();

            // initialize the maze
            // TODO: handle ALL of this somewhere else
            boardSize = 20;
            tileSizePixels = BOARD_SIZE_PIXELS / boardSize;
            thisMaze = new Maze(boardSize);
            thisMaze.SolveMazeFromStart();
            leftDebouncer = new Debouncer();
            rightDebouncer = new Debouncer();
            upDebouncer = new Debouncer();
            downDebouncer = new Debouncer();
            hintDebouncer = new Debouncer();
            shortestPathDebouncer = new Debouncer();
            breadcrumbsDebouncer = new Debouncer();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // all textures (floor tiles, circles, etc)
            // floor tiles
            m_texNone = this.Content.Load<Texture2D>("FloorTiles/none");
            m_texT = this.Content.Load<Texture2D>("FloorTiles/T");
            m_texL = this.Content.Load<Texture2D>("FloorTiles/L");
            m_texR = this.Content.Load<Texture2D>("FloorTiles/R");
            m_texB = this.Content.Load<Texture2D>("FloorTiles/B");
            m_texTL = this.Content.Load<Texture2D>("FloorTiles/TL");
            m_texTR = this.Content.Load<Texture2D>("FloorTiles/TR");
            m_texTB = this.Content.Load<Texture2D>("FloorTiles/TB");
            m_texLR = this.Content.Load<Texture2D>("FloorTiles/LR");
            m_texLB = this.Content.Load<Texture2D>("FloorTiles/LB");
            m_texRB = this.Content.Load<Texture2D>("FloorTiles/RB");
            m_texTLR = this.Content.Load<Texture2D>("FloorTiles/TLR");
            m_texTLB = this.Content.Load<Texture2D>("FloorTiles/TLB");
            m_texTRB = this.Content.Load<Texture2D>("FloorTiles/TRB");
            m_texLRB = this.Content.Load<Texture2D>("FloorTiles/LRB");
            m_texTLRB = this.Content.Load<Texture2D>("FloorTiles/TLRB");

            // overlay sprites
            m_texBlueCircle = this.Content.Load<Texture2D>("OverlaySprites/BlueCircle");
            m_texGreenCircle = this.Content.Load<Texture2D>("OverlaySprites/GreenCircle");
            m_texPinkCircle = this.Content.Load<Texture2D>("OverlaySprites/PinkCircle");
            m_texSmallDot = this.Content.Load<Texture2D>("OverlaySprites/SmallDot");
            m_texTransparentGreenCircle = this.Content.Load<Texture2D>("OverlaySprites/TransparentGreenCircle");
            m_texSuperTransparentGreenCircle = this.Content.Load<Texture2D>("OverlaySprites/SuperTransparentGreenCircle");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            processInput();


            base.Update(gameTime);
        }

        private void processInput()
        {
            var keyboardState = Keyboard.GetState();

            // UP
            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.I) || keyboardState.IsKeyDown(Keys.Up))
            {
                if (upDebouncer.Press())
                {
                    thisMaze.MoveUp();
                }
            }
            else
            {
                upDebouncer.Release();
            }

            // LEFT
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.J) || keyboardState.IsKeyDown(Keys.Left))
            {
                if (leftDebouncer.Press())
                {
                    thisMaze.MoveLeft();
                }
            }
            else
            {
                leftDebouncer.Release();
            }

            // RIGHT
            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.L) || keyboardState.IsKeyDown(Keys.Right))
            {
                if (rightDebouncer.Press())
                {
                    thisMaze.MoveRight();
                }
            }
            else
            {
                rightDebouncer.Release();
            }

            // DOWN
            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.K) || keyboardState.IsKeyDown(Keys.Down))
            {
                if (downDebouncer.Press())
                {
                    thisMaze.MoveDown();
                }
            }
            else
            {
                downDebouncer.Release();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            m_spriteBatch.Begin();

            // TODO: Add your drawing code here

            // draw the board
            for (int row = 0; row < boardSize; row++)
            {
                for (int col = 0; col < boardSize; col++)
                {
                    // need to determine which tile goes here
                    var textureName = "";
                    var thisSquare = thisMaze.mazeSquares[row, col];

                    if (thisSquare.TopWall.wallStatus != WallStatus.DISABLED)
                        textureName += "T";
                    if (thisSquare.LeftWall.wallStatus != WallStatus.DISABLED)
                        textureName += "L";
                    if (thisSquare.RightWall.wallStatus != WallStatus.DISABLED)
                        textureName += "R";
                    if (thisSquare.BottomWall.wallStatus != WallStatus.DISABLED)
                        textureName += "B";

                    if (textureName.Equals(""))
                        textureName = "none";

                    // now, use some ugly code to determine which texture is needed
                    var textureToLoad = textureName switch
                    {
                        "T" => m_texT,
                        "L" => m_texL,
                        "R" => m_texR,
                        "B" => m_texB,
                        "TL" => m_texTL,
                        "TR" => m_texTR,
                        "TB" => m_texTB,
                        "LR" => m_texLR,
                        "LB" => m_texLB,
                        "RB" => m_texRB,
                        "TLR" => m_texTLR,
                        "TRB" => m_texTRB,
                        "TLB" => m_texTLB,
                        "LRB" => m_texLRB,
                        "TLRB" => m_texTLRB,
                        "none" => m_texNone,
                        _ => m_texTLRB
                    };

                    // tuple holds position that this rect starts at
                    var position = (col * tileSizePixels + TOP_LEFT_CORNER.x,
                        row * tileSizePixels + TOP_LEFT_CORNER.y);

                    // draw this tile
                    var rect = new Rectangle(position.Item1, position.Item2, tileSizePixels, tileSizePixels);
                    m_spriteBatch.Draw(textureToLoad, rect, Color.White);

                    // if player is on this square, draw the pink circle
                    if (thisMaze.currentSquare == (row, col))
                    {
                        m_spriteBatch.Draw(m_texPinkCircle, rect, Color.White);
                    }

                    // if this is the end square, draw the blue circle
                    if (thisMaze.endSquare == (row, col))
                    {
                        m_spriteBatch.Draw(m_texBlueCircle, rect, Color.White);
                    }

                    // if this is a breadcrumb square and showBreadcrumbs is enabled, show a dot
                    if (showBreadcrumbs && thisMaze.mazeSquares[row, col].Visited)
                    {
                        m_spriteBatch.Draw(m_texSmallDot, rect, Color.White);
                    }

                    // if this is a solution square and showShortestPath is enabled, show a transparent green circle
                    if (showShortestPath && thisMaze.mazeSquares[row, col].PartOfSolution)
                    {
                        m_spriteBatch.Draw(m_texSuperTransparentGreenCircle, rect, Color.White);
                    }

                    if (showHint && thisMaze.hintSquare == (row, col))
                    {
                        m_spriteBatch.Draw(m_texTransparentGreenCircle, rect, Color.White);
                    }
                }
            }

            m_spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
