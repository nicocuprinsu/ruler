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
        private readonly List<CountryLineSegment> Segments;
        private readonly Trapezoid BoundingBox;
        private readonly TrapezoidalMap TrapezoidalMap;
        private readonly SearchGraph SearchGraph;

        public TrapezoidalDecomposition(IEnumerable<CountryLineSegment> a_segments)
        {
            Segments = new List<CountryLineSegment>(a_segments.ToArray());
            BoundingBox = RectToTrapezoid(BoundingBoxComputer.FromSegments(a_segments.Select(e => e.Segment).ToList()));
            TrapezoidalMap = new TrapezoidalMap(BoundingBox);
            SearchGraph = new SearchGraph(BoundingBox);

            Segments.Shuffle();

            foreach (CountryLineSegment seg in Segments)
            {
                Node oldTrapezoid = SearchGraph.Search(seg.Point1);
                List<Node> oldTrapezoids = FollowSegment(seg);

                // add to search graph & map
                List<Trapezoid> newTrapezoids = SearchGraph.Update(oldTrapezoids, seg);
                TrapezoidalMap.AddTrapezoids(newTrapezoids);
            }
        }

        private List<Node> FollowSegment(CountryLineSegment seg)
        {
            List<Node> result = new List<Node>();
            Trapezoid currentTrapezoid = (Trapezoid) SearchGraph.Search(seg.Point1).Value;
            result.Add(currentTrapezoid.AssocNode);

            while (seg.Point2.x > currentTrapezoid.RightPoint.x)
            {
                if (!seg.IsRightOf(currentTrapezoid.RightPoint))
                {
                    // rightp is above segment
                    currentTrapezoid = currentTrapezoid.LowerRightNeighbor;
                } else
                {
                    // rightp is below segment
                    currentTrapezoid = currentTrapezoid.UpperRightNeighbor;
                }
                result.Add(currentTrapezoid.AssocNode);
            }

            return result;
        }

        private Trapezoid RectToTrapezoid(Rect r)
        {
            CountryLineSegment top = new CountryLineSegment(new LineSegment(new Vector2(r.xMin, r.yMax), new Vector2(r.xMax, r.yMax)));
            CountryLineSegment bottom = new CountryLineSegment(new LineSegment(new Vector2(r.xMin, r.yMin), new Vector2(r.xMax, r.yMin)));
            Vector2 leftp = new Vector2(r.xMin, r.yMax);
            Vector2 rightp = new Vector2(r.xMax, r.yMax);

            return new Trapezoid(top, bottom, leftp, rightp, null, null, null, null);
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
