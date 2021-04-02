using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using static FinalProject_Tetris.Piece;
using static FinalProject_Tetris.Piece.PieceOrientation;

namespace FinalProject_Tetris
{
    public class AiController
    {
        private TetrisGameController _tetrisGameController;

        // reference to the move we're going to make
        // if null, we haven't decided on the move yet
        private Move _nextMove = null;

        // decides how fast the AI can move
        private readonly TimeSpan _moveRate = TimeSpan.FromMilliseconds(300);
        private TimeSpan _timeSinceLastMove = TimeSpan.Zero;

        public AiController(TetrisGameController tetrisGameController)
        {
            _tetrisGameController = tetrisGameController;
        }

        // this struct holds information about an object move
        private class Move
        {
            public PieceOrientation PieceOrientation;
            public int LeftMostColumn;
            public float MoveScore;
        }

        public void Update(GameTime gameTime)
        {
            // get the next move
            if (_nextMove == null)
                GetNextMove(_tetrisGameController.CurrentPiece, _tetrisGameController.TetrisSquares);

            // only do movement updates within the specified move rate
            _timeSinceLastMove += gameTime.ElapsedGameTime;
            if (_timeSinceLastMove < _moveRate) return;

            _timeSinceLastMove = TimeSpan.Zero;

        }


        // this code based off of resources from:
        // https://codemyroad.wordpress.com/2013/04/14/tetris-ai-the-near-perfect-player/
        // https://github.com/LeeYiyuan/tetrisai
        private void GetNextMove(Piece currentPiece, Square[,] boardSquares)
        {
            // given the current piece, try all combinations of moves
            var bestMove = new Move
            {
                PieceOrientation = Up,
                LeftMostColumn = 0,
                MoveScore = -10000f
            };

            // try each orientation
            // for each orientation
            foreach (var orientation in new[] {Up, Left, Right, Down})
            {
                for (var col = 0; col < 10; col++)
                {
                    var newPiece = Piece.GeneratePiece(currentPiece.Type);

                    // set piece to the correct orientation
                    while (newPiece.Orientation != orientation)
                        RotatePiece(false, newPiece, boardSquares);

                    // move to the correct spot
                    while (GetPieceLeftPosition(newPiece) > col)
                        MovePieceLeft(newPiece, boardSquares);

                    while (GetPieceLeftPosition(newPiece) < col)
                        if (!MovePieceRight(newPiece, boardSquares))
                            break;

                    // hard drop piece here and get resulting score
                    while (SoftDropPiece(newPiece, boardSquares))
                    {
                        // do nothing
                    }

                    // score = -this.heightWeight * _grid.aggregateHeight()
                    // + this.linesWeight * _grid.lines()
                    // - this.holesWeight * _grid.holes()
                    // - this.bumpinessWeight * _grid.bumpiness();
                    var heightWeight = 0.510066f;
                    var linesWeight = 0.760666f;
                    var holesWeight = 0.35663f;
                    var bumpinessWeight = 0.184483f;

                    var thisScore = -1 * heightWeight * AggregateHeight(boardSquares) +
                                    linesWeight * CompleteLines(boardSquares) -
                                    holesWeight * Holes(boardSquares) -
                                    bumpinessWeight * Bumpiness(boardSquares);

                    if (thisScore > bestMove.MoveScore)
                    {

                        bestMove = new Move
                        {
                            LeftMostColumn = col,
                            PieceOrientation = newPiece.Orientation,
                            MoveScore = thisScore
                        };
                    }

                    // clear the piece from the board
                    foreach (var square in newPiece.Squares)
                    {
                        boardSquares[square.PieceLocation.x, square.PieceLocation.y] = null;
                    }
                }
            }

            _nextMove = bestMove;
        }

        // move piece left one square
        private bool MovePieceLeft(Piece CurrentPiece, Square[,] TetrisSquares)
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
            if (CheckValidPiecePosition(newPosition, CurrentPiece, TetrisSquares))
            {
                FinalizePieceMove(oldPosition, newPosition, CurrentPiece, TetrisSquares);
                return true;
            }
            else
            {
                return false;
            }
        }

