namespace LunarLander
{
    public struct Lander
    {
        // lander mass: https://en.wikipedia.org/wiki/Apollo_Lunar_Module
        public const int Mass = 4280;       // kg

        // position: radians (0: north, pi / 2: 3:00 position on clock, etc)
        public float Orientation;

        // need to track lander velocity and position
        public (float x, float y) Velocity;
        public (float x, float y) Position;

        // Lander size: 15 x 15 meters
        public const float Size = 15f;
    }
}