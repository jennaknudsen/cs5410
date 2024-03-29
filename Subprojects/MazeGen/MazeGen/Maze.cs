﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeGen
{
    public class Maze
    {
        // multidimensional array holds all maze squares
        public MazeSquare[,] mazeSquares;

        // multidimensional array holds the solution squares
        // use this for our recursive memoizing algorithm
        private bool[,] visitedSquares;

        // tuple holds start and end squares
        public (int, int) startSquare = (0, 0);
        public (int, int) endSquare;

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

            // initialize the list of visited squares as not visited
            visitedSquares = new bool[BoardSize, BoardSize];
            for (int i = 0; i < visitedSquares.GetLength(0); i++)
                for (int j = 0; j < visitedSquares.GetLength(1); j++)
                    visitedSquares[i, j] = false;

            // next, fill the board with squares
            FillBoardWithSquares();

            // finally, generate the actual maze
            GenerateMaze();
        }

        public void FillBoardWithSquares()
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
                            MazeSquare.Wall.WallStatus.EDGE,
                            MazeSquare.Wall.Orientation.HORIZONTAL,
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
                            MazeSquare.Wall.WallStatus.EDGE,
                            MazeSquare.Wall.Orientation.VERTICAL,
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
                    MazeSquare.Wall.WallStatus rightStatus;
                    if (col == BoardSize - 1)
                    {
                        rightStatus = MazeSquare.Wall.WallStatus.EDGE;
                    }
                    else
                    {
                        rightStatus = MazeSquare.Wall.WallStatus.ENABLED;
                    }

                    rightWall = new MazeSquare.Wall(
                        rightStatus,
                        MazeSquare.Wall.Orientation.VERTICAL,
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
                    MazeSquare.Wall.WallStatus bottomStatus;
                    if (row == BoardSize - 1)
                    {
                        bottomStatus = MazeSquare.Wall.WallStatus.EDGE;
                    }
                    else
                    {
                        bottomStatus = MazeSquare.Wall.WallStatus.ENABLED;
                    }

                    bottomWall = new MazeSquare.Wall(
                        bottomStatus,
                        MazeSquare.Wall.Orientation.HORIZONTAL,
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
                int k = random.Next(n + 1);
                MazeSquare.Wall value = listOfWalls[k];
                listOfWalls[k] = listOfWalls[n];
                listOfWalls[n] = value;
            }

            foreach (MazeSquare.Wall wall in listOfWalls)
            {
                int leftID = wall.firstSquareRef.ID;
                int rightID = wall.secondSquareRef.ID;

                if (!(mazeSquaresDisjointSet.Find(leftID) ==
                      mazeSquaresDisjointSet.Find(rightID)))
                {
                    mazeSquaresDisjointSet.Union(leftID, rightID);
                    wall.wallStatus = MazeSquare.Wall.WallStatus.DISABLED;
                }
            }
        }

        // helper function to start recursion to solve the maze
        public void SolveMaze()
        {
            // start the recursive solution at start (0, 0)
            SolveMazeRecursive((0, 0));
        }

        private bool SolveMazeRecursive((int row, int col) currentSquare)
        {
            // inside, we need to check all four directions to see if the solution
            // could be built in that direction
            // check for: walls, visited squares (both false)
            // because our maze has no loops, the shortest path to the end is also the
            // only path to the end

            // mark current as visited, so we don't ever double back on it
            visitedSquares[currentSquare.row, currentSquare.col] = true;

            // get a reference to the current square
            MazeSquare thisSquare = mazeSquares[currentSquare.row, currentSquare.col];

            // if we're at the solution then we're done, return True
            if (currentSquare == endSquare)
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
            if (currentSquare.row != 0 &&
                !visitedSquares[currentSquare.row - 1, currentSquare.col] &&
                thisSquare.TopWall.wallStatus == MazeSquare.Wall.WallStatus.DISABLED)
            {
                topSolution = SolveMazeRecursive((currentSquare.row - 1, currentSquare.col));
            }
            else
            {
                topSolution = false;
            }

            // left
            if (currentSquare.col != 0 &&
                !visitedSquares[currentSquare.row, currentSquare.col - 1] &&
                thisSquare.LeftWall.wallStatus == MazeSquare.Wall.WallStatus.DISABLED)
            {
                leftSolution = SolveMazeRecursive((currentSquare.row, currentSquare.col - 1));
            }
            else
            {
                leftSolution = false;
            }

            // right
            if (currentSquare.col != BoardSize - 1 &&
                !visitedSquares[currentSquare.row, currentSquare.col + 1] &&
                thisSquare.RightWall.wallStatus == MazeSquare.Wall.WallStatus.DISABLED)
            {
                rightSolution = SolveMazeRecursive((currentSquare.row, currentSquare.col + 1));
            }
            else
            {
                rightSolution = false;
            }

            // bottom
            if (currentSquare.row != BoardSize - 1 &&
                !visitedSquares[currentSquare.row + 1, currentSquare.col] &&
                thisSquare.BottomWall.wallStatus == MazeSquare.Wall.WallStatus.DISABLED)
            {
                bottomSolution = SolveMazeRecursive((currentSquare.row + 1, currentSquare.col));
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
                return false;
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
                        MazeSquare.Wall.WallStatus.ENABLED => '-',
                        MazeSquare.Wall.WallStatus.EDGE => '=',
                        MazeSquare.Wall.WallStatus.DISABLED => ' ',
                        _ => 'F',
                    };
                    mazeChars[scaledRow, scaledCol + 1] = charToAdd;
                    mazeChars[scaledRow, scaledCol + 2] = charToAdd;

                    // left wall
                    charToAdd = mazeSquares[row, col].LeftWall.wallStatus switch
                    {
                        MazeSquare.Wall.WallStatus.ENABLED => '|',
                        MazeSquare.Wall.WallStatus.EDGE => '!',
                        MazeSquare.Wall.WallStatus.DISABLED => ' ',
                        _ => 'F',
                    };
                    mazeChars[scaledRow + 1, scaledCol] = charToAdd;

                    // right wall
                    charToAdd = mazeSquares[row, col].RightWall.wallStatus switch
                    {
                        MazeSquare.Wall.WallStatus.ENABLED => '|',
                        MazeSquare.Wall.WallStatus.EDGE => '!',
                        MazeSquare.Wall.WallStatus.DISABLED => ' ',
                        _ => 'F',
                    };
                    mazeChars[scaledRow + 1, scaledCol + 3] = charToAdd;

                    // bottom wall
                    charToAdd = mazeSquares[row, col].BottomWall.wallStatus switch
                    {
                        MazeSquare.Wall.WallStatus.ENABLED => '-',
                        MazeSquare.Wall.WallStatus.EDGE => '=',
                        MazeSquare.Wall.WallStatus.DISABLED => ' ',
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
