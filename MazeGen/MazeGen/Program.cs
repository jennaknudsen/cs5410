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
            Console.WriteLine("Done printing out maze.");
            Console.WriteLine("Now solving maze");
            maze.SolveMaze();
            maze.PrintMaze();
            Console.WriteLine("Done solving maze.");
            Console.WriteLine(DateTime.Now.ToString());
        }
    }
}