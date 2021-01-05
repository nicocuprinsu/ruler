namespace Util.Geometry.Trapezoidal
{
    using System;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using Util.Geometry.Polygon;
    using Util.Math;

    public class TrapezoidalDecomposition
    {
        private List<LineSegment> segments;
        private Trapezoid boundingBox;
        private TrapezoidalMap trapezoidalMap;

        public TrapezoidalDecomposition(IEnumerable<LineSegment> a_segments)
        {
            segments = new List<LineSegment>(a_segments.ToArray());
            boundingBox = RectToTrapezoid(BoundingBoxComputer.FromSegments(a_segments));
            trapezoidalMap = new TrapezoidalMap(boundingBox);

            segments.Shuffle();

            foreach (LineSegment seg in segments)
            {
                FollowSegment(seg);
                //check if
            }
        }

        private void ComputeDecomposition()
        {

        }

        private List<Trapezoid> FollowSegment(LineSegment seg)
        {
            return new List<Trapezoid>();
        }

        private Trapezoid RectToTrapezoid(Rect r)
        {
            LineSegment top = new LineSegment(new PolarPoint2D(new Vector2(r.xMin, r.yMax)), new PolarPoint2D(new Vector2(r.xMax, r.yMax)));
            LineSegment bottom = new LineSegment(new PolarPoint2D(new Vector2(r.xMin, r.yMin)), new PolarPoint2D(new Vector2(r.xMax, r.yMin)));
            Vector2 leftp = new Vector2(r.xMin, r.yMax);
            Vector2 rightp = new Vector2(r.xMax, r.yMax);

            return new Trapezoid(top, bottom, leftp, rightp, new LinkedList<Trapezoid>());
        }
    }

    static class Utils
    {
        private static System.Random rng = new System.Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
