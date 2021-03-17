using Microsoft.Xna.Framework.Input;

namespace LunarLander
{
    public class ControlScheme
    {
        // Control Scheme just holds three keys arrays
        public Keys[] ThrustKeys;
        public Keys[] RotateLeftKeys;
        public Keys[] RotateRightKeys;

        // can't call default constructor
        private ControlScheme()
        {
            // default constructor does nothing
        }

        public ControlScheme(Keys[] thrustKeys, Keys[] rotateLeftKeys, Keys[] rotateRightKeys)
        {
            ThrustKeys = thrustKeys;
            RotateLeftKeys = rotateLeftKeys;
            RotateRightKeys = rotateRightKeys;
        }
    }
}