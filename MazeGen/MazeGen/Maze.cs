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
            for (int y = 0; y < BoardSize; y++)
            {
                for (int x = 0; x < BoardSize; x++)
                {
                    // id will be numerical from [0, BoardSize^2)
                    mazeSquares[x, y] = new MazeSquare(x * 5 + y);

                    // set up all four walls
                    MazeSquare.Wall topWall;
                    MazeSquare.Wall leftWall;
                    MazeSquare.Wall rightWall;
                    MazeSquare.Wall bottomWall;

                    // top wall: if y = 0, make it the edge
                    // otherwise, grab bottom wall of previous
                    if (y == 0)
                    {
                        topWall = new MazeSquare.Wall(
                            MazeSquare.Wall.WallStatus.EDGE,
                            MazeSquare.Wall.Orientation.HORIZONTAL,
                            null,
                            mazeSquares[x, y]);
                    }
                    else
                    {
                        topWall = mazeSquares[x, y - 1].BottomWall;
                        // need to reassign second ref to this (it will have
                        // been set to null previously)
                        topWall.secondSquareRef = mazeSquares[x, y];
                    }

                    // left wall: if x = 0, make it the edge
                    // otherwise, grab right wall of previous
                    if (x == 0)
                    {
                        leftWall = new MazeSquare.Wall(
                            MazeSquare.Wall.WallStatus.EDGE,
                            MazeSquare.Wall.Orientation.VERTICAL,
                            null,
                            mazeSquares[x, y]);
                    }
                    else
                    {
                        leftWall = mazeSquares[x - 1, y].RightWall;
                        // need to reassign second ref to this (it will have
                        // been set to null previously)
                        leftWall.secondSquareRef = mazeSquares[x, y];
                    }

                    // right wall: if x = BoardSize - 1, make it the edge
                    // otherwise, make it a new wall 
                    MazeSquare.Wall.WallStatus rightStatus;
                    if (x == BoardSize - 1)
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
                        mazeSquares[x, y],
                        null);

                    // if this wasn't an edge, then add it to the ListOfWalls
                    // so that it might be disabled later
                    if (x != BoardSize - 1)
                    {
                        listOfWalls.Add(rightWall);
                    }

                    // bottom wall: if y = BoardSize - 1, make it the edge
                    // otherwise, make it a new wall 
                    MazeSquare.Wall.WallStatus bottomStatus;
                    if (y == BoardSize - 1)
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
                        mazeSquares[x, y],
                        null);

                    // if this wasn't an edge, then add it to the ListOfWalls
                    // so that it might be disabled later
                    if (y != BoardSize - 1)
                    {
                        listOfWalls.Add(bottomWall);
                    }

                    // Assign all of the walls we just created to this square
                    mazeSquares[x, y].TopWall = topWall;
                    mazeSquares[x, y].LeftWall = leftWall;
                    mazeSquares[x, y].RightWall = rightWall;
                    mazeSquares[x, y].BottomWall = bottomWall;
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

            for (int y = 0; y < BoardSize; y++)
            { 
                for (int x = 0; x < BoardSize; x++)
                {
                    int scaledX = 3 * x;
                    int scaledY = 3 * y;

                    // fill corners with '*'
                    mazeChars[scaledX, scaledY] = '*';
                    mazeChars[scaledX + 2, scaledY] = '*';
                    mazeChars[scaledX, scaledY + 2] = '*';
                    mazeChars[scaledX + 2, scaledY + 2] = '*';

                    // center character is ' '
                    mazeChars[scaledX + 1, scaledY + 1] = ' ';

                    // determine which edges need to be filled 
                    char charToAdd;

                    // top wall
                    charToAdd = 'F';
                    switch (mazeSquares[x, y].TopWall.wallStatus)
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
                    mazeChars[scaledX + 1, scaledY] = charToAdd;

                    // left wall
                    charToAdd = 'F';
                    switch (mazeSquares[x, y].LeftWall.wallStatus)
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
                    mazeChars[scaledX, scaledY + 1] = charToAdd;

                    // right wall
                    charToAdd = 'F';
                    switch (mazeSquares[x, y].RightWall.wallStatus)
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
                    mazeChars[scaledX + 2, scaledY + 1] = charToAdd;

                    // bottom wall
                    charToAdd = 'F';
                    switch (mazeSquares[x, y].BottomWall.wallStatus)
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
                    mazeChars[scaledX + 1, scaledY + 2] = charToAdd;
                }
            }

            // once our char array is created, print it out
            for (int y = 0; y < 3 * BoardSize; y++)
            { 
                for (int x = 0; x < 3 * BoardSize; x++)
                {
                    Console.Write(mazeChars[x, y]);
                }
                Console.Write('\n');
            }
        }
    }
}
