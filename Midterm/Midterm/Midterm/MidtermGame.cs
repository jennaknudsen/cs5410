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
        private Texture2D _texParticle;
        private Texture2D _texBackgroundDimmer;
        private SpriteFont _menuFont;
        private SpriteFont _gameFont;
        private Texture2D _texBomb;
        private Texture2D _texExplosion;
        private Texture2D _texCheckmark;
        private Texture2D _texNum0;
        private Texture2D _texNum1;
        private Texture2D _texNum2;
        private Texture2D _texNum3;
        private Texture2D _texNum4;
        private Texture2D _texNum5;
        private Texture2D _texNum6;
        private Texture2D _texNum7;
        private Texture2D _texNum8;
        private Texture2D _texNum9;

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

            _texBoard = this.Content.Load<Texture2D>("Background");
            _texParticle = this.Content.Load<Texture2D>("ParticleClear");
            _texBackgroundDimmer = this.Content.Load<Texture2D>("background-dimmer");
            _menuFont = this.Content.Load<SpriteFont>("MenuFont");
            _gameFont = this.Content.Load<SpriteFont>("GameFont");
            _texBomb = this.Content.Load <Texture2D> ("Bomb");
            _texExplosion = this.Content.Load<Texture2D>("Explosion");
            _texCheckmark = this.Content.Load<Texture2D>("Explosion");
            _texNum0 = this.Content.Load<Texture2D>("glass_numbers_0");
            _texNum1 = this.Content.Load<Texture2D>("glass_numbers_1");
            _texNum2 = this.Content.Load<Texture2D>("glass_numbers_2");
            _texNum3 = this.Content.Load<Texture2D>("glass_numbers_3");
            _texNum4 = this.Content.Load<Texture2D>("glass_numbers_4");
            _texNum5 = this.Content.Load<Texture2D>("glass_numbers_5");
            _texNum6 = this.Content.Load<Texture2D>("glass_numbers_6");
            _texNum7 = this.Content.Load<Texture2D>("glass_numbers_7");
            _texNum8 = this.Content.Load<Texture2D>("glass_numbers_8");
            _texNum9 = this.Content.Load<Texture2D>("glass_numbers_9");

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
                DrawMainMenu(gameTime);
            }
            else
            {
                DrawGameRunning(gameTime);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // draws the main menu
        private void DrawMainMenu(GameTime gameTime)
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
                    var vcColor = inputHandler.ViewCreditsButton.IsHovered
                        ? Color.Yellow
                        : Color.LightGray;

                    var newGameString = "New Game";
                    var highScoresString = "High Scores";
                    var customizeControlsString = "Customize Controls";
                    var viewCreditsString = "View Credits";

                    var (ngX, ngY) = GetMouseButtonPixelCoordinates(inputHandler.NewGameButton);
                    var (hsX, hsY) = GetMouseButtonPixelCoordinates(inputHandler.HighScoresButton);
                    var (vcX, vcY) = GetMouseButtonPixelCoordinates(inputHandler.ViewCreditsButton);

                    // now, draw the strings
                    _spriteBatch.DrawString(_menuFont, newGameString,
                        new Vector2(ngX, ngY),
                        ngColor);
                    _spriteBatch.DrawString(_menuFont, highScoresString,
                        new Vector2(hsX, hsY),
                        hsColor);
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

        // draws the pause menu
        private void DrawPauseMenu(GameTime gameTime)
        {
            var inputHandler = _midtermGameController.InputHandler;

            var pausedString = "=== PAUSED ===";
            var resumeString = "Resume";
            var quitString = "Quit";

            var (pausedX, pausedY) = GetAbsolutePixelCoordinates((10, 85));
            var (resumeX, resumeY) = GetMouseButtonPixelCoordinates(inputHandler.ResumeButton);
            var (quitX, quitY) = GetMouseButtonPixelCoordinates(inputHandler.QuitButton);

            var resumeColor = inputHandler.ResumeButton.IsHovered
                ? Color.Yellow
                : Color.LightGray;
            var quitColor = inputHandler.QuitButton.IsHovered
                ? Color.Yellow
                : Color.LightGray;

            // PAUSED
            _spriteBatch.DrawString(_menuFont, pausedString,
                new Vector2(pausedX, pausedY),
                Color.LightGray);
            // resume
            _spriteBatch.DrawString(_menuFont, resumeString,
                new Vector2(resumeX, resumeY),
                resumeColor);
            // quit to main menu
            _spriteBatch.DrawString(_menuFont, quitString,
                new Vector2(quitX, quitY),
                quitColor);
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
            var (scoreX, scoreY) = GetAbsolutePixelCoordinates((12f, 85f));

            _spriteBatch.DrawString(_gameFont, scoreString,
                new Vector2(scoreX, scoreY),
                Color.Black);

            // draw bombs
            var bombSize = RescaleUnitsToPixels(10);
            for (var i = 0; i < 12; i++)
            {
                if (_midtermGameController.Bombs[i].IsEnabled)
                {
                    var (x, y) = i switch
                    {
                        0 => (35, 30),
                        1 => (45, 30),
                        2 => (55, 30),
                        3 => (35, 40),
                        4 => (45, 40),
                        5 => (55, 40),
                        6 => (35, 50),
                        7 => (45, 50),
                        8 => (55, 50),
                        9 => (35, 60),
                        10 => (45, 60),
                        11 => (45, 60),
                        _ => (0, 0)
                    };

                    var (scaledX, scaledY) = GetAbsolutePixelCoordinates((x, y));
                    var (scaledNumX, scaledNumY) = GetAbsolutePixelCoordinates((x + 2.6f, y + 0.25f));
                    var bombRect = new Rectangle(scaledX, scaledY, bombSize, bombSize);
                    var numRect = new Rectangle(scaledNumX, scaledNumY, (int) (bombSize * 0.75), (int) (bombSize * 0.75));
                    _spriteBatch.Draw(
                        _texBomb,
                        bombRect,
                        null,
                        Color.White,
                        0,
                        new Vector2(0, _texBomb.Height),
                        SpriteEffects.None,
                        0
                        );
                    _spriteBatch.Draw(
                        GetNumTexture(_midtermGameController.Bombs[i].FuseTime),
                        numRect,
                        null,
                        Color.White,
                        0,
                        new Vector2(0, _texNum0.Height),
                        SpriteEffects.None,
                        0
                    );
                }
            }

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
            else if (_midtermGameController.GameState == Paused)
            {
                // pause menu will always have the background rectangle drawn
                // get pixel coordinates from board coordinates
                var (backX, backY) = GetAbsolutePixelCoordinates((0, BoardSize));
                var rectSizePixels = RescaleUnitsToPixels(BoardSize);
                // create the background rectangle
                var backgroundRect = new Rectangle(backX, backY, rectSizePixels, rectSizePixels);
                _spriteBatch.Draw(_texBackgroundDimmer, backgroundRect, Color.White);
                DrawPauseMenu(gameTime);
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

        // return the correct texture
        private Texture2D GetNumTexture(int num)
        {
            return num switch
            {
                1 => _texNum1,
                2 => _texNum2,
                3 => _texNum3,
                4 => _texNum4,
                5 => _texNum5,
                6 => _texNum6,
                7 => _texNum7,
                8 => _texNum8,
                9 => _texNum9,
                _ => _texNum0
            };
        }
    }
}
