namespace Util.Geometry.Trapezoidal
{
    using System;
    using UnityEngine;

    public class Node {
        private System.Object value;
        public Node LeftChild { get; set;  }
        public Node RightChild { get; set; }

        public System.Object Value {
            get { return value; }
            set
            {
                if (!(value is Vector2) && !(value is LineSegment) && !(value is Trapezoid))
                {
                    throw new Exception("Node value should be a point, segment or trapezoid");
                }
                this.value = value;
            }
        }

        public Node(System.Object value)
        {
            if (!(value is Vector2) && !(value is LineSegment) && !(value is Trapezoid))
            {
                throw new Exception("Node value should be a point, segment or trapezoid");
            }

            this.value = value;
        }
    }

    public enum NodeType
    {
        Point,
        Segment,
        Trapezoid
    }
}
