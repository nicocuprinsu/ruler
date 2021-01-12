namespace Util.Geometry.Trapezoidal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SearchGraph
    {
        private readonly Node RootNode;

        public SearchGraph(Trapezoid rootTrapezoid)
        {
            RootNode = new Node(rootTrapezoid);
        }

        public Node Search(Vector2 query)
        {
            Node currentNode = RootNode;

            while (true)
            {
                if (currentNode.Value is Trapezoid)
                {
                    return currentNode;
                } else if (currentNode.Value is Vector2)
                {
                    if (query.x <= ((Vector2) currentNode.Value).x)
                    {
                        currentNode = currentNode.LeftChild;
                    }
                    else
                    {
                        currentNode = currentNode.RightChild;
                    }
                }
                else if (currentNode.Value is CountryLineSegment)
                {
                    if (!(((CountryLineSegment) currentNode.Value).IsRightOf(query)))
                    {
                        // point is above segment
                        currentNode = currentNode.LeftChild;
                    }
                    else
                    {
                        // point is below segment
                        currentNode = currentNode.RightChild;
                    }
                }
            }
        }

        public List<Trapezoid> Update(List<Node> oldTrapezoids, CountryLineSegment seg)
        {
            List<Trapezoid> newTrapezoids = new List<Trapezoid>();
            Vector2 emptyVector = new Vector2();
            Debug.Log(oldTrapezoids[0].Value.GetType());
            Trapezoid top = (Trapezoid)oldTrapezoids[0].Value;
            Trapezoid bottom = (Trapezoid)oldTrapezoids[0].Value;

            for (int c = 0; c < oldTrapezoids.Count; c++)
            {
                // old trapezoid to be replaced by new trapezoids
                Trapezoid old = (Trapezoid)oldTrapezoids[c].Value;

                if (c == 0 && old.LeftPoint.x < seg.Point1.x)
                {
                    Trapezoid left;
                    // left trapezoid
                    if (old.LeftPoint.x < seg.Point1.x)
                    {
                        left = new Trapezoid(old.Top, old.Bottom, old.LeftPoint, seg.Point1, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null);
                        
                    } else
                    {
                        // three-point trapezoid
                        left = old.UpperLeftNeighbor;
                    }

                    // right point is set to empty vector for the moment as we do not know yet when the trapezoid will end (same for right neighbors)
                    top = new Trapezoid(old.Top, seg, seg.Point1, emptyVector, left, left, null, null);
                    bottom = new Trapezoid(seg, old.Bottom, seg.Point1, emptyVector, left, left, null, null);
                    left.UpperRightNeighbor = top;
                    left.LowerRightNeighbor = bottom;

                    // replace trapezoid node by (left) segment point node
                    oldTrapezoids[c].Value = seg.Point1;

                    // add appropriate children
                    if (old.LeftPoint.x < seg.Point1.x)
                    {
                        oldTrapezoids[c].LeftChild = left.AssocNode;
                    } 
                    Node segNode = new Node(seg);
                    oldTrapezoids[c].RightChild = segNode;
                    segNode.LeftChild = top.AssocNode;
                    segNode.RightChild = bottom.AssocNode;

                    // add to new trapezoids list
                    if (old.LeftPoint.x < seg.Point1.x)
                    {
                        newTrapezoids.Add(left);
                    }
                    newTrapezoids.Add(top);
                    newTrapezoids.Add(bottom);
                }
                else if (c == oldTrapezoids.Count - 1 && old.RightPoint.x < seg.Point2.x)
                {
                    // right trapezoid
                    Trapezoid right;
                    if (old.RightPoint.x < seg.Point2.x)
                    {
                        right = new Trapezoid(old.Top, old.Bottom, seg.Point2, old.RightPoint, null, null, old.UpperRightNeighbor, old.LowerRightNeighbor);
                    } else
                    {
                        right = old.UpperRightNeighbor;
                    }
                   

                    // top and bottom trapezoids are already created, we just need to fill in the missing fields
                    top.RightPoint = seg.Point2;
                    top.UpperRightNeighbor = right;
                    top.LowerRightNeighbor = right;
                    bottom.RightPoint = seg.Point2;
                    bottom.UpperRightNeighbor = right;
                    bottom.LowerRightNeighbor = right;

                    right.UpperLeftNeighbor = top;
                    right.LowerLeftNeighbor = bottom;

                    // replace trapezoid node by (right) segment point node
                    oldTrapezoids[c].Value = seg.Point2;

                    //add appropriate children
                    if (old.RightPoint.x < seg.Point2.x)
                    {
                        oldTrapezoids[c].RightChild = right.AssocNode;
                    }
                    Node segNode = new Node(seg);
                    oldTrapezoids[c].LeftChild = segNode;
                    segNode.LeftChild = top.AssocNode;
                    segNode.RightChild = bottom.AssocNode;

                    // top and bottom were already added in a previous step
                    if (old.RightPoint.x < seg.Point2.x)
                    {
                        newTrapezoids.Add(right);
                    }
                }
                else
                {
                    oldTrapezoids[c].Value = seg;
                    oldTrapezoids[c].LeftChild = top.AssocNode;
                    oldTrapezoids[c].RightChild = bottom.AssocNode;
                    if (!seg.IsRightOf(old.RightPoint))
                    {
                        // right point of current trapezoid is above segment
                        top.RightPoint = old.RightPoint;

                        // prepare next 'top' trapezoid
                        Trapezoid newTop = new Trapezoid(old.Top, seg, old.RightPoint, emptyVector, top, top, null, null);
                        top.UpperRightNeighbor = newTop;
                        top.LowerRightNeighbor = newTop;

                        top = newTop;
                        newTrapezoids.Add(top);
                    }
                    else
                    {
                        // right point of current trapezoid is below segment
                        bottom.RightPoint = old.RightPoint;

                        // prepare next 'bottom' trapezoid
                        Trapezoid newBottom = new Trapezoid(seg, old.Bottom, old.RightPoint, emptyVector, bottom, bottom, null, null);
                        bottom.UpperRightNeighbor = newBottom;
                        bottom.LowerRightNeighbor = newBottom;

                        bottom = newBottom;
                        newTrapezoids.Add(bottom);
                    }
                }
            }

            return newTrapezoids;
        }

        //public List<Trapezoid> Update(List<Node> oldTrapezoids, CountryLineSegment seg)
        //{
        //    List<Trapezoid> newTrapezoids = new List<Trapezoid>();
        //    if (oldTrapezoids.Count == 1)
        //    {
        //        Trapezoid old = (Trapezoid)oldTrapezoids[0].Value;
        //        oldTrapezoids[0].Value = seg.Point1.x <= seg.Point2.x ? seg.Point1 : seg.Point2;
        //        Node rightp = new Node(seg.Point1.x <= seg.Point2.x ? seg.Point2 : seg.Point1);
        //        Node segNode = new Node(seg);

        //        oldTrapezoids[0].RightChild = rightp;
        //        rightp.LeftChild = segNode;

        //        Trapezoid l = new Trapezoid(old.Top, old.Bottom, old.LeftPoint, seg.Point1, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null);
        //        newTrapezoids.Add(l);
        //        oldTrapezoids[0].LeftChild = l.AssocNode;

        //        Trapezoid t = new Trapezoid(old.Top, seg, seg.Point1, seg.Point2, l, l, null, null);
        //        l.UpperRightNeighbor = t;
        //        newTrapezoids.Add(t);
        //        segNode.LeftChild = t.AssocNode;

        //        Trapezoid b = new Trapezoid(seg, old.Bottom, seg.Point1, seg.Point2, l, l, null, null);
        //        l.LowerRightNeighbor = b;
        //        newTrapezoids.Add(b);
        //        segNode.LeftChild = b.AssocNode;

        //        Trapezoid r = new Trapezoid(old.Top, old.Bottom, seg.Point2, old.RightPoint, t, b, old.UpperRightNeighbor, old.LowerRightNeighbor);
        //        t.UpperRightNeighbor = r;
        //        t.LowerRightNeighbor = r;
        //        b.UpperRightNeighbor = r;
        //        b.LowerRightNeighbor = r;
        //        newTrapezoids.Add(r);
        //        rightp.RightChild = r.AssocNode;

        //        // update right neighbor(s) to new trapezoids
        //        old.UpperRightNeighbor.UpperLeftNeighbor = r;
        //        old.UpperRightNeighbor.LowerLeftNeighbor = r;
        //        old.LowerRightNeighbor.UpperLeftNeighbor = r;
        //        old.LowerRightNeighbor.LowerLeftNeighbor = r;
        //    } else
        //    {
        //        // counter for all 'old' trapezoids
        //        int c = 0;

        //        Trapezoid old = (Trapezoid)oldTrapezoids[0].Value;
        //        Trapezoid lastTop = old;
        //        Trapezoid lastBottom = old;

        //        while (c < oldTrapezoids.Count)
        //        {
        //            old = (Trapezoid)oldTrapezoids[c].Value;





        //            if (c == 0 && seg.Point1.x < old.LeftPoint.x)
        //            {
        //                oldTrapezoids[c].Value = seg.Point1;
        //                Node segNode = new Node(seg);
        //                oldTrapezoids[c].RightChild = segNode;

        //                Trapezoid l = new Trapezoid(old.Top, old.Bottom, old.LeftPoint, seg.Point1, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null);
        //                newTrapezoids.Add(l);
        //                oldTrapezoids[c].LeftChild = l.AssocNode;

        //            }
        //            if (c == 0 && seg.Point1.x < old.LeftPoint.x)
        //            {
        //                oldTrapezoids[c].Value = seg.Point1.x <= seg.Point2.x ? seg.Point1 : seg.Point2;
        //                Node rightp = new Node(seg.Point1.x <= seg.Point2.x ? seg.Point2 : seg.Point1);
        //                Node segNode = new Node(seg);

        //                oldTrapezoids[c].RightChild = rightp;
        //                rightp.LeftChild = segNode;

        //                Trapezoid l = new Trapezoid(old.Top, old.Bottom, old.LeftPoint, seg.Point1, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null);
        //                newTrapezoids.Add(l);
        //                oldTrapezoids[0].LeftChild = l.AssocNode;

        //                Trapezoid t = new Trapezoid(old.Top, seg, seg.Point1, old.RightPoint, l, l, null, null);
        //                l.UpperRightNeighbor = t;
        //                // we add it when the trapezoid is actually done
        //                //newTrapezoids.Add(t);
        //                segNode.LeftChild = t.AssocNode;

        //                Trapezoid b = new Trapezoid(seg, old.Bottom, seg.Point1, old.RightPoint, l, l, null, null);
        //                l.LowerRightNeighbor = b;
        //                // we add it when the trapezoid is actually done
        //                //newTrapezoids.Add(b);
        //                segNode.RightChild = b.AssocNode;

        //                lastTop = t;
        //                lastBottom = b;
        //            } else if (c == oldTrapezoids.Count - 1 && seg.Point2.x < old.RightPoint.x)
        //            {
        //                oldTrapezoids[c].Value = seg.Point2;
        //                Node segNode = new Node(seg);

        //                oldTrapezoids[c].LeftChild = segNode;

        //                lastTop.RightPoint = seg.Point2;
        //                newTrapezoids.Add(lastTop);
        //                segNode.LeftChild = lastTop.AssocNode;

        //                lastBottom.RightPoint = seg.Point2;
        //                newTrapezoids.Add(lastBottom);
        //                segNode.RightChild = lastBottom.AssocNode;

        //                Trapezoid r = new Trapezoid(old.Top, old.Bottom, seg.Point2, old.RightPoint, lastTop, lastBottom, old.UpperRightNeighbor, old.LowerRightNeighbor);
        //                newTrapezoids.Add(r);
        //                oldTrapezoids[c].RightChild = r.AssocNode;
        //                lastTop.UpperRightNeighbor = r;
        //                lastTop.LowerRightNeighbor = r;
        //                lastBottom.UpperRightNeighbor = r;
        //                lastBottom.LowerRightNeighbor = r;

        //                // update right neighbor(s) to new trapezoids
        //                old.UpperRightNeighbor.UpperLeftNeighbor = r;
        //                old.UpperRightNeighbor.LowerLeftNeighbor = r;
        //                old.LowerRightNeighbor.UpperLeftNeighbor = r;
        //                old.LowerRightNeighbor.LowerLeftNeighbor = r;
        //            } else
        //            {
        //                Node segNode = new Node(seg);
        //                oldTrapezoids[c].Value = segNode;
        //                if (!seg.IsRightOf(old.RightPoint))
        //                {
        //                    // rightp of old trapezoid is above segment
        //                    lastTop.RightPoint = old.RightPoint;
        //                    lastTop.UpperRightNeighbor = old.UpperRightNeighbor;
        //                    lastTop.LowerRightNeighbor = old.LowerRightNeighbor;

        //                    newTrapezoids.Add(lastTop);
        //                    segNode.LeftChild = lastTop.AssocNode;
        //                    segNode.RightChild = lastBottom.AssocNode;
        //                } else
        //                {
        //                    // rightp of old trapezoid is below segment
        //                    lastBottom.RightPoint = old.RightPoint;
        //                    lastBottom.UpperRightNeighbor = old.UpperRightNeighbor;
        //                    lastBottom.LowerRightNeighbor = old.LowerRightNeighbor;

        //                    newTrapezoids.Add(lastBottom);
        //                    segNode.LeftChild = lastTop.AssocNode;
        //                    segNode.RightChild = lastBottom.AssocNode;
        //                }
        //            }

        //            // go to next intersected trapezoid
        //            c++;
        //        }
        //    }

        //    return newTrapezoids;
        //}
    }
}
