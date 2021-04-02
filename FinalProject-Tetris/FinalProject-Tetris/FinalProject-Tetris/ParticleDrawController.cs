using System;
using System.Collections.Generic;
using System.Data;
using FinalProject_Tetris.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace FinalProject_Tetris
{
    public class ParticleDrawController
    {
        // reference to LanderGame's SpriteBatch
        private readonly SpriteBatch _spriteBatch;

        // List of emitters (all emitters will be updated every game update)
        private List<TimedAngleEmitter> _emittersList = new List<TimedAngleEmitter>();

        // Textures: Piece texture, Clear texture
        private Texture2D _texPiece;
        private Texture2D _texClearLine;

        // constructor sets the reference
        public ParticleDrawController(SpriteBatch spriteBatch, Texture2D texPiece, Texture2D texClearLine)
        {
            _spriteBatch = spriteBatch;
            _texPiece = texPiece;
            _texClearLine = texClearLine;
        }

        public void AddPieceEmitter(float boardX, float boardY, float angle, GameTime gameTime)
        {
            var (sourceX, sourceY) = TetrisGame.GetAbsolutePixelCoordinates((boardX, boardY));

            var emitter = new TimedAngleEmitter(
                TimeSpan.FromMilliseconds(10),
                sourceX,
                sourceY,
                5,
                2,
                TimeSpan.FromMilliseconds(1000),
                _texPiece,
                TimeSpan.FromMilliseconds(200),
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
            _spriteBatch.Begin();
            foreach (var emitter in _emittersList)
            {
                emitter.Draw(_spriteBatch);
            }
            _spriteBatch.End();
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