using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FinalProject_Tetris.Particles
{
    public class ParticleEmitter
    {
        // Dictionary to hold particles
        protected readonly Dictionary<int, Particle> Particles = new Dictionary<int, Particle>();

        // random number generator, specific to Particles
        protected readonly ParticleRandom ParticleRandom = new ParticleRandom();

        // attributes of this particle emitter
        protected TimeSpan Rate;
        public float SourceX;
        public float SourceY;
        protected int ParticleSize;
        protected int Speed;
        protected TimeSpan Lifetime;
        // protected TimeSpan Switchover;
        protected Texture2D _particleTexture;

        // can set gravity of particles
        public Vector2 Gravity { get; set; }

        // stores the accumulated time
        protected TimeSpan Accumulated = TimeSpan.Zero;

        // Constructor sets the attributes
        public ParticleEmitter(TimeSpan rate, int sourceX, int sourceY, int size, int speed,
            TimeSpan lifetime, Texture2D particleTexture)
        {
            Rate = rate;
            SourceX = sourceX;
            SourceY = sourceY;
            ParticleSize = size;
            Speed = speed;
            Lifetime = lifetime;
            _particleTexture = particleTexture;

            this.Gravity = new Vector2(0, 0);
        }

        /// <summary>
        /// Generates new particles, updates the state of existing ones and retires expired particles.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // adds all particles that need to be generated
            AddParticles(gameTime);

            // removes all particles that we need to remove
            RemoveParticles(gameTime);
        }

        public void ClearAllParticles()
        {
            Particles.Clear();
        }

        protected virtual void AddParticles(GameTime gameTime)
        {
            // Generate particles at the specified rate
            Accumulated += gameTime.ElapsedGameTime;
            while (Accumulated > Rate)
            {
                Accumulated -= Rate;

                var p = new Particle(
                    ParticleRandom.Next(),
                    new Vector2(SourceX, SourceY),
                    ParticleRandom.NextCircleVector(),
                    (float)ParticleRandom.NextGaussian(Speed, 1),
                    Lifetime);

                if (!Particles.ContainsKey(p.Name))
                {
                    Particles.Add(p.Name, p);
                }
            }
        }

        protected virtual void RemoveParticles(GameTime gameTime)
        {
            // For any existing particles, update them, if we find ones that have expired, add them
            // to the remove list.
            var removeMe = new List<int>();
            foreach (var p in Particles.Values)
            {
                p.Lifetime -= gameTime.ElapsedGameTime;
                if (p.Lifetime < TimeSpan.Zero)
                {
                    // Add to the remove list
                    removeMe.Add(p.Name);
                }

                // Update its position
                p.Position += (p.Direction * p.Speed);

                // Have it rotate proportional to its speed
                p.Rotation += p.Speed / 50.0f;

                // Apply some gravity
                p.Direction += this.Gravity;
            }

            //
            // Remove any expired particles
            foreach (var key in removeMe)
            {
                Particles.Remove(key);
            }
        }

        /// <summary>
        /// Renders the active particles
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            var r = new Rectangle(0, 0, ParticleSize, ParticleSize);
            foreach (var p in Particles.Values)
            {
                r.X = (int)p.Position.X;
                r.Y = (int)p.Position.Y;

                spriteBatch.Draw(
                    _particleTexture,
                    r,
                    null,
                    Color.White,
                    p.Rotation,
                    new Vector2(_particleTexture.Width / 2, _particleTexture.Height / 2),
                    SpriteEffects.None,
                    0);
            }
        }
    }
}
