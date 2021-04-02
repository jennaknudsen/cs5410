namespace FinalProject_Tetris.InputHandling
{
    public class MouseButton : Button
    {
        // an array of all bound keyboard keys to this Button
        public (int x, int y) StartPosition;
        public (int x, int y) EndPosition;

        // whether mouse is hovering over this button or not
        public bool IsHovered = false;

        // constructor
        public MouseButton((int x, int y) startPosition,
            (int x, int y) endPosition, bool isDebouncingButton) : base(isDebouncingButton)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
        }
    }
}