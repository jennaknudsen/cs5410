using Microsoft.Xna.Framework.Input;

namespace LunarLander
{
    public class Button
    {
        // whether this key should send a Pressed signal or not
        // only the Button itself should be able to set the Pressed property
        public bool Pressed { get; private set; } = false;

        // whether the button state is currently pressed or not
        private bool _buttonIsPressed = false;

        // whether this button needs to be debounced or not
        private readonly bool _isDebouncingButton;

        // an array of all bound keyboard keys to this Button
        public Keys[] BoundKeys;

        // constructor
        public Button(Keys[] boundKeys, bool isDebouncingButton)
        {
            BoundKeys = boundKeys;
            _isDebouncingButton = isDebouncingButton;
        }

        // presses the button (handling buttons that need to be debounced as well)
        public void PressButton()
        {
            // if this is a Debounce button and the button is already pressed, don't send another signal
            if (_isDebouncingButton && _buttonIsPressed)
            {
                Pressed = false;
            }
            else
            {
                _buttonIsPressed = true;
                Pressed = true;
            }
        }

        // releases the button
        public void ReleaseButton()
        {
            Pressed = false;
            _buttonIsPressed = false;
        }
    }
}
