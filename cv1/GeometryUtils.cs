﻿namespace cv1
{
    public class GeometryUtils
    {
        /// <summary>
        /// Zjednodusenie polygonu pomocou Ramer-Douglas-Peucker algoritmu.
        /// </summary>
        /// <param name="points">List bodov definované krivkou</param>
        /// <param name="epsilon">Prahová hodnota vzdialenosti pre zjednodušenie</param>
        /// <returns>List zjednodušených bodov</returns>
        public static List<Point> RamerDouglasPeucker(List<Point> points, double epsilon)
        {
            if (points == null || points.Count < 3)
                return points;

            int index = -1;
            double maxDist = 0;

            // Hladanie bodu s najvacsiou vzdialenostou
            for (int i = 1; i < points.Count - 1; i++)
            {
                double dist = PerpendicularDistance(points[i], points[0], points[points.Count - 1]);
                if (dist > maxDist)
                {
                    index = i;
                    maxDist = dist;
                }
            }

            // Ak je vzdialenost vacsia ako epsilon, rekurzivne volanie
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
                // Ak nie je, vrati zaciatocny a koncovy bod
                return new List<Point> { points[0], points[points.Count - 1] };
            }
        }

        /// <summary>
        /// Vypočíta vzdialenosť bodu od úsečky.
        /// </summary>
        private static double PerpendicularDistance(Point pt, Point lineStart, Point lineEnd)
        {
            double dx = lineEnd.X - lineStart.X;
            double dy = lineEnd.Y - lineStart.Y;

            if (dx == 0 && dy == 0)
            {
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
