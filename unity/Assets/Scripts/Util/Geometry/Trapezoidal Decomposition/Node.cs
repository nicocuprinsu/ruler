namespace Util.Geometry.Trapezoidal
{
    using UnityEngine;

    public abstract class Node { }

    public abstract class GenericNode<T> : Node
    {
        protected T val;
        public T Value { get { return val; } }
    }

    public class XNode : GenericNode<Vector2>
    {
        public XNode(Vector2 val)
        {
            this.val = val;
        }
    }

    public class YNode : GenericNode<LineSegment>
    {
        public YNode(LineSegment val)
        {
            this.val = val;
        }
    }

    public class TrapezoidNode : GenericNode<Trapezoid>
    {
        public TrapezoidNode(Trapezoid val)
        {
            this.val = val;
        }
    }
}
