using static LunarLander.GameState;
using static LunarLander.MenuState;

namespace LunarLander
{
    public class LanderMenuController
    {
        // holds the current menu state information
        public MenuState MenuState;

        // holds previous state information
        public GameState ReturnToGameState;

        // flag to exit menu
        public bool ExitMenu = false;

        // default constructor does nothing
        public LanderMenuController()
        {

        }

        public void OpenMenu()
        {
            MenuState = Main;
            ReturnToGameState = Paused;
        }

        public void OpenMenu(GameState returnToGameState)
        {
            MenuState = Main;
            ReturnToGameState = returnToGameState;
        }

        public void ProcessMenu(InputHandler inputHandler)
        {
            ExitMenu = false;
            switch (MenuState)
            {
                case Main:
                    if (inputHandler.MenuBackButton.Pressed)
                    {
                        ExitMenu = true;
                    }
                    break;
            }
        }
    }
}