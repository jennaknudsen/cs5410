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

        // public int LinesCleared = 0;

        // Scoring:
        // 1 line clear: 40 * (level + 1)
        // 2 line clear: 100 * (level + 1)
        // 3 line clear: 300 * (level + 1)
        // 4+ line clear: 1200 * (level + 1)
        public int Score;
        // add 1 point for each consecutive drop
        // private int _dropScore;

        // total lines cleared / 5
        // normally, tetris is every 10 lines, but I chose every 5 to speed up gameplay
        // public int Level = 0;

        // reference to the current game state
        public GameState GameState;

        // whether we are in Free Fall mode or not
        // private bool _inFreeFallMode;

        // 10 col, 20 row TetrisSquares array
        // where TetrisSquares[0, 0] is bottom left
        //       TetrisSquares[9, 19] is top right
        // public Square[,] TetrisSquares;

        // references to current piece
        // public Piece CurrentPiece;

        // a "bag" of pieces: ensures that we don't get repeats that are too frequent
        // public List<Piece> BagOfPieces;

        // // time since last gravity tick
        // private TimeSpan _timeSinceLastGravityTick;
        // private TimeSpan _timeSinceLastPieceTick;
        // private readonly TimeSpan _pieceTickTimeSpan = TimeSpan.FromMilliseconds(400);

        // references to various objects that this class uses
        public InputHandler InputHandler;
        // public AiController AiController;
        public MainMenuController MainMenuController;
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
            // SoundController.PlayMusic();
        }

        // this ticks every game loop
        public void Update(GameTime gameTime)
        {
            // first, always get user input
            InputHandler.HandleInput();

            // update the SoundController
            SoundController.Update(gameTime);

            // update the ParticleController
            ParticleController.Update(gameTime);

            switch (GameState)
            {
                case Running:
                case AttractMode:

                    // code to actually run the game
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