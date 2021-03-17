namespace LunarLander
{
    public enum GameState
    {
        Paused,
        ShipCrashed,
        PassedLevel,
        BeatGame,
        Running
    }

    public enum MenuState
    {
        Main,
        Controls,
        HighScores,
        Credits
    }

    public class MenuItem
    {
        public string ItemName;
        public bool Selected;

        public MenuItem(string itemName, bool selected)
        {
            ItemName = itemName;
            Selected = selected;
        }
    }
}