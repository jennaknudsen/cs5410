using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using Midterm.InputHandling;
using Microsoft.Xna.Framework.Input;

namespace Midterm.InputHandling
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
        public readonly KeyboardButton PauseGameButton;

        // main menu buttons
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

        // pause menu buttons
        public readonly MouseButton ResumeButton;
        public readonly MouseButton QuitButton;

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
            PauseGameButton = new KeyboardButton(new[] {Keys.Escape}, true);

            // buttons for main menu controls
            NewGameButton = new MouseButton((10, 76), (50, 82), true);
            HighScoresButton = new MouseButton((10, 57), (50, 63), true);
            CustomizeControlsButton = new MouseButton((10, 38), (50, 44), true);
            ViewCreditsButton = new MouseButton((10, 19), (50, 25), true);

            LeftControlButton = new MouseButton((10, 76), (40, 82), true);
            RightControlButton = new MouseButton((60, 76), (90, 82), true);
            DownControlButton = new MouseButton((10, 63), (40, 69), true);
            UpControlButton = new MouseButton((60, 63), (90, 69), true);
            CounterClockwiseControlButton = new MouseButton((10, 50), (40, 56), true);
            ClockwiseControlButton = new MouseButton((60, 50), (90, 56), true);

            ResetToDefaultsButton = new MouseButton((10, 23), (50, 29), true);
            BackToMainButton = new MouseButton((10, 14), (50, 20), true);

            // buttons for pause menu controls
            ResumeButton = new MouseButton((10, 23), (50, 29), true);
            QuitButton = new MouseButton((10, 14), (50, 20), true);

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
                PauseGameButton,

                // main menu buttons
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
                BackToMainButton,

                // pause menu buttons
                ResumeButton,
                QuitButton
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

            var (mouseX, mouseY) = MidtermGame.GetRelativeBoardCoordinates((mouseState.X, mouseState.Y));

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