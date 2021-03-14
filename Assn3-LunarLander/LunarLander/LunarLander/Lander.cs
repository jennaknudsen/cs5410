using Microsoft.Xna.Framework;

namespace LunarLander
{
    public class Lander
    {
        // lander mass: https://en.wikipedia.org/wiki/Apollo_Lunar_Module
        public const int Mass = 4280;       // kg

        // position: radians (0: north, pi / 2: 3:00 position on clock, etc)
        public float Orientation;

        // need to track lander velocity and position
        public (float x, float y) Velocity;
        public (float x, float y) Position;

        // Lander size: 7x7 meters
        public const float Size = 7f;

        public Lander((float x, float y) position)
        {
            // Rotated pi/2 to the right initially
            Orientation = MathHelper.PiOver2;

            // no initial velocity
            Velocity = (0, 0);

            // initial position in game units is from constructor argument
            Position = position;
        }
    }
}