namespace FinalProject_Tetris
{
    public class Square
    {
        // holds the PieceLocation (on the 10x20 board coordinate system)
        public (int x, int y) PieceLocation;

        // holds the piece's color
        public PieceColor Color;

        // holds enumerated list of piece colors
        public enum PieceColor
        {
            Red,
            Orange,
            Yellow,
            Green,
            Blue,
            Indigo,
            Violet,
            Gray
        }

        // used in Free Fall mode to determine which group a square is in
        public int SquareGroup = 0;

        // constructor
        public Square((int x, int y) pieceLocation, PieceColor color)
        {
            PieceLocation = pieceLocation;
            Color = color;
        }
    }
}