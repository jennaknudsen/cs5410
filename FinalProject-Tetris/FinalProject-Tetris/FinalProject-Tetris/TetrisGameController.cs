using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using FinalProject_Tetris.InputHandling;
using static FinalProject_Tetris.GameState;
using static FinalProject_Tetris.Square.PieceColor;

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

        // time since last gravity tick
        private TimeSpan _timeSinceLastTick;

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
            var correctSeconds = frames / 60d;

            // return this fraction of seconds as a TimeSpan
            return TimeSpan.FromSeconds(correctSeconds);
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
            _timeSinceLastTick = TimeSpan.Zero;

            // set the game state to be Running
            GameState = Running;
        }

        // this ticks every game loop
        public void Update(GameTime gameTime)
        {
            // first, always get user input
            InputHandler.HandleInput();

            switch (GameState)
            {
                case Running:
                case AttractMode:
                    if (GameState == Running)
                    {
                        // get user input for piece
                        if (InputHandler.MovePieceLeftButton.Pressed)
                        {
                            MovePieceLeft();
                        }
                    }
                    break;
            }
            // reset time since last tick since if we exceed gravity time span
            _timeSinceLastTick += gameTime.ElapsedGameTime;
            if (_timeSinceLastTick >= GetGravityTimeSpan(15))
            {
                _timeSinceLastTick = TimeSpan.Zero;
            }
        }

        private void MovePieceLeft()
        {
            // protect against nulls
            if (CurrentPiece == null) return;

            // make a new list with all of the positions translated to the left by 1
            var newPosition = new List<(int x, int y)>();
            foreach (var square in CurrentPiece.Squares)
            {
                newPosition.Add((square.PieceLocation.x - 1, square.PieceLocation.y));
            }

            // only move if it's a valid position
            if (CheckValidPiecePosition(newPosition))
            {
                for (var i = 0; i < 4; i++)
                {
                    CurrentPiece.Squares[i].PieceLocation = newPosition[i];
                }
            }
        }

        private void MovePieceRight()
        {

        }

        private void SoftDropPiece()
        {

        }

        private void HardDropPiece()
        {

        }

        private void RotatePieceCounterClockwise()
        {

        }

        private void RotatePieceClockwise()
        {

        }

        // given a List of piece coordinates for start and another for end position, check validity of the move
        // assume old position is valid
        private bool CheckValidPiecePosition(IEnumerable<(int x, int y)> newPosition)
        {
            // LINQ expression to select piece locations
            var oldPosition = CurrentPiece.Squares.Select(square => square.PieceLocation).ToList();

            foreach (var (x, y) in newPosition)
            {
                if (x < 0 || x > 9 || y < 0 || y > 19) return false;
                if (TetrisSquares[x, y] == null) continue;
                if (oldPosition.Contains((x, y))) continue;
                return false;
            }
            return true;
        }
    }
}