        // move piece right one square
        private bool MovePieceRight(Piece CurrentPiece, Square[,] TetrisSquares)
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
            if (CheckValidPiecePosition(newPosition, CurrentPiece, TetrisSquares))
            {
                FinalizePieceMove(oldPosition, newPosition, CurrentPiece, TetrisSquares);
                return true;
            }
            else
            {
                return false;
            }
        }

        // soft drop: down *one* level
        private bool SoftDropPiece(Piece CurrentPiece, Square[,] TetrisSquares)
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
            if (CheckValidPiecePosition(newPosition, CurrentPiece, TetrisSquares))
            {
                FinalizePieceMove(oldPosition, newPosition, CurrentPiece, TetrisSquares);
                // on soft drop, reset gravity timer to 0
                return true;
            }
            else
            {
                return false;
            }
        }

        // rotation can be either clockwise or counterclockwise
        private bool RotatePiece(bool isCounterClockwise, Piece CurrentPiece, Square[,] TetrisSquares)
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
            if (CheckValidPiecePosition(newPosition, CurrentPiece, TetrisSquares))
            {
                FinalizePieceMove(oldPosition, newPosition, CurrentPiece, TetrisSquares);
                CurrentPiece.Orientation = CurrentPiece.GetCorrectOrientation(isCounterClockwise);
                return true;
            }
            else
            {
                return false;
            }
        }

        // given an array of old square and a list of new position tuples, move the squares
        // we only call this function if the move is valid (i.e., check OUTSIDE this function)
        private void FinalizePieceMove(List<(int x, int y)> oldPosition, List<(int x, int y)> newPosition, Piece CurrentPiece, Square[,] TetrisSquares)
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
        private bool CheckValidPiecePosition(IEnumerable<(int x, int y)> newPosition, Piece CurrentPiece, Square[,] TetrisSquares)
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

        private int GetPieceLeftPosition(Piece CurrentPiece)
        {
            var leftmost = 9;
            foreach (var square in CurrentPiece.Squares)
            {
                if (square.PieceLocation.x < leftmost)
                    leftmost = square.PieceLocation.x;
            }

            return leftmost;
        }

        #region AI aggregation scores

        // gets the total height of all columns
        private float AggregateHeight(Square[,] boardSquares)
        {
            var runningTotal = 0;

            for (var col = 0; col < 10; col++)
            {
                for (var row = 19; row >= 0; row--)
                {
                    if (boardSquares[col, row] != null)
                    {
                        runningTotal += row + 1;
                        break;
                    }
                }
            }

            return runningTotal;
        }

        // gets total count of complete lines
        private float CompleteLines(Square[,] boardSquares)
        {
            var completeLineCount = 0;

            // iterate for each row, if it has an empty square then don't add it
            for (var row = 0; row < 20; row++)
            {
                var rowFull = true;
                for (var col = 0; col < 10; col++)
                {
                    if (boardSquares[col, row] == null)
                    {
                        rowFull = false;
                        break;
                    }
                }

                if (rowFull)
                    completeLineCount++;
            }

            return completeLineCount;
        }

        // get total number of squares in holes
        private float Holes(Square[,] boardSquares)
        {
            var holesCount = 0;

            for (var col = 0; col < 10; col++)
            {
                // counting gaps in this column; only apply them if we see a non-gap after we add a gap
                var gapsInColumn = 0;
                for (var row = 0; row < 20; row++)
                {
                    // if this is an empty square then increment gap count
                    if (boardSquares[col, row] == null)
                    {
                        gapsInColumn++;
                    }
                    // otherwise, apply the gap count and reset gap count to 0
                    else
                    {
                        holesCount += gapsInColumn;
                        gapsInColumn = 0;
                    }
                }
            }

            return holesCount;
        }

        // get total bumpiness of board
        private float Bumpiness(Square[,] boardSquares)
        {
            // get heights of each column
            var heights = new int[10];

            for (var col = 0; col < 10; col++)
            {
                for (var row = 19; row >= 0; row--)
                {
                    if (boardSquares[col, row] != null)
                    {
                        heights[col] = row + 1;
                        break;
                    }
                }
            }

            // now, sum the absolute differences between all columns
            var runningTotal = 0;
            for (var i = 0; i < heights.Length - 1; i++)
            {
                runningTotal += Math.Abs(heights[i] - heights[i + 1]);
            }

            return runningTotal;
        }

        #endregion

    }
}