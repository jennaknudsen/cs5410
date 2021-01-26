using System;
using System.Collections.Generic;
using System.Text;

namespace GameLoop
{
    class MyGame
    {
        // Internal class to hold information about game events
        class GameEvent
        {
            public string Name { get; set; }
            public int Duration { get; set; }
            public DateTime StartTime { get; set; }
            public int Count { get; set; }
        }

        // this list holds all game events
        private List<GameEvent> GameEvents;

        // the "constructor"
        public void initialize()
        {
            Console.WriteLine("GameLoop Demo Initializing...");
            GameEvents = new List<GameEvent>();
        }

        // where the actual game loop will take place
        public void run()
        {
            
        }

        private void fireEventIfNecessary(GameEvent ge)
        {
            if ((DateTime.Now - ge.StartTime).TotalMilliseconds > ge.Duration)
            {
                ge.Count--;
                Console.WriteLine("\tEvent: " + ge.Name + " (" + ge.Count + " remaining)");
                ge.StartTime = DateTime.Now;
            }
        }
    }
}
