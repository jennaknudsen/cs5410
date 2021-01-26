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
            string Name { get; set; }
            int Duration { get; set; }
            int Count { get; set; }
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
    }
}
