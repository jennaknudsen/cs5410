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
        private SpriteBatch _spriteBatch;

        // AngleEmitter to control thrust
        private AngleEmitter _thrustEmitter;

        // ExplosionEmitter to draw explosion
        private ExplosionEmitter _explosionEmitter;

        // constructor sets the reference
        public ParticleDrawController(SpriteBatch spriteBatch, ContentManager content, float boardSize)
        {
            _spriteBatch = spriteBatch;

            _thrustEmitter = new AngleEmitter(
                content,
                TimeSpan.FromMilliseconds(2),
                0,
                0,
                (int) (boardSize * 0.01f),
                (int) (boardSize * 0.01f),
                TimeSpan.FromSeconds(0.5),
                TimeSpan.FromSeconds(0.1),
                true
            );

            _thrustEmitter.Width = MathHelper.Pi / 3f;
        }

        // Generates and draws particles
        public void ShipThrust(GameTime gameTime, Lander lander, bool thrustOn)
        {
            _thrustEmitter.EmitParticles = thrustOn;

            _thrustEmitter.Angle = lander.Orientation;
            // _thrustEmitter.SourceX = lander.Position.x;
            // _thrustEmitter.SourceY = lander.Position.y;
            _thrustEmitter.SourceX = 400;
            _thrustEmitter.SourceY = 200;

            _thrustEmitter.Update(gameTime);
        }

        public void ShipCrash(Lander lander)
        {

        }

        public void DrawThrust()
        {
            _spriteBatch.Begin();
            _thrustEmitter.Draw(_spriteBatch);
            _spriteBatch.End();
        }
    }
}