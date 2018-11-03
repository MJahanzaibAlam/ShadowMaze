using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace ShadowMaze
{
    // This class will be used to draw lines from shadow copies to determine line of sight
    // and also for lighting from lamps.
    public class LightRay
    {
        private Vector2 v2End; // Stores the ending point of the line.
        private List<Point> listPoints; // Stores the list of points from the start to the end point.

        public Vector2 End
        {
            get { return v2End; }
        }

        public List<Point> Points
        {
            get { return listPoints; }
        }

        // Constructor to set the starting and ending positions of the line upon creation of the light ray.
        public LightRay(Vector2 v2CastPoint, Vector2 v2Direction, int intDistanceToCast, GridLayer grid)
        {
            Point PointStart = new Point((int)v2CastPoint.X, (int)v2CastPoint.Y); // Stores the start of the line.
            Vector2 v2EndPoint = v2CastPoint + v2Direction * intDistanceToCast; // Sets the end vector2 according to direction.

            Point PointEnd = new Point((int)v2EndPoint.X, (int)v2EndPoint.Y); // Stores the end point according to direction of the line.
            v2End = IntersectionPoint(grid, PointStart, PointEnd); // Calculate the line points using the start and end points.
        }

        // Finds the position where the line has intersected a non-traversable object (where the line has ended).
        private Vector2 IntersectionPoint(GridLayer grid, Point PointStart, Point PointEnd)
        {
            // Creates a line of points using the Bresenham's line algorithm.
            List<Point> Line = BresenhamLine(PointStart.X, PointStart.Y, PointEnd.X, PointEnd.Y);
            listPoints = new List<Point>();

            Vector2 EndPoint = new Vector2(PointEnd.X, PointEnd.Y); // Contains the position where the line has ended.

            for (int intPointIndex = 0; intPointIndex < Line.Count(); intPointIndex++)
            {
                Node nodeToCheck = grid.GetNode(Line[intPointIndex]); // Retrieve the node which the current point is within.

                if (!nodeToCheck.Traversable)
                { // If the node within which the point is located is not traversable, the intersection point has been found.
                    EndPoint = new Vector2(Line[intPointIndex].X, Line[intPointIndex].Y);

                    Line.RemoveRange(intPointIndex, Line.Count - intPointIndex); // Remove any excess points past the end point.
                    break;
                    // Then exit the loop as a point of intersection has been found (where the line must end).
                }
            }

            // Adds one point for every 40 points in the original list.
            for (int intListIndex = 0; intListIndex < Line.Count(); intListIndex += 40)
                listPoints.Add(Line[intListIndex]);

            return EndPoint;
        }
        


        // Returns a list of points containing the points between the starting and ending position of the line.
        private List<Point> BresenhamLine(int intStartX, int intStartY, int  intEndX, int intEndY)
        {
            listPoints = new List<Point>();
            int intXDiff = intEndX - intStartX;
            int intYDiff = intEndY - intStartY; // Calculate the difference in x and y values.
            int intDeltaX1 = 0, intDeltaY1 = 0, intDeltaX2 = 0, intDeltaY2 = 0;
            if (intXDiff < 0) intDeltaX1 = -1;
            else if (intXDiff > 0) intDeltaX1 = 1;

            // Sets the x and y change (delta x, delta y) based on whether the second x and y values were less than the first.
            // (If the difference in x was negative, the start of the line was further along the screen than the end).
            // (If the difference in y was negative, the start of the line was further down the screen than the end).
            if (intYDiff < 0) intDeltaY1 = -1; else if (intYDiff > 0) intDeltaY1 = 1;
            if (intXDiff < 0) intDeltaX2 = -1; else if (intXDiff > 0) intDeltaX2 = 1;
            int intLongest = Math.Abs(intXDiff);
            int intShortest = Math.Abs(intYDiff); // Calculate the absolute difference in x and y values (for distances).
            // If the difference in x was less than the difference in y, swap the longest and shortest distances.
            if (!(intLongest > intShortest))
            {
                intLongest = Math.Abs(intYDiff);
                intShortest = Math.Abs(intXDiff);
                if (intYDiff < 0) intDeltaY2 = -1;
                else if (intYDiff > 0) intDeltaY2 = 1;

                intDeltaX2 = 0;
            }
            // The above is needed to make the line algorithm work (as it works with steep lines).
            int intError = intLongest >> 1;
            for (int i = 0; i <= intLongest; i++)
            {
                listPoints.Add(new Point(intStartX, intStartY));
                intError += intShortest;
                if (!(intError < intLongest))
                {
                    intError -= intLongest;
                    intStartX += intDeltaX1;
                    intStartY += intDeltaY1;
                }
                else
                {
                    intStartX += intDeltaX2;
                    intStartY += intDeltaY2;
                }
            }
            return listPoints;
        }

        // Swaps two integers which are passed in by reference.
        private void SwapIntegers(ref int intA, ref int intB)
        {
            int intTemp = intB;
            intB = intA;
            intA = intTemp;
        }
        
    }
}
