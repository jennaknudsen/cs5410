namespace Midterm.InputHandling
{
    public class MouseButton : Button
    {
        // an array of all bound keyboard keys to this Button
        public (float x, float y) StartPosition;
        public (float x, float y) EndPosition;

        // whether mouse is hovering over this button or not
        public bool IsHovered = false;

        // constructor
        public MouseButton((float x, float y) startPosition,
            (float x, float y) endPosition, bool isDebouncingButton) : base(isDebouncingButton)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
        }
    }
}