using Microsoft.Xna.Framework.Input;

namespace Midterm.LocalStorage
{
    public class ControlScheme
    {
        // Control Scheme just holds three keys arrays
        // We *can't* make these readonly, otherwise the XML Serializer breaks
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public Keys[] LeftKeys;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public Keys[] RightKeys;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public Keys[] UpKeys;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public Keys[] DownKeys;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public Keys[] RotateClockwiseKeys;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public Keys[] RotateCounterClockwiseKeys;

        // can't call default constructor
        private ControlScheme()
        {
            // default constructor does nothing
        }

        public ControlScheme(Keys[] leftKeys, Keys[] rightKeys, Keys[] upKeys, Keys[] downKeys,
            Keys[] rotateClockwiseKeys, Keys[] rotateCounterClockwiseKeys)
        {
            LeftKeys = leftKeys;
            RightKeys = rightKeys;
            UpKeys = upKeys;
            DownKeys = downKeys;
            RotateClockwiseKeys = rotateClockwiseKeys;
            RotateCounterClockwiseKeys = rotateCounterClockwiseKeys;
        }
    }
}