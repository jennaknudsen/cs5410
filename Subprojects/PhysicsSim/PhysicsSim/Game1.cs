using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PhysicsSim
{
    public class Game1 : Game
    {
        private Rectangle _rectFreeFall = new Rectangle(150, 150, 100, 100);
        private Rectangle _rect150_150 = new Rectangle(145, 145, 10, 10);
        private Texture2D _texLander;
        private SpriteFont _spriteFont;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

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
            _texLander = this.Content.Load<Texture2D>("Images/Lander-2");
            _spriteFont = this.Content.Load<SpriteFont>("GameFont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            /*
             * using the function SpriteBatch.Draw with following arguments:
             *
             * void SpriteBatch.Draw(Texture2D texture,
             *                       Rectangle destinationRectangle,
             *                       Rectangle? sourceRectangle,
             *                       Color color,
             *                       float rotation,
             *                       Vector2 origin,
             *                       SpriteEffects effects,
             *                       float layerDepth) (+ 6 overloads)
             */
            var elapsedTime = gameTime.TotalGameTime.Seconds;
            // every 2 seconds, change position
            int vectorPos = (elapsedTime % 10d) switch
            {
                //0 or 1 => 0,
                0 => 0,
                1 => 0,
                2 => 100,
                3 => 100,
                4 => 200,
                5 => 200,
                6 => 300,
                7 => 300,
                8 => 400,
                9 => 400,
                _ => 1000

               // _ => throw new ArgumentOutOfRangeException()
            };

            var dimen = 400;
            var grayRect = new Texture2D(_graphics.GraphicsDevice, dimen, dimen);
            var data2 = new Color[dimen * dimen];
            for (var i = 0; i < data2.Length; i++)
            {
                data2[i] = Color.Gray;
            }
            grayRect.SetData(data2);

            _rectFreeFall.X = 150 - (vectorPos / 100 * 30);
            _rectFreeFall.Y = 150 - (vectorPos / 100 * 30);
            Console.WriteLine("Rect XY: " + _rectFreeFall.X);

            _spriteBatch.Draw(grayRect,
                _rectFreeFall,
                null,
                Color.Aqua,
                0,
                new Vector2(_texLander.Width / 2, _texLander.Height / 2),
                SpriteEffects.None,
                0);

            _spriteBatch.Draw(_texLander,
                _rectFreeFall,
                null,
                Color.Aqua,
                0,
                new Vector2(vectorPos, vectorPos),
                SpriteEffects.None,
                0);

            // this code block draws a 10*10 rectangle
            // From: https://stackoverflow.com/a/5751867
            var rect = new Texture2D(_graphics.GraphicsDevice, 5, 5);
            var data = new Color[5 * 5];
            for (var i = 0; i < data.Length; i++)
            {
                data[i] = Color.Black;
            }
            rect.SetData(data);
            _spriteBatch.Draw(rect, _rect150_150, Color.Aqua);

            _spriteBatch.DrawString(_spriteFont, "Vector position: " + vectorPos + ", " + vectorPos +
                "\nDimensions: " + dimen + " x " + dimen,
                new Vector2(400, 400), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
