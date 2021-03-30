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

        // converter from PieceColor to string
        public static string GetColor(PieceColor pieceColor)
        {
            return pieceColor switch
            {
                PieceColor.Red => "red",
                PieceColor.Orange => "orange",
                PieceColor.Yellow => "yellow",
                PieceColor.Green => "green",
                PieceColor.Blue => "blue",
                PieceColor.Indigo => "indigo",
                PieceColor.Violet => "violet",
                _ => ""
            };
        }
    }
}