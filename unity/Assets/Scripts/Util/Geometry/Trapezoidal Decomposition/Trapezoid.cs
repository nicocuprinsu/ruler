namespace Util.Geometry.Trapezoidal
{
    using UnityEngine;
    using System.Collections.Generic;

    public class Trapezoid
    {
        public CountryLineSegment Top { get; set; }
        public CountryLineSegment Bottom { get; set; }
        public Vector2 LeftPoint { get; set; }
        public Vector2 RightPoint { get; set; }
        public Trapezoid UpperLeftNeighbor { get; set; }
        public Trapezoid LowerLeftNeighbor { get; set; }
        public Trapezoid UpperRightNeighbor { get; set; }
        public Trapezoid LowerRightNeighbor { get; set; }
        public Node AssocNode { get; set; }

        public Trapezoid(CountryLineSegment top, CountryLineSegment bottom, Vector2 leftp, Vector2 rightp, Trapezoid upperLeftNeighbor, Trapezoid lowerLeftNeighbor, Trapezoid upperRightNeighbor, Trapezoid lowerRightNeighbor)
        {
            AssocNode = new Node(this);
            Top = top;
            Bottom = bottom;
            LeftPoint = leftp;
            RightPoint = rightp;
            UpperLeftNeighbor = upperLeftNeighbor;
            LowerLeftNeighbor = lowerLeftNeighbor;
            UpperRightNeighbor = upperRightNeighbor;
            LowerRightNeighbor = lowerRightNeighbor;
        }
    }
}
