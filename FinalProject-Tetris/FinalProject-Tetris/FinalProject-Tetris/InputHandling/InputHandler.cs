using System.Collections.Generic;
using System.Linq;
using FinalProject_Tetris.InputHandling;
using Microsoft.Xna.Framework.Input;

namespace FinalProject_Tetris.InputHandling
{
    public class InputHandler
    {
        public readonly KeyboardButton MovePieceLeftButton;
        public readonly KeyboardButton MovePieceRightButton;
        public readonly KeyboardButton SoftDropButton;
        public readonly KeyboardButton HardDropButton;
        public readonly KeyboardButton RotateCounterClockwiseButton;
        public readonly KeyboardButton RotateClockwiseButton;

        // menu buttons
        // public readonly KeyboardButton MenuUpButton;
        public readonly MouseButton MenuUpButton;
        public readonly KeyboardButton MenuDownButton;
        public readonly KeyboardButton MenuConfirmButton;
        public readonly KeyboardButton MenuBackButton;
        public readonly KeyboardButton PauseButton;

        private readonly List<Button> _listOfButtons;

        public InputHandler()
        {
            // buttons for game controls
            MovePieceLeftButton = new KeyboardButton(new[] {Keys.Left}, true);
            MovePieceRightButton = new KeyboardButton(new[] {Keys.Right}, true);
            SoftDropButton = new KeyboardButton(new[] {Keys.Down}, true);
            HardDropButton = new KeyboardButton(new[] {Keys.Up}, true);
            // TODO: fix these bindings in final submission
            RotateCounterClockwiseButton = new KeyboardButton(new[] {Keys.Home, Keys.Q}, true);
            RotateClockwiseButton = new KeyboardButton(new[] {Keys.PageUp, Keys.E}, true);

            // buttons for menu controls
            // MenuUpButton = new KeyboardButton(new[] {Keys.Up}, true);
            MenuUpButton = new MouseButton((10, 3), (11, 4), true);
            MenuDownButton = new KeyboardButton(new[] {Keys.Down}, true);
            MenuConfirmButton = new KeyboardButton(new[] {Keys.Enter}, true);
            MenuBackButton = new KeyboardButton(new[] {Keys.Escape}, true);
            PauseButton = new KeyboardButton(new[] {Keys.Escape}, true);


            // add all buttons to the listOfButtons
            _listOfButtons = new List<Button>
            {
                HardDropButton,
                MovePieceLeftButton,
                MovePieceRightButton,
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
            var mouseState = Mouse.GetState();
            var (mouseX, mouseY) = TetrisGame.GetRelativeBoardCoordinates((mouseState.X, mouseState.Y));

            // for each button, set its pressed to true if any of the corresponding keyboard keys are pressed
            foreach (var button in _listOfButtons)
            {
                // LINQ expression: if any of the keys in BoundKeys were pressed
                if (button is KeyboardButton keyboardButton &&
                    keyboardButton.BoundKeys.Any(key => keyboardState.IsKeyDown(key)))
                {
                    keyboardButton.PressButton();
                }
                else if (button is MouseButton mouseButton &&
                         mouseX > mouseButton.StartPosition.x && mouseX < mouseButton.EndPosition.x &&
                         mouseY > mouseButton.StartPosition.y && mouseY < mouseButton.EndPosition.y &&
                         mouseState.LeftButton == ButtonState.Pressed)
                {
                    mouseButton.PressButton();
                }
                // release the button if no corresponding keys were pressed
                else
                {
                    button.ReleaseButton();
                }
            }
        }

        // get all keys that are pressed
        public static Keys[] GetDepressedKeys()
        {
            return Keyboard.GetState().GetPressedKeys();
        }
    }
}