using System;

namespace FinalProjecct_Tetris
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new TetrisGame())
                game.Run();
        }
    }
}
