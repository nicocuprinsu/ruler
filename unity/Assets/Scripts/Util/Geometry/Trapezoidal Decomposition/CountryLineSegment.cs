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
        }

        public bool IsRightOf(Vector2 point)
        {
            return Segment.IsRightOf(point);
        }
    }
}
