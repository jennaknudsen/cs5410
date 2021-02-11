using System;
using System.Collections.Generic;
using System.Linq;

namespace MazeGen
{
    public class Maze
    {
        // multidimensional array holds all maze squares
        public MazeSquare[,] mazeSquares;
        // use a List to hold list of walls
        // using a List so that we can shuffle the walls;
        private List<MazeSquare.Wall> listOfWalls;

        // acceptable maze sizes are: 
        // 5x5, 10x10, 15x15, 20x20
        public int boardSize { get; }

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
            this.boardSize = boardSize;
            mazeSquares = new MazeSquare[boardSize, boardSize];

            // initialize the listOfWalls
            listOfWalls = new List<MazeSquare.Wall>();

            // next, fill the board with squares


            // finally, generate the actual maze
            GenerateMaze();
        }

        public void GenerateMaze()
        { 
            
        }
    }
}
