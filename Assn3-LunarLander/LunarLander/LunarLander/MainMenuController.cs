using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static LunarLander.GameState;
using static LunarLander.MenuState;

namespace LunarLander
{
    public class MainMenuController : MenuController
    {
        // holds the current menu state information
        public MenuState MenuState;

        // lists of menus
        private readonly List<MenuItem> _mainMenuList;
        private readonly List<MenuItem> _highScoresList;
        private readonly List<MenuItem> _controlsList;
        private readonly List<MenuItem> _creditsList;

        // all menu items
        public readonly MenuItem NewGameMenuItem;
        public readonly MenuItem HighScoresMenuItem;
        public readonly MenuItem BackToMainMenuItem;
        public readonly MenuItem CustomizeControlsMenuItem;
        public readonly MenuItem ThrustMenuItem;
        public readonly MenuItem RotateLeftMenuItem;
        public readonly MenuItem RotateRightMenuItem;
        public readonly MenuItem ResetDefaultsMenuItem;
        public readonly MenuItem ViewCreditsMenuItem;

        // used for control binding
        private bool _inControlBinding = false;
        private List<Keys> _bindingKeysList;
        public string BindingKeysString;
        private Button _rebindingButton;

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
            ResetDefaultsMenuItem = new MenuItem("Reset to Defaults");
            ViewCreditsMenuItem = new MenuItem("High Scores");

            // populate each menu item list
            _mainMenuList = new List<MenuItem>
            {
                NewGameMenuItem,
                HighScoresMenuItem,
                CustomizeControlsMenuItem,
                ViewCreditsMenuItem
            };
            _highScoresList = new List<MenuItem>
            {
                BackToMainMenuItem
            };
            _controlsList = new List<MenuItem>
            {
                ThrustMenuItem,
                RotateLeftMenuItem,
                RotateRightMenuItem,
                ResetDefaultsMenuItem,
                BackToMainMenuItem
            };
            _creditsList = new List<MenuItem>
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
            foreach (var item in _mainMenuList)
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
         * |      |--- Reset to Defaults
         * |      └--- Back to Main
         * |
         * |----- View Credits
         *        └--- Back to Main
         */
        public override void ProcessMenu(InputHandler inputHandler)
        {
            switch (MenuState)
            {
                case Main:
                    // on up/down press, just select the next / prev item
                    if (inputHandler.MenuUpButton.Pressed)
                    {
                        SelectPreviousItem(_mainMenuList);
                    }
                    else if (inputHandler.MenuDownButton.Pressed)
                    {
                        SelectNextItem(_mainMenuList);
                    }
                    // on confirm press, we need to determine which action to do
                    else if (inputHandler.MenuConfirmButton.Pressed)
                    {
                        // get selected index in Main Menu
                        var selectedIndex = GetSelectedIndex(_mainMenuList);
                        var selectedItem = _mainMenuList[selectedIndex];

                        // switch state based on what item we selected
                        if (selectedItem == NewGameMenuItem)
                        {
                            GameController.StartLevel(1);
                        }
                        else if (selectedItem == HighScoresMenuItem)
                        {
                            MenuState = HighScores;
                        }
                        else if (selectedItem == CustomizeControlsMenuItem)
                        {
                            MenuState = Controls;
                        }
                        else if (selectedItem == ViewCreditsMenuItem)
                        {
                            MenuState = Credits;
                        }
                    }
                    break;
                case Controls:
                    if (_inControlBinding)
                    {
                        var depressedKeys = InputHandler.GetDepressedKeys();
                        // if we've bound at least one key, but there are no depressed keys, then we're done
                        if (_bindingKeysList.Count != 0 && depressedKeys.Length == 0)
                        {
                            _rebindingButton.BoundKeys = _bindingKeysList.ToArray();
                            _inControlBinding = false;
                        }
                        else
                        {
                            // if we're still binding keys, we need to get all depressed keys, and for each one
                            // we haven't accounted for, add it as a bind
                            foreach (var key in depressedKeys)
                            {
                                if (!_bindingKeysList.Contains(key))
                                {
                                    _bindingKeysList.Add(key);
                                }
                            }

                            // set the string to show in the Controls menu
                            BindingKeysString = "";

                            foreach (var key in _bindingKeysList)
                            {
                                BindingKeysString += key.ToString();
                                Console.WriteLine(BindingKeysString);
                            }
                        }
                    }
                    // if we aren't currently binding, check if the enter button is pressed
                    else if (inputHandler.MenuConfirmButton.Pressed)
                    {
                        // get which button is pressed
                        var selectedIndex = GetSelectedIndex(_controlsList);
                        var selectedItem = _controlsList[selectedIndex];

                        // easy ones first
                        if (selectedItem == BackToMainMenuItem)
                        {
                            MenuState = Main;
                        }
                        else if (selectedItem == ResetDefaultsMenuItem)
                        {
                            // defaults: thrust -> up arrow, left -> left arrow, right -> right arrow
                            inputHandler.ThrustUpButton.BoundKeys = new[] {Keys.Up};
                            inputHandler.TurnShipLeftButton.BoundKeys = new[] {Keys.Left};
                            inputHandler.TurnShipRightButton.BoundKeys = new[] {Keys.Right};

                            // reselect "Back to Main" after resetting defaults (for convenience)
                            ResetDefaultsMenuItem.Selected = false;
                            BackToMainMenuItem.Selected = true;
                        }
                        else
                        {
                            // set the button to change properly
                            if (selectedItem == ThrustMenuItem)
                                _rebindingButton = inputHandler.ThrustUpButton;
                            else if (selectedItem == RotateLeftMenuItem)
                                _rebindingButton = inputHandler.TurnShipLeftButton;
                            else if (selectedItem == RotateRightMenuItem)
                                _rebindingButton = inputHandler.TurnShipRightButton;

                            // flag that we are in control binding
                            _inControlBinding = true;

                            // reset our list of keys we're binding
                            _bindingKeysList = new List<Keys>();
                        }
                    }
                    else if (inputHandler.MenuUpButton.Pressed)
                    {
                        SelectPreviousItem(_controlsList);
                    }
                    else if (inputHandler.MenuDownButton.Pressed)
                    {
                        SelectNextItem(_controlsList);
                    }
                    else if (inputHandler.MenuBackButton.Pressed)
                    {
                        MenuState = Main;
                    }
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