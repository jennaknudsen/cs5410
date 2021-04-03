using System;
using FinalProject_Tetris.InputHandling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using static FinalProject_Tetris.GameState;
using static FinalProject_Tetris.Menuing.MenuState;
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
            _texClearLineParticle = this.Content.Load<Texture2D>("ParticleClear");
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

            _tetrisGameController.ParticleController = new ParticleController(
                _spriteBatch,
                _texParticleRed,
                _texParticleOrange,
                _texParticleYellow,
                _texParticleGreen,
                _texParticleBlue,
                _texParticleIndigo,
                _texParticleViolet,
                _texClearLineParticle
                );
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

            if (_tetrisGameController.GameState == MainMenu)
            {
                // main menu will always have the background rectangle drawn
                _spriteBatch.Draw(_texBackgroundDimmer, backgroundRect, Color.White);
                DrawMenu(gameTime);
            }
            else
            {
                DrawGameRunning(gameTime);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        // draws the main menu
        private void DrawMenu(GameTime gameTime)
        {
            var inputHandler = _tetrisGameController.InputHandler;
            switch (_tetrisGameController.MainMenuController.MenuState)
            {
                case Main:
                    // draw input button
                    var ngColor = inputHandler.NewGameButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;
                    var hsColor = inputHandler.HighScoresButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;
                    var ccColor = inputHandler.CustomizeControlsButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;
                    var vcColor = inputHandler.ViewCreditsButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;

                    var newGameString = "New Game";
                    var highScoresString = "High Scores";
                    var customizeControlsString = "Customize Controls";
                    var viewCreditsString = "View Credits";

                    var (ngX, ngY) = GetMouseButtonPixelCoordinates(inputHandler.NewGameButton);
                    var (hsX, hsY) = GetMouseButtonPixelCoordinates(inputHandler.HighScoresButton);
                    var (ccX, ccY) = GetMouseButtonPixelCoordinates(inputHandler.CustomizeControlsButton);
                    var (vcX, vcY) = GetMouseButtonPixelCoordinates(inputHandler.ViewCreditsButton);

                    // now, draw the strings
                    _spriteBatch.DrawString(_menuFont, newGameString,
                        new Vector2(ngX, ngY),
                        ngColor);
                    _spriteBatch.DrawString(_menuFont, highScoresString,
                        new Vector2(hsX, hsY),
                        hsColor);
                    _spriteBatch.DrawString(_menuFont, customizeControlsString,
                        new Vector2(ccX, ccY),
                        ccColor);
                    _spriteBatch.DrawString(_menuFont, viewCreditsString,
                        new Vector2(vcX, vcY),
                        vcColor);
                    break;
                case HighScores:
                    var highScoresTitleString = "High Scores:";
                    var highScoresBodyString = "";

                    // this will only be null for one frame
                    // this will only be null for one frame
                    if (_tetrisGameController.MainMenuController.HighScoresIntList != null)
                    {
                        var position = 1;

                        foreach (var hs in _tetrisGameController.MainMenuController.HighScoresIntList)
                        {
                            highScoresBodyString += position + ") " + hs;
                            position++;

                            // only display the top 5 scores
                            if (position > 5) break;
                        }

                        if (highScoresBodyString.Equals(""))
                        {
                            highScoresBodyString = "No high scores yet.";
                        }
                    }

                    // get color of button
                    var mmString1 = "Main Menu";
                    var mmColor1 = inputHandler.BackToMainButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;

                    var (hsTitleX, hsTitleY) = GetAbsolutePixelCoordinates((3, 24));
                    var (hsContentsX, hsContentsY) = GetAbsolutePixelCoordinates((3, 20));
                    var returnToMainPos = GetMouseButtonPixelCoordinates(inputHandler.BackToMainButton);

                    // draw the menu items
                    _spriteBatch.DrawString(_menuFont, highScoresTitleString,
                        new Vector2(hsTitleX, hsTitleY),
                        Color.LightGray);
                    _spriteBatch.DrawString(_menuFont, highScoresBodyString,
                        new Vector2(hsContentsX, hsContentsY),
                        Color.LightGray);
                    _spriteBatch.DrawString(_menuFont, mmString1,
                        new Vector2(returnToMainPos.x, returnToMainPos.y),
                        mmColor1);

                    break;
                case Controls:
                    var leftColor = inputHandler.LeftControlButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;
                    var rightColor = inputHandler.RightControlButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;
                    var upColor = inputHandler.UpControlButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;
                    var downColor = inputHandler.DownControlButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;
                    var rotateClockwiseColor = inputHandler.ClockwiseControlButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;
                    var rotateCounterClockwiseColor = inputHandler.CounterClockwiseControlButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;
                    var resetDefaultsColor = inputHandler.ResetToDefaultsButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;
                    var mainMenuColor2 = inputHandler.BackToMainButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;

                    // buttons themselves
                    var moveLeftString = "Move left: ";
                    var moveRightString = "Move right: ";
                    var softDropString = "Soft drop: ";
                    var hardDropString = "Hard drop: ";
                    var rotateClockwiseString = "Rotate clockwise: ";
                    var rotateCounterClockwiseString = "Rotate counterclockwise: ";
                    var resetDefaultsString = "Reset to Defaults";
                    var mmString2 = "Main Menu";

                    // key bind indicators
                    var moveLeftChars = "";
                    var moveRightChars = "";
                    var softDropChars = "";
                    var hardDropChars = "";
                    var rotateClockwiseChars = "";
                    var rotateCounterClockwiseChars = "";

                    // key indicator colors
                    var moveLeftKeysColor = Color.White;
                    var moveRightKeysColor = Color.White;
                    var softDropKeysColor = Color.White;
                    var hardDropKeysColor = Color.White;
                    var rotateClockwiseKeysColor = Color.White;
                    var rotateCounterClockwiseKeysColor = Color.White;

                    var inBinding = _tetrisGameController.MainMenuController.InControlBinding;
                    var rebindingButton = _tetrisGameController.MainMenuController.RebindingButton;

                    // move left
                    if (inBinding && rebindingButton == inputHandler.MovePieceLeftButton)
                    {
                        moveLeftChars = _tetrisGameController.MainMenuController.BindingKeysString;
                        moveLeftKeysColor = Color.Yellow;
                    }
                    else
                    {
                        foreach (var key in inputHandler.MovePieceLeftButton.BoundKeys)
                            moveLeftChars += key.ToString() + ", ";
                        if (moveLeftChars.Length > 0)
                            moveLeftChars = moveLeftChars.Remove(moveLeftChars.Length - 2);
                    }

                    // move right
                    if (inBinding && rebindingButton == inputHandler.MovePieceRightButton)
                    {
                        moveRightChars = _tetrisGameController.MainMenuController.BindingKeysString;
                        moveRightKeysColor = Color.Yellow;
                    }
                    else
                    {
                        foreach (var key in inputHandler.MovePieceRightButton.BoundKeys)
                            moveRightChars += key.ToString() + ", ";
                        if (moveRightChars.Length > 0)
                            moveRightChars = moveRightChars.Remove(moveRightChars.Length - 2);
                    }

                    // soft drop
                    if (inBinding && rebindingButton == inputHandler.SoftDropButton)
                    {
                        softDropChars = _tetrisGameController.MainMenuController.BindingKeysString;
                        softDropKeysColor = Color.Yellow;
                    }
                    else
                    {
                        foreach (var key in inputHandler.SoftDropButton.BoundKeys)
                            softDropChars += key.ToString() + ", ";
                        if (softDropChars.Length > 0)
                            softDropChars = softDropChars.Remove(softDropChars.Length - 2);
                    }

                    // hard drop
                    if (inBinding && rebindingButton == inputHandler.HardDropButton)
                    {
                        hardDropChars = _tetrisGameController.MainMenuController.BindingKeysString;
                        hardDropKeysColor = Color.Yellow;
                    }
                    else
                    {
                        foreach (var key in inputHandler.HardDropButton.BoundKeys)
                            hardDropChars += key.ToString() + ", ";
                        if (hardDropChars.Length > 0)
                            hardDropChars = hardDropChars.Remove(hardDropChars.Length - 2);
                    }

                    // rotate clockwise
                    if (inBinding && rebindingButton == inputHandler.RotateClockwiseButton)
                    {
                        rotateClockwiseChars = _tetrisGameController.MainMenuController.BindingKeysString;
                        rotateClockwiseColor = Color.Yellow;
                    }
                    else
                    {
                        foreach (var key in inputHandler.RotateClockwiseButton.BoundKeys)
                            rotateClockwiseChars += key.ToString() + ", ";
                        if (rotateClockwiseChars.Length > 0)
                            rotateClockwiseChars = rotateClockwiseChars.Remove(rotateClockwiseChars.Length - 2);
                    }

                    // rotate counterclockwise
                    if (inBinding && rebindingButton == inputHandler.RotateCounterClockwiseButton)
                    {
                        rotateCounterClockwiseChars = _tetrisGameController.MainMenuController.BindingKeysString;
                        rotateClockwiseColor = Color.Yellow;
                    }
                    else
                    {
                        foreach (var key in inputHandler.RotateCounterClockwiseButton.BoundKeys)
                            rotateCounterClockwiseChars += key.ToString() + ", ";
                        if (rotateCounterClockwiseChars.Length > 0)
                            rotateCounterClockwiseChars = rotateCounterClockwiseChars.Remove(rotateCounterClockwiseChars.Length - 2);
                    }

                    // 3, 24 - title
                    var verticalOffset = 30;
                    var (titleX, titleY) = GetAbsolutePixelCoordinates((3, 24));
                    var (mlX, mlY) = GetMouseButtonPixelCoordinates(inputHandler.LeftControlButton);
                    var (mltX, mltY) = (mlX, mlY + verticalOffset);
                    var (mrX, mrY) = GetMouseButtonPixelCoordinates(inputHandler.RightControlButton);
                    var (mrtX, mrtY) = (mrX, mrY + verticalOffset);
                    var (sdX, sdY) = GetMouseButtonPixelCoordinates(inputHandler.DownControlButton);
                    var (sdtX, sdtY) = (sdX, sdY + verticalOffset);
                    var (hdX, hdY) = GetMouseButtonPixelCoordinates(inputHandler.UpControlButton);
                    var (hdtX, hdtY) = (hdX, hdY + verticalOffset);
                    var (rcX, rcY) = GetMouseButtonPixelCoordinates(inputHandler.ClockwiseControlButton);
                    var (rctX, rctY) = (rcX, rcY + verticalOffset);
                    var (rccX, rccY) = GetMouseButtonPixelCoordinates(inputHandler.CounterClockwiseControlButton);
                    var (rcctX, rcctY) = (rccX, rccY + verticalOffset);
                    var (defX, defY) = GetMouseButtonPixelCoordinates(inputHandler.ResetToDefaultsButton);
                    var (mm2X, mm2Y) = GetMouseButtonPixelCoordinates(inputHandler.BackToMainButton);

                    // draw strings for buttons
                    // left
                    _spriteBatch.DrawString(_menuFont, moveLeftString,
                        new Vector2(mlX, mlY),
                        leftColor);
                    _spriteBatch.DrawString(_gameFont, moveLeftChars,
                        new Vector2(mltX, mltY),
                        moveLeftKeysColor);
                    // right
                    _spriteBatch.DrawString(_menuFont, moveRightString,
                        new Vector2(mrX, mrY),
                        rightColor);
                    _spriteBatch.DrawString(_gameFont, moveRightChars,
                        new Vector2(mrtX, mrtY),
                        moveRightKeysColor);
                    // down
                    _spriteBatch.DrawString(_menuFont, softDropString,
                        new Vector2(sdX, sdY),
                        downColor);
                    _spriteBatch.DrawString(_gameFont, softDropChars,
                        new Vector2(sdtX, sdtY),
                        softDropKeysColor);
                    // up
                    _spriteBatch.DrawString(_menuFont, hardDropString,
                        new Vector2(hdX, hdY),
                        upColor);
                    _spriteBatch.DrawString(_gameFont, hardDropChars,
                        new Vector2(hdtX, hdtY),
                        hardDropKeysColor);
                    // rotate clockwise
                    _spriteBatch.DrawString(_menuFont, rotateClockwiseString,
                        new Vector2(rcX, rcY),
                        rotateClockwiseColor);
                    _spriteBatch.DrawString(_gameFont, rotateClockwiseChars,
                        new Vector2(rctX, rctY),
                        rotateCounterClockwiseKeysColor);
                    // rotate counterclockwise
                    _spriteBatch.DrawString(_menuFont, rotateCounterClockwiseString,
                        new Vector2(rccX, rccY),
                        rotateCounterClockwiseColor);
                    _spriteBatch.DrawString(_gameFont, rotateCounterClockwiseChars,
                        new Vector2(rcctX, rcctY),
                        rotateCounterClockwiseKeysColor);


                    break;

                case Credits:
                    var creditsString = "CREDITS:";
                    var creditsBodyString = @"All game logic, artwork, and sounds created
by me (Jonas Knudsen), with the exception of some
starter code provided by the instructor, and the
background cloud texture, which I found at:
https://opengameart.org/sites/default/
files/tilesetOpenGameBackground_1.png

Got information about how Tetris works from:
https://tetris.fandom.com/wiki/Tetris_Wiki

Got the AI for Attract Mode from:
https://codemyroad.wordpress.com/2013/04/14/
tetris-ai-the-near-perfect-player/

Used StackOverflow for a couple of small code
snippets. These snippets are cited in comments
found in the source code.";

                    var mmString3 = "Main Menu";
                    var mmColor3 = inputHandler.BackToMainButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;

                    var (crTitleX, crTitleY) = GetAbsolutePixelCoordinates((3, 24));
                    var (crContentsX, crContentsY) = GetAbsolutePixelCoordinates((3, 20));
                    var returnToMainPos3 = GetMouseButtonPixelCoordinates(inputHandler.BackToMainButton);

                    // draw the menu items
                    _spriteBatch.DrawString(_menuFont, creditsString,
                        new Vector2(crTitleX, crTitleY),
                        Color.LightGray);
                    _spriteBatch.DrawString(_gameFont, creditsBodyString,
                        new Vector2(crContentsX, crContentsY),
                        Color.LightGray);
                    _spriteBatch.DrawString(_menuFont, mmString3,
                        new Vector2(returnToMainPos3.x, returnToMainPos3.y),
                        mmColor3);

                    break;
                default:
                    break;
            }
        }

        // get pixel coordinates of a mouse button
        private (int x, int y) GetMouseButtonPixelCoordinates(MouseButton mouseButton)
        {
            var (x, _) = GetAbsolutePixelCoordinates(mouseButton.StartPosition);
            var (_, y1) = GetAbsolutePixelCoordinates(mouseButton.StartPosition);
            var (_, y2) = GetAbsolutePixelCoordinates(mouseButton.EndPosition);

            return (x, y1 + (y2 - y1) / 2);
        }

        // draws the game actually running
        private void DrawGameRunning(GameTime gameTime)
        {
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
                    r1Coord = GetAbsolutePixelCoordinates((22.5f, 24));
                    r2Coord = GetAbsolutePixelCoordinates((22.5f, 23));
                    r3Coord = GetAbsolutePixelCoordinates((23.5f, 23));
                    r4Coord = GetAbsolutePixelCoordinates((24.5f, 23));
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
