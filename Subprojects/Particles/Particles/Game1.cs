using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Particles.Particles;

namespace Particles
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private ExplosionEmitter _allEmitter;
        private AngleEmitter _angleEmitter;
        private AngleEmitter _angleEmitterNormalized;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            _allEmitter = new ExplosionEmitter(
                Content,
                new TimeSpan(0, 0, 0, 0, 3),
                200,
                200,
                10,
                1,
                new TimeSpan(0, 0, 0, 2),
                new TimeSpan(0, 0, 0, 0),
                new TimeSpan(0, 0, 0, 0, 300)
            );

            _angleEmitter = new AngleEmitter(
                Content,
                new TimeSpan(0, 0, 0, 0, 25),
                400,
                200,
                20,
                1,
                new TimeSpan(0, 0, 0, 6),
                new TimeSpan(0, 0, 0, 3),
                false
            );

            _angleEmitterNormalized = new AngleEmitter(
                Content,
                new TimeSpan(0, 0, 0, 0, 5),
                600,
                200,
                20,
                1,
                new TimeSpan(0, 0, 0, 6),
                new TimeSpan(0, 0, 0, 3),
                true
            );
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            _allEmitter.Update(gameTime);
            if (gameTime.TotalGameTime > TimeSpan.FromSeconds(3))
            {
                _allEmitter.FireExplosion();
            }

            if (gameTime.TotalGameTime > TimeSpan.FromSeconds(1) && gameTime.TotalGameTime < TimeSpan.FromSeconds(6))
            {
                _angleEmitter.EmitParticles = true;
                _angleEmitterNormalized.EmitParticles = true;
            }
            else
            {
                // _angleEmitter.EmitParticles = false;
                _angleEmitter.EmitParticles = true;
                // _angleEmitterNormalized.EmitParticles = false;
                _angleEmitterNormalized.EmitParticles = true;
            }

            // set angle stats here
            _angleEmitter.Angle = MathHelper.PiOver2;
            _angleEmitter.Width = MathHelper.PiOver2;
            _angleEmitterNormalized.Angle = 5 * MathHelper.PiOver4;
            _angleEmitterNormalized.Width = MathHelper.PiOver2;
            _angleEmitter.Update(gameTime);
            _angleEmitterNormalized.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here

            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.Additive);

            _allEmitter.Draw(_spriteBatch);
            _angleEmitter.Draw(_spriteBatch);
            _angleEmitterNormalized.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
