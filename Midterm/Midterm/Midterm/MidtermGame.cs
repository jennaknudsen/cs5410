using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Midterm.MidtermGameController;

namespace Midterm
{
    public class MidtermGame : Game
    {
        // import textures here
        // private Texture2D ...

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // width and height of window
        private static int _windowWidthPixels;
        private static int _windowHeightPixels;

        // reference to midterm game controller
        private MidtermGameController _midtermGameController;

        public MidtermGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Modify to change game render resolution
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.ApplyChanges();

            _midtermGameController = new MidtermGameController();

            var canvasBounds = GraphicsDevice.Viewport.Bounds;
            _windowWidthPixels = canvasBounds.Width;
            _windowHeightPixels = canvasBounds.Height;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
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

            base.Draw(gameTime);
        }

        // game board will have relative dimensions in a square
        // this function gets the absolute dimensions
        public static (int x, int y) GetAbsolutePixelCoordinates((float x, float y) relativeCoordinates)
        {
            // keep relative coordinates good
            if (relativeCoordinates.x < 0 || relativeCoordinates.x > BoardSize ||
                relativeCoordinates.y < 0 || relativeCoordinates.y > BoardSize)
            {
                // uncomment this line if we want to force to stay in safe area
                // throw new Exception("Relative coordinates must be between 0 and " + BoardSize + ".");
            }

            // get size of playable area (will be constrained by height)
            var sizeOfGameAreaPixels = _windowHeightPixels;

            // width will be square centered in screen, same dimensions as height
            var horizontalMarginPixels = (_windowWidthPixels - sizeOfGameAreaPixels) / 2;

            // properly rescale the coordinates to get the correct pixels
            var rescaledX = RescaleUnitsToPixels(relativeCoordinates.x) + horizontalMarginPixels;
            var rescaledY = RescaleUnitsToPixels(BoardSize - relativeCoordinates.y);

            // return rescaled coordinates
            return (rescaledX, rescaledY);
        }

        // this function gets the absolute dimensions
        public static (float x, float y) GetRelativeBoardCoordinates((int x, int y) absoluteCoordinates)
        {
            // keep relative coordinates good
            if (absoluteCoordinates.x < 0 || absoluteCoordinates.x > BoardSize ||
                absoluteCoordinates.y < 0 || absoluteCoordinates.y > BoardSize)
            {
                // uncomment this line if we want to force to stay in safe area
                // throw new Exception("Relative coordinates must be between 0 and " + BoardSize + ".");
            }

            // get size of playable area (will be constrained by height)
            var sizeOfGameAreaPixels = _windowHeightPixels;

            // width will be square centered in screen, same dimensions as height
            var horizontalMarginPixels = (_windowWidthPixels - sizeOfGameAreaPixels) / 2;

            // properly rescale the coordinates to get the correct pixels
            var rescaledX = RescalePixelsToUnits(absoluteCoordinates.x - horizontalMarginPixels);
            var rescaledY = RescalePixelsToUnits(sizeOfGameAreaPixels - absoluteCoordinates.y);

            // return rescaled coordinates
            return (rescaledX, rescaledY);
        }

        // given a unit count, rescale it to pixels
        public static int RescaleUnitsToPixels(float units)
        {
            // rescale by ratio of game area in pixels to board size
            var rescaledUnits = (int) (_windowHeightPixels / BoardSize * units);
            return rescaledUnits;
        }

        // given a pixel count, rescale to units
        public static float RescalePixelsToUnits(int pixels)
        {
            var rescaledUnits = (float) BoardSize / _windowHeightPixels * pixels;
            return rescaledUnits;
        }

    }
}
