using Microsoft.Xna.Framework.Input;

namespace LunarLander
{
    // This class is what is serialized into XML
    public class ControlScheme
    {
        // Control Scheme just holds three keys arrays
        // We *can't* make these readonly, otherwise the XML Serializer breaks
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public Keys[] ThrustKeys;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public Keys[] RotateLeftKeys;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
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