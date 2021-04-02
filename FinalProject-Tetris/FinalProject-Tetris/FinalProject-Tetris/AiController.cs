namespace FinalProject_Tetris
{
    public class AiController
    {
        private TetrisGameController _tetrisGameController;

        public AiController(TetrisGameController tetrisGameController)
        {
            _tetrisGameController = tetrisGameController;
        }

        // this code based off of resources from:
        // https://codemyroad.wordpress.com/2013/04/14/tetris-ai-the-near-perfect-player/
        // https://github.com/LeeYiyuan/tetrisai

        // gets the total height of all columns
        private float AggregateHeight(Square[,] boardSquares)
        {
            var runningTotal = 0;

            for (var col = 0; col < 10; col++)
            {
                for (var row = 19; row >= 0; row--)
                {
                    if (boardSquares[col, row] != null)
                    {
                        runningTotal += row + 1;
                        break;
                    }
                }
            }

            return runningTotal;
        }

        // gets total count of complete lines
        private float CompleteLines(Square[,] boardSquares)
        {
            var completeLineCount = 0;

            // iterate for each row, if it has an empty square then don't add it
            for (var row = 0; row < 20; row++)
            {
                var rowFull = true;
                for (var col = 0; col < 10; col++)
                {
                    if (boardSquares[col, row] == null)
                    {
                        rowFull = false;
                        break;
                    }
                }

                if (rowFull)
                    completeLineCount++;
            }

            return completeLineCount;
        }

        private float Holes(Square[,] boardSquares)
        {
            var holesCount = 0;

            return holesCount;
        }

    }
}