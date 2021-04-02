using System;
using System.Collections.Generic;
using System.Data;
using FinalProject_Tetris.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using static FinalProject_Tetris.Square;
using static FinalProject_Tetris.Square.PieceColor;

namespace FinalProject_Tetris
{
    public class ParticleController
    {
        // reference to LanderGame's SpriteBatch
        private readonly SpriteBatch _spriteBatch;

        // List of emitters (all emitters will be updated every game update)
        private readonly List<TimedAngleEmitter> _emittersList = new List<TimedAngleEmitter>();

        // Textures: Piece texture for each color, Clear texture
        private readonly Dictionary<PieceColor, Texture2D> _dictTextures;
        private Texture2D _texClearLine;

        // constructor sets the reference
        public ParticleController(
            SpriteBatch spriteBatch,
            Texture2D texParticleRed,
            Texture2D texParticleOrange,
            Texture2D texParticleYellow,
            Texture2D texParticleGreen,
            Texture2D texParticleBlue,
            Texture2D texParticleIndigo,
            Texture2D texParticleViolet,
            Texture2D texClearLine)
        {
            _spriteBatch = spriteBatch;
            _texClearLine = texClearLine;
            _dictTextures = new Dictionary<PieceColor, Texture2D>
            {
                {Red, texParticleRed},
                {Orange, texParticleOrange},
                {Yellow, texParticleYellow},
                {Green, texParticleGreen},
                {Blue, texParticleBlue},
                {Indigo, texParticleIndigo},
                {Violet, texParticleViolet}
            };
        }

        public void AddPieceEmitter(float boardX, float boardY, float angle, PieceColor color, GameTime gameTime)
        {
            var (sourceX, sourceY) = TetrisGame.GetAbsolutePixelCoordinates((boardX, boardY));

            var emitter = new TimedAngleEmitter(
                TimeSpan.FromMilliseconds(5),
                sourceX,
                sourceY,
                3,
                0.3f,
                TimeSpan.FromMilliseconds(250),
                _dictTextures[color],
                TimeSpan.FromMilliseconds(50),
                false
            );

            emitter.Angle = angle;
            emitter.Width = MathHelper.Pi;
            emitter.FireParticles();

            _emittersList.Add(emitter);
        }

        public void AddLineClearEmitter(float boardX, float boardY, float angle, GameTime gameTime)
        {
            var (sourceX, sourceY) = TetrisGame.GetAbsolutePixelCoordinates((boardX, boardY));

            var emitter = new TimedAngleEmitter(
                TimeSpan.FromMilliseconds(40),
                sourceX,
                sourceY,
                15,
                0.6f,
                TimeSpan.FromMilliseconds(500),
                _texClearLine,
                TimeSpan.FromMilliseconds(120),
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