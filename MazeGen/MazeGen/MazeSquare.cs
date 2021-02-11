﻿using System;
namespace MazeGen
{
    public class MazeSquare
    {
        /*
         A MazeSquare has the following:
         Four walls
         An ID number (for the disjoint set)

         A Wall has the following: 
         MazeEdge property (whether it is at the edge of the maze or not)
         Enabled / Disabled property (whether the wall exists or not)
         FirstSquareRef / SecondSquareRef properties (which two MazeSquares
                    the wall divides)
        */

        // walls and ID should be read only to other classes
        public Wall TopWall { get; }
        public Wall LeftWall { get; }
        public Wall RightWall { get; }
        public Wall BottomWall { get; }

        public int ID { get; }

        public MazeSquare(Wall topWall, Wall leftWall, Wall rightWall,
                          Wall bottomWall, int id)
        {
            TopWall = topWall;
            LeftWall = leftWall;
            RightWall = rightWall;
            BottomWall = bottomWall;
        }

        public class Wall
        {
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
