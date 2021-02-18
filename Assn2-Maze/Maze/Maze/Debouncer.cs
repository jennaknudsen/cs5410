namespace Maze
{
    public class Debouncer
    {
        // simple class implements a debouncer:
        // can only press the button *once* before it is released. (won't support press + hold)
        private bool _isButtonPressed = false;

        // returns True/False depending on whether button press was successful
        public bool Press()
        {
            if (_isButtonPressed)
                return false;
            _isButtonPressed = true;
            return true;
        }

        // release the button (self explanatory)
        public void Release()
        {
            _isButtonPressed = false;
        }
    }
}