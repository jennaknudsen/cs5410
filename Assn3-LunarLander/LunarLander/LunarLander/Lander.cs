namespace LunarLander
{
    public struct Lander
    {
        // lander mass: https://en.wikipedia.org/wiki/Apollo_Lunar_Module
        public const int LanderMass = 4280;       // kg

        // position: radians (0: north, pi / 2: 3:00 position on clock, etc)
        public float _orientation;

        // need to track lander velocity and position
        public (float x, float y) _velocity;
        public (float x, float y) _position;

        // Lander size: 15 x 15 meters
        public const float LanderSize = 15f;
    }
}