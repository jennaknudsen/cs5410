using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Particles.Particles
{
    public class ParticleEmitter
    {

        protected Dictionary<int, Particle> m_particles = new Dictionary<int, Particle>();
        protected Texture2D m_texSmoke;
        protected Texture2D m_texFire;
        protected ParticleRandom m_random = new ParticleRandom();

        protected TimeSpan m_rate;
        protected int m_sourceX;
        protected int m_sourceY;
        protected int m_sarticleSize;
        protected int m_speed;
        protected TimeSpan m_lifetime;
        protected TimeSpan m_switchover;

        public Vector2 Gravity { get; set; }

        public ParticleEmitter(ContentManager content, TimeSpan rate, int sourceX, int sourceY, int size, int speed, TimeSpan lifetime, TimeSpan switchover)
        {
            m_rate = rate;
            m_sourceX = sourceX;
            m_sourceY = sourceY;
            m_sarticleSize = size;
            m_speed = speed;
            m_lifetime = lifetime;
            m_switchover = switchover;

            m_texSmoke = content.Load<Texture2D>("Images/smoke");
            m_texFire = content.Load<Texture2D>("Images/fire");

            this.Gravity = new Vector2(0, 0);
        }

        protected TimeSpan m_accumulated = TimeSpan.Zero;

        /// <summary>
        /// Generates new particles, updates the state of existing ones and retires expired particles.
        /// </summary>
        public virtual void update(GameTime gameTime)
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
                    m_random.nextCircleVector(),
                    (float)m_random.nextGaussian(m_speed, 1),
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

        /// <summary>
        /// Renders the active particles
        /// </summary>
        public void draw(SpriteBatch spriteBatch)
        {
            Rectangle r = new Rectangle(0, 0, m_sarticleSize, m_sarticleSize);
            foreach (Particle p in m_particles.Values)
            {
                Texture2D texDraw;
                if (p.lifetime < m_switchover)
                {
                    texDraw = m_texSmoke;
                }
                else
                {
                    texDraw = m_texFire;
                }

                r.X = (int)p.position.X;
                r.Y = (int)p.position.Y;
                spriteBatch.Draw(
                    texDraw,
                    r,
                    null,
                    Color.White,
                    p.rotation,
                    new Vector2(texDraw.Width / 2, texDraw.Height / 2),
                    SpriteEffects.None,
                    0);
            }
        }
    }
}
