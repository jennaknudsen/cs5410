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
        public MainMenuController MainMenuController;
        public PauseMenuController PauseMenuController;
        public LocalStorageManager LocalStorageManager;

        // These controllers need assets, so they are instantiated by the TetrisGame itself
        public ParticleController ParticleController;

        // TODO delete this
        private bool _firedOnce = false;

        // Timeout for game over
        private readonly TimeSpan _gameOverEndTime = TimeSpan.FromSeconds(5);
        public TimeSpan GameOverTime = TimeSpan.Zero;

        public Bomb[] Bombs;

        public MidtermGameController()
        {
            // set up all of the needed controllers and handlers
            InputHandler = new InputHandler();
            MainMenuController = new MainMenuController(this);
            PauseMenuController = new PauseMenuController(this);
            LocalStorageManager = new LocalStorageManager();

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
            GameState = Running;

            // initialize bomb array
            Bombs = new Bomb[12];

            // reset the game over time
            GameOverTime = _gameOverEndTime;
        }

        // this ticks every game loop
        public void Update(GameTime gameTime)
        {
            // first, always get user input
            InputHandler.HandleInput();

            // update the ParticleController (unless game is paused)
            if (GameState != Paused) ParticleController.Update(gameTime);

            switch (GameState)
            {
                case Running:
                    // code to actually run the game


                    if (InputHandler.bomb1.Pressed)
                    {

                    }
                    if (InputHandler.PauseGameButton.Pressed)
                    {
                        PauseMenuController.OpenMenu();
                    }

                    // demonstration of particle effects
                    // if (gameTime.TotalGameTime >= TimeSpan.FromSeconds(6))
                    // {
                    //     ParticleController.StopGenericAngleEmitter();
                    // }
                    // else if (gameTime.TotalGameTime >= TimeSpan.FromSeconds(3))
                    // {
                    //     if (!_firedOnce)
                    //     {
                    //         ParticleController.AddTex1TimedAngleEmitter(30, 30, MathHelper.TwoPi);
                    //         _firedOnce = true;
                    //     }
                    //
                    //     ParticleController.FireGenericAngleEmitter(50, 50, MathHelper.PiOver2);
                    // }
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

        public void ClickBomb(Bomb bomb)
        {
            if (bomb.IsEnabled)
            {
                // TODO change this
                Console.WriteLine("Bomb clicked!");
            }
        }
    }
}