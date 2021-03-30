namespace FinalProject_Tetris
{
    public class Piece
    {
        // array holds all four squares
        public Square[] Squares = new Square[4];

        // holds the orientation
        public PieceOrientation Orientation;

        // holds the piece type
        public PieceType Type;

        // the orientation of the pieces
        public enum PieceOrientation
        {
            Up,
            Left,
            Right,
            Down
        }

        // the formation of the pieces
        public enum PieceType
        {
            I,
            J,
            L,
            O,
            S,
            T,
            Z
        }
    }
}