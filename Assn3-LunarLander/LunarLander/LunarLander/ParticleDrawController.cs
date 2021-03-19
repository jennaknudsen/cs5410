using System;
using System.Data;
using LunarLander.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace LunarLander
{
    public class ParticleDrawController
    {
        // reference to LanderGame's SpriteBatch
        private readonly SpriteBatch _spriteBatch;

        // AngleEmitter to control thrust
        private readonly AngleEmitter _thrustEmitter;

        // ExplosionEmitter to draw explosion
        private readonly ExplosionEmitter _explosionEmitter;

        // constructor sets the reference
        public ParticleDrawController(SpriteBatch spriteBatch, ContentManager content, float boardSize)
        {
            _spriteBatch = spriteBatch;

            _thrustEmitter = new AngleEmitter(
                content,
                TimeSpan.FromMilliseconds(3),
                0,
                0,
                LanderGame.RescaleUnitsToPixels(Lander.Size / 6),
                5,
                TimeSpan.FromMilliseconds(150),
                TimeSpan.FromMilliseconds(70),
                true
            );

            _thrustEmitter.Width = MathHelper.Pi / 3f;

            _explosionEmitter = new ExplosionEmitter(
                content,
                TimeSpan.FromMilliseconds(2),
                0,
                0,
                LanderGame.RescaleUnitsToPixels(Lander.Size / 4),
                // 50,
                0,
                TimeSpan.FromMilliseconds(700),
                TimeSpan.FromMilliseconds(220),
                TimeSpan.FromMilliseconds(400)
            );
        }

        // Generates and draws particles for thrust
        public void ShipThrust(GameTime gameTime, Lander lander, bool thrustOn)
        {
            _thrustEmitter.EmitParticles = thrustOn;

            // invert the angle
            _thrustEmitter.Angle = MathHelper.Pi + LanderGameController.GetCartesianOrientation(lander.Orientation);

            var (px, py) = LanderGame.GetAbsolutePixelCoordinates((lander.Position.x, lander.Position.y));
            _thrustEmitter.SourceX = px;
            _thrustEmitter.SourceY = py;

            _thrustEmitter.Update(gameTime);
        }

        // Generates and draws particles for ship crash
        public void ShipCrash(GameTime gameTime, Lander lander)
        {
            // internally, this wil determine whether to draw more particles or not
            _explosionEmitter.FireExplosion();

            var (px, py) = LanderGame.GetAbsolutePixelCoordinates((lander.Position.x, lander.Position.y));
            _explosionEmitter.SourceX = px;
            _explosionEmitter.SourceY = py;

            _explosionEmitter.Update(gameTime);
        }

        // Draws all particle effects
        public void Draw()
        {
            _spriteBatch.Begin();
            _thrustEmitter.Draw(_spriteBatch);
            _explosionEmitter.Draw(_spriteBatch);
            _spriteBatch.End();
        }

        // Clears all particle effects
        public void ClearAllParticles()
        {
            _thrustEmitter.ClearAllParticles();
            _explosionEmitter.ClearAllParticles();
            // when particles are cleared, we can now re-prime the explosion emitter
            _explosionEmitter.ReadyExplosion();
        }
    }
}