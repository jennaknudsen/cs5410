namespace Maze
{
    public class MazeSquare
    {
        /*
         A MazeSquare has the following:
         Four walls
         An ID number (for the disjoint set)
         PartOfCurrentSolution property
         Visited property

         A Wall has the following:
         MazeEdge property (whether it is at the edge of the maze or not)
         Enabled / Disabled property (whether the wall exists or not)
         FirstSquareRef / SecondSquareRef properties (which two MazeSquares
                    the wall divides)
        */

        public Wall TopWall = null;
        public Wall LeftWall = null;
        public Wall RightWall = null;
        public Wall BottomWall = null;

        public int ID { get; }

        // These should be public and modifiable by all other classes
        public bool PartOfOriginalSolution = false;
        public bool PartOfCurrentSolution = false;
        public bool Visited = false;

        // This constructor leaves the walls as null
        // Assuming that they will get filled in later
        public MazeSquare(int id)
        {
            ID = id;
        }

        /*
         Inner class representing a Wall
        */
        public class Wall
        {
            // Use enums to hold WallStatus and Orientation
            public enum WallStatus
            {
                ENABLED,
                DISABLED,
                EDGE
            }

            public enum Orientation
            {
                HORIZONTAL,
                VERTICAL
            }

            public WallStatus wallStatus;
            public Orientation orientation;

            public MazeSquare firstSquareRef;
            public MazeSquare secondSquareRef;

            public Wall(WallStatus wallStatus, Orientation orientation,
                        MazeSquare firstSquareRef, MazeSquare secondSquareRef)
            {
                this.wallStatus = wallStatus;
                this.orientation = orientation;
                this.firstSquareRef = firstSquareRef;
                this.secondSquareRef = secondSquareRef;
            }
        }
    }
}
