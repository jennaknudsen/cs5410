using Microsoft.Xna.Framework;

namespace LunarLander
{
    public class LanderGameController
    {
        public Lander Lander;

        // moon gravity: https://en.wikipedia.org/wiki/Moon
        private const float MoonGravity = 1.62f;    // m/(s^2)
        private readonly (float x, float y) _startPosition = (10, 130);

        public LanderGameController()
        {
            Lander = new Lander();
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}