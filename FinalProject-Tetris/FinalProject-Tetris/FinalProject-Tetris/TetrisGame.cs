using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using static FinalProject_Tetris.Piece.PieceType;
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
        private Texture2D _texPieceParticle;
        private Texture2D _texParticleRed;
        private Texture2D _texParticleOrange;
        private Texture2D _texParticleYellow;
        private Texture2D _texParticleGreen;
        private Texture2D _texParticleBlue;
        private Texture2D _texParticleIndigo;
        private Texture2D _texParticleViolet;
        private Texture2D _texClearLineParticle;
        private SoundEffect _soundBlockPlace;
        private SoundEffect _soundLineClear;
        private SoundEffect _soundGameOver;
        private SoundEffect _soundTetrisSong;
        private SpriteFont _gameFont;
        private SpriteFont _menuFont;

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
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
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
            _texParticleRed = this.Content.Load<Texture2D>("ParticleRed");
            _texParticleOrange = this.Content.Load<Texture2D>("ParticleOrange");
            _texParticleYellow = this.Content.Load<Texture2D>("ParticleYellow");
            _texParticleGreen = this.Content.Load<Texture2D>("ParticleGreen");
            _texParticleBlue = this.Content.Load<Texture2D>("ParticleBlue");
            _texParticleIndigo = this.Content.Load<Texture2D>("ParticleIndigo");
            _texParticleViolet = this.Content.Load<Texture2D>("ParticleViolet");
            _texPieceParticle = this.Content.Load<Texture2D>("PieceParticle");
            _texClearLineParticle = this.Content.Load<Texture2D>("PieceParticle");
            _texBackgroundDimmer = this.Content.Load<Texture2D>("background-dimmer");
            _soundBlockPlace = this.Content.Load<SoundEffect>("BlockPlace");
            _soundLineClear = this.Content.Load<SoundEffect>("LineClear");
            _soundGameOver = this.Content.Load<SoundEffect>("GameOver");
            _soundTetrisSong = this.Content.Load<SoundEffect>("Tetris");
            _gameFont = this.Content.Load<SpriteFont>("GameFont");
            _menuFont = this.Content.Load<SpriteFont>("MenuFont");

            // initialize the game controller's sound now that music is imported
            _tetrisGameController.SoundController = new SoundController(
                _soundBlockPlace, _soundLineClear, _soundGameOver, _soundTetrisSong);

            _tetrisGameController.ParticleController =
                new ParticleController(_spriteBatch, _texPieceParticle, _texClearLineParticle);
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
            // the width of a square on screen
            var squareWidth = RescaleUnitsToPixels(PieceSize);

            // draw each square
            foreach (var square in _tetrisGameController.TetrisSquares)
            {
                // don't render this if it's null
                if (square == null) continue;

                var (squareX, squareY) = GetPixelLocationFromPieceLocation(square.PieceLocation);
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

            // draw the next piece
            Texture2D texOfNextPiece;
            (int x, int y) r1Coord;
            (int x, int y) r2Coord;
            (int x, int y) r3Coord;
            (int x, int y) r4Coord;
            switch (_tetrisGameController.BagOfPieces[0].Type)
            {
                case I:
                    texOfNextPiece = GetColor(Indigo);
                    r1Coord = GetAbsolutePixelCoordinates((23.5f, 22));
                    r2Coord = GetAbsolutePixelCoordinates((23.5f, 23));
                    r3Coord = GetAbsolutePixelCoordinates((23.5f, 24));
                    r4Coord = GetAbsolutePixelCoordinates((23.5f, 25));
                    break;
                case J:
                    texOfNextPiece = GetColor(Blue);
                    r1Coord = GetAbsolutePixelCoordinates((22.5f, 23));
                    r2Coord = GetAbsolutePixelCoordinates((22.5f, 24));
                    r3Coord = GetAbsolutePixelCoordinates((23.5f, 24));
                    r4Coord = GetAbsolutePixelCoordinates((24.5f, 24));
                    break;
                case L:
                    texOfNextPiece = GetColor(Orange);
                    r1Coord = GetAbsolutePixelCoordinates((22.5f, 23));
                    r2Coord = GetAbsolutePixelCoordinates((23.5f, 23));
                    r3Coord = GetAbsolutePixelCoordinates((24.5f, 23));
                    r4Coord = GetAbsolutePixelCoordinates((24.5f, 24));
                    break;
                case O:
                    texOfNextPiece = GetColor(Yellow);
                    r1Coord = GetAbsolutePixelCoordinates((23, 23));
                    r2Coord = GetAbsolutePixelCoordinates((23, 24));
                    r3Coord = GetAbsolutePixelCoordinates((24, 23));
                    r4Coord = GetAbsolutePixelCoordinates((24, 24));
                    break;
                case S:
                    texOfNextPiece = GetColor(Green);
                    r1Coord = GetAbsolutePixelCoordinates((22.5f, 23));
                    r2Coord = GetAbsolutePixelCoordinates((23.5f, 23));
                    r3Coord = GetAbsolutePixelCoordinates((23.5f, 24));
                    r4Coord = GetAbsolutePixelCoordinates((24.5f, 24));
                    break;
                case T:
                    texOfNextPiece = GetColor(Violet);
                    r1Coord = GetAbsolutePixelCoordinates((22.5f, 23));
                    r2Coord = GetAbsolutePixelCoordinates((23.5f, 23));
                    r3Coord = GetAbsolutePixelCoordinates((23.5f, 24));
                    r4Coord = GetAbsolutePixelCoordinates((24.5f, 23));
                    break;
                case Z:
                    texOfNextPiece = GetColor(Red);
                    r1Coord = GetAbsolutePixelCoordinates((22.5f, 24));
                    r2Coord = GetAbsolutePixelCoordinates((23.5f, 24));
                    r3Coord = GetAbsolutePixelCoordinates((23.5f, 23));
                    r4Coord = GetAbsolutePixelCoordinates((24.5f, 23));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var rect1 = new Rectangle(r1Coord.x, r1Coord.y, squareWidth, squareWidth);
            var rect2 = new Rectangle(r2Coord.x, r2Coord.y, squareWidth, squareWidth);
            var rect3 = new Rectangle(r3Coord.x, r3Coord.y, squareWidth, squareWidth);
            var rect4 = new Rectangle(r4Coord.x, r4Coord.y, squareWidth, squareWidth);

            // draw next four pieces
            foreach (var rect in new[] {rect1, rect2, rect3, rect4})
            {
                _spriteBatch.Draw(
                    texOfNextPiece,
                    rect,
                    null,
                    Color.White,
                    0,
                    // center origin in the texture
                    new Vector2(0, texOfNextPiece.Height),
                    SpriteEffects.None,
                    0);
            }

            // draw score, level, lines cleared
            var scoreString = "Score: " + _tetrisGameController.Score;
            var levelString = "Level: " + _tetrisGameController.Level;
            var linesClearedString = "Lines cleared: " + _tetrisGameController.LinesCleared;

            var (scoreX, scoreY) = GetAbsolutePixelCoordinates((3f, 26.8f));
            var (levelX, levelY) = GetAbsolutePixelCoordinates((3f, 25.3f));
            var (linesX, linesY) = GetAbsolutePixelCoordinates((3f, 23.8f));

            _spriteBatch.DrawString(_gameFont, scoreString,
                new Vector2(scoreX, scoreY),
                Color.Black);
            _spriteBatch.DrawString(_gameFont, levelString,
                new Vector2(levelX, levelY),
                Color.Black);
            _spriteBatch.DrawString(_gameFont, linesClearedString,
                new Vector2(linesX, linesY),
                Color.Black);

            // draw particles
            _tetrisGameController.ParticleController.Draw();

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

        // this function gets the absolute dimensions
        public static (float x, float y) GetRelativeBoardCoordinates((int x, int y) absoluteCoordinates)
        {
            // keep relative coordinates good
            if (absoluteCoordinates.x < 0 || absoluteCoordinates.x > BoardSize ||
                absoluteCoordinates.y < 0 || absoluteCoordinates.y > BoardSize)
            {
                // uncomment this line if we want to force to stay in safe area
                // throw new Exception("Relative coordinates must be between 0 and " + BoardSize + ".");
            }

            // get size of playable area (will be constrained by height)
            var sizeOfGameAreaPixels = _windowHeightPixels;

            // width will be square centered in screen, same dimensions as height
            var horizontalMarginPixels = (_windowWidthPixels - sizeOfGameAreaPixels) / 2;

            // properly rescale the coordinates to get the correct pixels
            var rescaledX = RescalePixelsToUnits(absoluteCoordinates.x - horizontalMarginPixels);
            var rescaledY = RescalePixelsToUnits(sizeOfGameAreaPixels - absoluteCoordinates.y);

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

        // given a pixel count, rescale to units
        public static float RescalePixelsToUnits(int pixels)
        {
            var rescaledUnits = (float) BoardSize / _windowHeightPixels * pixels;
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
