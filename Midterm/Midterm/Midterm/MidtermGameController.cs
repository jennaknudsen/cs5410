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

        private int _level;

        // reference to the current game state
        public GameState GameState;

        // references to various objects that this class uses
        public InputHandler InputHandler;
        public MainMenuController MainMenuController;
        public PauseMenuController PauseMenuController;
        public LocalStorageManager LocalStorageManager;

        // These controllers need assets, so they are instantiated by the TetrisGame itself
        public ParticleController ParticleController;

        // Timeout for game over
        private readonly TimeSpan _gameOverEndTime = TimeSpan.FromSeconds(5);
        public TimeSpan GameOverTime = TimeSpan.Zero;

        // array holds all bombs
        public Bomb[] Bombs;
        public MouseButton[] BombButtons;

        // timespan of tick
        private readonly TimeSpan _tickTimeSpan = TimeSpan.FromSeconds(1.5);
        private TimeSpan _currentTickTimeSpan;

        // timespan of transition level
        private readonly TimeSpan _levelTransitionTimeSpan = TimeSpan.FromSeconds(3);
        public TimeSpan CurrentLevelTransitionTimeSpan;

        // total game time
        public TimeSpan GameTimeTimeSpan;

        public MidtermGameController()
        {
            // set up all of the needed controllers and handlers
            InputHandler = new InputHandler();
            MainMenuController = new MainMenuController(this);
            PauseMenuController = new PauseMenuController(this);
            LocalStorageManager = new LocalStorageManager();

            // load the overall high scores
            LocalStorageManager.LoadHighScores();

            // need to wait for high scores to be loaded
            while (LocalStorageManager.StoredHighScores == null)
            {
                Thread.Sleep(10);   // Added Sleep so that this thread can "breathe" will Async happens
            }

            // set the high scores list to what we just read in
            MainMenuController.HighScoresIntList = LocalStorageManager.StoredHighScores.HighScores;

            // load the time high scores
            LocalStorageManager.LoadTimeHighScores();

            // need to wait for high scores to be loaded
            while (LocalStorageManager.StoredTimeHighScores == null)
            {
                Thread.Sleep(10);   // Added Sleep so that this thread can "breathe" will Async happens
            }

            // set the high scores list to what we just read in
            MainMenuController.TimeHighScoresDoubleList = LocalStorageManager.StoredTimeHighScores.TimeHighScores;

            // set mouse buttons
            BombButtons = new[]
            {
                InputHandler.bomb1,
                InputHandler.bomb2,
                InputHandler.bomb3,
                InputHandler.bomb4,
                InputHandler.bomb5,
                InputHandler.bomb6,
                InputHandler.bomb7,
                InputHandler.bomb8,
                InputHandler.bomb9,
                InputHandler.bomb10,
                InputHandler.bomb11,
                InputHandler.bomb12,
            };

            // main state is menu
            GameState = MainMenu;
        }

        // this starts the game
        public void StartGame()
        {
            // reset score, board
            Score = 0;

            // set the game state to be Running
            GameState = TransitionLevel;

            // initialize bomb array
            Bombs = new Bomb[12];

            // game starts on level 1
            _level = 3;
            StartLevel(_level);

            // reset the game over time
            GameOverTime = _gameOverEndTime;

            // reset total elapsed game time
            GameTimeTimeSpan = TimeSpan.Zero;
        }

        public void StartLevel(int level)
        {
            PrimeBombs(level);
            _currentTickTimeSpan = TimeSpan.Zero;
            CurrentLevelTransitionTimeSpan = _levelTransitionTimeSpan;
            GameState = TransitionLevel;
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
                    if (InputHandler.PauseGameButton.Pressed)
                    {
                        PauseMenuController.OpenMenu();
                    }

                    // increment the total game time
                    GameTimeTimeSpan += gameTime.ElapsedGameTime;

                    // handle pressing the bombs
                    for (var i = 0; i < 12; i++)
                    {
                        if (BombButtons[i].Pressed)
                        {
                            if (Bombs[i].IsEnabled && !Bombs[i].Defused && !Bombs[i].Exploded)
                            {
                                Bombs[i].Defused = true;
                                Score += Bombs[i].FuseTime;
                            }
                        }
                    }

                    // increment current timespan tick time, see if we need to explode any bombs
                    _currentTickTimeSpan += gameTime.ElapsedGameTime;

                    if (_currentTickTimeSpan > _tickTimeSpan)
                    {
                        _currentTickTimeSpan = TimeSpan.Zero;
                        foreach (var bomb in Bombs)
                        {
                            if (bomb.IsEnabled && !bomb.Defused && !bomb.Exploded)
                            {
                                bomb.FuseTime--;
                                if (bomb.FuseTime == 0)
                                {
                                    bomb.Exploded = true;
                                    // subtract from score
                                    Score -= 3;
                                    if (Score < 0) Score = 0;
                                }
                            }
                        }
                    }

                    var areAllBombsComplete = true;
                    foreach (var bomb in Bombs)
                    {
                        if (bomb.IsEnabled && !bomb.Defused && !bomb.Exploded)
                        {
                            areAllBombsComplete = false;
                        }
                    }

                    // if no bombs were ticked, go to next level
                    if (areAllBombsComplete)
                    {
                        if (_level < 3)
                        {
                            _level++;
                            StartLevel(_level);
                        }
                        else
                        {
                            // TODO save high scores

                            // exit back to main menu
                            GameOverTime = _gameOverEndTime;
                            GameState = GameOver;

                        }
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
                case TransitionLevel:
                    // decrement transition time, if 0 then start the level
                    CurrentLevelTransitionTimeSpan -= gameTime.ElapsedGameTime;
                    if (CurrentLevelTransitionTimeSpan < TimeSpan.Zero)
                    {
                        CurrentLevelTransitionTimeSpan = _levelTransitionTimeSpan;
                        GameState = Running;
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

        public void PrimeBombs(int level)
        {
            var countdownArray = new List<int>
            {
                // TimeSpan.FromSeconds(3),
                // TimeSpan.FromSeconds(3),
                // TimeSpan.FromSeconds(2),
                // TimeSpan.FromSeconds(2),
                // TimeSpan.FromSeconds(1),
                // TimeSpan.FromSeconds(1)
                3,
                3,
                2,
                2,
                1,
                1
            };

            if (level > 1)
            {
                // countdownArray.Add(TimeSpan.FromSeconds(4));
                // countdownArray.Add(TimeSpan.FromSeconds(3));
                // countdownArray.Add(TimeSpan.FromSeconds(2));
                countdownArray.Add(4);
                countdownArray.Add(3);
                countdownArray.Add(2);
            }
            if (level > 2)
            {
                // countdownArray.Add(TimeSpan.FromSeconds(5));
                // countdownArray.Add(TimeSpan.FromSeconds(4));
                // countdownArray.Add(TimeSpan.FromSeconds(3));
                countdownArray.Add(5);
                countdownArray.Add(4);
                countdownArray.Add(3);
            }

            // shuffle this list
            var random = new Random();
            var secondList = new List<int>(countdownArray.OrderBy(a => random.Next()));

            // now, start creating new bombs
            for (var i = 0; i < 12; i++)
            // foreach (var timespan in secondList)
            {
                // 1-6: always on
                if (i < 6)
                    Bombs[i] = new Bomb(secondList[i], true);
                // 7-9: on if level is greater than 1
                else if (i < 9 && level > 1)
                    Bombs[i] = new Bomb(secondList[i], true);
                // 10-12: on if level is 3
                else if (level > 2)
                    Bombs[i] = new Bomb(secondList[i], true);
                // otherwise, not enabled
                else
                    Bombs[i] = new Bomb(99, false);
            }
        }
    }
}