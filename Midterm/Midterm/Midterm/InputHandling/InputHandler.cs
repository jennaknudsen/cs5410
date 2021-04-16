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
        public readonly MouseButton bomb1;
        public readonly MouseButton bomb2;
        public readonly MouseButton bomb3;
        public readonly MouseButton bomb4;
        public readonly MouseButton bomb5;
        public readonly MouseButton bomb6;
        public readonly MouseButton bomb7;
        public readonly MouseButton bomb8;
        public readonly MouseButton bomb9;
        public readonly MouseButton bomb10;
        public readonly MouseButton bomb11;
        public readonly MouseButton bomb12;
        public readonly KeyboardButton PauseGameButton;

        // main menu buttons
        public readonly MouseButton NewGameButton;
        public readonly MouseButton HighScoresButton;
        public readonly MouseButton ViewCreditsButton;
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
            bomb1 = new MouseButton((35, 30), (45, 40), true);
            bomb2 = new MouseButton((45, 30), (55, 40), true);
            bomb3 = new MouseButton((55, 30), (65, 40), true);

            bomb4 = new MouseButton((35, 40), (45, 50), true);
            bomb5 = new MouseButton((45, 40), (55, 50), true);
            bomb6 = new MouseButton((55, 40), (65, 50), true);

            bomb7 = new MouseButton((35, 50), (45, 60), true);
            bomb8 = new MouseButton((45, 50), (55, 60), true);
            bomb9 = new MouseButton((55, 50), (65, 60), true);

            bomb10 = new MouseButton((35, 60), (45, 70), true);
            bomb11 = new MouseButton((45, 60), (55, 70), true);
            bomb12 = new MouseButton((55, 60), (65, 70), true);
            PauseGameButton = new KeyboardButton(new[] {Keys.Escape}, true);

            // buttons for main menu controls
            NewGameButton = new MouseButton((10, 76), (50, 82), true);
            HighScoresButton = new MouseButton((10, 57), (50, 63), true);
            ViewCreditsButton = new MouseButton((10, 19), (50, 25), true);
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
                PauseGameButton,

                // main menu buttons
                NewGameButton,
                HighScoresButton,
                ViewCreditsButton,

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