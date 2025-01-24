using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cv1
{
    public class GeometryUtils
    {
        /// <summary>
        /// Simplifies a curve using the Ramer–Douglas–Peucker algorithm.
        /// </summary>
        /// <param name="points">List of points defining the curve.</param>
        /// <param name="epsilon">Threshold distance for simplification.</param>
        /// <returns>Simplified list of points.</returns>
        public static List<Point> RamerDouglasPeucker(List<Point> points, double epsilon)
        {
            if (points == null || points.Count < 3)
                return points;

            int index = -1;
            double maxDist = 0;

            // Find the point with the maximum distance
            for (int i = 1; i < points.Count - 1; i++)
            {
                double dist = PerpendicularDistance(points[i], points[0], points[points.Count - 1]);
                if (dist > maxDist)
                {
                    index = i;
                    maxDist = dist;
                }
            }

            // If max distance is greater than epsilon, recursively simplify
            if (maxDist > epsilon)
            {
                // Recursive call
                List<Point> recResults1 = RamerDouglasPeucker(points.GetRange(0, index + 1), epsilon);
                List<Point> recResults2 = RamerDouglasPeucker(points.GetRange(index, points.Count - index), epsilon);

                // Build the result list
                List<Point> result = new List<Point>(recResults1);
                result.AddRange(recResults2.GetRange(1, recResults2.Count - 1));
                return result;
            }
            else
            {
                // If max distance is less than epsilon, return start and end points
                return new List<Point> { points[0], points[points.Count - 1] };
            }
        }

        /// <summary>
        /// Calculates the perpendicular distance from a point to a line segment.
        /// </summary>
        private static double PerpendicularDistance(Point pt, Point lineStart, Point lineEnd)
        {
            double dx = lineEnd.X - lineStart.X;
            double dy = lineEnd.Y - lineStart.Y;

            if (dx == 0 && dy == 0)
            {
                // It's a point not a line segment.
                dx = pt.X - lineStart.X;
                dy = pt.Y - lineStart.Y;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            double numerator = Math.Abs(dy * pt.X - dx * pt.Y + lineEnd.X * lineStart.Y - lineEnd.Y * lineStart.X);
            double denominator = Math.Sqrt(dx * dx + dy * dy);
            return numerator / denominator;
        }
    }
}
