using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Midterm.InputHandling;
using Midterm.LocalStorage;
using Midterm.Menuing;
using static Midterm.GameState;

namespace Midterm
{
    public class MidtermGameController
    {
        public const int BoardSize = 100;

        public int Score;

        // reference to the current game state
        public GameState GameState;

        // references to various objects that this class uses
        public InputHandler InputHandler;
        // public AiController AiController;
        public MainMenuController MainMenuController;
        public PauseMenuController PauseMenuController;
        public LocalStorageManager LocalStorageManager;

        // These controllers need assets, so they are instantiated by the TetrisGame itself
        public SoundController SoundController;
        public ParticleController ParticleController;

        // // Counter for attract mode
        // private readonly TimeSpan _attractModeThreshold = TimeSpan.FromSeconds(10);
        // private TimeSpan _inactiveTime = TimeSpan.Zero;

        // Timeout for game over
        private readonly TimeSpan _gameOverEndTime = TimeSpan.FromSeconds(5);
        public TimeSpan GameOverTime = TimeSpan.Zero;

        public MidtermGameController()
        {
            // set up all of the needed controllers and handlers
            InputHandler = new InputHandler();
            // AiController = new AiController(this);
            MainMenuController = new MainMenuController(this);
            PauseMenuController = new PauseMenuController(this);
            LocalStorageManager = new LocalStorageManager();

            // in constructor, read in the control scheme and high scores initially
            LocalStorageManager.LoadControlScheme();

            // need to wait for a control scheme to be loaded
            while (LocalStorageManager.StoredControlScheme == null)
            {
                Thread.Sleep(10);   // Added Sleep so that this thread can "breathe" will Async happens
            }

            // set the correct control scheme after load
            InputHandler.MovePieceLeftButton.BoundKeys = LocalStorageManager.StoredControlScheme.LeftKeys;
            InputHandler.MovePieceRightButton.BoundKeys = LocalStorageManager.StoredControlScheme.RightKeys;
            InputHandler.SoftDropButton.BoundKeys = LocalStorageManager.StoredControlScheme.DownKeys;
            InputHandler.HardDropButton.BoundKeys = LocalStorageManager.StoredControlScheme.UpKeys;
            InputHandler.RotateClockwiseButton.BoundKeys = LocalStorageManager.StoredControlScheme.RotateClockwiseKeys;
            InputHandler.RotateCounterClockwiseButton.BoundKeys = LocalStorageManager.StoredControlScheme.RotateCounterClockwiseKeys;

            LocalStorageManager.LoadHighScores();

            // need to wait for high scores to be loaded
            while (LocalStorageManager.StoredHighScores == null)
            {
                Thread.Sleep(10);   // Added Sleep so that this thread can "breathe" will Async happens
            }

            // set the high scores list to what we just read in
            MainMenuController.HighScoresIntList = LocalStorageManager.StoredHighScores.HighScores;

            // main state is menu
            GameState = MainMenu;
        }

        // this starts the game
        public void StartGame(bool attractMode)
        {
            // reset score, board
            Score = 0;

            // set the game state to be Running
            GameState = attractMode? AttractMode: Running;

            // reset the game over time
            GameOverTime = _gameOverEndTime;

            // play the background music
            SoundController.PlayMusic();
        }

        // this ticks every game loop
        public void Update(GameTime gameTime)
        {
            // first, always get user input
            InputHandler.HandleInput();

            // update the SoundController
            SoundController.Update(gameTime);

            // update the ParticleController (unless game is paused)
            if (GameState != Paused) ParticleController.Update(gameTime);

            switch (GameState)
            {
                case Running:
                case AttractMode:
                    // code to actually run the game

                    if (InputHandler.PauseGameButton.Pressed)
                    {
                        PauseMenuController.OpenMenu();
                    }

                    break;
                case Paused:
                    PauseMenuController.ProcessMenu(InputHandler);
                    break;
                case MainMenu:
                    // check whether input was pressed
                    // if (InputHandler.KeyPressed || InputHandler.MouseMoved ||
                    //     MainMenuController.MenuState != MenuState.Main)
                    // {
                    //     _inactiveTime = TimeSpan.Zero;
                    // }
                    // else
                    // {
                    //     _inactiveTime += gameTime.ElapsedGameTime;
                    // }
                    ParticleController.ClearAllParticles();
                    MainMenuController.ProcessMenu(InputHandler);
                    //
                    // // if we exceed the threshold for inactivity, then start attract moe
                    // if (_inactiveTime >= _attractModeThreshold)
                    // {
                    //     _inactiveTime = TimeSpan.Zero;
                    //     StartGame(true);
                    // }
                    break;
                case GameOver:
                    GameOverTime -= gameTime.ElapsedGameTime;
                    if (GameOverTime < TimeSpan.Zero)
                        GameState = MainMenu;
                    break;
            }
        }
    }
}