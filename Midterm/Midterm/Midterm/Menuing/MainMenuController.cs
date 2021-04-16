using System.Collections.Generic;
using Midterm.InputHandling;
using Midterm.Menuing;
using Microsoft.Xna.Framework.Input;
using static Midterm.Menuing.MenuState;

namespace Midterm.Menuing
{
    public class MainMenuController : MenuController
    {
        // holds the state of this menu
        public MenuState MenuState;

        // holds a list of all high scores
        public List<int> HighScoresIntList;

        public MainMenuController(MidtermGameController controller)
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
                    else if (inputHandler.ViewCreditsButton.Pressed)
                    {
                        MenuState = Credits;
                    }
                    break;
                case HighScores:
                case Credits:
                    if (inputHandler.BackToMainButton.Pressed)
                        MenuState = Main;
                    break;
            }
        }
    }
}