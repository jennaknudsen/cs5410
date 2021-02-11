using System;

namespace MazeGen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Now generating a maze");
            Maze maze = new Maze(20);
            Console.WriteLine("Maze generated successfully.");
            Console.WriteLine("Now printing out maze");
            maze.PrintMaze();
        }
    }
}
