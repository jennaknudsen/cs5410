using System.Collections.Generic;
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
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3
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

        public PieceOrientation GetCorrectOrientation(bool isCounterClockwise)
        {
            PieceOrientation rotatedOrientation;
            if (isCounterClockwise)
            {
                if (Orientation == Up)
                    rotatedOrientation = Left;
                else
                    rotatedOrientation = Orientation - 1;
            }
            else
            {
                if (Orientation == Left)
                    rotatedOrientation = Up;
                else
                    rotatedOrientation = Orientation + 1;
            }

            return rotatedOrientation;
        }

        // List that holds all of the piece positions
        // Piece Type -> Piece Orientation -> Index -> (x, y) coordinates
        public readonly List<PieceRotationProperty> PieceRotationProperties = new List<PieceRotationProperty>
        {
            new PieceRotationProperty(I, Up, 0, (0, 2)),
            new PieceRotationProperty(I, Up, 1, (1, 2)),
            new PieceRotationProperty(I, Up, 2, (2, 2)),
            new PieceRotationProperty(I, Up, 3, (3, 2)),
            new PieceRotationProperty(I, Right, 0, (2, 3)),
            new PieceRotationProperty(I, Right, 1, (2, 2)),
            new PieceRotationProperty(I, Right, 2, (2, 1)),
            new PieceRotationProperty(I, Right, 3, (2, 0)),
            new PieceRotationProperty(I, Down, 0, (0, 1)),
            new PieceRotationProperty(I, Down, 1, (1, 1)),
            new PieceRotationProperty(I, Down, 2, (2, 1)),
            new PieceRotationProperty(I, Down, 3, (3, 1)),
            new PieceRotationProperty(I, Left, 0, (1, 3)),
            new PieceRotationProperty(I, Left, 1, (1, 2)),
            new PieceRotationProperty(I, Left, 2, (1, 1)),
            new PieceRotationProperty(I, Left, 3, (1, 0)),
        };

        // this gets a list of translation vectors for four squares in a rotation
        public List<(int x, int y)> GetRotationMatrix(bool isCounterClockwise)
        {
            // iterate through list until we get all of the properties we need
            var oldList = new List<PieceRotationProperty>();
            var newList = new List<PieceRotationProperty>();

            // get rotated orientation
            var rotatedOrientation = GetCorrectOrientation(isCounterClockwise);

            foreach (var property in PieceRotationProperties)
            {
                // add the correct properties
                if (property.PieceType == Type && property.PieceOrientation == Orientation)
                {
                    oldList.Add(property);
                }
                else if (property.PieceType == Type && property.PieceOrientation == rotatedOrientation)
                {
                    newList.Add(property);
                }
            }

            var returnList = new List<(int x, int y)>();
            for (var i = 0; i < 4; i++)
            {
                var diffX = newList[i].Position.x - oldList[i].Position.x;
                var diffY = newList[i].Position.y - oldList[i].Position.y;

                returnList.Add((diffX, diffY));
            }

            return returnList;
        }

        // this simple class holds a data row in our list of piece rotation properties
        public class PieceRotationProperty
        {
            public PieceType PieceType;
            public PieceOrientation PieceOrientation;
            public int Index;
            public (int x, int y) Position;

            public PieceRotationProperty(PieceType pieceType, PieceOrientation pieceOrientation,
                int index, (int x, int y) position)
            {
                PieceType = pieceType;
                PieceOrientation = pieceOrientation;
                Index = index;
                Position = position;
            }
        }
    }
}