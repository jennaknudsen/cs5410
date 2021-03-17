using System.Collections.Generic;
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

        // lists of menus
        public List<MenuItem> MainMenuList;
        public List<MenuItem> HighScoresList;
        public List<MenuItem> ControlsList;
        public List<MenuItem> CreditsList;

        // all menu items
        public MenuItem NewGameMenuItem;
        public MenuItem HighScoresMenuItem;
        public MenuItem BackToMainMenuItem;
        public MenuItem CustomizeControlsMenuItem;
        public MenuItem ThrustMenuItem;
        public MenuItem RotateLeftMenuItem;
        public MenuItem RotateRightMenuItem;
        public MenuItem ViewCreditsMenuItem;

        // default constructor does nothing
        public LanderMenuController()
        {
            // these are all of the menu items
            NewGameMenuItem = new MenuItem("New Game");
            HighScoresMenuItem = new MenuItem("High Scores");
            BackToMainMenuItem = new MenuItem("Back to Main");
            CustomizeControlsMenuItem = new MenuItem("Customize Controls");
            ThrustMenuItem = new MenuItem("Thrust");
            RotateLeftMenuItem = new MenuItem("Rotate Left");
            RotateRightMenuItem = new MenuItem("Rotate Right");
            ViewCreditsMenuItem = new MenuItem("High Scores");

            // populate each menu item list
            MainMenuList = new List<MenuItem>
            {
                NewGameMenuItem,
                HighScoresMenuItem,
                CustomizeControlsMenuItem,
                ViewCreditsMenuItem
            };
            HighScoresList = new List<MenuItem>
            {
                BackToMainMenuItem
            };
            ControlsList = new List<MenuItem>
            {
                ThrustMenuItem,
                RotateLeftMenuItem,
                RotateRightMenuItem,
                BackToMainMenuItem
            };
            CreditsList = new List<MenuItem>
            {
                BackToMainMenuItem
            };
        }

        // open the menu, and make it impossible to go back to previous screen
        public void OpenMenu()
        {
            ReturnToGameState = Paused;
            OpenMenuInner();
        }

        // open the menu, and make it possible to go back to previous screen
        public void OpenMenu(GameState returnToGameState)
        {
            ReturnToGameState = returnToGameState;
            OpenMenuInner();
        }

        // common code used to set up the menu
        private void OpenMenuInner()
        {
            MenuState = Main;
            foreach (var item in MainMenuList)
            {
                item.Selected = false;
            }

            NewGameMenuItem.Selected = true;
        }

        /*
         * Main Menu
         * |----- New Game
         * |
         * |----- High Scores
         * |      └--- Back to Main
         * |
         * |----- Customize Controls
         * |      |--- Thrust
         * |      |--- Rotate Left
         * |      |--- Rotate Right
         * |      └---- Back to Main
         * |
         * |----- View Credits
         *        └---- Back to Main
         */
        public void ProcessMenu(InputHandler inputHandler)
        {
            ExitMenu = false;
            switch (MenuState)
            {
                case Main:
                    // in main menu, if we press the menu back button, raise a flag that we want to exit the menu
                    if (inputHandler.MenuBackButton.Pressed)
                    {
                        ExitMenu = true;
                    }
                    else
                    {
                        // on up/down press, just select the next / prev item
                        if (inputHandler.MenuUpButton.Pressed)
                        {
                            SelectPreviousItem(MainMenuList);
                        }
                        else if (inputHandler.MenuDownButton.Pressed)
                        {
                            SelectNextItem(MainMenuList);
                        }
                        // on confirm press, we need to determine which action to do
                        else if (inputHandler.MenuConfirmButton.Pressed)
                        {
                            // get selected index in Main Menu
                            var selectedIndex = GetSelectedIndex(MainMenuList);
                            var selectedItem = MainMenuList[selectedIndex];
                        }
                    }
                    break;
            }
        }

        // gets the selected menu item
        private int GetSelectedIndex(IReadOnlyList<MenuItem> menuItems)
        {
            // iterate until finding a selected item
            for (var i = 0; i < menuItems.Count; i++)
            {
                if (menuItems[i].Selected)
                {
                    return i;
                }
            }

            // if nothing is selected, just select the first menu item and return
            menuItems[0].Selected = true;
            return 0;
        }

        private void SelectPreviousItem(IReadOnlyList<MenuItem> menuItems)
        {
            // find selected index, decrement it (with wraparound), deselect old and select new
            var selectedIndex = GetSelectedIndex(menuItems);
            int newIndex;
            if (selectedIndex == 0)
                newIndex = menuItems.Count - 1;
            else
                newIndex = selectedIndex - 1;

            menuItems[selectedIndex].Selected = false;
            menuItems[newIndex].Selected = true;
        }

        private void SelectNextItem(IReadOnlyList<MenuItem> menuItems)
        {
            // find selected index, increment it (with wraparound), deselect old and select new
            var selectedIndex = GetSelectedIndex(menuItems);
            int newIndex;
            if (selectedIndex == menuItems.Count - 1)
                newIndex = 0;
            else
                newIndex = selectedIndex + 1;

            menuItems[selectedIndex].Selected = false;
            menuItems[newIndex].Selected = true;
        }
    }
}