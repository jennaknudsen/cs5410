using static FinalProject_Tetris.Piece.PieceOrientation;
using static FinalProject_Tetris.Piece.PieceType;
using static FinalProject_Tetris.Square;
using static FinalProject_Tetris.Square.PieceColor;

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

        public static Piece GeneratePiece(PieceType pieceType)
        {
            var thisPiece = new Piece
            {
                // all pieces are generated in the Down orientation
                Orientation = Down,
                Type = pieceType
            };

            PieceColor thisColor;

            switch (pieceType)
            {
                case I:
                    thisColor = Indigo;
                    thisPiece.Squares[0] = new Square((3, 19), thisColor);
                    thisPiece.Squares[1] = new Square((4, 19), thisColor);
                    thisPiece.Squares[2] = new Square((5, 19), thisColor);
                    thisPiece.Squares[3] = new Square((6, 19), thisColor);
                    break;
                case J:
                    thisColor = Blue;
                    thisPiece.Squares[0] = new Square((4, 19), thisColor);
                    thisPiece.Squares[1] = new Square((5, 19), thisColor);
                    thisPiece.Squares[2] = new Square((6, 19), thisColor);
                    thisPiece.Squares[3] = new Square((6, 18), thisColor);
                    break;
                case L:
                    thisColor = Orange;
                    thisPiece.Squares[0] = new Square((4, 19), thisColor);
                    thisPiece.Squares[1] = new Square((5, 19), thisColor);
                    thisPiece.Squares[2] = new Square((6, 19), thisColor);
                    thisPiece.Squares[3] = new Square((4, 18), thisColor);
                    break;
                case O:
                    thisColor = Yellow;
                    thisPiece.Squares[0] = new Square((4, 19), thisColor);
                    thisPiece.Squares[1] = new Square((5, 19), thisColor);
                    thisPiece.Squares[2] = new Square((4, 18), thisColor);
                    thisPiece.Squares[3] = new Square((5, 18), thisColor);
                    break;
                case S:
                    thisColor = Green;
                    thisPiece.Squares[0] = new Square((5, 19), thisColor);
                    thisPiece.Squares[1] = new Square((6, 19), thisColor);
                    thisPiece.Squares[2] = new Square((4, 18), thisColor);
                    thisPiece.Squares[3] = new Square((5, 18), thisColor);
                    break;
                case T:
                    thisColor = Violet;
                    thisPiece.Squares[0] = new Square((4, 19), thisColor);
                    thisPiece.Squares[1] = new Square((5, 19), thisColor);
                    thisPiece.Squares[2] = new Square((6, 19), thisColor);
                    thisPiece.Squares[3] = new Square((5, 18), thisColor);
                    break;
                case Z:
                    thisColor = Red;
                    thisPiece.Squares[0] = new Square((4, 19), thisColor);
                    thisPiece.Squares[1] = new Square((5, 19), thisColor);
                    thisPiece.Squares[2] = new Square((5, 18), thisColor);
                    thisPiece.Squares[3] = new Square((6, 18), thisColor);
                    break;
            }

            return thisPiece;
        }
    }
}