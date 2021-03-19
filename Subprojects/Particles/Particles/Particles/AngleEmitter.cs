using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Particles.Particles
{
    public class AngleEmitter : ParticleEmitter
    {
        // both represented by radians
        // particle range: centered at angle, size is width
        public float Angle;
        public float Width;

        // whether we want to be currently emitting particles or not
        public bool EmitParticles;

        // whether we want the distribution to be uniform or normalized
        private readonly bool _normalized;

        // sets the attributes of the AngleEmitter
        public AngleEmitter(ContentManager content, TimeSpan rate, int sourceX, int sourceY,
            int size, int speed, TimeSpan lifetime, TimeSpan switchover, bool normalized) :
            base(content, rate, sourceX, sourceY, size, speed, lifetime, switchover)
        {
            EmitParticles = false;
            _normalized = normalized;
        }

        // adds the particles
        protected override void AddParticles(GameTime gameTime)
        {
            // don't generate particles unless we specify the emitter to
            if (!EmitParticles) return;

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