using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static LunarLander.GameState;
using static LunarLander.LanderGameController;
using static LunarLander.MenuState;

namespace LunarLander
{
    public class LanderGame : Game
    {
        // game controller handles all underlying logic
        private LanderGameController _landerGameController;

        // assets for this game
        private Texture2D _texLander;
        private Texture2D _texSpaceBackground;
        private Texture2D _texBackgroundDimmer;
        private Rectangle _positionRectangle;
        private SpriteFont _gameFont;
        private SpriteFont _menuFont;
        private BasicEffect _basicEffect;

        // stuff for terrain rendering
        private VertexPositionColor[] _terrainVertexPositionColors;
        private int[] _terrainIndexArray;

        // MonoGame stuff
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public LanderGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Uncomment to change game render resolution
            // _graphics.PreferredBackBufferWidth = 1280;
            // _graphics.PreferredBackBufferHeight = 720;
            // _graphics.ApplyChanges();

            _landerGameController = new LanderGameController();
            _landerGameController.StartLevel(1);

            _graphics.GraphicsDevice.RasterizerState = new RasterizerState
            {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,   // CullMode.None If you want to not worry about triangle winding order
                MultiSampleAntiAlias = true,
            };

            _basicEffect = new BasicEffect(_graphics.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up),
                Projection = Matrix.CreatePerspectiveOffCenter(0.0f, _graphics.GraphicsDevice.Viewport.Width,
                    _graphics.GraphicsDevice.Viewport.Height, 0, 1.0f, 1000.0f)
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _texLander = this.Content.Load<Texture2D>("Lander-2");
            _texSpaceBackground = this.Content.Load<Texture2D>("space-background");
            _texBackgroundDimmer = this.Content.Load<Texture2D>("background-dimmer");
            _gameFont = this.Content.Load<SpriteFont>("GameFont");
            _menuFont = this.Content.Load<SpriteFont>("MenuFont");
        }

        protected override void Update(GameTime gameTime)
        {
            // all updating is handled in the game controller
            _landerGameController.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // get pixel coordinates from board coordinates
            var (backX, backY) = GetAbsolutePixelCoordinates((0, BoardSize));
            var rectSizePixels = RescaleUnitsToPixels(BoardSize);
            // create the background rectangle
            var backgroundRect = new Rectangle(backX, backY, rectSizePixels, rectSizePixels);
            _spriteBatch.Draw(_texSpaceBackground, backgroundRect, Color.Gray);

            // end spritebatch here so we can draw terrain over background
            _spriteBatch.End();

            // next, draw the terrain (if generated)
            if (_landerGameController.TerrainGenerated)
            {
                // only calculate the terrain once
                if (_landerGameController.RecalculateTerrain)
                {
                    GenerateTerrainVertices();
                    _landerGameController.RecalculateTerrain = false;
                }

                // this will draw the terrain polygon itself
                foreach (var pass in _basicEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    _graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleStrip,
                        _terrainVertexPositionColors, 0, _terrainVertexPositionColors.Length,
                        _terrainIndexArray, 0, _terrainIndexArray.Length - 2
                    );
                }
            }

            // Now, draw the lander
            _spriteBatch.Begin();

            // set lander position rectangle
            // sets the top left of lander (we are re-adjusting the texture origin in the Draw() function)
            var lander = _landerGameController.Lander;
            var (landerX, landerY) = GetAbsolutePixelCoordinates((lander.Position.x,
                lander.Position.y));
            var landerSizePixels = RescaleUnitsToPixels(Lander.Size);
            _positionRectangle = new Rectangle(landerX, landerY, landerSizePixels, landerSizePixels);

            // run draw function
            _spriteBatch.Draw(_texLander,
                _positionRectangle,
                null,
                Color.Aqua,
                lander.Orientation,
                // center origin in the texture
                new Vector2(_texLander.Width / 2, _texLander.Width / 2),
                SpriteEffects.None,
                0);

            _spriteBatch.End();

