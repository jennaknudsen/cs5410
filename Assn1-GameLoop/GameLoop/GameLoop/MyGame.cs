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
        private List<GameEvent> GameEventsList;

        // the "constructor"
        public void initialize()
        {
            Console.WriteLine("GameLoop Demo Initializing...");
            GameEventsList = new List<GameEvent>();
        }

        // where the actual game loop will take place
        public void run()
        {
            // 3 step process
            // 1: fire all events if necessary
            // 2: clear out all empty events
            // 3: check for user input
            while (true)
            {
                // step 1
                // use this temp list to keep track of which events will need to be deleted in the future
                List<GameEvent> tempList = new List<GameEvent>();
                foreach (GameEvent ge in GameEventsList)
                {
                    fireEventIfNecessary(ge);

                    if (ge.Count == 0)
                        tempList.Add(ge);
                }

                // step 2
                foreach (GameEvent ge in tempList)
                {
                    GameEventsList.Remove(ge);
                }

                // step 3
                
            }
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
