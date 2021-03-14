using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace LunarLander
{
    public class LanderGameController
    {
        // the lander object for this game
        public Lander Lander;

        // the input handler that handles keyboard input
        private readonly InputHandler _inputHandler;

        // moon gravity: https://en.wikipedia.org/wiki/Moon
        private const float MoonGravity = 1.62f;    // m/(s^2)

        // lander start position
        private readonly (float x, float y) _startPosition = (10, 90);

        // the board size in units (and meters)
        public const float BoardSize = 100f;

        // whether terrain has been generated or not
        public bool TerrainGenerated = false;

        // whether the LanderGame should recalculate its terrain on the next render cycle
        public bool RecalculateTerrain = false;

        // List<> containing ordered pairs, representing the terrain
        public List<(float x, float y)> TerrainList;

        // holds the safe zone information
        public List<(float x_start, float x_stop)> SafeZones;

        public LanderGameController()
        {
            Lander = new Lander(_startPosition);
            _inputHandler = new InputHandler();
            TerrainList = new List<(float x, float y)>();
            SafeZones = new List<(float x_start, float x_stop)>();

            // TODO move this somewhere else?
            GenerateTerrain(1);
        }


        // level 1: two safe zones, each 30 meters long
        // level 2: one safe zone, 20 meters long
        private void GenerateTerrain(int difficultyLevel)
        {
            const float maxPointHeight = (BoardSize * 0.8f);
            var random = new Random();

            // first, generate random point at x=0 and x=maxBoardSize
            TerrainList.Add((0, (float) (random.NextDouble() * maxPointHeight)));
            TerrainList.Add((BoardSize, (float) (random.NextDouble() * maxPointHeight)));

            // next, generate safe zones

            // safe zones depend on difficulty level
            var safeZoneLength = difficultyLevel switch
            {
                1 => Lander.Size * 2,
                2 => Lander.Size * (4f / 3),
                _ => throw new ArgumentOutOfRangeException(nameof(difficultyLevel), difficultyLevel, null)
            };

            // safe zones must be 15% from the border
            var safeStart = (int) (0.15f * BoardSize);
            var safeStop = (int) (BoardSize - 0.15f * BoardSize - safeZoneLength);

            var numSafeZonesToGen = difficultyLevel switch
            {
                1 => 2,
                2 => 1,
                _ => throw new ArgumentOutOfRangeException(nameof(difficultyLevel), difficultyLevel, null)
            };

            // generate all needed safe zones
            while (numSafeZonesToGen > 0)
            {
                // attempt to generate a new safezone within board bounds
                var thisStart = random.Next(safeStart, safeStop);
                var thisStop = thisStart + safeZoneLength;

                var isValid = true;

                // check validity
                foreach (var (xStart, xStop) in SafeZones)
                {
                    // not a valid safezone if either start or end lies within 0.1 of the
                    // boundaries of a safezone
                    // this will stop safezones from generating right next to each other
                    if (xStart - (safeZoneLength * 0.1) < thisStart &&
                        xStop + (safeZoneLength * 0.1) > thisStart ||
                        xStart - (safeZoneLength * 0.1) < thisStop &&
                        xStop + (safeZoneLength * 0.1) > thisStop)
                    {
                        isValid = false;
                        break;
                    }
                }

                // add this safe zone if it's valid
                if (isValid)
                {
                    // get a random height for the safezone
                    var height = (float) random.NextDouble() * maxPointHeight;

                    // add both safezone points to the terrain list
                    TerrainList.Add((thisStart, height));
                    TerrainList.Add((thisStop, height));

                    // add this range to safezones list
                    SafeZones.Add((thisStart, thisStop));
                    numSafeZonesToGen--;
                }
            }

            // order safe zones and terrain by x coordinates
            TerrainList.Sort((first, second) => first.x.CompareTo(second.x));
            SafeZones.Sort((first, second) => first.x_start.CompareTo(second.x_start));

            Console.WriteLine("Safezones:");
            foreach (var (xStart, xStop) in SafeZones)
            {
                Console.WriteLine("start: " + xStart + ", stop: " + xStop);
            }

            // recursively generate terrain levels
            void GenerateTerrainLevel((float x, float y) start, (float x, float y) end,
                int recursionDepth, int maxRecursionDepth)
            {

            }
            // generate terrain for each safe zone
            // TerrainList.Add((0, 40));
            // TerrainList.Add((3, 20));
            // TerrainList.Add((7, 4));
            // TerrainList.Add((10, 50));
            // TerrainList.Add((20, 100));
            // TerrainList.Add((30, 120));
            // TerrainList.Add((40, 95));
            // TerrainList.Add((50, 76));
            // TerrainList.Add((60, 52));
            // TerrainList.Add((70, 30));
            // TerrainList.Add((80, 11));
            // TerrainList.Add((90, 5));
            // TerrainList.Add((100, 18));
            // TerrainList.Add((110, 22));
            // TerrainList.Add((120, 22));
            // TerrainList.Add((130, 22));
            // TerrainList.Add((140, 43));
            // TerrainList.Add((150, 73));

            TerrainGenerated = true;
            RecalculateTerrain = true;
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
            // F = ma =6=> a = F/m
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