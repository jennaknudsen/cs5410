using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Midterm.Particles
{
    public class TimedAngleEmitter : ParticleEmitter
    {
        // both represented by radians
        // particle range: centered at angle, size is width
        public float Angle;
        public float Width;

        // whether we want to be currently emitting particles or not
        public bool EmitParticles;

        // whether we want the distribution to be uniform or normalized
        private readonly bool _normalized;

        // TimeSpan holds how long a particle emission will last
        public TimeSpan FireTime;

        // whether we are currently firing or not
        private bool _fireNow = false;

        // sets the attributes of the AngleEmitter
        public TimedAngleEmitter(TimeSpan rate, int sourceX, int sourceY,
            int size, float speed, TimeSpan lifetime, Texture2D particleTexture,
            TimeSpan fireTime, bool normalized) :
            base(rate, sourceX, sourceY, size, speed, lifetime, particleTexture)
        {
            EmitParticles = false;
            FireTime = fireTime;
            _normalized = normalized;
        }

        // simple function sets the "Fire Now" flag to true
        public void FireParticles()
        {
            _fireNow = true;
        }

        // adds the particles
        protected override void AddParticles(GameTime gameTime)
        {
            // if we are firing, we need to subtract from the remaining time
            if (_fireNow)
            {
                FireTime -= gameTime.ElapsedGameTime;
            }

            // Generate particles at the specified rate
            Accumulated += gameTime.ElapsedGameTime;
            while (Accumulated > Rate)
            {
                Accumulated -= Rate;

                // only fire if we have ExplosionTime left
                if (!_fireNow || FireTime <= TimeSpan.Zero) continue;

                // get direction based on whether we are normalizing or not
                var direction = _normalized
                    ? ParticleRandom.NextAngleVectorNormalized(Angle, Width)
                    : ParticleRandom.NextAngleVector(Angle, Width);

                // create and add the particle
                var p = new Particle(
                    ParticleRandom.Next(),
                    new Vector2(SourceX, SourceY),
                    direction,
                    (float) Math.Abs(ParticleRandom.NextGaussian(Speed, Speed / 2)),
                    Lifetime);

                if (!Particles.ContainsKey(p.Name))
                {
                    Particles.Add(p.Name, p);
                }

                // add some gravity
                p.Direction += new Vector2(0, 0.5f);
            }
        }
    }
}