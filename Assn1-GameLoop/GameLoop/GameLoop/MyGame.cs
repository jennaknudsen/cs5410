using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        // boolean represents whether we are ready to process user input or not
        private bool processUserInput = false;

        // boolean represents whether user pressed Enter (whether render() should show a new line)
        private bool userPressedEnter = false;

        // DateTime holds the last time the update() function was called
        private DateTime lastUpdateTime = DateTime.Now;

        // the "constructor"
        public void initialize()
        {
            Console.WriteLine("GameLoop Demo Initializing...");
            GameEventsList = new List<GameEvent>();
            eventsToRender = new List<GameEvent>();

            // For the first time, display the [cmd:] prompt
            Console.Write("[cmd:] ");
        }

        // where the actual game loop will take place
        public void run()
        {
            while (true)
            {
                // when there isn't a key available, just run the internal game loop
                while (!Console.KeyAvailable)
                {
                    // pass elapsed time as a parameter
                    update(DateTime.Now - lastUpdateTime);

                    lastUpdateTime = DateTime.Now;

                    // render everything needed on screen
                    render();

                    // sleep the thread for a tiny amount so that the MS difference between last update and now will be greater than 1
                    Thread.Sleep(2);
                }

                // once there is a key available, then process the input
                processInput();
            }
        }

        // This is where keyboard input is handled
        public void processInput()
        {
            ConsoleKeyInfo c = Console.ReadKey();

            // ENTER = 13 on ASCII table
            if (c.KeyChar == (char) 13)
            {
                processUserInput = true;
                userPressedEnter = true;
            } 
            // BACKSPACE = 8 on ASCII table
            else if (c.KeyChar == (char) 8)
            {
                // only backspace if string not empty
                if (userInput.Length > 0)
                {
                    userInput = userInput.Remove(userInput.Length - 1, 1);
                }
            }
            else
            {
                userInput += c.KeyChar;
            }
        }

        // Handles event simulation
        // elapsedTime: how much time has elapsed since the last update
        public void update(TimeSpan elapsedTime)
        {
            int elapsedTimeMillis = (int) elapsedTime.TotalMilliseconds;

            // create this temp list to keep track of which events should be removed 
            List<GameEvent> eventsToRemove = new List<GameEvent>();

            // loop through all game events, see which ones need to be updated / removed
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

            // now, see if user input is needed to be read
            if (processUserInput)
            {
                // divide user input by spaces into a string array
                string[] inputSplit = userInput.Split();

                if (inputSplit[0].Equals("quit"))
                {
                    Environment.Exit(0);
                } 
                else if (inputSplit[0].Equals("create") && inputSplit[1].Equals("event"))
                {
                    // proper user input for creating an event looks like:
                    // create event NAME [duration (ms)] [count]
                    // create event ExampleEvent 1000 4
                    try
                    {
                        string eventName;
                        int eventDuration;
                        int eventCount;

                        eventName = inputSplit[2];
                        eventDuration = int.Parse(inputSplit[3]);
                        eventCount = int.Parse(inputSplit[4]);

                        // once all necessary information is gathered from gameloop, create the event
                        GameEventsList.Add(new GameEvent()
                        {
                            Name = eventName,
                            DurationMillis = eventDuration,
                            Count = eventCount
                        });
                    }
                    catch
                    {
                        // do nothing on exception
                    }
                }

                // reset the input flags and stored user input
                processUserInput = false;
                userInput = "";
            }
        }

        // Events fired are displayed in this method
        public void render()
        {
            // use this bool to determine whether to show [cmd:] text or not
            bool showedLine = false;

            // render all events that need to be rendered
            foreach (GameEvent ge in eventsToRender)
            {
                showedLine = true;
                Console.WriteLine("\n\tEvent: " + ge.Name + " (" + ge.Count + " remaining)");
            }
            eventsToRender.Clear();

            // show [cmd:] line if user pressed Enter or if the game loop rendered something
            if (userPressedEnter)
            {
                Console.Write("\n[cmd:] " + userInput);
                userPressedEnter = false;
            }
            else if (showedLine)
            {
                Console.Write("[cmd:] " + userInput);
            }
        }
    }
}
