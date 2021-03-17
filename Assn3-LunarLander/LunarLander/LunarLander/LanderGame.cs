using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static LunarLander.GameState;

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
        private SpriteFont _spriteFont;
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
            _spriteFont = this.Content.Load<SpriteFont>("GameFont");
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
            var (backX, backY) = GetAbsolutePixelCoordinates((0, LanderGameController.BoardSize));
            var rectSizePixels = RescaleUnitsToPixels(LanderGameController.BoardSize);
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

            // set the correct colors
            var fuelColor = fuel > 0d ? Color.Green : Color.White;
            var speedColor = speed < 2f ? Color.Green : Color.White;
            var angleColor = angle > 355f || angle < 5f ? Color.Green : Color.White;

            // set the formatted strings
            var fuelString = "Fuel: " +
                               fuel.ToString("0.000") + " s";
            var speedString = "Speed: " +
                               speed.ToString("0.000") + " m/s";
            var angleString = "Angle: " +
                               angle.ToString("0.000") + " degrees";

            // draw on-screen elements
            _spriteBatch.Begin();

            var (textPosX, textPosY) = GetAbsolutePixelCoordinates((LanderGameController.BoardSize * 0.65f,
                LanderGameController.BoardSize * 0.95f));

            _spriteBatch.DrawString(_spriteFont, fuelString,
                new Vector2(textPosX, textPosY),
                fuelColor);
            _spriteBatch.DrawString(_spriteFont, speedString,
                new Vector2(textPosX, textPosY + 20),
                speedColor);
            _spriteBatch.DrawString(_spriteFont, angleString,
                new Vector2(textPosX, textPosY + 40),
                angleColor);

            _spriteBatch.End();

            // dim entire screen on pause
            if (_landerGameController.GameState == Paused)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(_texBackgroundDimmer, backgroundRect, Color.Gray);

                // code to determine which menu elements to draw on pause

                _spriteBatch.End();
            }
            base.Draw(gameTime);
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
            if (relativeCoordinates.x < 0 || relativeCoordinates.x > LanderGameController.BoardSize ||
                relativeCoordinates.y < 0 || relativeCoordinates.y > LanderGameController.BoardSize)
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
            var rescaledY = RescaleUnitsToPixels(LanderGameController.BoardSize - relativeCoordinates.y);

            // return rescaled coordinates
            return (rescaledX, rescaledY);
        }

        // given a unit count, rescale it to pixels
        private int RescaleUnitsToPixels(float units)
        {
            // get absolute pixel dimensions
            var sizeOfGameAreaPixels = GraphicsDevice.Viewport.Bounds.Height;

            // rescale by ratio of game area in pixels to board size
            var rescaledUnits = (int) (sizeOfGameAreaPixels / LanderGameController.BoardSize * units);
            return rescaledUnits;
        }
    }
}
