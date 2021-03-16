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
        public readonly Button MenuUpButton;
        public readonly Button MenuDownButton;
        public readonly Button MenuConfirmButton;
        public readonly Button MenuBackButton;
        public readonly Button PauseButton;

        private readonly List<Button> _listOfButtons;

        public InputHandler()
        {
            // buttons for game controls
            ThrustUpButton = new Button(new[] {Keys.Up}, false);
            TurnShipLeftButton = new Button(new[] {Keys.Left}, false);
            TurnShipRightButton = new Button(new[] {Keys.Right}, false);

            // buttons for menu controls
            MenuUpButton = new Button(new[] {Keys.Up}, true);
            MenuDownButton = new Button(new[] {Keys.Down}, true);
            MenuConfirmButton = new Button(new[] {Keys.Enter}, true);
            MenuBackButton = new Button(new[] {Keys.Escape}, true);
            PauseButton = new Button(new[] {Keys.Escape}, true);


            // add all buttons to the listOfButtons
            _listOfButtons = new List<Button>
            {
                ThrustUpButton,
                TurnShipLeftButton,
                TurnShipRightButton,
                MenuUpButton,
                MenuDownButton,
                MenuConfirmButton,
                MenuBackButton,
                PauseButton,
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