using System;
using System.Collections.Generic;
using System.Data;
using Midterm.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Midterm
{
    public class ParticleController
    {
        // reference to LanderGame's SpriteBatch
        private readonly SpriteBatch _spriteBatch;

        // List of emitters (all emitters will be updated every game update)
        private readonly List<TimedAngleEmitter> _emittersList = new List<TimedAngleEmitter>();

        // constructor sets the reference
        public ParticleController(
            SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;

        }

        public void AddAngleEmitter(float boardX, float boardY, float angle, Texture2D texture, GameTime gameTime)
        {
            var (sourceX, sourceY) = MidtermGame.GetAbsolutePixelCoordinates((boardX, boardY));

            var emitter = new TimedAngleEmitter(
                TimeSpan.FromMilliseconds(5),
                sourceX,
                sourceY,
                3,
                0.3f,
                TimeSpan.FromMilliseconds(250),
                texture,
                TimeSpan.FromMilliseconds(50),
                false
            );

            emitter.Angle = angle;
            emitter.Width = MathHelper.Pi;
            emitter.FireParticles();

            _emittersList.Add(emitter);
        }

        public void Update(GameTime gameTime)
        {
            var emittersToRemove = new List<TimedAngleEmitter>();

            // update all emitters, mark the ones we don't want for removal
            foreach (var emitter in _emittersList)
            {
                emitter.Update(gameTime);

                // remove all emitters that haven't fired for at least 3 seconds
                if (emitter.FireTime < TimeSpan.FromSeconds(-3))
                {
                    emittersToRemove.Add(emitter);
                }
            }

            // remove all emitters we don't need anymore
            foreach (var emitter in emittersToRemove)
            {
                _emittersList.Remove(emitter);
            }
        }

        // Draws all particle effects
        public void Draw()
        {
            foreach (var emitter in _emittersList)
            {
                emitter.Draw(_spriteBatch);
            }
        }

        // Clears all particle effects
        public void ClearAllParticles()
        {
            foreach (var emitter in _emittersList)
            {
                emitter.ClearAllParticles();
            }
        }
    }
}