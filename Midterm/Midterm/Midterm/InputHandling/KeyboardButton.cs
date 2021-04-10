using Microsoft.Xna.Framework.Input;

namespace Midterm.InputHandling
{
    public class KeyboardButton : Button
    {
        // an array of all bound keyboard keys to this Button
        public Keys[] BoundKeys;

        // constructor
        public KeyboardButton(Keys[] boundKeys, bool isDebouncingButton)
        : base(isDebouncingButton)
        {
            BoundKeys = boundKeys;
        }
    }
}