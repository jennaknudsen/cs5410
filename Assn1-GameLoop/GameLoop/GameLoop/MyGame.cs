using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace GameLoop
{
    class MyGame
    {
        // Internal class to hold information about game events
        class GameEvent
        {
            public string Name { get; set; }
            public int DurationMillis { get; set; }
            public int ElapsedTimeMillis { get; set; }
            public int Count { get; set; }
        }

        // this list holds all game events
        private List<GameEvent> GameEventsList;
        private List<GameEvent> eventsToRender;

        // this string holds whatever the user is typing
        private string userInput = "";

        private DateTime lastUpdateTime = DateTime.Now;

        // the "constructor"
        public void initialize()
        {
            Console.WriteLine("GameLoop Demo Initializing...");
            GameEventsList = new List<GameEvent>();
            eventsToRender = new List<GameEvent>();
        }

        // where the actual game loop will take place
        public void run()
        {
            // TEST - remove this 
            GameEventsList.Add(new GameEvent
            {
                Name = "FirstEvent",
                DurationMillis = 500,
                ElapsedTimeMillis = 0,
                Count = 3
            }) ;
            GameEventsList.Add(new GameEvent
            {
                Name = "SecondEvent",
                DurationMillis = 1000,
                ElapsedTimeMillis = 0,
                Count = 6
            });

            while (true)
            {
                while (!Console.KeyAvailable)
                {
                    update(DateTime.Now - lastUpdateTime);
                    lastUpdateTime = DateTime.Now;

                    render();

                    // sleep the thread for a tiny amount so that the MS difference between last update and now will be greater than 1
                    Thread.Sleep(1);
                }

                processInput();

                Debug.WriteLine("====|" + userInput + "|====");
                
            }
        }

        // This is where keyboard input is handled
        public void processInput()
        {
            ConsoleKeyInfo c = Console.ReadKey();
            userInput += c.KeyChar;
        }

        // Handles event simulation
        // elapsedTime: how much time has elapsed since the last update
        public void update(TimeSpan elapsedTime)
        {
            int elapsedTimeMillis = (int) elapsedTime.TotalMilliseconds;

            List<GameEvent> eventsToRemove = new List<GameEvent>();
            foreach (GameEvent ge in GameEventsList)
            {
                ge.ElapsedTimeMillis += elapsedTimeMillis;

                if (ge.ElapsedTimeMillis >= ge.DurationMillis)
                {
                    eventsToRender.Add(ge);
                    ge.ElapsedTimeMillis = 0;
                    ge.Count--;
                }
                if (ge.Count == 0)
                {
                    eventsToRemove.Add(ge);
                }
            }

            // now, remove all expired events
            // they'll still exist in eventsToRender 
            foreach (GameEvent ge in eventsToRemove)
            {
                GameEventsList.Remove(ge);
            }
        }

        // Events fired are displayed in this method
        public void render()
        {
            foreach (GameEvent ge in eventsToRender)
            {
                Console.WriteLine("\tEvent: " + ge.Name + " (" + ge.Count + " remaining)");
            }
            eventsToRender.Clear();
        }
    }
}
