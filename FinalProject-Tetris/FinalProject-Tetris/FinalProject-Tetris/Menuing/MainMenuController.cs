using System.Collections.Generic;
using FinalProject_Tetris.InputHandling;
using FinalProject_Tetris.Menuing;
using Microsoft.Xna.Framework.Input;
using static FinalProject_Tetris.Menuing.MenuState;

namespace FinalProject_Tetris
{
    public class MainMenuController : MenuController
    {
        // holds the state of this menu
        public MenuState MenuState;

        // holds a list of all high scores
        public List<int> HighScoresIntList;

        // used for control binding
        public bool InControlBinding = false;
        private bool _waitingToReleaseEnter = false;
        private List<Keys> _bindingKeysList;
        public string BindingKeysString;
        public KeyboardButton RebindingButton;

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
                case HighScores:
                case Credits:
                    if (inputHandler.BackToMainButton.Pressed)
                        MenuState = Main;
                    break;
                case Controls:
                    // if we're currently binding controls, don't activate any other buttons
                    if (InControlBinding)
                    {
                        // get the keys that are currently depressed
                        var depressedKeys = InputHandler.GetDepressedKeys();

                        // if we've bound at least one key, but there are no depressed keys, then we're done
                        if (_bindingKeysList.Count != 0 && depressedKeys.Length == 0)
                        {
                            RebindingButton.BoundKeys = _bindingKeysList.ToArray();

                            // need to save this to persistent storage
                            var handler = GameController.InputHandler;

                            var leftKeys = handler.MovePieceLeftButton.BoundKeys;
                            var rightKeys = handler.MovePieceRightButton.BoundKeys;
                            var upKeys = handler.HardDropButton.BoundKeys;
                            var downKeys = handler.SoftDropButton.BoundKeys;
                            var rotateClockwiseKeys = handler.RotateClockwiseButton.BoundKeys;
                            var rotateCounterClockwiseKeys = handler.RotateCounterClockwiseButton.BoundKeys;

                            // save keys to storage
                            GameController.LocalStorageManager.SaveControlScheme(
                                leftKeys,
                                rightKeys,
                                upKeys,
                                downKeys,
                                rotateClockwiseKeys,
                                rotateCounterClockwiseKeys
                            );

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
                    // binding controls for each of the six buttons
                    else if (inputHandler.LeftControlButton.Pressed)
                        StartRebindingButton(inputHandler.MovePieceLeftButton);
                    else if (inputHandler.RightControlButton.Pressed)
                        StartRebindingButton(inputHandler.MovePieceRightButton);
                    else if (inputHandler.UpControlButton.Pressed)
                        StartRebindingButton(inputHandler.HardDropButton);
                    else if (inputHandler.DownControlButton.Pressed)
                        StartRebindingButton(inputHandler.SoftDropButton);
                    else if (inputHandler.ClockwiseControlButton.Pressed)
                        StartRebindingButton(inputHandler.RotateClockwiseButton);
                    else if (inputHandler.CounterClockwiseControlButton.Pressed)
                        StartRebindingButton(inputHandler.RotateCounterClockwiseButton);
                    // reset to defaults
                    else if (inputHandler.ResetToDefaultsButton.Pressed)
                    {
                        var leftKeys = new[] {Keys.Left};
                        var rightKeys = new[] {Keys.Right};
                        var upKeys = new[] {Keys.Up};
                        var downKeys = new[] {Keys.Down};
                        var rotateClockwiseKeys = new[] {Keys.PageUp, Keys.E};
                        var rotateCounterClockwiseKeys = new[] {Keys.Home, Keys.Q};

                        inputHandler.MovePieceLeftButton.BoundKeys = leftKeys;
                        inputHandler.MovePieceRightButton.BoundKeys = rightKeys;
                        inputHandler.HardDropButton.BoundKeys = upKeys;
                        inputHandler.SoftDropButton.BoundKeys = downKeys;
                        inputHandler.RotateClockwiseButton.BoundKeys = rotateClockwiseKeys;
                        inputHandler.RotateCounterClockwiseButton.BoundKeys = rotateCounterClockwiseKeys;

                        // save this as default scheme
                        GameController.LocalStorageManager.SaveControlScheme(leftKeys, rightKeys, upKeys, downKeys,
                            rotateClockwiseKeys, rotateCounterClockwiseKeys);
                    }
                    // main menu
                    else if (inputHandler.BackToMainButton.Pressed)
                    {
                        MenuState = Main;
                    }
                    break;
            }
        }

        private void StartRebindingButton(KeyboardButton keyboardButton)
        {
            // set the button we want to rebind
            RebindingButton = keyboardButton;
            // flag that we are in control binding
            InControlBinding = true;
            // reset list of keys we're binding
            _bindingKeysList = new List<Keys>();
            // set message
            BindingKeysString = "Press and release any combination of key(s)...";
        }
    }
}