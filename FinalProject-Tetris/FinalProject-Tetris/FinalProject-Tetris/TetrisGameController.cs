using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using FinalProject_Tetris.InputHandling;
using FinalProject_Tetris.LocalStorage;
using FinalProject_Tetris.Menuing;
using static FinalProject_Tetris.GameState;
using static FinalProject_Tetris.Piece.PieceType;
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
        // 4+ line clear: 1200 * (level + 1)
        public int Score;
        // add 1 point for each consecutive drop
        private int _dropScore;

        // total lines cleared / 5
        // normally, tetris is every 10 lines, but I chose every 5 to speed up gameplay
        public int Level = 0;

        // reference to the current game state
        public GameState GameState;

        // whether we are in Free Fall mode or not
        private bool _inFreeFallMode;

        // 10 col, 20 row TetrisSquares array
        // where TetrisSquares[0, 0] is bottom left
        //       TetrisSquares[9, 19] is top right
        public Square[,] TetrisSquares;

        // references to current piece
        public Piece CurrentPiece;

        // a "bag" of pieces: ensures that we don't get repeats that are too frequent
        public List<Piece> BagOfPieces;

        // time since last gravity tick
        private TimeSpan _timeSinceLastGravityTick;
        private TimeSpan _timeSinceLastPieceTick;
        private readonly TimeSpan _pieceTickTimeSpan = TimeSpan.FromMilliseconds(400);

        // references to various objects that this class uses
        public InputHandler InputHandler;
        public AiController AiController;
        public MainMenuController MainMenuController;
        public LocalStorageManager LocalStorageManager;

        // These controllers need assets, so they are instantiated by the TetrisGame itself
        public SoundController SoundController;
        public ParticleController ParticleController;

        // Counter for attract mode
        private readonly TimeSpan _attractModeThreshold = TimeSpan.FromSeconds(10);
        private TimeSpan _inactiveTime = TimeSpan.Zero;

        // Timeout for game over
        private readonly TimeSpan _gameOverEndTime = TimeSpan.FromSeconds(5);
        public TimeSpan GameOverTime = TimeSpan.Zero;

        public TetrisGameController()
        {
            // set up all of the needed controllers and handlers
            InputHandler = new InputHandler();
            AiController = new AiController(this);
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
            LinesCleared = 0;
            Score = 0;
            _dropScore = 0;
            Level = 0;
            TetrisSquares = new Square[10, 20];

            // make new bag of pieces and pop first piece
            BagOfPieces = new List<Piece>();
            GenerateBag();
            PopBag();

            // reset gravity tick
            _timeSinceLastGravityTick = TimeSpan.Zero;
            _timeSinceLastPieceTick = TimeSpan.Zero;

            // set the game state to be Running
            GameState = attractMode? AttractMode: Running;

            // we aren't in free fall mode
            _inFreeFallMode = false;

            // reset the game over time
            GameOverTime = _gameOverEndTime;

            // play the background music
            SoundController.PlayMusic();
        }

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
                    if (!_inFreeFallMode)
                    {
                        if (GameState == Running)
                        {
                            // get user input for piece
                            if (InputHandler.MovePieceLeftButton.Pressed)
                            {
                                MovePieceLeft();
                            }
                            else if (InputHandler.MovePieceRightButton.Pressed)
                            {
                                MovePieceRight();
                            }
                            else if (InputHandler.SoftDropButton.Pressed)
                            {
                                SoftDropPiece(true);
                            }
                            else if (InputHandler.HardDropButton.Pressed)
                            {
                                HardDropPiece();
                            }
                            else if (InputHandler.RotateCounterClockwiseButton.Pressed)
                            {
                                RotatePiece(true);
                            }
                            else if (InputHandler.RotateClockwiseButton.Pressed)
                            {
                                RotatePiece(false);
                            }
                        }
                        else if (GameState == AttractMode)
                        {
                            // in attract mode, pressing any key or moving the mouse resets to main menu
                            if (InputHandler.KeyPressed || InputHandler.MouseMoved)
                            {
                                GameState = MainMenu;
                                SoundController.StopMusic();
                                return;
                            }
                            AiController.Update(gameTime);
                        }

                        // check for a gravity update
                        _timeSinceLastGravityTick += gameTime.ElapsedGameTime;
                        _timeSinceLastPieceTick += gameTime.ElapsedGameTime;

                        if (_timeSinceLastGravityTick >= GetGravityTimeSpan(Level))
                        {
                            // on gravity update, reset time to 0
                            _timeSinceLastGravityTick = TimeSpan.Zero;

                            // try to drop the piece, if we can't then we have a collision
                            if (CurrentPiece != null && !SoftDropPiece(false))
                            {
                                // increment score by drop score
                                Score += _dropScore;
                                _dropScore = 0;

                                // emit particles from this point
                                var coordinatesList = new List<(float x, float y)>();
                                var locationsList = CurrentPiece.Squares.Select(
                                    square => square.PieceLocation).ToList();

                                var color = CurrentPiece.Squares[0].Color;

                                foreach (var (x, y) in locationsList)
                                {
                                    // right emitter
                                    if (!locationsList.Contains((x + 1, y)))
                                    {
                                        var (boardX, boardY) = TetrisGame.GetBoardLocationFromPieceLocation((x, y));
                                        ParticleController.AddPieceEmitter(
                                            boardX + 1, boardY + 0.5f, 0, color, gameTime);
                                    }

                                    // left emitter
                                    if (!locationsList.Contains((x - 1, y)))
                                    {
                                        var (boardX, boardY) = TetrisGame.GetBoardLocationFromPieceLocation((x, y));
                                        ParticleController.AddPieceEmitter(
                                            boardX, boardY + 0.5f, MathHelper.Pi, color, gameTime);
                                    }

                                    // top emitter
                                    if (!locationsList.Contains((x, y + 1)))
                                    {
                                        var (boardX, boardY) = TetrisGame.GetBoardLocationFromPieceLocation((x, y));
                                        ParticleController.AddPieceEmitter(boardX + 0.5f, boardY + 1,
                                            MathHelper.PiOver2, color, gameTime);
                                    }

                                    // bottom emitter
                                    if (!locationsList.Contains((x, y - 1)))
                                    {
                                        var (boardX, boardY) = TetrisGame.GetBoardLocationFromPieceLocation((x, y));
                                        ParticleController.AddPieceEmitter(boardX + 0.5f, boardY,
                                            3 * MathHelper.PiOver2, color, gameTime);
                                    }
                                }

                                // check for line clears, if not then we just have a piece drop
                                if (!ClearLines(gameTime))
                                {
                                    SoundController.PlayBlockPlace();
                                }

                                Console.WriteLine("Score: " + Score);

                                // clear the current piece
                                CurrentPiece = null;

                                // reset "best move" for AI
                                if (GameState == AttractMode)
                                    AiController.NextMove = null;

                                // on a collision, reset time since last piece tick
                                _timeSinceLastPieceTick = TimeSpan.Zero;
                            }
                        }

                        // check for new piece
                        if (_timeSinceLastPieceTick >= _pieceTickTimeSpan)
                        {
                            _timeSinceLastPieceTick = TimeSpan.Zero;

                            // get a new piece if we need
                            if (CurrentPiece == null)
                            {
                                GetNewPiece();
                            }
                        }
                    }
                    // free fall mode: we need to calculate where pieces will drop
                    else
                    {
                        // check for a piece tick update
                        _timeSinceLastPieceTick += gameTime.ElapsedGameTime;

                        if (_timeSinceLastPieceTick >= _pieceTickTimeSpan)
                        {
                            _timeSinceLastPieceTick = TimeSpan.Zero;

                            // use these to exit free fall
                            var canExitFreeFall = false;
                            var clearedLines = ClearLines(gameTime);
                            var droppedAny = false;

                            // do this as many times as necessary
                            var allPiecesDropped = false;
                            while (!allPiecesDropped)
                            {
                                // we need to move all pieces down to lowest level, using sticky gravity

                                // reset each square group
                                var numGroups = 0;

                                foreach (var square in TetrisSquares)
                                {
                                    if (square != null)
                                    {
                                        square.SquareGroup = 0;
                                    }
                                }

                                // mark all squares as unvisited for now
                                var visitedSquares = new bool[10, 20];
                                for (var i = 0; i < 10; i++)
                                for (var j = 0; j < 20; j++)
                                    visitedSquares[i, j] = false;

                                // local function to traverse squares and assign groups to them
                                void TraverseSquares((int x, int y) position, int group)
                                {
                                    // only traverse if not visited and not null
                                    if (position.x < 0 || position.x > 9) return;
                                    if (position.y < 0 || position.y > 19) return;
                                    if (visitedSquares[position.x, position.y]) return;
                                    if (TetrisSquares[position.x, position.y] == null) return;

                                    visitedSquares[position.x, position.y] = true;
                                    TetrisSquares[position.x, position.y].SquareGroup = group;

                                    // recur on all neighbors
                                    var newCoords = new List<(int x, int y)>
                                    {
                                        (position.x + 1, position.y),
                                        (position.x - 1, position.y),
                                        (position.x, position.y + 1),
                                        (position.x, position.y - 1)
                                    };

                                    foreach (var coord in newCoords)
                                    {
                                        TraverseSquares(coord, group);
                                    }
                                }

                                // now, iterate through each square by row and assign groups
                                for (var i = 0; i < 10; i++)
                                {
                                    for (var j = 0; j < 20; j++)
                                    {
                                        if (TetrisSquares[i, j] != null && !visitedSquares[i, j])
                                        {
                                            TraverseSquares((i, j), numGroups);
                                            numGroups++;
                                        }
                                    }
                                }

                                // if there are no groups, then all pieces have been dropped
                                if (numGroups == 0)
                                {
                                    allPiecesDropped = true;
                                }
                                else
                                {
                                    var droppedAGroup = false;
                                    for (var group = 0; group < numGroups; group++)
                                    {
                                        // for each group, find the max squares we can drop down
                                        // only need to check the bottom row square for each col
                                        var maxSquaresToFall = 100;
                                        for (int col = 0; col < 10; col++)
                                        {
                                            var emptySpaces = 0;
                                            for (var row = 0; row < 20; row++)
                                            {
                                                // only checking the bottom element in this col
                                                if (TetrisSquares[col, row] == null)
                                                {
                                                    emptySpaces++;
                                                    continue;
                                                }
                                                else if (TetrisSquares[col, row].SquareGroup != group)
                                                {
                                                    emptySpaces = 0;
                                                    continue;
                                                }
                                                else
                                                {
                                                    if (emptySpaces < maxSquaresToFall)
                                                    {
                                                        maxSquaresToFall = emptySpaces;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        // drop all squares in this group by this max (if it's not 0)
                                        if (maxSquaresToFall > 0)
                                        {
                                            for (int col = 0; col < 10; col++)
                                            {
                                                for (var row = 0; row < 20 - maxSquaresToFall; row++)
                                                {
                                                    if (TetrisSquares[col, row + maxSquaresToFall] != null &&
                                                        TetrisSquares[col, row + maxSquaresToFall].SquareGroup == group)
                                                    {
                                                        // update both the reference in the array AND the piece location
                                                        TetrisSquares[col, row] =
                                                            TetrisSquares[col, row + maxSquaresToFall];
                                                        TetrisSquares[col, row].PieceLocation = (col, row);
                                                        TetrisSquares[col, row + maxSquaresToFall] = null;
                                                        droppedAny = true;
                                                    }
                                                }
                                            }

                                            droppedAGroup = true;
                                        }
                                    }

                                    // all pieces have been dropped if we did not drop a group
                                    allPiecesDropped = !droppedAGroup;
                                }
                            }

                            // we can only exit free fall if we didn't drop any and we didn't clear any lines
                            canExitFreeFall = !(droppedAny || clearedLines);

                            if (canExitFreeFall)
                            {
                                _inFreeFallMode = false;
                                // get new piece at end of free fall (this will smoothen up gameplay)
                                GetNewPiece();
                            }
                        }
                    }
                    break;
                case MainMenu:
                    // check whether input was pressed
                    if (InputHandler.KeyPressed || InputHandler.MouseMoved ||
                        MainMenuController.MenuState != MenuState.Main)
                    {
                        _inactiveTime = TimeSpan.Zero;
                    }
                    else
                    {
                        _inactiveTime += gameTime.ElapsedGameTime;
                    }
                    ParticleController.ClearAllParticles();
                    MainMenuController.ProcessMenu(InputHandler);

                    // if we exceed the threshold for inactivity, then start attract moe
                    if (_inactiveTime >= _attractModeThreshold)
                    {
                        _inactiveTime = TimeSpan.Zero;
                        StartGame(true);
                    }
                    break;
                case GameOver:
                    GameOverTime -= gameTime.ElapsedGameTime;
                    if (GameOverTime < TimeSpan.Zero)
                        GameState = MainMenu;
                    break;
            }
        }

        // move piece left one square
        public bool MovePieceLeft()
        {
            // protect against nulls
            if (CurrentPiece == null) return false;

            // make a list with initial positions
            var oldPosition = new List<(int x, int y)>();
            foreach (var square in CurrentPiece.Squares)
            {
                oldPosition.Add(square.PieceLocation);
            }

            // make a new list with all of the positions translated to the left by 1
            var newPosition = new List<(int x, int y)>();
            foreach (var square in CurrentPiece.Squares)
            {
                newPosition.Add((square.PieceLocation.x - 1, square.PieceLocation.y));
            }

            // only move if it's a valid position
            if (CheckValidPiecePosition(newPosition))
            {
                FinalizePieceMove(oldPosition, newPosition);
                // reset drop score on this move
                _dropScore = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        // move piece right one square
        public bool MovePieceRight()
        {
            // protect against nulls
            if (CurrentPiece == null) return false;

            // make a list with initial positions
            var oldPosition = new List<(int x, int y)>();
            foreach (var square in CurrentPiece.Squares)
            {
                oldPosition.Add(square.PieceLocation);
            }

            // make a new list with all of the positions translated to the left by 1
            var newPosition = new List<(int x, int y)>();
            foreach (var square in CurrentPiece.Squares)
            {
                newPosition.Add((square.PieceLocation.x + 1, square.PieceLocation.y));
            }

            // only move if it's a valid position
            if (CheckValidPiecePosition(newPosition))
            {
                FinalizePieceMove(oldPosition, newPosition);
                // reset drop score on this move
                _dropScore = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        // soft drop: down *one* level
        private bool SoftDropPiece(bool incrementDropScore)
        {
            // protect against nulls
            if (CurrentPiece == null) return false;

            // make a list with initial positions
            var oldPosition = new List<(int x, int y)>();
            foreach (var square in CurrentPiece.Squares)
            {
                oldPosition.Add(square.PieceLocation);
            }

            // make a new list with all of the positions translated down by 1
            var newPosition = new List<(int x, int y)>();
            foreach (var square in CurrentPiece.Squares)
            {
                newPosition.Add((square.PieceLocation.x, square.PieceLocation.y - 1));
            }

            // only move if it's a valid position
            if (CheckValidPiecePosition(newPosition))
            {
                FinalizePieceMove(oldPosition, newPosition);
                // on soft drop, reset gravity timer to 0
                _timeSinceLastGravityTick = TimeSpan.Zero;
                if (incrementDropScore)
                    _dropScore++;
                return true;
            }
            else
            {
                return false;
            }
        }

        // hard drop: keep soft dropping until we can't
        public bool HardDropPiece()
        {
            // return value will be based on if we could soft drop *once*
            var returnVal = SoftDropPiece(true);

            // keep dropping down until we can't anymore
            while (SoftDropPiece(true))
            {
                // do nothing
            }

            // hard drop should immediately lock into place
            // we'll set the gravity tick to something real high
            if (CurrentPiece != null)
            {
                _timeSinceLastGravityTick = TimeSpan.FromSeconds(5);
            }

            return returnVal;
        }

        // rotation can be either clockwise or counterclockwise
        public bool RotatePiece(bool isCounterClockwise)
        {
            // protect against nulls
            if (CurrentPiece == null) return false;

            // make a list with initial positions
            var oldPosition = new List<(int x, int y)>();
            foreach (var square in CurrentPiece.Squares)
            {
                oldPosition.Add(square.PieceLocation);
            }

            // get the rotation matrix for this rotation
            // we'll use the rotation matrix to determine how to translate these points
            var rotationMatrix = CurrentPiece.GetRotationMatrix(isCounterClockwise);

            // make a new list with all of the positions translated down by 1
            var newPosition = new List<(int x, int y)>();

            // translate based on rotation matrix
            for (int i = 0; i < 4; i++)
            {
                newPosition.Add((CurrentPiece.Squares[i].PieceLocation.x + rotationMatrix[i].x,
                    CurrentPiece.Squares[i].PieceLocation.y + rotationMatrix[i].y));
            }

            // wall kick
            // get min and max X value on this piece
            var minX = 9;
            var maxX = 0;
            for (var i = 0; i < 4; i++)
            {
                var curX = newPosition[i].x;
                if (curX < minX)
                    minX = curX;
                if (curX > maxX)
                    maxX = curX;
            }
            // if min X is less than 0 then translate all pieces by 0 - minX
            // if max X is greater than 9 then translate all pieces by 9 - maxX
            if (minX < 0)
            {
                for (var i = 0; i < 4; i++)
                {
                    newPosition[i] = (newPosition[i].x - minX, newPosition[i].y);
                }
            }
            else if (maxX > 9)
            {
                for (var i = 0; i < 4; i++)
                {
                    newPosition[i] = (newPosition[i].x + 9 - maxX, newPosition[i].y);
                }
            }

            // only move if it's a valid position
            if (CheckValidPiecePosition(newPosition))
            {
                FinalizePieceMove(oldPosition, newPosition);
                CurrentPiece.Orientation = CurrentPiece.GetCorrectOrientation(isCounterClockwise);
                // reset drop score on this move
                _dropScore = 0;
                return true;
            }
            else
            {
                return false;
            }
        }

        // given an array of old square and a list of new position tuples, move the squares
        // we only call this function if the move is valid (i.e., check OUTSIDE this function)
        private void FinalizePieceMove(List<(int x, int y)> oldPosition, List<(int x, int y)> newPosition)
        {
            for (var i = 0; i < 4; i++)
            {
                CurrentPiece.Squares[i].PieceLocation = newPosition[i];
            }

            // remove old squares from board
            foreach (var (x, y) in oldPosition)
            {
                TetrisSquares[x, y] = null;
            }

            // add new squares to board
            for (var i = 0; i < 4; i++)
            {
                var (x, y) = CurrentPiece.Squares[i].PieceLocation;
                TetrisSquares[x, y] = CurrentPiece.Squares[i];
            }
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

        // generates a new bag of pieces
        private void GenerateBag()
        {
            // create one of each piece
            var pieceList = new List<Piece.PieceType>
            {
                I,
                J,
                L,
                O,
                S,
                T,
                Z
            };

            // shuffle this list
            // https://stackoverflow.com/a/4262134
            var random = new Random();
            var secondList = pieceList.OrderBy(a => random.Next());

            // for each piece type, generate a new piece and add it
            foreach (var pieceType in secondList)
            {
                var newPiece = Piece.GeneratePiece(pieceType);
                BagOfPieces.Add(newPiece);
            }
        }

        // removes the first item in the bag and makes that the current piece
        // returns false if we can't (i.e., game over)
        private bool PopBag()
        {
            CurrentPiece = BagOfPieces[0];

            foreach (var square in CurrentPiece.Squares)
            {
                var (x, y) = square.PieceLocation;

                // if this square is already occupied then we end
                if (TetrisSquares[x, y] != null) return false;

                TetrisSquares[x, y] = square;
            }

            BagOfPieces.RemoveAt(0);

            // if bag is empty, then let's make a new bag
            if (BagOfPieces.Count == 0)
                GenerateBag();

            return true;
        }

        // clear lines if needed
        private bool ClearLines(GameTime gameTime)
        {
            // check for line clears
            var listOfFullLines = GetFullLines();

            if (listOfFullLines.Count != 0)
            {
                // increment score
                // Scoring:
                // 1 line clear: 40 * (level + 1)
                // 2 line clear: 100 * (level + 1)
                // 3 line clear: 300 * (level + 1)
                // 4+ line clear: 1200 * (level + 1)
                var incrementScore = listOfFullLines.Count switch
                {
                    0 => 40 * (Level + 1),
                    1 => 100 * (Level + 1),
                    2 => 300 * (Level + 1),
                    _ => 1200 * (Level + 1)
                };

                // increment the score by the correct amount
                Score += incrementScore;
                LinesCleared += listOfFullLines.Count;

                // every 10 lines cleared, increase level
                Level = LinesCleared / 10;

                // clear all rows
                for (var row = 0; row < listOfFullLines.Count; row++)
                {
                    Console.WriteLine("Line clear at row " + listOfFullLines[row]);
                    for (var col = 0; col < 10; col++)
                    {
                        TetrisSquares[col, listOfFullLines[row]] = null;

                        // add particles
                        var (x, y) = TetrisGame.GetBoardLocationFromPieceLocation((col, listOfFullLines[row]));

                        // add emitter to left and right
                        if (col == 0)
                            ParticleController.AddLineClearEmitter(x, y + 0.5f, MathHelper.Pi, gameTime);
                        else if (col == 9)
                            ParticleController.AddLineClearEmitter(x + 1f, y + 0.5f, 0, gameTime);

                        // add below if first row
                        if (row == 0)
                            ParticleController.AddLineClearEmitter(x + 0.5f, y, 3 * MathHelper.PiOver2, gameTime);
                        // add above if last row
                        if (row == listOfFullLines.Count - 1)
                            ParticleController.AddLineClearEmitter(x + 0.5f, y + 1f, MathHelper.PiOver2, gameTime);
                    }
                }

                // play the sound for clearing the lines
                SoundController.PlayLineClear();

                // generate particles

                _timeSinceLastPieceTick = TimeSpan.Zero;
                _inFreeFallMode = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        // gets a list of all line numbers that are full
        private List<int> GetFullLines()
        {
            var returnList = new List<int>();

            // iterate for each row, if it has an empty square then don't add it
            for (var row = 0; row < 20; row++)
            {
                var rowFull = true;
                for (var col = 0; col < 10; col++)
                {
                    if (TetrisSquares[col, row] == null)
                    {
                        rowFull = false;
                        break;
                    }
                }

                if (rowFull)
                    returnList.Add(row);
            }

            return returnList;
        }

        // retrieves a new piece from the bag and places it
        // or enters GameOver state if can't
        private void GetNewPiece()
        {
            if (!PopBag())
            {
                // end the game here
                Console.WriteLine("Game over!");

                // change all pieces to gray
                foreach (var square in TetrisSquares)
                {
                    if (square != null)
                        square.Color = Gray;
                }

                // save high score if not in attract mode
                if (GameState != AttractMode)
                {
                    // add score to the list
                    MainMenuController.HighScoresIntList.Add(Score);

                    // sort list from high to low
                    MainMenuController.HighScoresIntList.Sort((a, b) => b.CompareTo(a));

                    // actually save the high scores
                    LocalStorageManager.SaveHighScores(MainMenuController.HighScoresIntList);
                }

                GameState = GameOver;

                SoundController.StopMusic();
                SoundController.PlayGameOver();
            }
            else
            {
                Console.WriteLine("Current piece: " + CurrentPiece.Type);
                Console.WriteLine("Next piece: " + BagOfPieces[0].Type);
            }
        }

        // given a list of float pairs, return only those that are unique
        private List<(float x, float y)> GetUniqueElements(List<(float x, float y)> inputList)
        {
            var returnList = new List<(float x, float y)>();
            var itemsToRemove = new List<(float x, float y)>();

            // mark duplicates, add unique
            foreach (var item in inputList)
            {
                if (returnList.Contains(item))
                    itemsToRemove.Add(item);
                else
                    returnList.Add(item);
            }

            // remove all duplicates
            foreach (var item in itemsToRemove)
                returnList.Remove(item);

            return returnList;
        }
    }
}