using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeGen
{
    public class Maze
    {
        // multidimensional array holds all maze squares
        public MazeSquare[,] mazeSquares;

        //
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

            // next, fill the board with squares
            for (int x = 0; x < BoardSize; x++) 
            { 
                for (int y = 0; y < BoardSize; y++)
                {
                    // id will be numerical from [0, BoardSize^2)
                    mazeSquares[x, y] = new MazeSquare(x * 5 + y);

                    // set up all four walls
                    MazeSquare.Wall topWall;
                    MazeSquare.Wall leftWall;
                    MazeSquare.Wall rightWall;
                    MazeSquare.Wall bottomWall;

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
                    }
                }
            }

            // finally, generate the actual maze
            GenerateMaze();
        }

        public void GenerateMaze()
        { 
            
        }
    }
}
