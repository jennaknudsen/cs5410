using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Midterm.Particles
{
    public class AngleEmitter : ParticleEmitter
    {
        // both represented by radians
        // particle range: centered at angle, size is width
        public float Angle;
        public float Width;

        // whether we want the distribution to be uniform or normalized
        private readonly bool _normalized;

        // whether we are currently firing or not
        private bool _fireNow = false;

        // sets the attributes of the AngleEmitter
        public AngleEmitter(TimeSpan rate, int sourceX, int sourceY,
            int size, float speed, TimeSpan lifetime, Texture2D particleTexture,
            bool normalized) :
            base(rate, sourceX, sourceY, size, speed, lifetime, particleTexture)
        {
            _normalized = normalized;
        }

        // simple function sets the "Fire Now" flag to true
        public void FireParticles(bool fireNow)
        {
            _fireNow = fireNow;
        }

        // adds the particles
        protected override void AddParticles(GameTime gameTime)
        {
            // only generate particles if we are firing now
            if (!_fireNow) return;

            // Generate particles at the specified rate
            Accumulated += gameTime.ElapsedGameTime;
            while (Accumulated > Rate)
            {
                Accumulated -= Rate;

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
            }
        }
    }
}