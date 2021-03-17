using System;
using System.Collections.Generic;
using static LunarLander.GameState;
using static LunarLander.MenuState;

namespace LunarLander
{
    public class MainMenuController : MenuController
    {
        // holds the current menu state information
        public MenuState MenuState;

        // lists of menus
        public readonly List<MenuItem> MainMenuList;
        public readonly List<MenuItem> HighScoresList;
        public readonly List<MenuItem> ControlsList;
        public readonly List<MenuItem> CreditsList;

        // all menu items
        public readonly MenuItem NewGameMenuItem;
        public readonly MenuItem HighScoresMenuItem;
        public readonly MenuItem BackToMainMenuItem;
        public readonly MenuItem CustomizeControlsMenuItem;
        public readonly MenuItem ThrustMenuItem;
        public readonly MenuItem RotateLeftMenuItem;
        public readonly MenuItem RotateRightMenuItem;
        public readonly MenuItem ViewCreditsMenuItem;

        public MainMenuController(LanderGameController controller)
        {
            // set the reference
            GameController = controller;

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
        public override void OpenMenu()
        {
            // set calling controller's state to MainMenu
            GameController.GameState = MainMenu;

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
        public override void ProcessMenu(InputHandler inputHandler)
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
                            GameController.StartLevel(1);
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