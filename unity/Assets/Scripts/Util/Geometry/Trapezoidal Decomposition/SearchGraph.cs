namespace Util.Geometry.Trapezoidal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SearchGraph
    {
        private Dictionary<Node, LinkedList<Node>> adjList = new Dictionary<Node, LinkedList<Node>>();

        public SearchGraph(Trapezoid rootTrapezoid)
        {
            TrapezoidNode root = new TrapezoidNode(rootTrapezoid);
            adjList.Add(root, new LinkedList<Node>());
        }

        public void Search(Vector2 query)
        {

        }

        public void Update(Node oldTrapezoid, LineSegment seg, LinkedList<Trapezoid> newTrapezoids)
        {
            XNode leftp = new XNode(seg.Point1.x <= seg.Point2.x ? seg.Point1 : seg.Point2);
            XNode rightp = new XNode(seg.Point1.x <= seg.Point2.x ? seg.Point2 : seg.Point1);
            YNode segNode = new YNode(seg);

            oldTrapezoid = leftp;

            this.Add(oldTrapezoid, rightp);
            this.Add(rightp, segNode);

            foreach (Trapezoid t in newTrapezoids)
            {
                if (t.LeftPoint.x < leftp.Value.x)
                {
                    this.Add(leftp, new TrapezoidNode(t));
                } else if (t.RightPoint.x > rightp.Value.x)
                {
                    this.Add(rightp, new TrapezoidNode(t));
                } else
                {
                    this.Add(segNode, new TrapezoidNode(t));
                }
            }
        }

        public void Update(LinkedList<Node> oldTrapezoids, LineSegment seg, LinkedList<Trapezoid> newTrapezoids)
        {

        }

        public void Add(Node prevNode, Node node)
        {
            if (!adjList.ContainsKey(prevNode))
            {
                throw new System.Exception("Tried to add a successor to an invalid node.");
            } else if (!adjList[prevNode].Contains(node))
            {
                adjList[prevNode].AddLast(node);
            }
        }

        public void Remove(TrapezoidNode node)
        {
            adjList.Remove(node);
            foreach (LinkedList<Node> l in adjList.Values)
            {
                l.Remove(node);
            }
        }
    }
}
