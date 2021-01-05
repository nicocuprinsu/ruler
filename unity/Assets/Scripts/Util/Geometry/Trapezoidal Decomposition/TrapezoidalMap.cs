namespace Util.Geometry.Trapezoidal
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using Util.Geometry.Polygon;
    using Util.Math;

    public class TrapezoidalMap
    {
        private LinkedList<Trapezoid> trapezoids;

        public TrapezoidalMap(Trapezoid init)
        {
            trapezoids = new LinkedList<Trapezoid>();
            trapezoids.AddLast(init);
        }

        public void AddTrapezoid(Trapezoid trapezoid)
        {
            trapezoids.AddLast(trapezoid);
        }
    }
}
