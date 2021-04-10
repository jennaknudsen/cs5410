using Midterm.InputHandling;
using static Midterm.GameState;

namespace Midterm.Menuing
{
    public class PauseMenuController : MenuController
    {
        // all menu items
        public readonly MenuItem ContinueMenuItem;
        public readonly MenuItem QuitMenuItem;

        public PauseMenuController(MidtermGameController controller)
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
            GameController.GameState = GameState.Paused;
            GameController.SoundController.PauseMusic();
        }

        // this will be very simple:
        // switch state to Running on continue, Main Menu on quit
        public override void ProcessMenu(InputHandler inputHandler)
        {
            // if the confirm button is pressed, then activate the selected button
            if (inputHandler.ResumeButton.Pressed || inputHandler.PauseGameButton.Pressed)
            {
                GameController.GameState = Running;
                GameController.SoundController.PlayMusic();
            }
            else if (inputHandler.QuitButton.Pressed)
            {
                GameController.MainMenuController.OpenMenu();
                GameController.SoundController.StopMusic();
            }
        }
    }
}