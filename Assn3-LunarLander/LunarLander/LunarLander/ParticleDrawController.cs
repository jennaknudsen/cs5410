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
                TimeSpan.FromMilliseconds(3),
                0,
                0,
                LanderGame.RescaleUnitsToPixels(Lander.Size / 6),
                5,
                TimeSpan.FromMilliseconds(150),
                TimeSpan.FromMilliseconds(70),
                true
            );

            // Console.WriteLine(LanderGame.RescaleUnitsToPixels(Lander.Size / 10));
            // Console.WriteLine(LanderGame.RescaleUnitsToPixels(Lander.Size / 300));

            _thrustEmitter.Width = MathHelper.Pi / 3f;
        }

        // Generates and draws particles
        public void ShipThrust(GameTime gameTime, Lander lander, bool thrustOn)
        {
            _thrustEmitter.EmitParticles = thrustOn;

            // invert the angle
            _thrustEmitter.Angle = MathHelper.Pi + LanderGameController.GetCartesianOrientation(lander.Orientation);

            var (px, py) = LanderGame.GetAbsolutePixelCoordinates((lander.Position.x, lander.Position.y));
            _thrustEmitter.SourceX = px;
            _thrustEmitter.SourceY = py;

            // if (thrustOn)
            //     Console.WriteLine("sourceX=" + px + ", sourceY=" + py + ", orientation=" + lander.Orientation);

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

        public void ClearAllParticles()
        {
            _thrustEmitter.ClearAllParticles();
        }
    }
}