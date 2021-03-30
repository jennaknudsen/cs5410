using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static FinalProject_Tetris.TetrisGameController;

namespace FinalProject_Tetris
{
    public class TetrisGame : Game
    {
        // assets for this game
        private Texture2D _texRed;
        private Texture2D _texBlue;
        private Texture2D _texBoard;
        private Texture2D _texBackgroundDimmer;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // width and height of window
        private static int _windowWidthPixels;
        private static int _windowHeightPixels;

        public TetrisGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Modify to change game render resolution
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            var canvasBounds = GraphicsDevice.Viewport.Bounds;
            _windowWidthPixels = canvasBounds.Width;
            _windowHeightPixels = canvasBounds.Height;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            // _texLander = this.Content.Load<Texture2D>("Lander-3");
            // _texSpaceBackground = this.Content.Load<Texture2D>("space-background");
            // _texBackgroundDimmer = this.Content.Load<Texture2D>("background-dimmer");
            // _gameFont = this.Content.Load<SpriteFont>("GameFont");
            // _menuFont = this.Content.Load<SpriteFont>("MenuFont");
            // _rocketSound = this.Content.Load<SoundEffect>("rocket-sound");
            // _rocketSoundInstance = _rocketSound.CreateInstance();
            // _explosionSound= this.Content.Load<SoundEffect>("explosion");
            // _explosionSoundInstance = _explosionSound.CreateInstance();
            // _successSound= this.Content.Load<SoundEffect>("success");
            // _successSoundInstance = _successSound.CreateInstance();
            _texRed = this.Content.Load<Texture2D>("red");
            _texBlue = this.Content.Load<Texture2D>("blue");
            _texBoard = this.Content.Load<Texture2D>("board");
            _texBackgroundDimmer = this.Content.Load<Texture2D>("background-dimmer");
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
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            // get pixel coordinates from board coordinates
            var (backX, backY) = GetAbsolutePixelCoordinates((0, BoardSize));
            var rectSizePixels = RescaleUnitsToPixels(BoardSize);
            // create the background rectangle
            var backgroundRect = new Rectangle(backX, backY, rectSizePixels, rectSizePixels);
            _spriteBatch.Draw(_texBoard, backgroundRect, Color.White);

            var (sqx, sqy) = GetAbsolutePixelCoordinates((10, 3));
            var (sqx2, sqy2) = GetAbsolutePixelCoordinates(
                GetBoardLocationFromPieceLocation((0, 1)));
            var (sqx3, sqy3) = GetPixelLocationFromPieceLocation((1, 0));
            var (sqx4, sqy4) = GetPixelLocationFromPieceLocation((1, 1));
            var squareWidth = RescaleUnitsToPixels(PieceSize);
            var rect = new Rectangle(sqx, sqy, squareWidth, squareWidth);
            var rect2 = new Rectangle(sqx2, sqy2, squareWidth, squareWidth);
            var rect3 = new Rectangle(sqx3, sqy3, squareWidth, squareWidth);
            var rect4 = new Rectangle(sqx4, sqy4, squareWidth, squareWidth);

            // run draw function
            foreach (var rectangle in new[] {rect, rect2, rect3, rect4})
            {
                _spriteBatch.Draw(_texRed,
                    rectangle,
                    null,
                    Color.White,
                    0,
                    // center origin in the texture
                    new Vector2(0, _texRed.Height),
                    SpriteEffects.None,
                    0);
            }

            base.Draw(gameTime);

            _spriteBatch.End();
        }

        // game board will have relative dimensions in a square
        // this function gets the absolute dimensions
        public static (int x, int y) GetAbsolutePixelCoordinates((float x, float y) relativeCoordinates)
        {
            // keep relative coordinates good
            if (relativeCoordinates.x < 0 || relativeCoordinates.x > BoardSize ||
                relativeCoordinates.y < 0 || relativeCoordinates.y > BoardSize)
            {
                // uncomment this line if we want to force spaceship to stay in safe area
                // throw new Exception("Relative coordinates must be between 0 and " + BoardSize + ".");
            }

            // get size of playable area (will be constrained by height)
            var sizeOfGameAreaPixels = _windowHeightPixels;

            // width will be square centered in screen, same dimensions as height
            var horizontalMarginPixels = (_windowWidthPixels - sizeOfGameAreaPixels) / 2;

            // properly rescale the coordinates to get the correct pixels
            var rescaledX = RescaleUnitsToPixels(relativeCoordinates.x) + horizontalMarginPixels;
            var rescaledY = RescaleUnitsToPixels(BoardSize - relativeCoordinates.y);

            // return rescaled coordinates
            return (rescaledX, rescaledY);
        }

        // given a unit count, rescale it to pixels
        public static int RescaleUnitsToPixels(float units)
        {
            // rescale by ratio of game area in pixels to board size
            var rescaledUnits = (int) (_windowHeightPixels / BoardSize * units);
            return rescaledUnits;
        }

        // given a grid location, return a board location
        public static (int x, int y) GetBoardLocationFromPieceLocation((int x, int y) pieceLocation)
        {
            return (pieceLocation.x + 10, pieceLocation.y + 3);
        }

        // given a grid location, return a pixel location
        public static (int x, int y) GetPixelLocationFromPieceLocation((int x, int y) pieceLocation)
        {
            return GetAbsolutePixelCoordinates(
                GetBoardLocationFromPieceLocation((pieceLocation.x, pieceLocation.y))
                );
        }
    }
}
