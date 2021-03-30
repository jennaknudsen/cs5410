using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static FinalProject_Tetris.Square;
using static FinalProject_Tetris.Square.PieceColor;
using static FinalProject_Tetris.TetrisGameController;

namespace FinalProject_Tetris
{
    public class TetrisGame : Game
    {
        // assets for this game
        private Texture2D _texRed;
        private Texture2D _texOrange;
        private Texture2D _texYellow;
        private Texture2D _texGreen;
        private Texture2D _texBlue;
        private Texture2D _texIndigo;
        private Texture2D _texViolet;
        private Texture2D _texGray;
        private Texture2D _texBoard;
        private Texture2D _texBackgroundDimmer;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // width and height of window
        private static int _windowWidthPixels;
        private static int _windowHeightPixels;

        // references to needed components
        private TetrisGameController _tetrisGameController;

        public TetrisGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Modify to change game render resolution
            _graphics.PreferredBackBufferWidth = 2560;
            _graphics.PreferredBackBufferHeight = 1440;
            _graphics.ApplyChanges();

            _tetrisGameController = new TetrisGameController();

            var canvasBounds = GraphicsDevice.Viewport.Bounds;
            _windowWidthPixels = canvasBounds.Width;
            _windowHeightPixels = canvasBounds.Height;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _texRed = this.Content.Load<Texture2D>("red");
            _texOrange = this.Content.Load<Texture2D>("orange");
            _texYellow = this.Content.Load<Texture2D>("yellow");
            _texGreen = this.Content.Load<Texture2D>("green");
            _texBlue = this.Content.Load<Texture2D>("blue");
            _texIndigo = this.Content.Load<Texture2D>("indigo");
            _texViolet = this.Content.Load<Texture2D>("violet");
            _texGray = this.Content.Load<Texture2D>("gray");
            _texBoard = this.Content.Load<Texture2D>("board");
            _texBackgroundDimmer = this.Content.Load<Texture2D>("background-dimmer");
        }

        protected override void Update(GameTime gameTime)
        {
            // all updating is handled in the GameController
            _tetrisGameController.Update(gameTime);

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

            // draw each square
            foreach (var square in _tetrisGameController.TetrisSquares)
            {
                // don't render this if it's null
                if (square == null) continue;

                var (squareX, squareY) = GetPixelLocationFromPieceLocation(square.PieceLocation);
                var squareWidth = RescaleUnitsToPixels(PieceSize);
                var renderRect = new Rectangle(squareX, squareY, squareWidth, squareWidth);

                _spriteBatch.Draw(
                    GetColor(square.Color),
                    renderRect,
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
                // uncomment this line if we want to force to stay in safe area
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

        // converter from PieceColor to correct texture
        private Texture2D GetColor(PieceColor pieceColor)
        {
            return pieceColor switch
            {
                Red => _texRed,
                Orange => _texOrange,
                Yellow => _texYellow,
                Green => _texGreen,
                Blue => _texBlue,
                Indigo => _texIndigo,
                Violet => _texViolet,
                Gray => _texGray,
                _ => throw new ArgumentOutOfRangeException(nameof(pieceColor), pieceColor, null)
            };
        }
    }
}
