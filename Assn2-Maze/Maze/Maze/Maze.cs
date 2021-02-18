using System;
using System.Collections.Generic;
using System.Linq;
using static Maze.MazeSquare.Wall;

namespace Maze
{
    public class Maze
    {
        // multidimensional array holds all maze squares
        public MazeSquare[,] mazeSquares;

        // multidimensional array holds the solution squares
        // use this for our recursive algorithm
        private bool[,] visitedSquares;

        // tuple holds start and end squares
        public (int row, int col) startSquare = (0, 0);
        public (int row, int col) endSquare;
        public (int row, int col) currentSquare;
        public (int row, int col) hintSquare;

        // DisjointSet for all of the squares
        // When all squares are part of the same disjoint set, the maze
        // is solvable
        public DisjointSet mazeSquaresDisjointSet;

        // use a List to hold list of walls
        // using a List so that we can shuffle the walls;
        private List<MazeSquare.Wall> listOfWalls;

        // acceptable maze sizes are:
        // 5x5, 10x10, 15x15, 20x20
        public readonly int BoardSize;

        // constructor just initializes the mazeSquares array itself
        // doesn't do anything to construct the maze at all
        public Maze(int boardSize)
        {
            if (!new int[] {5, 10, 15, 20}.Contains(boardSize))
            {
                throw new Exception("Board size must be 5x5, 10x10, "
                                    + "15x15, or 20x20.");
            }

            // assuming boardsize matches
            this.BoardSize = boardSize;
            mazeSquares = new MazeSquare[BoardSize, BoardSize];

            // ending square will be bottom-right corner
            this.endSquare = (BoardSize - 1, BoardSize - 1);

            // initialize the listOfWalls
            listOfWalls = new List<MazeSquare.Wall>();

            // initialize the DisjointSet
            mazeSquaresDisjointSet = new DisjointSet(BoardSize * BoardSize);

            // next, fill the board with squares
            FillBoardWithSquares();

            // finally, generate and solve the actual maze
            GenerateMaze();
            SolveMazeFromStart();

            // start player at (0, 0)
            currentSquare = startSquare;
            mazeSquares[startSquare.row, startSquare.col].Visited = true;
        }

