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
                    break;
                default:
                    break;
            }
        }
    }
}