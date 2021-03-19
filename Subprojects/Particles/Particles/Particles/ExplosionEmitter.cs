using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Particles.Particles
{
    public class ExplosionEmitter : ParticleEmitter
    {
        private bool _fireNow;
        public TimeSpan ExplosionTime;

        public ExplosionEmitter(ContentManager content, TimeSpan rate, int sourceX, int sourceY,
            int size, int speed, TimeSpan lifetime, TimeSpan switchover) :
            base(content, rate, sourceX, sourceY, size, speed, lifetime, switchover)
        {
            ReadyExplosion();
        }

        // used to ready an explosion
        public void ReadyExplosion()
        {
            _fireNow = false;
            ExplosionTime = TimeSpan.FromMilliseconds(300);
        }

        public void FireExplosion()
        {
            _fireNow = true;
        }

        protected override void AddParticles(GameTime gameTime)
        {
            // Generate particles at the specified rate

            if (_fireNow)
            {
                ExplosionTime -= gameTime.ElapsedGameTime;
            }

            m_accumulated += gameTime.ElapsedGameTime;
            while (m_accumulated > m_rate)
            {
                m_accumulated -= m_rate;

                // only fire if we have ExplosionTime left
                if (_fireNow && ExplosionTime > TimeSpan.Zero)
                {
                    Particle p = new Particle(
                        m_random.Next(),
                        new Vector2(m_sourceX, m_sourceY),
                        m_random.nextCircleVector(),
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
}