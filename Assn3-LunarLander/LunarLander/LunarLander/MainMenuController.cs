using System;
using System.Collections.Generic;
using static LunarLander.GameState;
using static LunarLander.MenuState;

namespace LunarLander
{
    public class MainMenuController : MenuController
    {
        // holds a reference to a LanderGameController
        private LanderGameController _gameController;

        // holds the current menu state information
        public MenuState MenuState;

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

        public MainMenuController(LanderGameController controller)
        {
            // set the reference
            _gameController = controller;

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

        // common code used to set up the menu
        public void OpenMenu()
        {
            // set calling controller's state to MainMenu
            _gameController.GameState = MainMenu;

            // set this menu's state to Main
            MenuState = Main;

            // deselect all items in the menu
            foreach (var item in MainMenuList)
            {
                item.Selected = false;
            }

            // finally, select "New Game"
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
            switch (MenuState)
            {
                case Main:
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

                        // create a new game
                        if (selectedItem == NewGameMenuItem)
                        {
                            _gameController.StartLevel(1);
                        }
                        // TODO handle other buttons
                    }
                    break;
                case Controls:
                    break;
                case HighScores:
                    break;
                case Credits:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}