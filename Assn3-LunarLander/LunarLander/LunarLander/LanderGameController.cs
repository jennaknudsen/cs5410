using System;
using Microsoft.Xna.Framework;

namespace LunarLander
{
    public class LanderGameController
    {
        public Lander Lander;
        private readonly InputHandler _inputHandler;

        // moon gravity: https://en.wikipedia.org/wiki/Moon
        private const float MoonGravity = 1.62f;    // m/(s^2)

        // lander start position
        private readonly (float x, float y) _startPosition = (10, 130);

        public LanderGameController()
        {
            Lander = new Lander(_startPosition);
            _inputHandler = new InputHandler();
        }

        public void Update(GameTime gameTime)
        {
            // first, handle input
            _inputHandler.HandleInput();

            // 10,000 ticks in one millisecond => 10,000,000 ticks in one second
            var elapsedSeconds = gameTime.ElapsedGameTime.Ticks / 10_000_000f;

            // turning rate: pi rads / sec
            const float turningRate = MathHelper.Pi;
            var newOrientation = Lander.Orientation;

            // turn left / right if the buttons are pressed
            var turnLeft = _inputHandler.TurnShipLeftButton.Pressed;
            var turnRight = _inputHandler.TurnShipRightButton.Pressed;

            if (turnLeft && !turnRight)
                newOrientation -= (turningRate * elapsedSeconds);
            else if (turnRight && !turnLeft)
                newOrientation += (turningRate * elapsedSeconds);

            // we need to use kinematic formulas to calculate position using forces
            // first, calculate forces
            // F = ma
            const float baseForceX = 0f;
            const float baseForceY = Lander.Mass * (-1 * MoonGravity);       // gravity force is negative

            // thrust: 5 m/s^2
            // F = ma ==> F = 4280 * 5 = 21400 N
            const float thrustAcceleration = 5f;
            const float thrustForce = Lander.Mass * thrustAcceleration;

            // additional x / y forces from the thruster
            var modForceX = 0f;
            var modForceY = 0f;

            var thrusterOn = _inputHandler.ThrustUpButton.Pressed;
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
            var accelerationX = finalForceX / Lander.Mass;
            var accelerationY = finalForceY / Lander.Mass;

            // next, calculate velocity
            // vf = vo + at
            var velocityX = Lander.Velocity.x + accelerationX * elapsedSeconds;
            var velocityY = Lander.Velocity.y + accelerationY * elapsedSeconds;

            // finally, calculate new position
            // deltaPos = vt + (1/2)at^2
            var deltaX = Lander.Velocity.x * elapsedSeconds
                         + 0.5f * accelerationX * (float) Math.Pow(elapsedSeconds, 2);
            var deltaY = Lander.Velocity.y * elapsedSeconds
                         + 0.5f * accelerationY * (float) Math.Pow(elapsedSeconds, 2);

            // translate the lander
            (float x, float y) newPosition = (Lander.Position.x + deltaX, Lander.Position.y + deltaY);

            // set new force, acceleration, velocity
            Lander.Velocity = (velocityX, velocityY);
            Lander.Position = newPosition;
            Lander.Orientation = newOrientation;
        }
    }
}