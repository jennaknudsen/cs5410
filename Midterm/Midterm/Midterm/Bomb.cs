using System;

namespace Midterm
{
    public class Bomb
    {
        public TimeSpan FuseTime;
        public bool Exploded;
        public bool IsEnabled;

        public Bomb(TimeSpan fuseTime, bool isEnabled)
        {
            Exploded = false;
            FuseTime = fuseTime;
            IsEnabled = isEnabled;
        }
    }
}