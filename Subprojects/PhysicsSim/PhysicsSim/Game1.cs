using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PhysicsSim
{
    public class Game1 : Game
    {
        // assets for this demo
        private Texture2D _texLander;
        private SpriteFont _spriteFont;

        // lander XY position
        private (int x, int y) _landerPosition;
        private readonly (int x, int y) _startPosition = (400, 200);

        // moon gravity: https://en.wikipedia.org/wiki/Moon
        private const float MoonGravity = 1.62f;    // m/(s^2)

        // lander mass: https://en.wikipedia.org/wiki/Apollo_Lunar_Module
        private const int LanderMass = 4280;        // kg

        // position: degrees (on Cartesian coordinate system)
        private float _orientation;

        // ship forces and velocities
        private (float x, float y) _acceleration;
        private (float x, float y) _velocity;
        private (float x, float y) _force;

        // board size (1000)
        private const int BoardSize = 1000;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _acceleration = (0, 0);
            _velocity = (0, 0);
            _force = (0, 0);
            _orientation = 90;
            _landerPosition = _startPosition;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _texLander = this.Content.Load<Texture2D>("Images/Lander-2");
            _spriteFont = this.Content.Load<SpriteFont>("GameFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            // CALCULATE NEW SHIP POSITION
            /*
             * steps:
             * 1) calculate all forces
             * 2)
             * 2) use kinematic formulas, as well as change in gameTime, to get change in position
             */




            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            /*
             * using the function SpriteBatch.Draw with following arguments:
             *
             * void SpriteBatch.Draw(Texture2D texture,
             *                       Rectangle destinationRectangle,
             *                       Rectangle? sourceRectangle,
             *                       Color color,
             *                       float rotation,
             *                       Vector2 origin,
             *                       SpriteEffects effects,
             *                       float layerDepth) (+ 6 overloads)
             */

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // game board will have relative dimensions in a square
        // this function gets the absolute dimensions
        private (int x, int y) GetAbsolutePixelCoordinates((int x, int y) relativeCoordinates)
        {
            // keep relative coordinates good
            if (relativeCoordinates.x < 0 || relativeCoordinates.x > BoardSize ||
                relativeCoordinates.y < 0 || relativeCoordinates.y > BoardSize)
            {
                throw new Exception("Relative coordinates must be between 0 and " + BoardSize + ".");
            }

            // get absolute pixel dimensions
            var canvasBounds = GraphicsDevice.Viewport.Bounds;
            var canvasWidthPixels = canvasBounds.Width;
            var canvasHeightPixels = canvasBounds.Height;

            // get size of playable area
            var sizeOfGameAreaPixels = canvasHeightPixels;
            var horizontalMarginPixels = (canvasWidthPixels - sizeOfGameAreaPixels) / 2;

            // height will be from bottom to top
            // width will be square centered in screen, same dimensions as height

            // multiply the coordinate (units) by ratio of pixels to units to get pixels
            var (x, y) = relativeCoordinates;
            var rescaledX = (sizeOfGameAreaPixels / BoardSize) * x + horizontalMarginPixels;
            var rescaledY = (sizeOfGameAreaPixels / BoardSize) * y;

            return (rescaledX, rescaledY);
        }
    }
}
