using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeGen
{
    public class Maze
    {
        // multidimensional array holds all maze squares
        public MazeSquare[,] mazeSquares;

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
            if (!new int[]{ 5, 10, 15, 20 }.Contains(boardSize))
            { 
                throw new Exception("Board size must be 5x5, 10x10, "
                    + "15x15, or 20x20.");
            }

            // assuming boardsize matches
            this.BoardSize = boardSize;
            mazeSquares = new MazeSquare[BoardSize, BoardSize];

            // initialize the listOfWalls
            listOfWalls = new List<MazeSquare.Wall>();

            // initialize the DisjointSet
            mazeSquaresDisjointSet = new DisjointSet(boardSize * boardSize);

            // next, fill the board with squares
            fillBoardWithSquares();

            // finally, generate the actual maze
            GenerateMaze();
        }

        public void fillBoardWithSquares()
        {
            // next, fill the board with squares
            for (int row = 0; row < BoardSize; row++)
            {
                for (int col = 0; col < BoardSize; col++)
                {
                    // id will be numerical from [0, BoardSize^2)
                    mazeSquares[row, col] = new MazeSquare(row * 5 + col);

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
            // TODO: write this
            
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
            char[,] mazeChars = new char[BoardSize * 3, BoardSize * 3];

            // print each cell in a 3x3 char grid
            for (int row = 0; row < BoardSize; row++)
            { 
                for (int col = 0; col < BoardSize; col++)
                {
                    int scaledRow = 3 * row;
                    int scaledCol = 3 * col;

                    // fill corners with '*'
                    mazeChars[scaledRow, scaledCol] = '*';
                    mazeChars[scaledRow + 2, scaledCol] = '*';
                    mazeChars[scaledRow, scaledCol + 2] = '*';
                    mazeChars[scaledRow + 2, scaledCol + 2] = '*';

                    // center character is ' '
                    mazeChars[scaledRow + 1, scaledCol + 1] = ' ';

                    // determine which edges need to be filled 
                    char charToAdd;

                    // top wall
                    charToAdd = 'F';
                    switch (mazeSquares[row, col].TopWall.wallStatus)
                    {
                        case MazeSquare.Wall.WallStatus.ENABLED:
                            charToAdd = '-';
                            break;
                        case MazeSquare.Wall.WallStatus.EDGE:
                            charToAdd = '=';
                            break;
                        default:
                            charToAdd = ' ';
                            break;
                    }
                    mazeChars[scaledRow, scaledCol + 1] = charToAdd;

                    // left wall
                    charToAdd = 'F';
                    switch (mazeSquares[row, col].LeftWall.wallStatus)
                    {
                        case MazeSquare.Wall.WallStatus.ENABLED:
                            charToAdd = '|';
                            break;
                        case MazeSquare.Wall.WallStatus.EDGE:
                            charToAdd = '!';
                            break;
                        default:
                            charToAdd = ' ';
                            break;
                    }
                    mazeChars[scaledRow + 1, scaledCol] = charToAdd;

                    // right wall
                    charToAdd = 'F';
                    switch (mazeSquares[row, col].RightWall.wallStatus)
                    {
                        case MazeSquare.Wall.WallStatus.ENABLED:
                            charToAdd = '|';
                            break;
                        case MazeSquare.Wall.WallStatus.EDGE:
                            charToAdd = '!';
                            break;
                        default:
                            charToAdd = ' ';
                            break;
                    }
                    mazeChars[scaledRow + 1, scaledCol + 2] = charToAdd;

                    // bottom wall
                    charToAdd = 'F';
                    switch (mazeSquares[row, col].BottomWall.wallStatus)
                    {
                        case MazeSquare.Wall.WallStatus.ENABLED:
                            charToAdd = '-';
                            break;
                        case MazeSquare.Wall.WallStatus.EDGE:
                            charToAdd = '=';
                            break;
                        default:
                            charToAdd = ' ';
                            break;
                    }
                    mazeChars[scaledRow + 2, scaledCol + 1] = charToAdd;
                }
            }

            // once our char array is created, print it out
            for (int row = 0; row < 3 * BoardSize; row++)
            { 
                for (int col = 0; col < 3 * BoardSize; col++)
                {
                    Console.Write(mazeChars[row, col]);
                }
                // at end of row, go to next line
                Console.Write('\n');
            }
        }
    }
}
