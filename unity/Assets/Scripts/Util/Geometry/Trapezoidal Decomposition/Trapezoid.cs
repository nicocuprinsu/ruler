namespace Util.Geometry.Trapezoidal
{
    using UnityEngine;
    using System.Collections.Generic;

    public class Trapezoid
    {
        private readonly LineSegment top;
        private readonly LineSegment bottom;
        private readonly Vector2 leftp;
        private readonly Vector2 rightp;
        private readonly LinkedList<Trapezoid> neighbors;

        public LineSegment Top { get { return top; } }
        public LineSegment Bottom { get { return bottom; } }
        public Vector2 LeftPoint { get { return leftp; } }
        public Vector2 RightPoint { get { return rightp; } }
        public ICollection<Trapezoid> Neighbors { get { return neighbors; } }

        public Trapezoid(LineSegment top, LineSegment bottom, Vector2 leftp, Vector2 rightp, LinkedList<Trapezoid> neighbors)
        {
            this.top = top;
            this.bottom = bottom;
            this.leftp = leftp;
            this.rightp = rightp;
            this.neighbors = neighbors;
        }
    }
}