            // get current fuel, speed, angle values
            var fuel = _landerGameController.Lander.FuelCapacity.TotalSeconds;
            var speed = _landerGameController.Lander.VelocityTotal;
            var angle = _landerGameController.Lander.OrientationDegrees;
            var safeArea = _landerGameController.InSafeArea ? "Yes" : "No";

            // set the correct colors
            var fuelColor = fuel > 0d ? Color.Green : Color.White;
            var speedColor = speed < MaxSpeed ? Color.Green : Color.White;
            var angleColor = angle > MinAngle || angle < MaxAngle ?
                Color.Green : Color.White;
            var safeAreaColor = safeArea.Equals("Yes") ? Color.Green : Color.White;

            // set the formatted strings
            var fuelString = "Fuel: " +
                               fuel.ToString("0.000") + " s";
            var speedString = "Speed: " +
                               speed.ToString("0.000") + " m/s";
            var angleString = "Angle: " +
                               angle.ToString("0.000") + " degrees";
            var safeAreaString = "In safe area? " +
                               safeArea;

            // draw on-screen elements
            _spriteBatch.Begin();

            var (textPosX, textPosY) = GetAbsolutePixelCoordinates((BoardSize * 0.65f,
                BoardSize * 0.95f));

            _spriteBatch.DrawString(_gameFont, fuelString,
                new Vector2(textPosX, textPosY),
                fuelColor);
            _spriteBatch.DrawString(_gameFont, speedString,
                new Vector2(textPosX, textPosY + 20),
                speedColor);
            _spriteBatch.DrawString(_gameFont, angleString,
                new Vector2(textPosX, textPosY + 40),
                angleColor);
            _spriteBatch.DrawString(_gameFont, safeAreaString,
                new Vector2(textPosX, textPosY + 60),
                safeAreaColor);

            _spriteBatch.End();

            // TODO particles here

            // now, depending on game state, draw other things
            DrawBasedOnState();

