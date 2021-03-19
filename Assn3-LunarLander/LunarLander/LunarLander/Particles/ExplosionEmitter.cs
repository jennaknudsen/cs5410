using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace LunarLander.Particles
{
    public class ExplosionEmitter : ParticleEmitter
    {
        // flag as to whether we want to fire now
        private bool _fireNow;

        // TimeSpan holds how long an explosion will last
        private readonly TimeSpan _explosionTime;

        // TimeSpan holds the remaining explosion time
        private TimeSpan _remainingExplosionTime;

        // constructor sets the attributes and readies an explosion
        public ExplosionEmitter(ContentManager content, TimeSpan rate, int sourceX, int sourceY,
            int size, int speed, TimeSpan lifetime, TimeSpan switchover, TimeSpan explosionTime) :
            base(content, rate, sourceX, sourceY, size, speed, lifetime, switchover)
        {
            _explosionTime = explosionTime;
            ReadyExplosion();
        }

        // used to ready an explosion
        public void ReadyExplosion()
        {
            _fireNow = false;
            _remainingExplosionTime = _explosionTime;
        }

        // simple function sets the "Fire Now" flag to true
        public void FireExplosion()
        {
            _fireNow = true;
        }

        // adds the particles
        protected override void AddParticles(GameTime gameTime)
        {
            // if we are firing, we need to subtract from the remaining time
            if (_fireNow)
            {
                _remainingExplosionTime -= gameTime.ElapsedGameTime;
            }

            // Generate particles at the specified rate
            Accumulated += gameTime.ElapsedGameTime;
            while (Accumulated > Rate)
            {
                Accumulated -= Rate;

                // only fire if we have ExplosionTime left
                if (!_fireNow || _remainingExplosionTime <= TimeSpan.Zero) continue;

                var p = new Particle(
                    ParticleRandom.Next(),
                    new Vector2(SourceX, SourceY),
                    ParticleRandom.NextCircleVector(),
                    (float) Math.Abs(ParticleRandom.NextGaussian(Speed, 1)),
                    Lifetime);

                if (!Particles.ContainsKey(p.Name))
                {
                    Particles.Add(p.Name, p);
                }
            }
        }
    }
}