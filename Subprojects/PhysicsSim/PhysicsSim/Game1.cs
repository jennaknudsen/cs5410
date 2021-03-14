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
        private readonly (float x, float y) _startPosition = (10, 130);

        // moon gravity: https://en.wikipedia.org/wiki/Moon
        private const float MoonGravity = 1.62f;    // m/(s^2)

        // lander mass: https://en.wikipedia.org/wiki/Apollo_Lunar_Module
        private const int LanderMass = 4280;        // kg

        // position: radians (0: north, pi / 2: 3:00 position on clock, etc)
        private float _orientation;

        // need to track lander velocity and position
        private (float x, float y) _velocity;
        private (float x, float y) _position;

        // sizes in units (board size: 150x150 units (meters), lander: 10x10 units (meters))
        private const float BoardSize = 150f;
        private const float LanderSize = 15f;

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
            _velocity = (0, 0);
            _orientation = MathHelper.PiOver2;
            _position = _startPosition;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _texLander = this.Content.Load<Texture2D>("Images/Lander-2");
            _spriteFont = this.Content.Load<SpriteFont>("GameFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // 10,000 ticks in one millisecond => 10,000,000 ticks in one second
            var elapsedSeconds = gameTime.ElapsedGameTime.Ticks / 10_000_000f;

            // process input
            var thrusterOn = false;
            var turnLeft = false;
            var turnRight = false;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                thrusterOn = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                turnLeft = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                turnRight = true;

            // turning rate: pi rads / sec
            const float turningRate = MathHelper.Pi;
            var newOrientation = _orientation;
            if (turnLeft && !turnRight)
                newOrientation -= (turningRate * elapsedSeconds);
            else if (turnRight && !turnLeft)
                newOrientation += (turningRate * elapsedSeconds);

            // we need to use kinematic formulas to calculate position using forces
            // first, calculate forces
            // F = ma
            const float baseForceX = 0f;
            const float baseForceY = LanderMass * (-1 * MoonGravity);       // gravity force is negative

            // thrust: 5 m/s^2
            // F = ma ==> F = 4280 * 5 = 21400 N
            const float thrustAcceleration = 5f;
            const float thrustForce = LanderMass * thrustAcceleration;

            // additional x / y forces from the thruster
            var modForceX = 0f;
            var modForceY = 0f;

            if (thrusterOn)
            {
                // because of the way MonoGame uses radians, we need to do some conversion here
                // Cartesian radians: 0, pi/2, pi, 3pi/2 (x, y, -x, -y directions)
                // MonoGame radians: pi/2, 0, 3pi/2, pi  (x, y, -x, -y directions)
                // convert MG radians to standard:
                // standard_radians = -(MonoGame radians) + pi/2
                var cartesianOrientation = -1 * newOrientation + MathHelper.PiOver2;

                // Force equations: multiply total force by sin / cos theta to get x / y component
                modForceX = thrustForce * (float) Math.Cos(cartesianOrientation);
                modForceY = thrustForce * (float) Math.Sin(cartesianOrientation);
            }

            // add forces together to get final x/y forces
            var finalForceX = baseForceX + modForceX;
            var finalForceY = baseForceY + modForceY;

            // next, calculate acceleration from the force
            // F = ma ==> a = F/m
            var accelerationX = finalForceX / LanderMass;
            var accelerationY = finalForceY / LanderMass;

            // next, calculate velocity
            // vf = vo + at
            var velocityX = _velocity.x + accelerationX * elapsedSeconds;
            var velocityY = _velocity.y + accelerationY * elapsedSeconds;

            // finally, calculate new position
            // deltaPos = vt + (1/2)at^2
            var deltaX = _velocity.x * elapsedSeconds
                         + 0.5f * accelerationX * (float) Math.Pow(elapsedSeconds, 2);
            var deltaY = _velocity.y * elapsedSeconds
                         + 0.5f * accelerationY * (float) Math.Pow(elapsedSeconds, 2);

            // translate the lander
            (float x, float y) newPosition = (_position.x + deltaX, _position.y + deltaY);

            // set new force, acceleration, velocity
            _velocity = (velocityX, velocityY);
            _position = newPosition;
            _orientation = newOrientation;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // for debugging purposes, draw the background square in center of screen
            // get pixel coordinates from board coordinates
            var (backX, backY) = GetAbsolutePixelCoordinates((0, BoardSize));
            var rectSizePixels = RescaleUnitsToPixels(BoardSize);
            // create the MG Rectangle
            var backgroundRect = new Rectangle(backX, backY, rectSizePixels, rectSizePixels);
            // make a generic Gray texture
            var grayTexture = new Texture2D(_graphics.GraphicsDevice, 10, 10);
            var texData = new Color[10 * 10];
            for (var i = 0; i < texData.Length; i++)
                texData[i] = Color.Gray;
            grayTexture.SetData(texData);
            _spriteBatch.Draw(grayTexture, backgroundRect, Color.Gray);

            // Now, draw the lander

            // set lander position rectangle
            // sets the top left of lander (we are re-adjusting the texture origin in the Draw() function)
            var (landerX, landerY) = GetAbsolutePixelCoordinates((_position.x,
                _position.y));
            var landerSizePixels = RescaleUnitsToPixels(LanderSize);
            _positionRectangle = new Rectangle(landerX, landerY, landerSizePixels, landerSizePixels);

            // run draw function
            _spriteBatch.Draw(_texLander,
                              _positionRectangle,
                              null,
                              Color.Aqua,
                              _orientation,
                              // center origin in the texture
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
