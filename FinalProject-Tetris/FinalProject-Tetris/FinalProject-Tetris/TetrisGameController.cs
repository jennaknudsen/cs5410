using System;
using Microsoft.Xna.Framework;
using static FinalProject_Tetris.GameState;

namespace FinalProject_Tetris
{
    public class TetrisGameController
    {
        public const int BoardSize = 30;
        public const int PieceSize = 1;

        public int LinesCleared = 0;

        // Scoring:
        // 1 line clear: 40 * (level + 1)
        // 2 line clear: 100 * (level + 1)
        // 3 line clear: 300 * (level + 1)
        // 4 line clear: 1200 * (level + 1)
        public int Score = 0;

        // total lines cleared / 10
        public int Level = 0;

        // reference to the current game state
        public GameState GameState;

        // 10 col, 20 row TetrisSquares array
        // where TetrisSquares[0, 0] is bottom left
        //       TetrisSquares[9, 19] is top right
        public Square[,] TetrisSquares = new Square[10, 20];

        // references to current and next piece
        public Piece CurrentPiece;
        public Piece NextPiece;

        // references to various objects that this class uses
        public InputHandler InputHandler = new InputHandler();
        public ParticleController ParticleController = new ParticleController();
        public SoundController SoundController = new SoundController();
        public AiController AiController = new AiController();
        public MainMenuController MainMenuController = new MainMenuController();

        // this gets the TimeSpan between gravity ticks
        private static TimeSpan GetGravityTimeSpan(int level)
        {
            int frames;

            // gets the total amount of frames that the gravity should be active for
            // assuming that game runs at 60 fps
            switch (level)
            {
                case 0:
                    frames = 48;
                    break;
                case 1:
                    frames = 43;
                    break;
                case 2:
                    frames = 38;
                    break;
                case 3:
                    frames = 33;
                    break;
                case 4:
                    frames = 28;
                    break;
                case 5:
                    frames = 23;
                    break;
                case 6:
                    frames = 18;
                    break;
                case 7:
                    frames = 13;
                    break;
                case 8:
                    frames = 8;
                    break;
                case 9:
                    frames = 6;
                    break;
                default:
                {
                    if (level >= 10 && level <= 12)
                        frames = 5;
                    else if (level >= 13 && level <= 15)
                        frames = 4;
                    else if (level >= 16 && level <= 18)
                        frames = 3;
                    else if (level >= 19 && level <= 28)
                        frames = 2;
                    else
                        frames = 1;
                    break;
                }
            }

            // convert this to fraction of seconds
            var correctMillis = 60d / frames;

            // return this fraction of seconds as a TimeSpan
            return TimeSpan.FromMilliseconds(correctMillis);
        }

        // this starts the game
        public void StartGame()
        {
            // reset score, board
            LinesCleared = 0;
            Score = 0;
            Level = 0;
            TetrisSquares = new Square[10, 20];
            CurrentPiece = null;
            NextPiece = null;

            // set the game state to be Running
            GameState = Running;

        }

        // this ticks every game loop
        public void Update(GameTime gameTime)
        {

        }
    }
}