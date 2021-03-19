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

        public AngleEmitter(ContentManager content, TimeSpan rate, int sourceX, int sourceY,
            int size, int speed, TimeSpan lifetime, TimeSpan switchover) :
            base(content, rate, sourceX, sourceY, size, speed, lifetime, switchover)
        {
            // do nothing besides what's done in base
        }

        public override void update(GameTime gameTime)
        {
            //
            // Generate particles at the specified rate
            m_accumulated += gameTime.ElapsedGameTime;
            while (m_accumulated > m_rate)
            {
                m_accumulated -= m_rate;

                Particle p = new Particle(
                    m_random.Next(),
                    new Vector2(m_sourceX, m_sourceY),
                    m_random.nextAngleVector(Angle, Width),
                    (float)Math.Abs(m_random.nextGaussian(m_speed, 1)),
                    m_lifetime);

                if (!m_particles.ContainsKey(p.name))
                {
                    m_particles.Add(p.name, p);
                }
            }


            //
            // For any existing particles, update them, if we find ones that have expired, add them
            // to the remove list.
            List<int> removeMe = new List<int>();
            foreach (Particle p in m_particles.Values)
            {
                p.lifetime -= gameTime.ElapsedGameTime;
                if (p.lifetime < TimeSpan.Zero)
                {
                    //
                    // Add to the remove list
                    removeMe.Add(p.name);
                }
                //
                // Update its position
                p.position += (p.direction * p.speed);
                //
                // Have it rotate proportional to its speed
                p.rotation += p.speed / 50.0f;
                //
                // Apply some gravity
                p.direction += this.Gravity;
            }

            //
            // Remove any expired particles
            foreach (int Key in removeMe)
            {
                m_particles.Remove(Key);
            }
        }
    }
}