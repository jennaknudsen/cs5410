using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TerrainGen
{
    public class Game1 : Game
    {
        private int[] m_indexTriStrip;
        private VertexPositionColor[] m_vertsTriStrip;
        private GraphicsDeviceManager m_graphics;
        private SpriteBatch _spriteBatch;
        private BasicEffect m_effect;

        public Game1()
        {
            m_graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Add your initialization logic here
            m_graphics.PreferredBackBufferWidth = 1000;
            m_graphics.PreferredBackBufferHeight = 800;

            m_graphics.ApplyChanges();

            m_graphics.GraphicsDevice.RasterizerState = new RasterizerState
            {
                FillMode = FillMode.Solid,
                //CullMode = CullMode.CullCounterClockwiseFace,   // CullMode.None If you want to not worry about triangle winding order
                CullMode = CullMode.None,
                MultiSampleAntiAlias = true,
            };

            m_effect = new BasicEffect(m_graphics.GraphicsDevice)
            {
                VertexColorEnabled = true,
                View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 1.0f), Vector3.Zero, Vector3.Up),
                Projection = Matrix.CreatePerspectiveOffCenter(0.0f, m_graphics.GraphicsDevice.Viewport.Width, m_graphics.GraphicsDevice.Viewport.Height, 0, 1.0f, 1000.0f),
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            // Define the data for 3 triangles in a triangle strip
            var vertPosColorList = new List<VertexPositionColor>
            {
                new VertexPositionColor
                {
                    Position = new Vector3(0, 0, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(0, 500, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(100, 0, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(100, 300, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(200, 0, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(200, 300, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(300, 0, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(300, 400, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(400, 0, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(400, 200, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(500, 0, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(500, 100, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(600, 0, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(600, 100, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(700, 0, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(700, 200, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(800, 0, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(800, 400, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(900, 0, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(900, 800, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(1000, 0, 0),
                    Color = Color.Black
                },
                new VertexPositionColor
                {
                    Position = new Vector3(1000, 700, 0),
                    Color = Color.Black
                },
            };
            m_vertsTriStrip = vertPosColorList.ToArray();

            m_indexTriStrip = new int[m_vertsTriStrip.Length];
            for (var i = 0; i < m_indexTriStrip.Length; i++)
                m_indexTriStrip[i] = i;

            Console.WriteLine(" ");
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
            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                // TODO: Add your drawing code here
                m_graphics.GraphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.TriangleStrip,
                    m_vertsTriStrip, 0, m_vertsTriStrip.Length,
                    m_indexTriStrip, 0, 20);

            }

            base.Draw(gameTime);
        }
    }
}
