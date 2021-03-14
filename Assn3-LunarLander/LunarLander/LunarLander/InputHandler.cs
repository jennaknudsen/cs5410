using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace LunarLander
{
    public class InputHandler
    {
        public readonly Button TurnShipLeftButton;
        public readonly Button TurnShipRightButton;
        public readonly Button ThrustUpButton;

        private List<Button> _listOfButtons;

        public InputHandler()
        {
            // buttons for game controls
            ThrustUpButton = new Button(new[] {Keys.Up}, false);
            TurnShipLeftButton = new Button(new[] {Keys.Left}, false);
            TurnShipRightButton = new Button(new[] {Keys.Right}, false);

            // add all buttons to the listOfButtons
            _listOfButtons = new List<Button>
            {
                ThrustUpButton,
                TurnShipLeftButton,
                TurnShipRightButton
            };
        }

        // reads raw keyboard input and translates that to button presses
        public void HandleInput()
        {
            var keyboardState = Keyboard.GetState();

            // for each button, set its pressed to true if any of the corresponding keyboard keys are pressed
            foreach (var button in _listOfButtons)
            {
                // LINQ expression: if any of the keys in BoundKeys were pressed
                if (button.BoundKeys.Any(key => keyboardState.IsKeyDown(key)))
                {
                    button.PressButton();
                }
                // release the button if no corresponding keys were pressed
                else
                {
                    button.ReleaseButton();
                }
            }
        }
    }
}