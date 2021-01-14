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
        public readonly Trapezoid BoundingBox;
        public readonly TrapezoidalMap TrapezoidalMap;
        public readonly SearchGraph SearchGraph;

        public TrapezoidalDecomposition(IEnumerable<CountryLineSegment> a_segments)
        {
            Segments = new List<CountryLineSegment>(a_segments.ToArray());
            BoundingBox = RectToTrapezoid(BoundingBoxComputer.FromSegments(a_segments.Select(e => e.Segment).ToList()));
            TrapezoidalMap = new TrapezoidalMap(BoundingBox);
            SearchGraph = new SearchGraph(BoundingBox);

            Segments.Shuffle();

            foreach (CountryLineSegment seg in Segments)
            {
                
                
                List<Node> oldTrapezoids = FollowSegment(seg);

                foreach (Node n in oldTrapezoids)
                {
                    TrapezoidalMap.Trapezoids.Remove((Trapezoid)n.Value);
                }

                List<Trapezoid> newTrapezoids = SearchGraph.Update(ref oldTrapezoids, seg);
                TrapezoidalMap.AddTrapezoids(newTrapezoids);
            }
        }

        private List<Node> FollowSegment(CountryLineSegment seg)
        {
            List<Node> result = new List<Node>();
            Node trapezoidNode = SearchGraph.Search(seg);
            Trapezoid currentTrapezoid = (Trapezoid)trapezoidNode.Value;
            result.Add(trapezoidNode);
            while (seg.Point2.x > currentTrapezoid.RightPoint.x)
            {
                if (seg.isAbove(currentTrapezoid.RightPoint) == 1)
                {
                    // rightp is above segment
                    currentTrapezoid = currentTrapezoid.LowerRightNeighbor;
                }
                else
                {
                    // rightp is below segment
                    currentTrapezoid = currentTrapezoid.UpperRightNeighbor;
                }
                if (currentTrapezoid == null || !(currentTrapezoid.AssocNode.Value is Trapezoid))
                {
                    Debug.Log("This shouldn't happen... (trapezoid is null or linking is broken)");
                    break;
                }
                result.Add(currentTrapezoid.AssocNode);
            }
            return result;
        }

        private Trapezoid RectToTrapezoid(Rect r)
        {
            CountryLineSegment top = new CountryLineSegment(new LineSegment(new Vector2(r.x - 0.1f, r.y + r.height + 0.1f), new Vector2(r.x + r.width + 0.1f, r.y + r.height + 0.1f)));
            CountryLineSegment bottom = new CountryLineSegment(new LineSegment(new Vector2(r.x - 0.1f, r.y - 0.1f), new Vector2(r.x + r.width + 0.1f, r.y - 0.1f)));
            Vector2 leftp = top.Point1;
            Vector2 rightp = bottom.Point2;

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
