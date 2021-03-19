using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Particles.Particles
{
    public class AngleEmitter : ParticleEmitter
    {
        // both represented by radians
        // particle range: centered at angle, size is width
        public float Angle;
        public float Width;
        public bool EmitParticles;
        private bool _normalized;

        public AngleEmitter(ContentManager content, TimeSpan rate, int sourceX, int sourceY,
            int size, int speed, TimeSpan lifetime, TimeSpan switchover, bool normalized) :
            base(content, rate, sourceX, sourceY, size, speed, lifetime, switchover)
        {
            EmitParticles = false;
            _normalized = normalized;
        }

        protected override void AddParticles(GameTime gameTime)
        {
            // Generate particles at the specified rate
            if (!EmitParticles) return;

            m_accumulated += gameTime.ElapsedGameTime;
            while (m_accumulated > m_rate)
            {
                m_accumulated -= m_rate;

                Vector2 direction;
                if (_normalized)
                    direction = m_random.nextAngleVectorNormalized(Angle, Width);
                else
                    direction = m_random.nextAngleVector(Angle, Width);

                Particle p = new Particle(
                    m_random.Next(),
                    new Vector2(m_sourceX, m_sourceY),
                    direction,
                    (float) Math.Abs(m_random.nextGaussian(m_speed, 1)),
                    m_lifetime);

                if (!m_particles.ContainsKey(p.name))
                {
                    m_particles.Add(p.name, p);
                }
            }

        }
    }
}