            base.Draw(gameTime);
        }

        private void DrawBasedOnState()
        {
            // some declaration so things don't break on us
            var rectSizePixels = RescaleUnitsToPixels(BoardSize);
            int textPosX;
            int textPosY;

            switch (_landerGameController.GameState)
            {
                // draw different items based on game state
                case ShipCrashed:
                {
                    _spriteBatch.Begin();

                    // draw the backdrop rectangle for the text
                    var (gameOverRectPosX, gameOverRectPosY) = GetAbsolutePixelCoordinates((
                        0, BoardSize * 0.65f));
                    var gameOverRectHeight = (int) (rectSizePixels * 0.3);
                    var gameOverRect = new Rectangle(gameOverRectPosX, gameOverRectPosY, rectSizePixels, gameOverRectHeight);

                    _spriteBatch.Draw(_texBackgroundDimmer, gameOverRect, Color.Aqua);

                    // draw the game over text itself
                    var crashedString = "Game over. Your ship crashed.";
                    var restartingString = "Press ESC to start a new game.";

                    (textPosX, textPosY) = GetAbsolutePixelCoordinates((BoardSize * 0.1f,
                        BoardSize * 0.55f));
                    _spriteBatch.DrawString(_menuFont, crashedString,
                        new Vector2(textPosX, textPosY),
                        Color.Red);
                    _spriteBatch.DrawString(_menuFont, restartingString,
                        new Vector2(textPosX, textPosY + 30),
                        Color.Red);

                    _spriteBatch.End();
                    break;
                }
                case PassedLevel:
                {
                    _spriteBatch.Begin();

                    // draw the backdrop rectangle for the text
                    var (passedRectPosX, passedRectPosY) = GetAbsolutePixelCoordinates((
                        0, BoardSize * 0.60f));
                    var passedRectHeight = (int) (rectSizePixels * 0.2);
                    var passedRect = new Rectangle(passedRectPosX, passedRectPosY, rectSizePixels, passedRectHeight);

                    _spriteBatch.Draw(_texBackgroundDimmer, passedRect, Color.Aqua);

                    // draw the passed level text
                    var passedAdvancingIn = "Level passed! Advancing in: ";

                    // get how many seconds left before loading
                    if (_landerGameController.LoadingTime.TotalSeconds > 2)
                        passedAdvancingIn += "3";
                    else if (_landerGameController.LoadingTime.TotalSeconds > 1)
                        passedAdvancingIn += "2";
                    else
                        passedAdvancingIn += "1";

                    // now, get text coordinates and draw the string
                    (textPosX, textPosY) = GetAbsolutePixelCoordinates((BoardSize * 0.1f,
                        BoardSize * 0.52f));

                    _spriteBatch.DrawString(_menuFont, passedAdvancingIn,
                        new Vector2(textPosX, textPosY),
                        Color.GreenYellow);

                    _spriteBatch.End();
                    break;
                }
                // dim entire screen on pause
                case BeatGame:
                {
                    _spriteBatch.Begin();

                    // draw the backdrop rectangle for the text
                    var (beatGameRectPosX, beatGameRectPosY) = GetAbsolutePixelCoordinates((
                        0, BoardSize * 0.65f));
                    var beatGameRectHeight = (int) (rectSizePixels * 0.3);
                    var beatGameRect = new Rectangle(beatGameRectPosX, beatGameRectPosY, rectSizePixels, beatGameRectHeight);

                    _spriteBatch.Draw(_texBackgroundDimmer, beatGameRect, Color.Aqua);

                    // draw the won the game text itself
                    var beatGameString = "You beat the game! Congrats!";
                    var restartingString = "Press ESC to start a new game.";

                    (textPosX, textPosY) = GetAbsolutePixelCoordinates((BoardSize * 0.1f,
                        BoardSize * 0.55f));
                    _spriteBatch.DrawString(_menuFont, beatGameString,
                        new Vector2(textPosX, textPosY),
                        Color.Yellow);
                    _spriteBatch.DrawString(_menuFont, restartingString,
                        new Vector2(textPosX, textPosY + 30),
                        Color.Yellow);

                    _spriteBatch.End();
                    break;
                }
                case Paused:
                    DrawMenu();
                    break;
                case Running:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Drawing the menu is very long, so put it in its own function
        private void DrawMenu()
        {
            _spriteBatch.Begin();

            // get pixel coordinates from board coordinates
            var (backX, backY) = GetAbsolutePixelCoordinates((0, BoardSize));
            var rectSizePixels = RescaleUnitsToPixels(BoardSize);
            // create the background rectangle
            var backgroundRect = new Rectangle(backX, backY, rectSizePixels, rectSizePixels);
            _spriteBatch.Draw(_texBackgroundDimmer, backgroundRect, Color.Gray);

            // code to determine which menu elements to draw on pause
            if (_landerGameController.MenuController.MenuState == Main)
            {
                // for main menu, draw 4 elements on screen

                // get correct colors
                var ngColor = _landerGameController.MenuController.NewGameMenuItem.Selected
                    ? Color.Yellow
                    : Color.LightGray;
                var hsColor = _landerGameController.MenuController.HighScoresMenuItem.Selected
                    ? Color.Yellow
                    : Color.LightGray;
                var ccColor = _landerGameController.MenuController.CustomizeControlsMenuItem.Selected
                    ? Color.Yellow
                    : Color.LightGray;
                var vcColor = _landerGameController.MenuController.ViewCreditsMenuItem.Selected
                    ? Color.Yellow
                    : Color.LightGray;

                // get correct pixel coordinates for menu items
                var (xPos, _) = GetAbsolutePixelCoordinates((BoardSize * 0.1f, 0));
                var (_, yPos1) = GetAbsolutePixelCoordinates((0, BoardSize * 0.8f));
                var (_, yPos2) = GetAbsolutePixelCoordinates((0, BoardSize * 0.6f));
                var (_, yPos3) = GetAbsolutePixelCoordinates((0, BoardSize * 0.4f));
                var (_, yPos4) = GetAbsolutePixelCoordinates((0, BoardSize * 0.2f));

                // set proper strings
                var newGameString = "New Game";
                var highScoresString = "High Scores";
                var customizeControlsString = "Customize Controls";
                var viewCreditsString = "View Credits";

                // now, draw the strings
                _spriteBatch.DrawString(_menuFont, newGameString,
                    new Vector2(xPos, yPos1),
                    ngColor);
                _spriteBatch.DrawString(_menuFont, highScoresString,
                    new Vector2(xPos, yPos2),
                    hsColor);
                _spriteBatch.DrawString(_menuFont, customizeControlsString,
                    new Vector2(xPos, yPos3),
                    ccColor);
                _spriteBatch.DrawString(_menuFont, viewCreditsString,
                    new Vector2(xPos, yPos4),
                    vcColor);

                _spriteBatch.End();
            }
        }
        // generate the necessary polygon vertices for the terrain
        private void GenerateTerrainVertices()
        {
            // create a list of all vertices in the terrain
            var terrainVertexList = new List<VertexPositionColor>();
            foreach (var (x, y) in _landerGameController.TerrainList)
            {
                // get pixel coordinates
                var (scaledX, scaledY) = GetAbsolutePixelCoordinates((x, y));
                var (_, scaled0) = GetAbsolutePixelCoordinates((x, 0));

                // add two new vertices: one at zero Y value, one at correct Y value
                terrainVertexList.Add(new VertexPositionColor
                {
                    Position = new Vector3(scaledX, scaled0, 0),
                    Color = Color.Gray
                });
                terrainVertexList.Add(new VertexPositionColor
                {
                    Position = new Vector3(scaledX, scaledY, 0),
                    Color = Color.Gray
                });
            }

            // store this list of vertices as an array (to be used by Draw())
            _terrainVertexPositionColors = terrainVertexList.ToArray();

            // create an array of ints in ascending order (to be used by Draw())
            _terrainIndexArray = new int[_terrainVertexPositionColors.Length];
            for (var i = 0; i < _terrainIndexArray.Length; i++)
                _terrainIndexArray[i] = i;
        }

        // game board will have relative dimensions in a square
        // this function gets the absolute dimensions
        private (int x, int y) GetAbsolutePixelCoordinates((float x, float y) relativeCoordinates)
        {
            // keep relative coordinates good
            if (relativeCoordinates.x < 0 || relativeCoordinates.x > BoardSize ||
                relativeCoordinates.y < 0 || relativeCoordinates.y > BoardSize)
            {
                // uncomment this line if we want to force spaceship to stay in safe area
                // throw new Exception("Relative coordinates must be between 0 and " + BoardSize + ".");
            }

            // get absolute pixel dimensions
            var canvasBounds = GraphicsDevice.Viewport.Bounds;
            var canvasWidthPixels = canvasBounds.Width;
            var canvasHeightPixels = canvasBounds.Height;

            // get size of playable area (will be constrained by height)
            var sizeOfGameAreaPixels = canvasHeightPixels;

            // width will be square centered in screen, same dimensions as height
            var horizontalMarginPixels = (canvasWidthPixels - sizeOfGameAreaPixels) / 2;

            // properly rescale the coordinates to get the correct pixels
            var rescaledX = RescaleUnitsToPixels(relativeCoordinates.x) + horizontalMarginPixels;
            var rescaledY = RescaleUnitsToPixels(BoardSize - relativeCoordinates.y);

            // return rescaled coordinates
            return (rescaledX, rescaledY);
        }

        // given a unit count, rescale it to pixels
        private int RescaleUnitsToPixels(float units)
        {
            // get absolute pixel dimensions
            var sizeOfGameAreaPixels = GraphicsDevice.Viewport.Bounds.Height;

            // rescale by ratio of game area in pixels to board size
            var rescaledUnits = (int) (sizeOfGameAreaPixels / BoardSize * units);
            return rescaledUnits;
        }
    }
}
