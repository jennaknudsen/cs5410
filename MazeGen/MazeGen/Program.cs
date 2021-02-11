using System;

namespace MazeGen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Now generating a maze");
            Maze maze = new Maze(10);
            Console.WriteLine("Maze generated successfully.");
            Console.WriteLine("Now printing out maze");
            maze.PrintMaze();
            Console.WriteLine("Done printing out maze.");
            Console.WriteLine(DateTime.Now.ToString());
        }
    }
}