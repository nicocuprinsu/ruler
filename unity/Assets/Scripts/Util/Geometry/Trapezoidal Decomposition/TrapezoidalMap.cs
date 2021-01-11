namespace Util.Geometry.Trapezoidal
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using Util.Geometry.Polygon;
    using Util.Math;

    public class TrapezoidalMap
    {
        private List<Trapezoid> Trapezoids;

        public TrapezoidalMap(Trapezoid init)
        {
            Trapezoids = new List<Trapezoid>();
            Trapezoids.Add(init);
        }

        public void AddTrapezoid(Trapezoid trapezoid)
        {
            Trapezoids.Add(trapezoid);
        }

        public void AddTrapezoids(List<Trapezoid> trapezoids)
        {
            Trapezoids.AddRange(trapezoids);
        }
    }
}
