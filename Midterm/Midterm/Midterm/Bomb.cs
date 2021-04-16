using System;
using Microsoft.Xna.Framework;

namespace Midterm
{
    public class Bomb
    {
        // public TimeSpan FuseTime;
        public int FuseTime;
        public bool Exploded;
        public bool Defused;
        public bool IsEnabled;
        private TimeSpan _explodeTimeRemaining;
        public bool IsDoneExploding;

        public Bomb(int fuseTime, bool isEnabled)
        {
            Exploded = false;
            FuseTime = fuseTime;
            IsEnabled = isEnabled;
            Defused = false;
            _explodeTimeRemaining = TimeSpan.FromSeconds(1.0);
            IsDoneExploding = false;
        }

        public void TickDownBombExplosion(GameTime gameTime)
        {
            if (_explodeTimeRemaining.TotalSeconds > 0)
            {
                _explodeTimeRemaining -= gameTime.ElapsedGameTime;
                Console.WriteLine("Exploding");
            }

            if (_explodeTimeRemaining.TotalSeconds <= 0)
            {
                _explodeTimeRemaining = TimeSpan.Zero;
                IsDoneExploding = true;
                Console.WriteLine("Here");
            }
        }
    }
}