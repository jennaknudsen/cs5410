using static LunarLander.GameState;

namespace LunarLander
{
    public class PauseMenuController : MenuController
    {
        // all menu items
        public readonly MenuItem ContinueMenuItem;
        public readonly MenuItem QuitMenuItem;

        public PauseMenuController(LanderGameController controller)
        {
            // set the reference
            GameController = controller;

            ContinueMenuItem = new MenuItem("Continue");
            QuitMenuItem = new MenuItem("Quit");
        }

        // on open menu, set game state to paused
        // select continue
        public override void OpenMenu()
        {
            // set calling controller's state to Paused
            GameController.GameState = Paused;

            // select Continue, deselect Quit
            ContinueMenuItem.Selected = true;
            QuitMenuItem.Selected = false;
        }

        // this will be very simple:
        // switch state to Running on continue, Main Menu on quit
        public override void ProcessMenu(InputHandler inputHandler)
        {
            // if the confirm button is pressed, then activate the selected button
            if (inputHandler.MenuConfirmButton.Pressed)
            {
                if (ContinueMenuItem.Selected)
                {
                    GameController.GameState = Running;
                }
                else
                {
                    GameController.MainMenuController.OpenMenu();
                }
            }
            else if (inputHandler.MenuDownButton.Pressed || inputHandler.MenuUpButton.Pressed)
            {
                ContinueMenuItem.Selected = !ContinueMenuItem.Selected;
                QuitMenuItem.Selected = !QuitMenuItem.Selected;
            }
            else if (inputHandler.MenuBackButton.Pressed)
            {
                GameController.GameState = Running;
            }
        }
    }
}