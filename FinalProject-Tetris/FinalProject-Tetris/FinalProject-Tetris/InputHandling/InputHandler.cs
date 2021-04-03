using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using FinalProject_Tetris.InputHandling;
using Microsoft.Xna.Framework.Input;

namespace FinalProject_Tetris.InputHandling
{
    public class InputHandler
    {
        // game buttons
        public readonly KeyboardButton MovePieceLeftButton;
        public readonly KeyboardButton MovePieceRightButton;
        public readonly KeyboardButton SoftDropButton;
        public readonly KeyboardButton HardDropButton;
        public readonly KeyboardButton RotateCounterClockwiseButton;
        public readonly KeyboardButton RotateClockwiseButton;

        // menu buttons
        public readonly MouseButton NewGameButton;
        public readonly MouseButton HighScoresButton;
        public readonly MouseButton CustomizeControlsButton;
        public readonly MouseButton ViewCreditsButton;
        public readonly MouseButton LeftControlButton;
        public readonly MouseButton RightControlButton;
        public readonly MouseButton UpControlButton;
        public readonly MouseButton DownControlButton;
        public readonly MouseButton CounterClockwiseControlButton;
        public readonly MouseButton ClockwiseControlButton;
        public readonly MouseButton ResetToDefaultsButton;
        public readonly MouseButton BackToMainButton;

        // debounce the left mouse button itself with this
        public readonly Button PhysicalMouseButton;


        // list holds all of our buttons (so we can iterate through them)
        private readonly List<Button> _listOfButtons;

        // holds the coordinates of the mouse (so we can detect movement)
        private (int x, int y) _mousePosition = (0, 0);
        public bool MouseMoved = false;
        public bool KeyPressed = false;

        public InputHandler()
        {
            // buttons for game controls
            MovePieceLeftButton = new KeyboardButton(new[] {Keys.Left}, true);
            MovePieceRightButton = new KeyboardButton(new[] {Keys.Right}, true);
            SoftDropButton = new KeyboardButton(new[] {Keys.Down, Keys.D}, true);
            HardDropButton = new KeyboardButton(new[] {Keys.Up}, true);
            RotateCounterClockwiseButton = new KeyboardButton(new[] {Keys.Home, Keys.Q}, true);
            RotateClockwiseButton = new KeyboardButton(new[] {Keys.PageUp, Keys.E}, true);

            // buttons for menu controls
            NewGameButton = new MouseButton((3, 23), (20, 25), true);
            HighScoresButton = new MouseButton((3, 17), (20, 19), true);
            CustomizeControlsButton = new MouseButton((3, 11), (20, 13), true);
            ViewCreditsButton = new MouseButton((3, 5), (20, 7), true);

            LeftControlButton = new MouseButton((3, 23), (13, 25), true);
            RightControlButton = new MouseButton((18, 23), (28, 25), true);
            DownControlButton = new MouseButton((3, 18), (13, 20), true);
            UpControlButton = new MouseButton((18, 18), (28, 20), true);
            CounterClockwiseControlButton = new MouseButton((3, 11), (13, 13), true);
            ClockwiseControlButton = new MouseButton((18, 11), (28, 13), true);

            ResetToDefaultsButton = new MouseButton((3, 7), (20, 9), true);
            BackToMainButton = new MouseButton((3, 5), (20, 7), true);

            // the physical left mouse button
            PhysicalMouseButton = new Button(true);

            // add all buttons to the listOfButtons
            _listOfButtons = new List<Button>
            {
                // game buttons
                MovePieceLeftButton,
                MovePieceRightButton,
                SoftDropButton,
                HardDropButton,
                RotateCounterClockwiseButton,
                RotateClockwiseButton,

                // menu buttons
                NewGameButton,
                HighScoresButton,
                CustomizeControlsButton,
                ViewCreditsButton,

                LeftControlButton,
                RightControlButton,
                UpControlButton,
                DownControlButton,
                CounterClockwiseControlButton,
                ClockwiseControlButton,
                ResetToDefaultsButton,
                BackToMainButton
            };
        }

        // reads raw keyboard input and translates that to button presses
        public void HandleInput()
        {
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();

            // flag for whether movement happened on this frame or not
            if (mouseState.X != _mousePosition.x || mouseState.Y != _mousePosition.y)
            {
                MouseMoved = true;
            }
            else
            {
                MouseMoved = false;
            }
            _mousePosition.x = mouseState.X;
            _mousePosition.y = mouseState.Y;

            // flag for whether key was pressed this frame or not
            KeyPressed = keyboardState.GetPressedKeys().Any();

            var (mouseX, mouseY) = TetrisGame.GetRelativeBoardCoordinates((mouseState.X, mouseState.Y));

            // get the mouse button input
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                PhysicalMouseButton.PressButton();
            }
            else
            {
                PhysicalMouseButton.ReleaseButton();
            }

            // for each button, set its pressed to true if any of the corresponding keyboard keys are pressed
            foreach (var button in _listOfButtons)
            {
                // LINQ expression: if any of the keys in BoundKeys were pressed
                if (button is KeyboardButton keyboardButton &&
                    keyboardButton.BoundKeys.Any(key => keyboardState.IsKeyDown(key)))
                {
                    keyboardButton.PressButton();
                }
                else if (button is MouseButton mouseButton)
                {
                    if (mouseX > mouseButton.StartPosition.x && mouseX < mouseButton.EndPosition.x &&
                        mouseY > mouseButton.StartPosition.y && mouseY < mouseButton.EndPosition.y)
                    {
                        mouseButton.IsHovered = true;
                        if (PhysicalMouseButton.Pressed)
                        {
                            mouseButton.PressButton();
                        }
                        else
                        {
                            mouseButton.ReleaseButton();
                        }
                    }
                    else
                    {
                        mouseButton.IsHovered = false;
                        mouseButton.ReleaseButton();
                    }
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