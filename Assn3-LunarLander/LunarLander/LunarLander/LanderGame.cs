using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LunarLander
{
    public class LanderGame : Game
    {
        // game controller handles all underlying logic
        private LanderGameController _landerGameController;

        // assets for this game
        private Texture2D _texLander;
        private Rectangle _positionRectangle;
        private SpriteFont _spriteFont;

        // size of board in units
        private const float BoardSize = 150f;

        // MonoGame stuff
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public LanderGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _landerGameController = new LanderGameController();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _texLander = this.Content.Load<Texture2D>("Lander-2");
            _spriteFont = this.Content.Load<SpriteFont>("GameFont");
        }

        protected override void Update(GameTime gameTime)
        {
            // all updating is handled in the game controller
            _landerGameController.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            // for debugging purposes, draw the background square in center of screen
            // get pixel coordinates from board coordinates
            var (backX, backY) = GetAbsolutePixelCoordinates((0, BoardSize));
            var rectSizePixels = RescaleUnitsToPixels(BoardSize);
            // create the MG Rectangle
            var backgroundRect = new Rectangle(backX, backY, rectSizePixels, rectSizePixels);
            // make a generic Gray texture
            var grayTexture = new Texture2D(_graphics.GraphicsDevice, 10, 10);
            var texData = new Color[10 * 10];
            for (var i = 0; i < texData.Length; i++)
                texData[i] = Color.Gray;
            grayTexture.SetData(texData);
            _spriteBatch.Draw(grayTexture, backgroundRect, Color.Gray);

            // Now, draw the lander

            // set lander position rectangle
            // sets the top left of lander (we are re-adjusting the texture origin in the Draw() function)
            var lander = _landerGameController.Lander;
            var (landerX, landerY) = GetAbsolutePixelCoordinates((lander.Position.x,
                lander.Position.y));
            var landerSizePixels = RescaleUnitsToPixels(Lander.Size);
            _positionRectangle = new Rectangle(landerX, landerY, landerSizePixels, landerSizePixels);

            // run draw function
            _spriteBatch.Draw(_texLander,
                _positionRectangle,
                null,
                Color.Aqua,
                lander.Orientation,
                // center origin in the texture
                new Vector2(_texLander.Width / 2, _texLander.Width / 2),
                SpriteEffects.None,
                0);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        // game board will have relative dimensions in a square
        // this function gets the absolute dimensions
        private (int x, int y) GetAbsolutePixelCoordinates((float x, float y) relativeCoordinates)
        {
            // keep relative coordinates good
            if (relativeCoordinates.x < 0 || relativeCoordinates.x > BoardSize ||
                relativeCoordinates.y < 0 || relativeCoordinates.y > BoardSize)
            {
                // uncomment this line if we want to force spaceship to stay in safe area
                // throw new Exception("Relative coordinates must be between 0 and " + BoardSize + ".");
            }

            // get absolute pixel dimensions
            var canvasBounds = GraphicsDevice.Viewport.Bounds;
            var canvasWidthPixels = canvasBounds.Width;
            var canvasHeightPixels = canvasBounds.Height;

            // get size of playable area (will be constrained by height)
            var sizeOfGameAreaPixels = canvasHeightPixels;

            // width will be square centered in screen, same dimensions as height
            var horizontalMarginPixels = (canvasWidthPixels - sizeOfGameAreaPixels) / 2;

            // properly rescale the coordinates to get the correct pixels
            var rescaledX = RescaleUnitsToPixels(relativeCoordinates.x) + horizontalMarginPixels;
            var rescaledY = RescaleUnitsToPixels(BoardSize - relativeCoordinates.y);

            // return rescaled coordinates
            return (rescaledX, rescaledY);
        }

        // given a unit count, rescale it to pixels
        private int RescaleUnitsToPixels(float units)
        {
            // get absolute pixel dimensions
            var sizeOfGameAreaPixels = GraphicsDevice.Viewport.Bounds.Height;

            // rescale by ratio of game area in pixels to board size
            var rescaledUnits = (int) (sizeOfGameAreaPixels / BoardSize * units);
            return rescaledUnits;
        }
    }
}
