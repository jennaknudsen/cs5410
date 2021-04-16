using System;

namespace Midterm
{
    public class Bomb
    {
        // public TimeSpan FuseTime;
        public int FuseTime;
        public bool Exploded;
        public bool IsEnabled;

        public Bomb(int fuseTime, bool isEnabled)
        {
            Exploded = false;
            FuseTime = fuseTime;
            IsEnabled = isEnabled;
        }
    }
}