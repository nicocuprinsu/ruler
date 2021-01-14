namespace Util.Geometry.Trapezoidal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class CountryLineSegment
    {

        public LineSegment Segment { get; set; }
        public string Country { get; set; }

        public Vector2 Point1 { get { return Segment.Point1; } }
        public Vector2 Point2 { get { return Segment.Point2; } }

        public CountryLineSegment(LineSegment seg)
        {
            Segment = seg;
            Country = "null";
        }

        public CountryLineSegment(Vector2 point1, Vector2 point2, string country)
        {
            if (point1.x < point2.x)
            {
                Segment = new LineSegment(point1, point2);
            } else
            {
                Segment = new LineSegment(point2, point1);
            }
            Country = country;
        }

        public bool IsRightOf(Vector2 point)
        {
            return !Segment.Line.PointAbove(point);
        }

        public int isAbove(Vector2 point)
        {
            if (Segment.IsEndpoint(point))
            {
                return 0;
            } else if (Segment.Line.PointAbove(point))
            {
                return 1;
            } else
            {
                return -1;
            }
        }
    }
}