        private void FillBoardWithSquares()
        {
            // next, fill the board with squares
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    // id will be numerical from [0, BoardSize^2)
                    mazeSquares[row, col] = new MazeSquare(row * BoardSize + col);

                    // set up all four walls
                    MazeSquare.Wall topWall;
                    MazeSquare.Wall leftWall;
                    MazeSquare.Wall rightWall;
                    MazeSquare.Wall bottomWall;

                    // top wall: if row = 0, make it the edge
                    // otherwise, grab bottom wall of previous
                    if (row == 0)
                    {
                        topWall = new MazeSquare.Wall(
                            WallStatus.EDGE,
                            Orientation.HORIZONTAL,
                            null,
                            mazeSquares[row, col]);
                    }
                    else
                    {
                        topWall = mazeSquares[row - 1, col].BottomWall;
                        // need to reassign second ref to this (it will have
                        // been set to null previously)
                        topWall.secondSquareRef = mazeSquares[row, col];
                    }

                    // left wall: if col = 0, make it the edge
                    // otherwise, grab right wall of previous
                    if (col == 0)
                    {
                        leftWall = new MazeSquare.Wall(
                            WallStatus.EDGE,
                            Orientation.VERTICAL,
                            null,
                            mazeSquares[row, col]);
                    }
                    else
                    {
                        leftWall = mazeSquares[row, col - 1].RightWall;
                        // need to reassign second ref to this (it will have
                        // been set to null previously)
                        leftWall.secondSquareRef = mazeSquares[row, col];
                    }

                    // right wall: if col = BoardSize - 1, make it the edge
                    // otherwise, make it a new wall
                    WallStatus rightStatus;
                    if (col == BoardSize - 1)
                    {
                        rightStatus = WallStatus.EDGE;
                    }
                    else
                    {
                        rightStatus = WallStatus.ENABLED;
                    }

                    rightWall = new MazeSquare.Wall(
                        rightStatus,
                        Orientation.VERTICAL,
                        mazeSquares[row, col],
                        null);

                    // if this wasn't an edge, then add it to the ListOfWalls
                    // so that it might be disabled later
                    if (col != BoardSize - 1)
                    {
                        listOfWalls.Add(rightWall);
                    }

                    // bottom wall: if row = BoardSize - 1, make it the edge
                    // otherwise, make it a new wall
                    WallStatus bottomStatus;
                    if (row == BoardSize - 1)
                    {
                        bottomStatus = WallStatus.EDGE;
                    }
                    else
                    {
                        bottomStatus = WallStatus.ENABLED;
                    }

                    bottomWall = new MazeSquare.Wall(
                        bottomStatus,
                        Orientation.HORIZONTAL,
                        mazeSquares[row, col],
                        null);

                    // if this wasn't an edge, then add it to the ListOfWalls
                    // so that it might be disabled later
                    if (row != BoardSize - 1)
                    {
                        listOfWalls.Add(bottomWall);
                    }

                    // Assign all of the walls we just created to this square
                    mazeSquares[row, col].TopWall = topWall;
                    mazeSquares[row, col].LeftWall = leftWall;
                    mazeSquares[row, col].RightWall = rightWall;
                    mazeSquares[row, col].BottomWall = bottomWall;
                }
            }
        }

        // this function will actually go ahead and generate the maze
        // knocking out all walls randomly until each square is in the same
        // disjoint set
        public void GenerateMaze()
        {
            /*
             shuffle all walls in the list
             then, for each wall, determine if two cells in wall are in same DJ set
             if not, then union the two sets and disable the wall
            */

            // shuffle a list in place:
            // https://stackoverflow.com/a/1262619
            Random random = new Random();
            int n = listOfWalls.Count;
            while (n > 1)
            {
                n--;
                var k = random.Next(n + 1);
                var value = listOfWalls[k];
                listOfWalls[k] = listOfWalls[n];
                listOfWalls[n] = value;
            }

            foreach (var wall in listOfWalls)
            {
                var leftID = wall.firstSquareRef.ID;
                var rightID = wall.secondSquareRef.ID;

                if (mazeSquaresDisjointSet.Find(leftID) !=
                      mazeSquaresDisjointSet.Find(rightID))
                {
                    mazeSquaresDisjointSet.Union(leftID, rightID);
                    wall.wallStatus = WallStatus.DISABLED;
                }
            }
        }

        // helper function to start recursion to solve the maze
        public void SolveMazeFromStart()
        {
            // start the recursive solution at start (will usually be (0, 0) but could be different theoretically)
            SolveMazeFromPoint(startSquare);
        }

        private void SolveMazeFromPoint((int row, int col) currentSquareRecursive)
        {
            // initialize the list of visited squares as not visited
            visitedSquares = new bool[BoardSize, BoardSize];
            for (int i = 0; i < visitedSquares.GetLength(0); i++)
                for (int j = 0; j < visitedSquares.GetLength(1); j++)
                    visitedSquares[i, j] = false;

            // start the recursive solution at currentSquare
            SolveMazeRecursive(currentSquareRecursive);

            // get next hint
            GenerateNextHint();
        }

        private bool SolveMazeRecursive((int row, int col) currentSquareRecursive)
        {
            // inside, we need to check all four directions to see if the solution
            // could be built in that direction
            // check for: walls, visited squares (both false)
            // because our maze has no loops, the shortest path to the end is also the
            // only path to the end

            // mark current as visited, so we don't ever double back on it
            visitedSquares[currentSquareRecursive.row, currentSquareRecursive.col] = true;

            // get a reference to the current square
            var thisSquare = mazeSquares[currentSquareRecursive.row, currentSquareRecursive.col];

            // if we're at the solution then we're done, return True
            if (currentSquareRecursive == endSquare)
            {
                thisSquare.PartOfSolution = true;
                return true;
            }

            // each holds whether the direction was used in the solution
            bool topSolution, leftSolution, rightSolution, bottomSolution;

            // if not at boundary
            // and if not already visited
            // and if wall is disabled
            // then recurse and check for solution
            // otherwise, it's just false

            // top
            if (currentSquareRecursive.row != 0 &&
                !visitedSquares[currentSquareRecursive.row - 1, currentSquareRecursive.col] &&
                thisSquare.TopWall.wallStatus == WallStatus.DISABLED)
            {
                topSolution = SolveMazeRecursive((currentSquareRecursive.row - 1, currentSquareRecursive.col));
            }
            else
            {
                topSolution = false;
            }

            // left
            if (currentSquareRecursive.col != 0 &&
                !visitedSquares[currentSquareRecursive.row, currentSquareRecursive.col - 1] &&
                thisSquare.LeftWall.wallStatus == WallStatus.DISABLED)
            {
                leftSolution = SolveMazeRecursive((currentSquareRecursive.row, currentSquareRecursive.col - 1));
            }
            else
            {
                leftSolution = false;
            }

            // right
            if (currentSquareRecursive.col != BoardSize - 1 &&
                !visitedSquares[currentSquareRecursive.row, currentSquareRecursive.col + 1] &&
                thisSquare.RightWall.wallStatus == WallStatus.DISABLED)
            {
                rightSolution = SolveMazeRecursive((currentSquareRecursive.row, currentSquareRecursive.col + 1));
            }
            else
            {
                rightSolution = false;
            }

            // bottom
            if (currentSquareRecursive.row != BoardSize - 1 &&
                !visitedSquares[currentSquareRecursive.row + 1, currentSquareRecursive.col] &&
                thisSquare.BottomWall.wallStatus == WallStatus.DISABLED)
            {
                bottomSolution = SolveMazeRecursive((currentSquareRecursive.row + 1, currentSquareRecursive.col));
            }
            else
            {
                bottomSolution = false;
            }

            // return true if there is any solution among the four directions
            if (topSolution || leftSolution || rightSolution || bottomSolution)
            {
                thisSquare.PartOfSolution = true;
                return true;
            }
            else
            {
                thisSquare.PartOfSolution = false;
                return false;
            }

        }

        # region movementFunctions
        // for each move function:
        // check if wall above/left/right/down is DISABLED, and if so, move in that direction
        // then, re-solve maze from this position
        public void MoveUp()
        {
            if (mazeSquares[currentSquare.row, currentSquare.col].TopWall.wallStatus == WallStatus.DISABLED)
            {
                currentSquare.row -= 1;
                mazeSquares[currentSquare.row, currentSquare.col].Visited = true;
                // need to re-solve maze every time we move
                SolveMazeFromPoint(currentSquare);
            }
        }

        public void MoveLeft()
        {
            if (mazeSquares[currentSquare.row, currentSquare.col].LeftWall.wallStatus == WallStatus.DISABLED)
            {
                currentSquare.col -= 1;
                mazeSquares[currentSquare.row, currentSquare.col].Visited = true;
                // need to re-solve maze every time we move
                SolveMazeFromPoint(currentSquare);
            }
        }

        public void MoveRight()
        {
            if (mazeSquares[currentSquare.row, currentSquare.col].RightWall.wallStatus == WallStatus.DISABLED)
            {
                currentSquare.col += 1;
                mazeSquares[currentSquare.row, currentSquare.col].Visited = true;
                // need to re-solve maze every time we move
                SolveMazeFromPoint(currentSquare);
            }
        }

        public void MoveDown()
        {
            if (mazeSquares[currentSquare.row, currentSquare.col].BottomWall.wallStatus == WallStatus.DISABLED)
            {
                currentSquare.row += 1;
                mazeSquares[currentSquare.row, currentSquare.col].Visited = true;
                // need to re-solve maze every time we move
                SolveMazeFromPoint(currentSquare);
            }
        }
        # endregion

        private void GenerateNextHint()
        {
            // if at end of maze, then hint is just current
            // else, have to look for available moves and see which is part of solution
            if (currentSquare == endSquare)
            {
                hintSquare = endSquare;
            }
            else
            {
                // hint: UP
                if (mazeSquares[currentSquare.row, currentSquare.col].TopWall.wallStatus == WallStatus.DISABLED &&
                    mazeSquares[currentSquare.row - 1, currentSquare.col].PartOfSolution == true)
                {
                    hintSquare = (currentSquare.row - 1, currentSquare.col);
                }
                // hint: LEFT
                else if (mazeSquares[currentSquare.row, currentSquare.col].LeftWall.wallStatus == WallStatus.DISABLED &&
                    mazeSquares[currentSquare.row, currentSquare.col - 1].PartOfSolution == true)
                {
                    hintSquare = (currentSquare.row, currentSquare.col - 1);
                }
                // hint: RIGHT
                else if (mazeSquares[currentSquare.row, currentSquare.col].RightWall.wallStatus == WallStatus.DISABLED &&
                    mazeSquares[currentSquare.row, currentSquare.col + 1].PartOfSolution == true)
                {
                    hintSquare = (currentSquare.row, currentSquare.col + 1);
                }
                // hint: BOTTOM
                else if (mazeSquares[currentSquare.row, currentSquare.col].BottomWall.wallStatus == WallStatus.DISABLED &&
                    mazeSquares[currentSquare.row + 1, currentSquare.col].PartOfSolution == true)
                {
                    hintSquare = (currentSquare.row + 1, currentSquare.col);
                }
                // should NEVER get to this point
                else
                {
                    throw new Exception("Unable to generate hint! Something bad has happened.");
                }
            }
        }

        // for debugging purposes, this will print the maze
        public void PrintMaze()
        {
            // symbols:
            // ! -> vertical edge
            // | -> vertical wall
            // = -> horizontal edge
            // - -> horizontal wall
            // * -> corner piece
            char[,] mazeChars = new char[BoardSize * 3, BoardSize * 4];

            // print each cell in a 3x3 char grid
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    int scaledRow = 3 * row;
                    int scaledCol = 4 * col;

                    // fill corners with '*'
                    mazeChars[scaledRow, scaledCol] = '*';
                    mazeChars[scaledRow + 2, scaledCol] = '*';
                    mazeChars[scaledRow, scaledCol + 3] = '*';
                    mazeChars[scaledRow + 2, scaledCol + 3] = '*';

                    // center character is ' ' if not part of solution,
                    // OO if part of solution
                    var charToAdd = mazeSquares[row, col].PartOfSolution switch
                    {
                        true => 'O',
                        false => ' '

                    };
                    mazeChars[scaledRow + 1, scaledCol + 1] = charToAdd;
                    mazeChars[scaledRow + 1, scaledCol + 2] = charToAdd;

                    // top wall
                    charToAdd = mazeSquares[row, col].TopWall.wallStatus switch
                    {
                        WallStatus.ENABLED => '-',
                        WallStatus.EDGE => '=',
                        WallStatus.DISABLED => ' ',
                        _ => 'F',
                    };
                    mazeChars[scaledRow, scaledCol + 1] = charToAdd;
                    mazeChars[scaledRow, scaledCol + 2] = charToAdd;

                    // left wall
                    charToAdd = mazeSquares[row, col].LeftWall.wallStatus switch
                    {
                        WallStatus.ENABLED => '|',
                        WallStatus.EDGE => '!',
                        WallStatus.DISABLED => ' ',
                        _ => 'F',
                    };
                    mazeChars[scaledRow + 1, scaledCol] = charToAdd;

                    // right wall
                    charToAdd = mazeSquares[row, col].RightWall.wallStatus switch
                    {
                        WallStatus.ENABLED => '|',
                        WallStatus.EDGE => '!',
                        WallStatus.DISABLED => ' ',
                        _ => 'F',
                    };
                    mazeChars[scaledRow + 1, scaledCol + 3] = charToAdd;

                    // bottom wall
                    charToAdd = mazeSquares[row, col].BottomWall.wallStatus switch
                    {
                        WallStatus.ENABLED => '-',
                        WallStatus.EDGE => '=',
                        WallStatus.DISABLED => ' ',
                        _ => 'F',
                    };
                    mazeChars[scaledRow + 2, scaledCol + 1] = charToAdd;
                    mazeChars[scaledRow + 2, scaledCol + 2] = charToAdd;
                }
            }

            // once our char array is created, print it out
            for (int row = 0; row < 3 * BoardSize; row++)
            {
                for (int col = 0; col < 4 * BoardSize; col++)
                {
                    Console.Write(mazeChars[row, col]);
                }
                // at end of row, go to next line
                Console.Write('\n');
            }
        }
    }
}