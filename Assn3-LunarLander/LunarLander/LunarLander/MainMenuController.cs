using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool InControlBinding = false;
        private bool _waitingToReleaseEnter = false;
        private List<Keys> _bindingKeysList;
        public string BindingKeysString;
        public Button RebindingButton;

        // used so we don't read high scores a bunch of times
        private bool _haveProcessedHighScores = false;
        public List<string> HighScoresStringList;

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
            ChangeState(Main);
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
                            ChangeState(HighScores);
                        }
                        else if (selectedItem == CustomizeControlsMenuItem)
                        {
                            ChangeState(Controls);
                        }
                        else if (selectedItem == ViewCreditsMenuItem)
                        {
                            ChangeState(Credits);
                        }
                    }
                    break;
                case Controls:
                    // this state will just "spin" until the user releases enter, then it will
                    // allow for keys to be re-bound
                    if (_waitingToReleaseEnter)
                    {
                        if (!InputHandler.GetDepressedKeys().Contains(Keys.Enter))
                        {
                            _waitingToReleaseEnter = false;
                        }
                    }
                    else if (InControlBinding)
                    {
                        var depressedKeys = InputHandler.GetDepressedKeys();
                        // if we've bound at least one key, but there are no depressed keys, then we're done
                        if (_bindingKeysList.Count != 0 && depressedKeys.Length == 0)
                        {
                            RebindingButton.BoundKeys = _bindingKeysList.ToArray();
                            InControlBinding = false;
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

                            // display of what keys are available for binding
                            foreach (var key in _bindingKeysList)
                            {
                                BindingKeysString += key.ToString() + ", ";
                            }

                            // Strip the last ", " from the string
                            if (BindingKeysString.Length > 0)
                                BindingKeysString = BindingKeysString.Remove(BindingKeysString.Length - 2);
                            else
                            {
                                BindingKeysString = "Press and release any combination of key(s)...";
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
                            ChangeState(Main);
                        }
                        else if (selectedItem == ResetDefaultsMenuItem)
                        {
                            // defaults: thrust -> up arrow, left -> left arrow, right -> right arrow
                            inputHandler.ThrustUpButton.BoundKeys = new[] {Keys.Up};
                            inputHandler.TurnShipLeftButton.BoundKeys = new[] {Keys.Left};
                            inputHandler.TurnShipRightButton.BoundKeys = new[] {Keys.Right};
                        }
                        else
                        {
                            // set the button to change properly
                            if (selectedItem == ThrustMenuItem)
                                RebindingButton = inputHandler.ThrustUpButton;
                            else if (selectedItem == RotateLeftMenuItem)
                                RebindingButton = inputHandler.TurnShipLeftButton;
                            else if (selectedItem == RotateRightMenuItem)
                                RebindingButton = inputHandler.TurnShipRightButton;

                            // flag that we are in control binding
                            InControlBinding = true;

                            // also, flag that we need to release the Enter key
                            _waitingToReleaseEnter = true;

                            // reset our list of keys we're binding
                            _bindingKeysList = new List<Keys>();

                            // set the binding keys string to placeholder message
                            BindingKeysString = "Press and release any combination of key(s)...";
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
                        ChangeState(Main);
                    }
                    break;
                // High Scores and Credits only have one type of input
                case HighScores:
                    if (inputHandler.MenuBackButton.Pressed || inputHandler.MenuConfirmButton.Pressed)
                        ChangeState(Main);
                    // need to read in high scores
                    else
                    {
                        if (!_haveProcessedHighScores)
                        {
                            ReadInHighScores();
                        }
                    }
                    break;
                case Credits:
                    if (inputHandler.MenuBackButton.Pressed || inputHandler.MenuConfirmButton.Pressed)
                        ChangeState(Main);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeState(MenuState state)
        {
            // change the menu state here
            MenuState = state;

            // use this method to select the first item in the menu
            List<MenuItem> items;
            switch (state)
            {
                case Main:
                    items = _mainMenuList;
                    break;
                case Controls:
                    items = _controlsList;
                    break;
                case HighScores:
                    items = _highScoresList;
                    break;
                case Credits:
                    items = _highScoresList;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            SelectFirstItem(items);
        }

        // this function reads in high scores, and stores it in the High Scores string list
        private void ReadInHighScores()
        {
            // TODO write this function
            HighScoresStringList = new List<string>();
            HighScoresStringList.Add("1) 234.564");
            HighScoresStringList.Add("2) 153.949");
            HighScoresStringList.Add("3) 89.123");
            HighScoresStringList.Add("4) 35.203");
            _haveProcessedHighScores = true;
        }
    }
}