using System;

namespace Midterm
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new MidtermGame())
                game.Run();
        }
    }
}
