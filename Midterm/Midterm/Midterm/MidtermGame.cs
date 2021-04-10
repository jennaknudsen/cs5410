using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Midterm.InputHandling;
using static Midterm.GameState;
using static Midterm.Menuing.MenuState;
using static Midterm.MidtermGameController;

namespace Midterm
{
    public class MidtermGame : Game
    {
        // import textures here
        private Texture2D _texBoard;
        private SoundEffect _soundBlockPlace;
        private Texture2D _texParticle;
        private Texture2D _texBackgroundDimmer;
        private SpriteFont _menuFont;
        private SpriteFont _gameFont;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // width and height of window
        private static int _windowWidthPixels;
        private static int _windowHeightPixels;

        // reference to midterm game controller
        private MidtermGameController _midtermGameController;

        public MidtermGame()
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

            _midtermGameController = new MidtermGameController();

            var canvasBounds = GraphicsDevice.Viewport.Bounds;
            _windowWidthPixels = canvasBounds.Width;
            _windowHeightPixels = canvasBounds.Height;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _texBoard = this.Content.Load<Texture2D>("board");
            _texParticle = this.Content.Load<Texture2D>("ParticleClear");
            _texBackgroundDimmer = this.Content.Load<Texture2D>("background-dimmer");
            _soundBlockPlace = this.Content.Load<SoundEffect>("BlockPlace");
            _menuFont = this.Content.Load<SpriteFont>("MenuFont");
            _gameFont = this.Content.Load<SpriteFont>("GameFont");

            // initialize the game controller's sound now that music is imported
            _midtermGameController.SoundController = new SoundController(
                _soundBlockPlace);

            // initialize the particle controller
            _midtermGameController.ParticleController = new ParticleController(
                _spriteBatch, _texParticle);
        }

        protected override void Update(GameTime gameTime)
        {
            _midtermGameController.Update(gameTime);

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

            if (_midtermGameController.GameState == MainMenu)
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
            var inputHandler = _midtermGameController.InputHandler;
            switch (_midtermGameController.MainMenuController.MenuState)
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
                    if (_midtermGameController.MainMenuController.HighScoresIntList != null)
                    {
                        var position = 1;

                        foreach (var hs in _midtermGameController.MainMenuController.HighScoresIntList)
                        {
                            highScoresBodyString += position + ") " + hs + "\n";
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

                    var (hsTitleX, hsTitleY) = GetAbsolutePixelCoordinates((10, 80));
                    var (hsContentsX, hsContentsY) = GetAbsolutePixelCoordinates((10, 67));
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

                    var inBinding = _midtermGameController.MainMenuController.InControlBinding;
                    var rebindingButton = _midtermGameController.MainMenuController.RebindingButton;

                    // move left
                    if (inBinding && rebindingButton == inputHandler.MovePieceLeftButton)
                    {
                        moveLeftChars = _midtermGameController.MainMenuController.BindingKeysString;
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
                        moveRightChars = _midtermGameController.MainMenuController.BindingKeysString;
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
                        softDropChars = _midtermGameController.MainMenuController.BindingKeysString;
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
                        hardDropChars = _midtermGameController.MainMenuController.BindingKeysString;
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
                        rotateClockwiseChars = _midtermGameController.MainMenuController.BindingKeysString;
                        rotateClockwiseKeysColor = Color.Yellow;
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
                        rotateCounterClockwiseChars = _midtermGameController.MainMenuController.BindingKeysString;
                        rotateCounterClockwiseKeysColor = Color.Yellow;
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
                        rotateClockwiseKeysColor);
                    // rotate counterclockwise
                    _spriteBatch.DrawString(_menuFont, rotateCounterClockwiseString,
                        new Vector2(rccX, rccY),
                        rotateCounterClockwiseColor);
                    _spriteBatch.DrawString(_gameFont, rotateCounterClockwiseChars,
                        new Vector2(rcctX, rcctY),
                        rotateCounterClockwiseKeysColor);
                    // defaults
                    _spriteBatch.DrawString(_menuFont, resetDefaultsString,
                        new Vector2(defX, defY),
                        resetDefaultsColor);
                    // back to main menu
                    _spriteBatch.DrawString(_menuFont, mmString2,
                        new Vector2(mm2X, mm2Y),
                        mainMenuColor2);

                    break;

                case Credits:
                    var creditsString = "CREDITS:";
                    var creditsBodyString = @"CREDITS TO COME
AT A LATER TIME (DO THIS).";

                    var mmString3 = "Main Menu";
                    var mmColor3 = inputHandler.BackToMainButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;

                    var (crTitleX, crTitleY) = GetAbsolutePixelCoordinates((10, 80));
                    var (crContentsX, crContentsY) = GetAbsolutePixelCoordinates((10, 67));
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

            return (x, y1 + (y2 - y1) * 3 / 4);
        }

        // draws the game actually running
        private void DrawGameRunning(GameTime gameTime)
        {

            // draw score, level, lines cleared
            var scoreString = "Score: " + _midtermGameController.Score;
            var (scoreX, scoreY) = GetAbsolutePixelCoordinates((3f, 26.8f));

            _spriteBatch.DrawString(_gameFont, scoreString,
                new Vector2(scoreX, scoreY),
                Color.Black);

            // draw particles
            _midtermGameController.ParticleController.Draw();

            // if game over, draw the game over text
            if (_midtermGameController.GameState == GameOver)
            {
                var (rectX, rectY) = GetAbsolutePixelCoordinates((0, 18));
                var width = RescaleUnitsToPixels(30);
                var height = RescaleUnitsToPixels(6);
                var gameOverRect = new Rectangle(rectX, rectY, width, height);

                _spriteBatch.Draw(_texBackgroundDimmer, gameOverRect, Color.White);

                // draw the text over the dimmed background
                var gameOverText = "Game Over. Final score: " + _midtermGameController.Score + "\n" +
                                   "Returning to main menu in ";

                var resetTime = _midtermGameController.GameOverTime;
                if (resetTime.TotalSeconds > 4)
                    gameOverText += "5";
                else if (resetTime.TotalSeconds > 3)
                    gameOverText += "4";
                else if (resetTime.TotalSeconds > 2)
                    gameOverText += "3";
                else if (resetTime.TotalSeconds > 1)
                    gameOverText += "2";
                else
                    gameOverText += "1";

                // get position of the text on screen
                var (textPosX, textPosY) = GetAbsolutePixelCoordinates((7, 16));

                _spriteBatch.DrawString(_menuFont, gameOverText,
                    new Vector2(textPosX, textPosY),
                    Color.OrangeRed);
            }
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
            var rescaledUnits = (int) (_windowHeightPixels / (float) BoardSize * units);
            return rescaledUnits;
        }

        // given a pixel count, rescale to units
        public static float RescalePixelsToUnits(int pixels)
        {
            var rescaledUnits = (float) BoardSize / _windowHeightPixels * pixels;
            return rescaledUnits;
        }

    }
}
