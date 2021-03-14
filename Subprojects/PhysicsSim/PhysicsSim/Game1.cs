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
        private Rectangle _positionRectangle;
        private SpriteFont _spriteFont;

        // lander XY position
        // both represent lander center
        private (float x, float y) _landerPosition;
        private readonly (float x, float y) _startPosition = (10, 130);

        // moon gravity: https://en.wikipedia.org/wiki/Moon
        private const float MoonGravity = 1.62f;    // m/(s^2)

        // lander mass: https://en.wikipedia.org/wiki/Apollo_Lunar_Module
        private const int LanderMass = 4280;        // kg

        // position: radians (0: north, pi / 2: 3:00 position on clock, etc)
        private float _orientation;

        // ship forces and velocities
        private (float x, float y) _acceleration;
        private (float x, float y) _velocity;
        private (float x, float y) _force;

        // sizes in units (board size: 150x150 units (meters), lander: 10x10 units (meters))
        private const float BoardSize = 150f;
        private const float LanderSize = 10f;

        // MonoGame stuff
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
            //
            // _graphics.PreferredBackBufferWidth = 1400;
            // _graphics.PreferredBackBufferHeight = 700;
            // _graphics.ApplyChanges();

            _acceleration = (0, 0);
            _velocity = (0, 0);
            _force = (0, 0);
            // TODO change back
            _orientation = 0;
            // _orientation = MathHelper.PiOver2;
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

            // we need to use kinematic formulas to calculate position using forces
            (float x, float y) newLanderPosition;

            // first, calculate forces
            // F = ma
            var baseForceX = 0f;
            var baseForceY = LanderMass * (-1 * MoonGravity);   // gravity force is negative

            // TODO: get thrust forces with some math shit
            var modForceX = 0f;
            var modForceY = 0f;

            // add forces together
            var finalForceX = baseForceX + modForceX;
            var finalForceY = baseForceY + modForceY;

            // next, calculate acceleration based on force
            // a = m/F
            var accelerationX = finalForceX / LanderMass;
            var accelerationY = finalForceY / LanderMass;

            // next, calculate velocity
            // 10,000 ticks in one millisecond => 10,000,000 ticks in one second
            // use doubles here for more precision TODO change back
            // vf = vo + at
            // var elapsedSeconds = gameTime.ElapsedGameTime.Ticks / 10_000_000d;
            var elapsedSeconds = gameTime.ElapsedGameTime.Ticks / 10_000_000f;
            // var velocityX = _velocity.x + (float) (accelerationX * elapsedSeconds);
            // var velocityY = _velocity.y + (float) (accelerationY * elapsedSeconds);
            var velocityX = _velocity.x + accelerationX * elapsedSeconds;
            var velocityY = _velocity.y + accelerationY * elapsedSeconds;

            // finally, calculate new position
            // deltaPos = vt + (1/2)at^2
            var deltaX = _velocity.x * elapsedSeconds
                         + 0.5f * accelerationX * (float) Math.Pow(elapsedSeconds, 2);
            var deltaY = _velocity.y * elapsedSeconds
                         + 0.5f * accelerationY * (float) Math.Pow(elapsedSeconds, 2);

            // translate the lander
            newLanderPosition = (_landerPosition.x + deltaX, _landerPosition.y + deltaY);
            _landerPosition = newLanderPosition;

            // set new force, acceleration, velocity
            _force = (finalForceX, finalForceY);
            _velocity = (velocityX, velocityY);
            _acceleration = (accelerationX, accelerationY);

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

            // for debugging purposes, draw the background rectangle
            var (backX, backY) = GetAbsolutePixelCoordinates((0, BoardSize));
            var rectSizePixels = RescaleUnitsToPixels(BoardSize);
            var backgroundRect = new Rectangle(backX, backY, rectSizePixels, rectSizePixels);
            var grayTexture = new Texture2D(_graphics.GraphicsDevice, 10, 10);
            var texData = new Color[10 * 10];
            for (var i = 0; i < texData.Length; i++)
                texData[i] = Color.Gray;
            grayTexture.SetData(texData);
            _spriteBatch.Draw(grayTexture, backgroundRect, Color.Blue);
            // delete up to here

            // Draw the lander

            // set lander position rectangle
            // sets the top left of lander (we are re-moving the texture in the Draw() function)
            var (landerX, landerY) = GetAbsolutePixelCoordinates((_landerPosition.x,
                _landerPosition.y));
            var landerSizePixels = RescaleUnitsToPixels(LanderSize);
            _positionRectangle = new Rectangle(landerX, landerY, landerSizePixels, landerSizePixels);

            // run draw function
            _spriteBatch.Draw(_texLander,
                              _positionRectangle,
                              null,
                              Color.Aqua,
                              MathHelper.Pi / 2,
                              new Vector2(_texLander.Width / 2, _texLander.Width / 2),
                              SpriteEffects.None,
                              0);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // game board will have relative dimensions in a square
        // this function gets the absolute dimensions
        private (int x, int y) GetAbsolutePixelCoordinates((float x, float y) relativeCoordinates)
        {
            // keep relative coordinates good
            if (relativeCoordinates.x < 0 || relativeCoordinates.x > BoardSize ||
                relativeCoordinates.y < 0 || relativeCoordinates.y > BoardSize)
            {
                // throw new Exception("Relative coordinates must be between 0 and " + BoardSize + ".");
            }

            // get absolute pixel dimensions
            var canvasBounds = GraphicsDevice.Viewport.Bounds;
            var canvasWidthPixels = canvasBounds.Width;
            var canvasHeightPixels = canvasBounds.Height;

            // get size of playable area
            var sizeOfGameAreaPixels = canvasHeightPixels;

            // height will be from bottom to top
            // width will be square centered in screen, same dimensions as height
            var horizontalMarginPixels = (canvasWidthPixels - sizeOfGameAreaPixels) / 2;

            // multiply the coordinate (units) by ratio of pixels to units to get pixels
            var (x, y) = relativeCoordinates;
            var rescaledX = (int) (sizeOfGameAreaPixels / BoardSize * x + horizontalMarginPixels);
            // y coordinate should be inversed (i.e., max unit is at TOP of window, not bottom)
            var rescaledY = (int) (sizeOfGameAreaPixels / BoardSize * (BoardSize - y));

            return (rescaledX, rescaledY);
        }

        // given a unit count, rescale it to pixels
        private int RescaleUnitsToPixels(float units)
        {
            // get absolute pixel dimensions
            var canvasBounds = GraphicsDevice.Viewport.Bounds;
            var canvasWidthPixels = canvasBounds.Width;
            var canvasHeightPixels = canvasBounds.Height;

            // get size of playable area
            var sizeOfGameAreaPixels = canvasHeightPixels;

            // rescale
            var rescaledUnits = (int) (sizeOfGameAreaPixels / BoardSize * units);
            return rescaledUnits;
        }
    }
}
