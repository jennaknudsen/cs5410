using System;

namespace LunarLander
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new LanderGame())
                game.Run();
        }
    }
}
