using FinalProject_Tetris.InputHandling;
using FinalProject_Tetris.Menuing;
using static FinalProject_Tetris.Menuing.MenuState;

namespace FinalProject_Tetris
{
    public class MainMenuController : MenuController
    {
        public MenuState MenuState;

        public MainMenuController(TetrisGameController controller)
        {
            GameController = controller;
        }

        public override void OpenMenu()
        {
            GameController.GameState = GameState.MainMenu;
            MenuState = Main;
        }

        public override void ProcessMenu(InputHandler inputHandler)
        {
            switch (MenuState)
            {
                case Main:
                    if (inputHandler.NewGameButton.Pressed)
                    {
                        GameController.StartGame(false);
                    }
                    else if (inputHandler.HighScoresButton.Pressed)
                    {
                        MenuState = HighScores;
                    }
                    else if (inputHandler.CustomizeControlsButton.Pressed)
                    {
                        MenuState = Controls;
                    }
                    else if (inputHandler.ViewCreditsButton.Pressed)
                    {
                        MenuState = Credits;
                    }
                    break;
                default:
                    break;
            }
        }
    }
}