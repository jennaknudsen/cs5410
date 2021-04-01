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

        // generates a piece and its starting position, given a piece type
        public static Piece GeneratePiece(PieceType pieceType)
        {
            var thisPiece = new Piece
            {
                // all pieces are generated in the Down orientation
                Orientation = Up,
                Type = pieceType
            };

            PieceColor thisColor;

            switch (pieceType)
            {
                case I:
                    thisColor = Indigo;
                    // for the I, spawn it in Left orientation, for better rotation
                    thisPiece.Orientation = Left;
                    thisPiece.Squares[0] = new Square((4, 19), thisColor);
                    thisPiece.Squares[1] = new Square((4, 18), thisColor);
                    thisPiece.Squares[2] = new Square((4, 17), thisColor);
                    thisPiece.Squares[3] = new Square((4, 16), thisColor);
                    break;
                case J:
                    thisColor = Blue;
                    thisPiece.Squares[0] = new Square((4, 19), thisColor);
                    thisPiece.Squares[1] = new Square((4, 18), thisColor);
                    thisPiece.Squares[2] = new Square((5, 18), thisColor);
                    thisPiece.Squares[3] = new Square((6, 18), thisColor);
                    break;
                case L:
                    thisColor = Orange;
                    thisPiece.Squares[0] = new Square((6, 19), thisColor);
                    thisPiece.Squares[1] = new Square((6, 18), thisColor);
                    thisPiece.Squares[2] = new Square((5, 18), thisColor);
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
                    thisPiece.Squares[0] = new Square((5, 19), thisColor);
                    thisPiece.Squares[1] = new Square((4, 18), thisColor);
                    thisPiece.Squares[2] = new Square((5, 18), thisColor);
                    thisPiece.Squares[3] = new Square((6, 18), thisColor);
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

        // gets correct orientation after rotation, given clockwise or c-clockwise
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
            new PieceRotationProperty(J, Up, 0, (1, 3)),
            new PieceRotationProperty(J, Up, 1, (1, 2)),
            new PieceRotationProperty(J, Up, 2, (2, 2)),
            new PieceRotationProperty(J, Up, 3, (3, 2)),
            new PieceRotationProperty(J, Right, 0, (3, 3)),
            new PieceRotationProperty(J, Right, 1, (2, 3)),
            new PieceRotationProperty(J, Right, 2, (2, 2)),
            new PieceRotationProperty(J, Right, 3, (2, 1)),
            new PieceRotationProperty(J, Down, 0, (1, 2)),
            new PieceRotationProperty(J, Down, 1, (2, 2)),
            new PieceRotationProperty(J, Down, 2, (3, 2)),
            new PieceRotationProperty(J, Down, 3, (3, 1)),
            new PieceRotationProperty(J, Left, 0, (1, 1)),
            new PieceRotationProperty(J, Left, 1, (2, 1)),
            new PieceRotationProperty(J, Left, 2, (2, 2)),
            new PieceRotationProperty(J, Left, 3, (2, 3)),
            new PieceRotationProperty(L, Up, 0, (3, 3)),
            new PieceRotationProperty(L, Up, 1, (3, 2)),
            new PieceRotationProperty(L, Up, 2, (2, 2)),
            new PieceRotationProperty(L, Up, 3, (1, 2)),
            new PieceRotationProperty(L, Right, 0, (2, 3)),
            new PieceRotationProperty(L, Right, 1, (2, 2)),
            new PieceRotationProperty(L, Right, 2, (2, 1)),
            new PieceRotationProperty(L, Right, 3, (3, 1)),
            new PieceRotationProperty(L, Down, 0, (1, 2)),
            new PieceRotationProperty(L, Down, 1, (2, 2)),
            new PieceRotationProperty(L, Down, 2, (3, 2)),
            new PieceRotationProperty(L, Down, 3, (1, 1)),
            new PieceRotationProperty(L, Left, 0, (1, 3)),
            new PieceRotationProperty(L, Left, 1, (2, 3)),
            new PieceRotationProperty(L, Left, 2, (2, 2)),
            new PieceRotationProperty(L, Left, 3, (2, 1)),
            new PieceRotationProperty(O, Up, 0, (0, 0)),
            new PieceRotationProperty(O, Up, 1, (0, 0)),
            new PieceRotationProperty(O, Up, 2, (0, 0)),
            new PieceRotationProperty(O, Up, 3, (0, 0)),
            new PieceRotationProperty(O, Right, 0, (0, 0)),
            new PieceRotationProperty(O, Right, 1, (0, 0)),
            new PieceRotationProperty(O, Right, 2, (0, 0)),
            new PieceRotationProperty(O, Right, 3, (0, 0)),
            new PieceRotationProperty(O, Down, 0, (0, 0)),
            new PieceRotationProperty(O, Down, 1, (0, 0)),
            new PieceRotationProperty(O, Down, 2, (0, 0)),
            new PieceRotationProperty(O, Down, 3, (0, 0)),
            new PieceRotationProperty(O, Left, 0, (0, 0)),
            new PieceRotationProperty(O, Left, 1, (0, 0)),
            new PieceRotationProperty(O, Left, 2, (0, 0)),
            new PieceRotationProperty(O, Left, 3, (0, 0)),
            new PieceRotationProperty(S, Up, 0, (2, 3)),
            new PieceRotationProperty(S, Up, 1, (3, 3)),
            new PieceRotationProperty(S, Up, 2, (1, 2)),
            new PieceRotationProperty(S, Up, 3, (2, 2)),
            new PieceRotationProperty(S, Right, 0, (2, 3)),
            new PieceRotationProperty(S, Right, 1, (2, 2)),
            new PieceRotationProperty(S, Right, 2, (3, 2)),
            new PieceRotationProperty(S, Right, 3, (3, 1)),
            new PieceRotationProperty(S, Down, 0, (2, 2)),
            new PieceRotationProperty(S, Down, 1, (3, 2)),
            new PieceRotationProperty(S, Down, 2, (1, 1)),
            new PieceRotationProperty(S, Down, 3, (2, 1)),
            new PieceRotationProperty(S, Left, 0, (1, 3)),
            new PieceRotationProperty(S, Left, 1, (1, 2)),
            new PieceRotationProperty(S, Left, 2, (2, 2)),
            new PieceRotationProperty(S, Left, 3, (2, 1)),
            new PieceRotationProperty(T, Up, 0, (2, 3)),
            new PieceRotationProperty(T, Up, 1, (1, 2)),
            new PieceRotationProperty(T, Up, 2, (2, 2)),
            new PieceRotationProperty(T, Up, 3, (3, 2)),
            new PieceRotationProperty(T, Right, 0, (2, 3)),
            new PieceRotationProperty(T, Right, 1, (2, 2)),
            new PieceRotationProperty(T, Right, 2, (3, 2)),
            new PieceRotationProperty(T, Right, 3, (2, 1)),
            new PieceRotationProperty(T, Down, 0, (1, 2)),
            new PieceRotationProperty(T, Down, 1, (2, 2)),
            new PieceRotationProperty(T, Down, 2, (3, 2)),
            new PieceRotationProperty(T, Down, 3, (2, 1)),
            new PieceRotationProperty(T, Left, 0, (2, 3)),
            new PieceRotationProperty(T, Left, 1, (1, 2)),
            new PieceRotationProperty(T, Left, 2, (2, 2)),
            new PieceRotationProperty(T, Left, 3, (2, 1)),
            new PieceRotationProperty(Z, Up, 0, (1, 3)),
            new PieceRotationProperty(Z, Up, 1, (2, 3)),
            new PieceRotationProperty(Z, Up, 2, (2, 2)),
            new PieceRotationProperty(Z, Up, 3, (3, 2)),
            new PieceRotationProperty(Z, Right, 0, (3, 3)),
            new PieceRotationProperty(Z, Right, 1, (2, 2)),
            new PieceRotationProperty(Z, Right, 2, (3, 2)),
            new PieceRotationProperty(Z, Right, 3, (2, 1)),
            new PieceRotationProperty(Z, Down, 0, (1, 2)),
            new PieceRotationProperty(Z, Down, 1, (2, 2)),
            new PieceRotationProperty(Z, Down, 2, (2, 1)),
            new PieceRotationProperty(Z, Down, 3, (3, 1)),
            new PieceRotationProperty(Z, Left, 0, (2, 3)),
            new PieceRotationProperty(Z, Left, 1, (1, 2)),
            new PieceRotationProperty(Z, Left, 2, (2, 2)),
            new PieceRotationProperty(Z, Left, 3, (1, 1))
        };

        // this gets a list of translation vectors for four squares in a rotation
        public List<(int x, int y)> GetRotationMatrix(bool isCounterClockwise)
        {
            // iterate through list until we get all of the properties we need
            var oldList = new List<PieceRotationProperty>();
            var newList = new List<PieceRotationProperty>();

            // get rotated orientation
            var rotatedOrientation = GetCorrectOrientation(isCounterClockwise);

            // for each rotation property, add it to the correct list if applicable
            foreach (var property in PieceRotationProperties)
            {
                if (property.PieceType == Type && property.PieceOrientation == Orientation)
                {
                    oldList.Add(property);
                }
                else if (property.PieceType == Type && property.PieceOrientation == rotatedOrientation)
                {
                    newList.Add(property);
                }
            }

            // return list will hold the difference between the original list coordinates and
            // the return list coordinates
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