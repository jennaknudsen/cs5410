namespace Midterm.InputHandling
{
    public class Button
    {
        // whether this key should send a Pressed signal or not
        // only the Button itself should be able to set the Pressed property
        public bool Pressed { get; private set; }

        // whether the button state is currently pressed or not
        private bool _buttonIsPressed;

        // whether this button needs to be debounced or not
        private readonly bool _isDebouncingButton;

        // parent constructor to assign this readonly bool
        public Button(bool isDebouncingButton)
        {
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