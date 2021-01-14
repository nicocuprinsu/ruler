namespace Util.Geometry.Trapezoidal
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SearchGraph
    {
        public readonly Node RootNode;

        public SearchGraph(Trapezoid rootTrapezoid)
        {
            RootNode = new Node(rootTrapezoid);
        }

        public Node Search(CountryLineSegment seg)
        {
            Node currentNode = RootNode;
            while (true)
            {
                if (currentNode.Value is Trapezoid)
                {
                    return currentNode;
                }
                else if (currentNode.Value is Vector2)
                {
                    if (seg.Point1.x < ((Vector2)currentNode.Value).x)
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
                    if (((CountryLineSegment)currentNode.Value).isAbove(seg.Point1) == 1) {
                        currentNode = currentNode.LeftChild;
                    }
                    else if (((CountryLineSegment)currentNode.Value).isAbove(seg.Point1) == -1)
                    {
                        currentNode = currentNode.RightChild;
                    } else
                    {
                        if (((CountryLineSegment)currentNode.Value).isAbove(seg.Point2) == 1)
                        {
                            currentNode = currentNode.LeftChild;
                        } else
                        {
                            currentNode = currentNode.RightChild;
                        }
                    }
                }
            }
        }

        //public void PrintTree()
        //{
        //    Debug.Log(RootNode.Value);
        //    if (RootNode.LeftChild != null)
        //    {
        //        Debug.Log("Left: " + RootNode.LeftChild.Value);
        //    }
        //    if (RootNode.RightChild != null)
        //    {
        //        Debug.Log("Right: " + RootNode.RightChild.Value);
        //    }

        //    if (RootNode.LeftChild.LeftChild != null)
        //    {
        //        Debug.Log("Left.Left: " + RootNode.LeftChild.LeftChild.Value);
        //        if (RootNode.LeftChild.LeftChild.LeftChild != null)
        //        {
        //            Debug.Log("Left.Left.Left: " + RootNode.LeftChild.LeftChild.LeftChild.Value);
        //        }
        //        if (RootNode.LeftChild.LeftChild.RightChild != null)
        //        {
        //            Debug.Log("Left.Left.Right: " + RootNode.LeftChild.LeftChild.RightChild.Value);
        //        }
        //    }
        //    if (RootNode.LeftChild.RightChild != null)
        //    {
        //        Debug.Log("Left.Right: " + RootNode.LeftChild.RightChild.Value);
        //        if (RootNode.LeftChild.RightChild.LeftChild != null)
        //        {
        //            Debug.Log("Left.Right.Left: " + RootNode.LeftChild.RightChild.LeftChild.Value);
        //        }
        //        if (RootNode.LeftChild.RightChild.RightChild != null)
        //        {
        //            Debug.Log("Left.Right.Right: " + RootNode.LeftChild.RightChild.RightChild.Value);
        //        }
        //    }
        //    if (RootNode.RightChild.LeftChild != null)
        //    {
        //        Debug.Log("Right.Left: " + RootNode.RightChild.LeftChild.Value);
        //        if (RootNode.RightChild.LeftChild.LeftChild != null)
        //        {
        //            Debug.Log("Right.Left.Left: " + RootNode.RightChild.LeftChild.LeftChild.Value);
        //        }
        //        if (RootNode.RightChild.LeftChild.RightChild != null)
        //        {
        //            Debug.Log("Right.Left.Right: " + RootNode.RightChild.LeftChild.RightChild.Value);
        //        }
        //    }
        //    if (RootNode.RightChild.RightChild != null)
        //    {
        //        Debug.Log("Right.Right: " + RootNode.RightChild.RightChild.Value);
        //        if (RootNode.RightChild.RightChild.LeftChild != null)
        //        {
        //            Debug.Log("Right.Right.Left: " + RootNode.RightChild.RightChild.LeftChild.Value);
        //        }
        //        if (RootNode.RightChild.RightChild.RightChild != null)
        //        {
        //            Debug.Log("Right.Right.Right: " + RootNode.RightChild.RightChild.RightChild.Value);
        //        }
        //    }
        //}

        public Node Search(Vector2 query)
        {
            Node currentNode = RootNode;
            while (true)
            {
                if (currentNode.Value is Trapezoid)
                {
                    Trapezoid res = (Trapezoid)currentNode.Value;
                    //Debug.Log("Found trapezoid at " + res.Bottom.Country + ": " + res.LeftPoint + res.RightPoint + res.Top.Segment + res.Bottom.Segment);
                    return currentNode;
                } else if (currentNode.Value is Vector2)
                {
                    //Debug.Log(currentNode.Value);
                    if (query.x < ((Vector2) currentNode.Value).x)
                    {
                        //Debug.Log("-> LEFT");
                        currentNode = currentNode.LeftChild;
                    }
                    else
                    {
                        //Debug.Log("-> RIGHT");
                        currentNode = currentNode.RightChild;
                    }
                }
                else if (currentNode.Value is CountryLineSegment)
                {
                    //Debug.Log(((CountryLineSegment)currentNode.Value).Segment);
                    if (!(((CountryLineSegment) currentNode.Value).IsRightOf(query)))
                    {
                        //Debug.Log("-> ABOVE");
                        // point is above segment
                        currentNode = currentNode.LeftChild;
                    }
                    else
                    {
                        //Debug.Log("-> BELOW");
                        // point is below segment
                        currentNode = currentNode.RightChild;
                    }
                }
            }
        }

        public bool onSegment(CountryLineSegment seg, Vector2 point)
        {
            return seg.Segment.IsEndpoint(point);
        }

        public List<Trapezoid> Update(ref List<Node> oldTrapezoids, CountryLineSegment seg)
        {
            List<Trapezoid> newTrapezoids = new List<Trapezoid>();

            // trapezoids storing 'not fully computed' top and bottom trapezoids
            Trapezoid top = null;
            Trapezoid bottom = null;

            if (oldTrapezoids.Count == 1)
            {
                // only one intersected trapezoid
                Trapezoid old = (Trapezoid)oldTrapezoids[0].Value;

                // temporary trapezoids
                Trapezoid left = null;
                Trapezoid right = null;

                top = new Trapezoid(old.Top, seg, seg.Point1, seg.Point2, null, null, null, null);
                bottom = new Trapezoid(seg, old.Bottom, seg.Point1, seg.Point2, null, null, null, null);
                newTrapezoids.Add(top);
                newTrapezoids.Add(bottom);

                if (old.LeftPoint.x < seg.Point1.x)
                {
                    // there exists a 'left' trapezoid
                    left = new Trapezoid(old.Top, old.Bottom, old.LeftPoint, seg.Point1, old.UpperLeftNeighbor, old.LowerLeftNeighbor, top, bottom);
                    top.UpperLeftNeighbor = left;
                    top.LowerLeftNeighbor = left;
                    bottom.UpperLeftNeighbor = left;
                    bottom.LowerLeftNeighbor = left;

                    // update old's neighbors
                    if (old.LowerLeftNeighbor != null)
                    {
                        if (old.LowerLeftNeighbor.UpperRightNeighbor == old)
                        {
                            old.LowerLeftNeighbor.UpperRightNeighbor = left;
                        }
                        if (old.LowerLeftNeighbor.LowerRightNeighbor == old)
                        {
                            old.LowerLeftNeighbor.LowerRightNeighbor = left;
                        }
                    }
                    if (old.UpperLeftNeighbor != null)
                    {
                        if (old.UpperLeftNeighbor.UpperRightNeighbor == old)
                        {
                            old.UpperLeftNeighbor.UpperRightNeighbor = left;
                        }
                        if (old.UpperLeftNeighbor.LowerRightNeighbor == old)
                        {
                            old.UpperLeftNeighbor.LowerRightNeighbor = left;
                        }
                    }

                    // add created trapezoid
                    newTrapezoids.Add(left);
                }
                else
                {
                    // since we assume there are no two points with the same x,
                    // we know that seg starts on old.leftp now
                    if (old.Bottom.isAbove(seg.Point1) == 1 && old.Top.isAbove(seg.Point1) == -1)
                    {
                        // seg starts in 'middle'
                        top.UpperLeftNeighbor = old.UpperLeftNeighbor;
                        top.LowerLeftNeighbor = old.UpperLeftNeighbor;
                        bottom.UpperLeftNeighbor = old.LowerLeftNeighbor;
                        bottom.LowerLeftNeighbor = old.LowerLeftNeighbor;

                        // update old's neighbors
                        if (old.LowerLeftNeighbor != null)
                        {
                            if (old.LowerLeftNeighbor.UpperRightNeighbor == old)
                            {
                                Debug.Log("BAD POW POW");
                                old.LowerLeftNeighbor.UpperRightNeighbor = bottom;
                            }
                            if (old.LowerLeftNeighbor.LowerRightNeighbor == old)
                            {
                                old.LowerLeftNeighbor.LowerRightNeighbor = bottom;
                            }
                        }
                        if (old.UpperLeftNeighbor != null)
                        {
                            if (old.UpperLeftNeighbor.UpperRightNeighbor == old)
                            {
                                Debug.Log("POW POW");
                                old.UpperLeftNeighbor.UpperRightNeighbor = top;
                            }
                            else {
                                Debug.Log(old.UpperLeftNeighbor.UpperRightNeighbor.Bottom.Country);
                                Debug.Log("Good: " + old.UpperLeftNeighbor.UpperRightNeighbor.LeftPoint + old.UpperLeftNeighbor.UpperRightNeighbor.RightPoint + old.UpperLeftNeighbor.UpperRightNeighbor.Top.Segment + old.UpperLeftNeighbor.UpperRightNeighbor.Bottom.Segment);
                                Debug.Log(old.Bottom.Country);
                                Debug.Log("Good: " + old.LeftPoint + old.RightPoint + old.Top.Segment + old.Bottom.Segment);
                                Debug.Log("NOT POWPOW");
                            }
                            if (old.UpperLeftNeighbor.LowerRightNeighbor == old)
                            {
                                old.UpperLeftNeighbor.LowerRightNeighbor = top;
                            }
                        }
                    }
                    else if (old.Bottom.isAbove(seg.Point1) == 1)
                    {
                        // seg starts on 'old.top'
                        bottom.UpperLeftNeighbor = old.UpperLeftNeighbor;
                        bottom.LowerLeftNeighbor = old.LowerLeftNeighbor;

                        // update old's neighbors
                        if (old.LowerLeftNeighbor != null)
                        {
                            if (old.LowerLeftNeighbor.UpperRightNeighbor == old)
                            {
                                old.LowerLeftNeighbor.UpperRightNeighbor = bottom;
                            }
                            if (old.LowerLeftNeighbor.LowerRightNeighbor == old)
                            {
                                old.LowerLeftNeighbor.LowerRightNeighbor = bottom;
                            }
                        }
                        if (old.UpperLeftNeighbor != null)
                        {
                            if (old.UpperLeftNeighbor.UpperRightNeighbor == old)
                            {
                                old.UpperLeftNeighbor.UpperRightNeighbor = bottom;
                            }
                            if (old.UpperLeftNeighbor.LowerRightNeighbor == old)
                            {
                                old.UpperLeftNeighbor.LowerRightNeighbor = bottom;
                            }
                        }
                    }
                    else
                    {
                        // seg starts on 'old.bottom'
                        top.UpperLeftNeighbor = old.UpperLeftNeighbor;
                        top.LowerLeftNeighbor = old.LowerLeftNeighbor;

                        // update old's neighbors
                        if (old.LowerLeftNeighbor != null)
                        {
                            if (old.LowerLeftNeighbor.UpperRightNeighbor == old)
                            {
                                old.LowerLeftNeighbor.UpperRightNeighbor = top;
                            }
                            if (old.LowerLeftNeighbor.LowerRightNeighbor == old)
                            {
                                old.LowerLeftNeighbor.LowerRightNeighbor = top;
                            }
                        }
                        if (old.UpperLeftNeighbor != null)
                        {
                            if (old.UpperLeftNeighbor.UpperRightNeighbor == old)
                            {
                                old.UpperLeftNeighbor.UpperRightNeighbor = top;
                            }
                            if (old.UpperLeftNeighbor.LowerRightNeighbor == old)
                            {
                                old.UpperLeftNeighbor.LowerRightNeighbor = top;
                            }
                        }
                    }
                }

                if (seg.Point2.x < old.RightPoint.x)
                {
                    // there exists a 'right' trapezoid
                    right = new Trapezoid(old.Top, old.Bottom, seg.Point2, old.RightPoint, top, bottom, old.UpperRightNeighbor, old.LowerRightNeighbor);
                    top.UpperRightNeighbor = right;
                    top.LowerRightNeighbor = right;
                    bottom.UpperRightNeighbor = right;
                    bottom.LowerRightNeighbor = right;

                    // update old's neighbors
                    if (old.LowerRightNeighbor != null)
                    {
                        if (old.LowerRightNeighbor.UpperLeftNeighbor == old)
                        {
                            old.LowerRightNeighbor.UpperLeftNeighbor = right;
                        }
                        if (old.LowerRightNeighbor.LowerLeftNeighbor == old)
                        {
                            old.LowerRightNeighbor.LowerLeftNeighbor = right;
                        }
                    }
                    if (old.UpperRightNeighbor != null)
                    {
                        if (old.UpperRightNeighbor.UpperLeftNeighbor == old)
                        {
                            old.UpperRightNeighbor.UpperLeftNeighbor = right;
                        }
                        if (old.UpperRightNeighbor.LowerLeftNeighbor == old)
                        {
                            old.UpperRightNeighbor.LowerLeftNeighbor = right;
                        }
                    }
                    if (old.LowerLeftNeighbor != null)
                    {
                        if (old.LowerLeftNeighbor.UpperRightNeighbor == old)
                        {
                            old.LowerLeftNeighbor.UpperRightNeighbor = right;
                        }
                        if (old.LowerLeftNeighbor.LowerRightNeighbor == old)
                        {
                            old.LowerLeftNeighbor.LowerRightNeighbor = right;
                        }
                    }
                    if (old.UpperLeftNeighbor != null)
                    {
                        if (old.UpperLeftNeighbor.UpperRightNeighbor == old)
                        {
                            old.UpperLeftNeighbor.UpperRightNeighbor = right;
                        }
                        if (old.UpperLeftNeighbor.LowerRightNeighbor == old)
                        {
                            old.UpperLeftNeighbor.LowerRightNeighbor = right;
                        }
                    }

                    // add created trapezoid
                    newTrapezoids.Add(right);
                }
                else
                {
                    if (old.Bottom.isAbove(seg.Point2) == 1 && old.Top.isAbove(seg.Point2) == -1)
                    {
                        // seg ends in 'middle''
                        top.UpperRightNeighbor = old.UpperRightNeighbor;
                        top.LowerRightNeighbor = old.UpperRightNeighbor;
                        bottom.UpperRightNeighbor = old.LowerRightNeighbor;
                        bottom.LowerRightNeighbor = old.LowerRightNeighbor;

                        // update old's neighbors
                        if (old.LowerRightNeighbor != null)
                        {
                            old.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                            old.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                        }
                        if (old.UpperRightNeighbor != null)
                        {
                            old.UpperRightNeighbor.UpperLeftNeighbor = top;
                            old.UpperRightNeighbor.LowerLeftNeighbor = top;
                        }
                    }
                    else if (old.Bottom.isAbove(seg.Point2) == 1)
                    {
                        // seg ends on 'old.top'
                        bottom.UpperRightNeighbor = old.UpperRightNeighbor;
                        bottom.LowerRightNeighbor = old.LowerRightNeighbor;

                        // update old's neighbors
                        if (old.LowerRightNeighbor != null)
                        {
                            if (old.LowerRightNeighbor.UpperLeftNeighbor == old)
                            {
                                old.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                            }
                            if (old.LowerRightNeighbor.LowerLeftNeighbor == old)
                            {
                                old.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                            }
                        }
                        if (old.UpperRightNeighbor != null)
                        {
                            if (old.UpperRightNeighbor.UpperLeftNeighbor == old)
                            {
                                old.UpperRightNeighbor.UpperLeftNeighbor = bottom;
                            }
                            if (old.UpperRightNeighbor.LowerLeftNeighbor == old)
                            {
                                old.UpperRightNeighbor.LowerLeftNeighbor = bottom;
                            }
                        }
                    }
                    else
                    {
                        // seg ends on 'old.bottom'
                        top.UpperRightNeighbor = old.UpperRightNeighbor;
                        top.LowerRightNeighbor = old.LowerRightNeighbor;

                        // update old's neighbors
                        if (old.LowerRightNeighbor != null)
                        {
                            if (old.LowerRightNeighbor.UpperLeftNeighbor == old)
                            {
                                old.LowerRightNeighbor.UpperLeftNeighbor = top;
                            }
                            if (old.LowerRightNeighbor.LowerLeftNeighbor == old)
                            {
                                old.LowerRightNeighbor.LowerLeftNeighbor = top;
                            }
                        }
                        if (old.UpperRightNeighbor != null)
                        {
                            if (old.UpperRightNeighbor.UpperLeftNeighbor == old)
                            {
                                old.UpperRightNeighbor.UpperLeftNeighbor = top;
                            }
                            if (old.UpperRightNeighbor.LowerLeftNeighbor == old)
                            {
                                old.UpperRightNeighbor.LowerLeftNeighbor = top;
                            }
                        }
                    }
                }

                Node segNode = new Node(seg);
                segNode.LeftChild = top.AssocNode;
                segNode.RightChild = bottom.AssocNode;

                if (left != null)
                {
                    // replace trapezoid node by (left) segment point node
                    oldTrapezoids[0].Value = seg.Point1;
                    oldTrapezoids[0].LeftChild = left.AssocNode;
                    oldTrapezoids[0].RightChild = segNode;
                }
                if (right != null)
                {
                    if (left != null)
                    {
                        // add appropriate children
                        oldTrapezoids[0].RightChild = new Node(seg.Point2)
                        {
                            LeftChild = segNode,
                            RightChild = right.AssocNode
                        };
                    }
                    else
                    {
                        oldTrapezoids[0].Value = seg.Point2;
                        oldTrapezoids[0].LeftChild = segNode;
                        oldTrapezoids[0].RightChild = right.AssocNode;
                    }
                }

                if (left == null && right == null)
                {
                    oldTrapezoids[0].Value = seg;
                    oldTrapezoids[0].LeftChild = top.AssocNode;
                    oldTrapezoids[0].RightChild = bottom.AssocNode;
                }

                

                return newTrapezoids;
            }

            for (int c = 0; c < oldTrapezoids.Count; c++)
            {
                if (c == 0)
                {
                    // first trapezoid
                    Trapezoid old = (Trapezoid)oldTrapezoids[c].Value;

                    // temporary trapezoids
                    Trapezoid left = null;

                    top = new Trapezoid(old.Top, seg, seg.Point1, Vector2.one, null, null, null, null);
                    bottom = new Trapezoid(seg, old.Bottom, seg.Point1, Vector2.one, null, null, null, null);
                    newTrapezoids.Add(top);
                    newTrapezoids.Add(bottom);

                    if (old.LeftPoint.x < seg.Point1.x)
                    {
                        // there exists a 'left' trapezoid
                        left = new Trapezoid(old.Top, old.Bottom, old.LeftPoint, seg.Point1, old.UpperLeftNeighbor, old.LowerLeftNeighbor, top, bottom);
                        top.UpperLeftNeighbor = left;
                        top.LowerLeftNeighbor = left;
                        bottom.UpperLeftNeighbor = left;
                        bottom.LowerLeftNeighbor = left;

                        // update old's neighbors
                        if (old.LowerLeftNeighbor != null)
                        {
                            if (old.LowerLeftNeighbor.UpperRightNeighbor == old)
                            {
                                old.LowerLeftNeighbor.UpperRightNeighbor = left;
                            }
                            if (old.LowerLeftNeighbor.LowerRightNeighbor == old)
                            {
                                old.LowerLeftNeighbor.LowerRightNeighbor = left;
                            }
                        }
                        if (old.UpperLeftNeighbor != null)
                        {
                            if (old.UpperLeftNeighbor.UpperRightNeighbor == old)
                            {
                                old.UpperLeftNeighbor.UpperRightNeighbor = left;
                            }
                            if (old.UpperLeftNeighbor.LowerRightNeighbor == old)
                            {
                                old.UpperLeftNeighbor.LowerRightNeighbor = left;
                            }
                        }

                        // add created trapezoid
                        newTrapezoids.Add(left);
                    }
                    else
                    {
                        // since we assume there are no two points with the same x,
                        // we know that seg starts on old.leftp now
                        if (old.Bottom.isAbove(seg.Point1) == 1 && old.Top.isAbove(seg.Point1) == -1)
                        {
                            // seg starts in 'middle'
                            top.UpperLeftNeighbor = old.UpperLeftNeighbor;
                            top.LowerLeftNeighbor = old.UpperLeftNeighbor;
                            bottom.UpperLeftNeighbor = old.LowerLeftNeighbor;
                            bottom.LowerLeftNeighbor = old.LowerLeftNeighbor;

                            // update old's neighbors
                            if (old.LowerLeftNeighbor != null)
                            {
                                old.LowerLeftNeighbor.UpperRightNeighbor = bottom;
                                old.LowerLeftNeighbor.LowerRightNeighbor = bottom;
                            }
                            if (old.UpperLeftNeighbor != null)
                            {
                                old.UpperLeftNeighbor.UpperRightNeighbor = top;
                                old.UpperLeftNeighbor.LowerRightNeighbor = top;
                            }
                        }
                        else if (old.Bottom.isAbove(seg.Point1) == 1)
                        {
                            // seg starts on 'old.top'
                            bottom.UpperLeftNeighbor = old.UpperLeftNeighbor;
                            bottom.LowerLeftNeighbor = old.LowerLeftNeighbor;

                            // update old's neighbors
                            if (old.LowerLeftNeighbor != null)
                            {
                                if (old.LowerLeftNeighbor.UpperRightNeighbor == old)
                                {
                                    old.LowerLeftNeighbor.UpperRightNeighbor = bottom;
                                }
                                if (old.LowerLeftNeighbor.LowerRightNeighbor == old)
                                {
                                    old.LowerLeftNeighbor.LowerRightNeighbor = bottom;
                                }
                            }
                            if (old.UpperLeftNeighbor != null)
                            {
                                if (old.UpperLeftNeighbor.UpperRightNeighbor == old)
                                {
                                    old.UpperLeftNeighbor.UpperRightNeighbor = bottom;
                                }
                                if (old.UpperLeftNeighbor.LowerRightNeighbor == old)
                                {
                                    old.UpperLeftNeighbor.LowerRightNeighbor = bottom;
                                }
                            }
                        }
                        else
                        {
                            // seg starts on 'old.bottom'
                            top.UpperLeftNeighbor = old.UpperLeftNeighbor;
                            top.LowerLeftNeighbor = old.LowerLeftNeighbor;

                            // update old's neighbors
                            if (old.LowerLeftNeighbor != null)
                            {
                                if (old.LowerLeftNeighbor.UpperRightNeighbor == old)
                                {
                                    old.LowerLeftNeighbor.UpperRightNeighbor = top;
                                }
                                if (old.LowerLeftNeighbor.LowerRightNeighbor == old)
                                {
                                    old.LowerLeftNeighbor.LowerRightNeighbor = top;
                                }
                            }
                            if (old.UpperLeftNeighbor != null)
                            {
                                if (old.UpperLeftNeighbor.UpperRightNeighbor == old)
                                {
                                    old.UpperLeftNeighbor.UpperRightNeighbor = top;
                                }
                                if (old.UpperLeftNeighbor.LowerRightNeighbor == old)
                                {
                                    old.UpperLeftNeighbor.LowerRightNeighbor = top;
                                }
                            }
                        }
                    }

                    

                    if (left != null)
                    {
                        // replace trapezoid node by (left) segment point node
                        Node segNode = new Node(seg)
                        {
                            LeftChild = top.AssocNode,
                            RightChild = bottom.AssocNode
                        };
                        oldTrapezoids[c].Value = seg.Point1;
                        oldTrapezoids[c].LeftChild = left.AssocNode;
                        oldTrapezoids[c].RightChild = segNode;

                    }
                    else
                    {
                        // replace trapezoid node by segment node
                        oldTrapezoids[c].Value = seg;
                        oldTrapezoids[c].LeftChild = top.AssocNode;
                        oldTrapezoids[c].RightChild = bottom.AssocNode;
                    }

                }
                else if (c == oldTrapezoids.Count - 1)
                {
                    Trapezoid old = (Trapezoid)oldTrapezoids[c].Value;
                    Trapezoid right = null;

                    // 'finish' last top/bottom
                    if (seg.isAbove(old.LeftPoint) == 1)
                    {
                        // old leftp is above segment ==> 'top' is finished
                        Trapezoid newTop = new Trapezoid(old.Top, seg, old.LeftPoint, seg.Point2, top, top, null, null);
                        if (old.UpperLeftNeighbor != old.LowerLeftNeighbor)
                        {
                            newTop.UpperLeftNeighbor = old.UpperLeftNeighbor;
                        }
                        top.RightPoint = old.LeftPoint;
                        
                        top.LowerRightNeighbor = newTop;

                        if (old.UpperLeftNeighbor.UpperRightNeighbor == old)
                        {
                            top.UpperRightNeighbor = newTop;

                            // update old neighbors
                            old.UpperLeftNeighbor.UpperRightNeighbor = newTop;
                            old.UpperLeftNeighbor.LowerRightNeighbor = newTop;
                        }
                        else
                        {
                            // account for splitting into 'lower' trapezoid
                            top.UpperRightNeighbor = old.UpperLeftNeighbor.UpperRightNeighbor;
                        }

                        // update old neighbors
                        //if (old.UpperLeftNeighbor.UpperRightNeighbor != null)
                        //{
                        //    old.UpperLeftNeighbor.UpperRightNeighbor.UpperLeftNeighbor = top;
                        //    old.UpperLeftNeighbor.UpperRightNeighbor.LowerLeftNeighbor = top;
                        //    old.LowerLeftNeighbor.UpperRightNeighbor.UpperLeftNeighbor = top;
                        //    old.LowerLeftNeighbor.UpperRightNeighbor.LowerLeftNeighbor = top;
                        //}

                        // add new top
                        top = newTop;
                        newTrapezoids.Add(top);

                        // end trapezoid, so add rightp to 'bottom'
                        bottom.RightPoint = seg.Point2;
                    }
                    else
                    {
                        // old leftp is below segment ==> 'bottom' is finished
                        // maybe problem here with lower left?
                        Trapezoid newBottom = new Trapezoid(seg, old.Bottom, old.LeftPoint, seg.Point2, bottom, bottom, null, null);
                        if (old.UpperLeftNeighbor != old.LowerLeftNeighbor)
                        {
                            newBottom.LowerLeftNeighbor = old.LowerLeftNeighbor;
                        }

                        bottom.RightPoint = old.LeftPoint;
                        bottom.UpperRightNeighbor = newBottom;

                        if (old.LowerLeftNeighbor.LowerRightNeighbor == old)
                        {
                            bottom.LowerRightNeighbor = newBottom;

                            // update old neighbors
                            old.LowerLeftNeighbor.LowerRightNeighbor = newBottom;
                            old.LowerLeftNeighbor.UpperRightNeighbor = newBottom;
                        }
                        else
                        {
                            // account for splitting into 'higher' trapezoid
                            bottom.LowerRightNeighbor = old.LowerLeftNeighbor.LowerRightNeighbor;
                        }

                        // update old neighbors
                        //if (old.LowerLeftNeighbor.LowerRightNeighbor != null)
                        //{
                        //    old.LowerLeftNeighbor.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                        //    old.LowerLeftNeighbor.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                        //    old.UpperLeftNeighbor.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                        //    old.UpperLeftNeighbor.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                        //}

                        // add new bottom
                        bottom = newBottom;
                        newTrapezoids.Add(bottom);

                        // end trapezoid, so add rightp to 'top'
                        top.RightPoint = seg.Point2;
                    }

                    // last trapezoid
                    if (seg.Point2.x < old.RightPoint.x)
                    {
                        // there exists a 'right' trapezoid
                        right = new Trapezoid(old.Top, old.Bottom, seg.Point2, old.RightPoint, top, bottom, old.UpperRightNeighbor, old.LowerRightNeighbor);
                        top.UpperRightNeighbor = right;
                        top.LowerRightNeighbor = right;
                        bottom.UpperRightNeighbor = right;
                        bottom.LowerRightNeighbor = right;

                        // update old's neighbors
                        if (old.LowerRightNeighbor != null)
                        {
                            if (old.LowerRightNeighbor.UpperLeftNeighbor == old)
                            {
                                old.LowerRightNeighbor.UpperLeftNeighbor = right;
                            }
                            if (old.LowerRightNeighbor.LowerLeftNeighbor == old)
                            {
                                old.LowerRightNeighbor.LowerLeftNeighbor = right;
                            }
                        }
                        if (old.UpperRightNeighbor != null)
                        {
                            if (old.UpperRightNeighbor.UpperLeftNeighbor == old)
                            {
                                old.UpperRightNeighbor.UpperLeftNeighbor = right;
                            }
                            if (old.UpperRightNeighbor.LowerLeftNeighbor == old)
                            {
                                old.UpperRightNeighbor.LowerLeftNeighbor = right;
                            }
                        }
                        if (old.LowerLeftNeighbor != null)
                        {
                            if (old.LowerLeftNeighbor.UpperRightNeighbor == old)
                            {
                                old.LowerLeftNeighbor.UpperRightNeighbor = right;
                            }
                            if (old.LowerLeftNeighbor.LowerRightNeighbor == old)
                            {
                                old.LowerLeftNeighbor.LowerRightNeighbor = right;
                            }
                        }
                        if (old.UpperLeftNeighbor != null)
                        {
                            if (old.UpperLeftNeighbor.UpperRightNeighbor == old)
                            {
                                old.UpperLeftNeighbor.UpperRightNeighbor = right;
                            }
                            if (old.UpperLeftNeighbor.LowerRightNeighbor == old)
                            {
                                old.UpperLeftNeighbor.LowerRightNeighbor = right;
                            }
                        }

                        // add created trapezoid
                        newTrapezoids.Add(right);
                    }
                    else
                    {
                        if (old.Bottom.isAbove(seg.Point2) == 1 && old.Top.isAbove(seg.Point2) == -1)
                        {
                            // seg ends in 'middle''
                            top.UpperRightNeighbor = old.UpperRightNeighbor;
                            top.LowerRightNeighbor = old.UpperRightNeighbor;
                            bottom.UpperRightNeighbor = old.LowerRightNeighbor;
                            bottom.LowerRightNeighbor = old.LowerRightNeighbor;

                            // update old's neighbors
                            if (old.LowerRightNeighbor != null)
                            {
                                if (old.LowerRightNeighbor.UpperLeftNeighbor == old)
                                {
                                    old.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                                }
                                if (old.LowerRightNeighbor.LowerLeftNeighbor == old)
                                {
                                    old.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                                }
                            }
                            if (old.UpperRightNeighbor != null)
                            {
                                if (old.UpperRightNeighbor.UpperLeftNeighbor == old)
                                {
                                    old.UpperRightNeighbor.UpperLeftNeighbor = top;
                                }
                                if (old.UpperRightNeighbor.LowerLeftNeighbor == old)
                                {
                                    old.UpperRightNeighbor.LowerLeftNeighbor = top;
                                }
                            }
                        }
                        else if (old.Bottom.isAbove(seg.Point2) == 1)
                        {
                            // seg ends on 'old.top'
                            bottom.UpperRightNeighbor = old.UpperRightNeighbor;
                            bottom.LowerRightNeighbor = old.LowerRightNeighbor;

                            // update old's neighbors
                            if (old.LowerRightNeighbor != null)
                            {
                                if (old.LowerRightNeighbor.UpperLeftNeighbor == old)
                                {
                                    old.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                                }
                                if (old.LowerRightNeighbor.LowerLeftNeighbor == old)
                                {
                                    old.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                                }
                            }
                            if (old.UpperRightNeighbor != null)
                            {
                                if (old.UpperRightNeighbor.UpperLeftNeighbor == old)
                                {
                                    old.UpperRightNeighbor.UpperLeftNeighbor = bottom;
                                }
                                if (old.UpperRightNeighbor.LowerLeftNeighbor == old)
                                {
                                    old.UpperRightNeighbor.LowerLeftNeighbor = bottom;
                                }
                            }
                        }
                        else
                        {
                            // seg ends on 'old.bottom'
                            top.UpperRightNeighbor = old.UpperRightNeighbor;
                            top.LowerRightNeighbor = old.LowerRightNeighbor;

                            // update old's neighbors
                            if (old.LowerRightNeighbor != null)
                            {
                                if (old.LowerRightNeighbor.UpperLeftNeighbor == old)
                                {
                                    old.LowerRightNeighbor.UpperLeftNeighbor = top;
                                }
                                if (old.LowerRightNeighbor.LowerLeftNeighbor == old)
                                {
                                    old.LowerRightNeighbor.LowerLeftNeighbor = top;
                                }
                            }
                            if (old.UpperRightNeighbor != null)
                            {
                                if (old.UpperRightNeighbor.UpperLeftNeighbor == old)
                                {
                                    old.UpperRightNeighbor.UpperLeftNeighbor = top;
                                }
                                if (old.UpperRightNeighbor.LowerLeftNeighbor == old)
                                {
                                    old.UpperRightNeighbor.LowerLeftNeighbor = top;
                                }
                            }
                        }
                    }

                    if (right != null)
                    {
                        // replace trapezoid node by (right) segment point node
                        Node segNode = new Node(seg)
                        {
                            LeftChild = top.AssocNode,
                            RightChild = bottom.AssocNode
                        };
                        oldTrapezoids[c].Value = seg.Point2;
                        oldTrapezoids[c].LeftChild = segNode;
                        oldTrapezoids[c].RightChild = right.AssocNode;
                    }
                    else
                    {
                        oldTrapezoids[c].Value = seg;
                        oldTrapezoids[c].LeftChild = top.AssocNode;
                        oldTrapezoids[c].RightChild = bottom.AssocNode;
                    }
                }
                else
                {
                    // intermediary trapezoid
                    Trapezoid old = (Trapezoid)oldTrapezoids[c].Value;

                    // 'finish' last top/bottom
                    if (seg.isAbove(old.LeftPoint) == 1)
                    {
                        // old leftp is above segment ==> 'top' is finished
                        Trapezoid newTop = new Trapezoid(old.Top, seg, old.LeftPoint, Vector2.one, top, top, null, null);
                        if (old.UpperLeftNeighbor != old.LowerLeftNeighbor)
                        {
                            newTop.UpperLeftNeighbor = old.UpperLeftNeighbor;
                        }

                        top.RightPoint = old.LeftPoint;
                        top.LowerRightNeighbor = newTop;

                        if (old.UpperLeftNeighbor.UpperRightNeighbor == old)
                        {
                            top.UpperRightNeighbor = newTop;

                            // update old neighbors
                            old.UpperLeftNeighbor.UpperRightNeighbor = newTop;
                            old.UpperLeftNeighbor.LowerRightNeighbor = newTop;
                        }
                        else
                        {
                            // account for splitting into 'lower' trapezoid
                            top.UpperRightNeighbor = old.UpperLeftNeighbor.UpperRightNeighbor;
                        }

                        // update old neighbors
                        //if (old.UpperLeftNeighbor.UpperRightNeighbor != null)
                        //{
                        //    old.UpperLeftNeighbor.UpperRightNeighbor.UpperLeftNeighbor = top;
                        //    old.UpperLeftNeighbor.UpperRightNeighbor.LowerLeftNeighbor = top;
                        //    old.LowerLeftNeighbor.UpperRightNeighbor.UpperLeftNeighbor = top;
                        //    old.LowerLeftNeighbor.UpperRightNeighbor.LowerLeftNeighbor = top;
                        //}

                        // add new top
                        top = newTop;
                        newTrapezoids.Add(top);
                    }
                    else
                    {
                        // old leftp is below segment ==> 'bottom' is finished
                        Trapezoid newBottom = new Trapezoid(seg, old.Bottom, old.LeftPoint, Vector2.one, bottom, bottom, null, null);
                        if (old.UpperLeftNeighbor != old.LowerLeftNeighbor)
                        {
                            newBottom.LowerLeftNeighbor = old.LowerLeftNeighbor;
                        }

                        bottom.RightPoint = old.LeftPoint;
                        bottom.UpperRightNeighbor = newBottom;

                        if (old.LowerLeftNeighbor.LowerRightNeighbor == old)
                        {
                            bottom.LowerRightNeighbor = newBottom;

                            old.LowerLeftNeighbor.LowerRightNeighbor = newBottom;
                            old.LowerLeftNeighbor.UpperRightNeighbor = newBottom;
                        }
                        else
                        {
                            // account for splitting into 'higher' trapezoid
                            bottom.LowerRightNeighbor = old.LowerLeftNeighbor.LowerRightNeighbor;
                        }


                        // update old neighbors
                        //if (old.LowerLeftNeighbor.LowerRightNeighbor != null)
                        //{
                        //    old.LowerLeftNeighbor.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                        //    old.LowerLeftNeighbor.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                        //    old.UpperLeftNeighbor.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                        //    old.UpperLeftNeighbor.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                        //}

                        // add new bottom
                        bottom = newBottom;
                        newTrapezoids.Add(bottom);
                    }

                    // add seg node to tree
                    oldTrapezoids[c].Value = seg;
                    oldTrapezoids[c].LeftChild = top.AssocNode;
                    oldTrapezoids[c].RightChild = bottom.AssocNode;
                }
            }

            return newTrapezoids;
        }

        public List<Trapezoid> UpdateLastWorking(ref List<Node> oldTrapezoids, CountryLineSegment seg)
        {
            List<Trapezoid> newTrapezoids = new List<Trapezoid>();

            // trapezoids storing 'not fully computed' top and bottom trapezoids
            Trapezoid top = null;
            Trapezoid bottom = null;

            for (int c = 0; c < oldTrapezoids.Count; c++)
            {
                // old trapezoid to be replaced by new trapezoids
                Trapezoid old = (Trapezoid)oldTrapezoids[c].Value;

                if (c == 0)
                {
                    // first trapezoid
                    if (old.LeftPoint.x < seg.Point1.x)
                    {
                        // segment starts in 'middle' of the trapezoid
                        Trapezoid left = new Trapezoid(old.Top, old.Bottom, old.LeftPoint, seg.Point1, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null);
                        top = new Trapezoid(old.Top, seg, seg.Point1, Vector2.one, left, left, null, null);
                        bottom = new Trapezoid(seg, old.Bottom, seg.Point1, Vector2.one, left, left, null, null);

                        left.UpperRightNeighbor = top;
                        left.LowerRightNeighbor = bottom;

                        // update old neighbors
                        if (old.UpperLeftNeighbor != null)
                        {
                            old.UpperLeftNeighbor.UpperRightNeighbor = left;
                            old.UpperLeftNeighbor.LowerRightNeighbor = left;
                        }
                        if (old.LowerLeftNeighbor != null)
                        {
                            old.LowerLeftNeighbor.UpperRightNeighbor = left;
                            old.LowerLeftNeighbor.LowerRightNeighbor = left;
                        }

                        // add created trapezoids
                        newTrapezoids.Add(left);
                        newTrapezoids.Add(top);
                        newTrapezoids.Add(bottom);

                        // replace trapezoid node by (left) segment point node
                        oldTrapezoids[c].Value = seg.Point1;
                        // add appropriate children
                        Node segNode = new Node(seg);
                        oldTrapezoids[c].LeftChild = left.AssocNode;
                        oldTrapezoids[c].RightChild = segNode;
                        segNode.LeftChild = top.AssocNode;
                        segNode.RightChild = bottom.AssocNode;

                        if (oldTrapezoids.Count > 1)
                        {
                            if (seg.isAbove(old.RightPoint) == -1)
                            {
                                bottom.RightPoint = old.RightPoint;
                                bottom.UpperRightNeighbor = old.UpperRightNeighbor;
                                bottom.LowerRightNeighbor = old.LowerRightNeighbor;
                                // update old neighbors
                                if (old.UpperRightNeighbor != null)
                                {
                                    old.UpperRightNeighbor.UpperRightNeighbor = bottom;
                                    old.UpperRightNeighbor.LowerRightNeighbor = bottom;
                                }
                                if (old.LowerRightNeighbor != null)
                                {
                                    old.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                                    old.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                                }

                                if (old.UpperRightNeighbor != null)
                                {
                                    bottom = new Trapezoid(bottom.Top, old.UpperRightNeighbor.Bottom, old.RightPoint, Vector2.one, bottom, bottom, null, null);
                                }
                            }
                            else
                            {
                                top.RightPoint = old.RightPoint;
                                top.UpperRightNeighbor = old.UpperRightNeighbor;
                                top.LowerRightNeighbor = old.LowerRightNeighbor;
                                // update old neighbors
                                if (old.UpperRightNeighbor != null)
                                {
                                    old.UpperRightNeighbor.UpperRightNeighbor = top;
                                    old.UpperRightNeighbor.LowerRightNeighbor = top;
                                }
                                if (old.LowerRightNeighbor != null)
                                {
                                    old.LowerRightNeighbor.UpperLeftNeighbor = top;
                                    old.LowerRightNeighbor.LowerLeftNeighbor = top;
                                }

                                if (old.UpperRightNeighbor != null)
                                {
                                    top = new Trapezoid(old.UpperRightNeighbor.Bottom, top.Bottom, old.RightPoint, Vector2.one, top, top, null, null);
                                }

                            }
                        }
                    }
                    else
                    {
                        // segment start point coincides with left point
                        if (old.Bottom.isAbove(old.LeftPoint) == 0 && old.Top.isAbove(old.LeftPoint) == 0)
                        {
                            // trilateral trapezoid
                            top = new Trapezoid(old.Top, seg, seg.Point1, Vector2.one, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null);
                            bottom = new Trapezoid(seg, old.Bottom, seg.Point1, Vector2.one, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null);
                            // add created trapezoids
                            newTrapezoids.Add(top);
                            newTrapezoids.Add(bottom);

                            // replace trapezoid node by segment node
                            oldTrapezoids[c].Value = seg;
                            // add appropriate children
                            oldTrapezoids[c].LeftChild = top.AssocNode;
                            oldTrapezoids[c].RightChild = bottom.AssocNode;
                        }
                        else if (old.Bottom.isAbove(old.LeftPoint) == 0)
                        {
                            // left point is on 'bottom' -> 'top' gets 'old' neighbors
                            top = new Trapezoid(old.Top, seg, seg.Point1, Vector2.one, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null);
                            bottom = new Trapezoid(seg, old.Bottom, seg.Point1, Vector2.one, null, null, null, null); // no 'left' neighbors

                            // update old neighbors
                            if (old.UpperLeftNeighbor != null)
                            {
                                old.UpperLeftNeighbor.UpperRightNeighbor = top;
                            }
                            if (old.LowerLeftNeighbor != null)
                            {
                                old.LowerLeftNeighbor.UpperRightNeighbor = top;
                            }
                            // add created trapezoids
                            newTrapezoids.Add(top);
                            newTrapezoids.Add(bottom);

                            // replace trapezoid node by segment node
                            oldTrapezoids[c].Value = seg;
                            // add appropriate children
                            oldTrapezoids[c].LeftChild = top.AssocNode;
                            oldTrapezoids[c].RightChild = bottom.AssocNode;

                            //if (oldTrapezoids.Count > 1)
                            //{
                            //    Trapezoid newBottom = new Trapezoid(bottom.Top, old.UpperRightNeighbor.Bottom, old.RightPoint, Vector2.one, bottom, bottom, null, null);
                            //    bottom.RightPoint = old.RightPoint;
                            //    bottom.UpperRightNeighbor = newBottom;
                            //    bottom.LowerRightNeighbor = newBottom;
                            //    // update old neighbors
                            //    if (old.UpperRightNeighbor != null)
                            //    {
                            //        old.UpperRightNeighbor.UpperRightNeighbor = bottom;
                            //        old.UpperRightNeighbor.LowerRightNeighbor = bottom;
                            //    }
                            //    if (old.LowerRightNeighbor != null)
                            //    {
                            //        old.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                            //        old.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                            //    }

                            //    bottom = newBottom;
                            //}
                        }
                        else if (old.Top.isAbove(old.LeftPoint) == 0)
                        {
                            // left point is on 'top' -> 'bottom' gets 'old' neighbors
                            top = new Trapezoid(old.Top, seg, seg.Point1, Vector2.one, null, null, null, null);
                            bottom = new Trapezoid(seg, old.Bottom, seg.Point1, Vector2.one, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null); // no 'left' neighbors

                            // update old neighbors
                            if (old.UpperLeftNeighbor != null)
                            {
                                old.UpperLeftNeighbor.LowerRightNeighbor = bottom;
                            }
                            if (old.LowerLeftNeighbor != null)
                            {
                                old.LowerLeftNeighbor.LowerRightNeighbor = bottom;
                            }
                            // add created trapezoids
                            newTrapezoids.Add(top);
                            newTrapezoids.Add(bottom);

                            // replace trapezoid node by segment node
                            oldTrapezoids[c].Value = seg;
                            // add appropriate children
                            oldTrapezoids[c].LeftChild = top.AssocNode;
                            oldTrapezoids[c].RightChild = bottom.AssocNode;
                        }
                        else
                        {
                            // left point is in 'middle'
                            top = new Trapezoid(old.Top, seg, seg.Point1, Vector2.one, old.UpperLeftNeighbor, old.UpperLeftNeighbor, null, null);
                            bottom = new Trapezoid(seg, old.Bottom, seg.Point1, Vector2.one, old.LowerLeftNeighbor, old.LowerLeftNeighbor, null, null);

                            // update old neighbors
                            if (old.UpperLeftNeighbor != null)
                            {
                                old.UpperLeftNeighbor.UpperRightNeighbor = top;
                                old.UpperLeftNeighbor.LowerRightNeighbor = top;
                            }
                            if (old.LowerLeftNeighbor != null)
                            {
                                old.LowerLeftNeighbor.UpperRightNeighbor = bottom;
                                old.LowerLeftNeighbor.LowerRightNeighbor = bottom;
                            }
                            // add created trapezoids
                            newTrapezoids.Add(top);
                            newTrapezoids.Add(bottom);

                            // replace trapezoid node by segment node
                            oldTrapezoids[c].Value = seg;
                            // add appropriate children
                            oldTrapezoids[c].LeftChild = top.AssocNode;
                            oldTrapezoids[c].RightChild = bottom.AssocNode;
                        }
                    }
                }

                if (c == oldTrapezoids.Count - 1)
                {
                    // last trapezoid
                    if (seg.Point2.x < old.RightPoint.x)
                    {
                        // segment ends in 'middle' of trapezoid
                        Trapezoid right = new Trapezoid(old.Top, old.Bottom, seg.Point2, old.RightPoint, top, bottom, old.UpperRightNeighbor, old.LowerRightNeighbor);

                        // 'finish' top and bottom
                        top.RightPoint = seg.Point2;
                        bottom.RightPoint = seg.Point2;
                        top.UpperRightNeighbor = right;
                        top.LowerRightNeighbor = right;
                        bottom.UpperRightNeighbor = right;
                        bottom.LowerRightNeighbor = right;

                        // update old neighbors
                        if (old.UpperRightNeighbor != null)
                        {
                            old.UpperRightNeighbor.UpperLeftNeighbor = right;
                            old.UpperRightNeighbor.LowerLeftNeighbor = right;
                        }
                        if (old.LowerRightNeighbor != null)
                        {
                            old.LowerRightNeighbor.UpperLeftNeighbor = right;
                            old.LowerRightNeighbor.LowerLeftNeighbor = right;
                        }

                        // add created trapezoid
                        newTrapezoids.Add(right);

                        if (c != 0)
                        {
                            // replace trapezoid node by (right) segment point node
                            oldTrapezoids[c].Value = seg.Point2;
                            //add appropriate children
                            Node segNode = new Node(seg);
                            oldTrapezoids[c].RightChild = right.AssocNode;
                            oldTrapezoids[c].LeftChild = segNode;
                            segNode.LeftChild = top.AssocNode;
                            segNode.RightChild = bottom.AssocNode;
                        }
                        else
                        {
                            // end trapezoid is the same as start trapezoid
                            //add appropriate children
                            if (old.LeftPoint.x < seg.Point1.x)
                            {
                                // segment starts in 'middle' of old trapezoid
                                Node rightp = new Node(seg.Point2);
                                Node segNode = new Node(seg);
                                oldTrapezoids[c].RightChild = rightp;
                                rightp.RightChild = right.AssocNode;
                                rightp.LeftChild = segNode;
                                segNode.LeftChild = top.AssocNode;
                                segNode.RightChild = bottom.AssocNode;
                            }
                            else
                            {
                                // segment starts in the leftpoint of the old trapezoid
                                // replace trapezoid node by (right) segment point node
                                oldTrapezoids[c].Value = seg.Point2;
                                Node segNode = new Node(seg);
                                oldTrapezoids[c].RightChild = right.AssocNode;
                                oldTrapezoids[c].LeftChild = segNode;
                                segNode.LeftChild = top.AssocNode;
                                segNode.RightChild = bottom.AssocNode;
                            }
                        }
                    }
                    else
                    {
                        // segment end coincides with right point
                        // 'finish' top and bottom
                        top.RightPoint = seg.Point2;
                        bottom.RightPoint = seg.Point2;

                        // segment start point coincides with left point
                        if (old.Bottom.isAbove(seg.Point2) == 0 && old.Top.isAbove(seg.Point2) == 0)
                        {
                            // trilateral trapezoid
                            top.UpperRightNeighbor = old.UpperRightNeighbor;
                            top.LowerRightNeighbor = old.LowerRightNeighbor;
                            bottom.UpperRightNeighbor = old.UpperRightNeighbor;
                            bottom.LowerRightNeighbor = old.LowerRightNeighbor;
                        }
                        else if (old.Bottom.isAbove(seg.Point2) == 0)
                        {
                            // right point is on 'bottom' -> 'top' gets 'old' neighbors
                            top.UpperRightNeighbor = old.UpperRightNeighbor;
                            top.LowerRightNeighbor = old.LowerRightNeighbor;

                            // update old neighbors
                            if (old.UpperRightNeighbor != null)
                            {
                                old.UpperRightNeighbor.UpperLeftNeighbor = top;
                                old.UpperRightNeighbor.LowerLeftNeighbor = top;
                            }
                            if (old.LowerRightNeighbor != null)
                            {
                                old.LowerRightNeighbor.UpperLeftNeighbor = top;
                                old.LowerRightNeighbor.LowerLeftNeighbor = top;
                            }
                        }
                        else if (old.Top.isAbove(seg.Point2) == 0)
                        {

                            // right point is on 'top' -> 'bottom' gets 'old' neighbors
                            bottom.UpperRightNeighbor = old.UpperRightNeighbor;
                            bottom.LowerRightNeighbor = old.LowerRightNeighbor;

                            // update old neighbors
                            if (old.UpperRightNeighbor != null)
                            {
                                old.UpperRightNeighbor.UpperLeftNeighbor = bottom;
                                old.UpperRightNeighbor.LowerLeftNeighbor = bottom;
                            }
                            if (old.LowerRightNeighbor != null)
                            {
                                old.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                                old.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                            }
                        }
                        else
                        {
                            // right point is in 'middle'
                            top.UpperRightNeighbor = old.UpperRightNeighbor;
                            top.LowerRightNeighbor = old.UpperRightNeighbor;
                            bottom.UpperRightNeighbor = old.LowerRightNeighbor;
                            bottom.LowerRightNeighbor = old.LowerRightNeighbor;

                            // update old neighbors
                            if (old.UpperRightNeighbor != null)
                            {
                                old.UpperRightNeighbor.UpperLeftNeighbor = top;
                                old.UpperRightNeighbor.LowerLeftNeighbor = top;
                            }
                            if (old.LowerRightNeighbor != null)
                            {
                                old.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                                old.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                            }
                        }

                        if (c != 0)
                        {
                            // replace trapezoid node by segment node
                            oldTrapezoids[c].Value = seg;
                            // add appropriate children
                            oldTrapezoids[c].LeftChild = top.AssocNode;
                            oldTrapezoids[c].RightChild = bottom.AssocNode;
                        }
                        // if start trapezoid is the same as end trapezoid,
                        // the appropriate nodes were already added in the previous step
                    }
                }

                if (c > 0 && c < oldTrapezoids.Count - 1)
                {
                    // intermediary trapezoid
                    // update tree
                    Debug.Log("FOUND OOOONE:  " + seg.Segment);
                    oldTrapezoids[c].Value = seg;
                    oldTrapezoids[c].LeftChild = top.AssocNode;
                    oldTrapezoids[c].RightChild = bottom.AssocNode;
                    // prepare next top/bottom
                    if (old.Top.isAbove(old.RightPoint) == -1 && old.Bottom.isAbove(old.RightPoint) == 1)
                    {
                        Debug.Log("Yuuup im here");
                        // right point is in 'middle'
                        if (seg.isAbove(old.RightPoint) == -1)
                        {
                            //seg is above right point
                            // prepare next 'bottom' trapezoid
                            bottom.RightPoint = old.RightPoint;
                            Trapezoid newBottom = new Trapezoid(seg, old.UpperRightNeighbor.Bottom, old.RightPoint, Vector2.one, bottom, bottom, null, null);
                            bottom.UpperRightNeighbor = newBottom;
                            bottom.LowerRightNeighbor = old.LowerRightNeighbor;

                            bottom = newBottom;
                            // add created trapezoid
                            newTrapezoids.Add(bottom);

                            // add old neighbors
                            if (old.LowerRightNeighbor != null)
                            {
                                old.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                                old.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                            }
                        }
                        else
                        {
                            // seg is below right point
                            // prepare next 'top' trapezoid
                            top.RightPoint = old.RightPoint;
                            Trapezoid newTop = new Trapezoid(old.LowerRightNeighbor.Top, seg, old.RightPoint, Vector2.one, top, top, null, null);
                            top.UpperRightNeighbor = old.UpperRightNeighbor;
                            top.LowerRightNeighbor = newTop;

                            top = newTop;
                            // add created trapezoid
                            newTrapezoids.Add(top);

                            // add old neighbors
                            if (old.UpperRightNeighbor != null)
                            {
                                old.UpperRightNeighbor.UpperLeftNeighbor = top;
                                old.UpperRightNeighbor.LowerLeftNeighbor = top;
                            }
                        }
                    }
                    else
                    {
                        if (seg.isAbove(old.RightPoint) == 1)
                        {
                            // right point of current trapezoid is above segment
                            top.RightPoint = old.RightPoint;

                            // prepare next 'top' trapezoid
                            Trapezoid newTop = new Trapezoid(old.Top, seg, old.RightPoint, Vector2.one, top, top, null, null);
                            top.UpperRightNeighbor = newTop;
                            top.LowerRightNeighbor = newTop;

                            top = newTop;
                            // add created trapezoid
                            newTrapezoids.Add(top);
                        }
                        else
                        {
                            // right point of current trapezoid is below segment
                            bottom.RightPoint = old.RightPoint;

                            // prepare next 'bottom' trapezoid
                            Trapezoid newBottom = new Trapezoid(seg, old.Bottom, old.RightPoint, Vector2.one, bottom, bottom, null, null);
                            bottom.UpperRightNeighbor = newBottom;
                            bottom.LowerRightNeighbor = newBottom;

                            bottom = newBottom;
                            // add created trapezoid
                            newTrapezoids.Add(bottom);
                        }
                    }
                }
            }



            return newTrapezoids;
        }

        /// <summary>
        /// Updates old trapezoids to new trapezoids cut by input segment.
        /// Requires seg.Point1.x < seg.Point2.x and no intersections
        /// </summary>
        /// <param name="oldTrapezoids"></param>
        /// <param name="seg"></param>
        /// <returns></returns>
        public List<Trapezoid> UpdateOld(ref List<Node> oldTrapezoids, CountryLineSegment seg)
        {
            List<Trapezoid> newTrapezoids = new List<Trapezoid>();

            // trapezoids storing 'not fully computed' top and bottom trapezoids
            Trapezoid top = null;
            Trapezoid bottom = null;

            for (int c = 0; c < oldTrapezoids.Count; c++) {
                // old trapezoid to be replaced by new trapezoids
                Trapezoid old = (Trapezoid)oldTrapezoids[c].Value;

                if (c == 0)
                {
                    // first trapezoid
                    if (old.LeftPoint.x < seg.Point1.x)
                    {
                        // segment starts in 'middle' of the trapezoid
                        Trapezoid left = new Trapezoid(old.Top, old.Bottom, old.LeftPoint, seg.Point1, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null);
                        top = new Trapezoid(old.Top, seg, seg.Point1, Vector2.one, left, left, null, null);
                        bottom = new Trapezoid(seg, old.Bottom, seg.Point1, Vector2.one, left, left, null, null);

                        left.UpperRightNeighbor = top;
                        left.LowerRightNeighbor = bottom;

                        // update old neighbors
                        if (old.UpperLeftNeighbor != null)
                        {
                            old.UpperLeftNeighbor.UpperRightNeighbor = left;
                            old.UpperLeftNeighbor.LowerRightNeighbor = left;
                        }
                        if (old.LowerLeftNeighbor != null)
                        {
                            old.LowerLeftNeighbor.UpperRightNeighbor = left;
                            old.LowerLeftNeighbor.LowerRightNeighbor = left;
                        }

                        // add created trapezoids
                        newTrapezoids.Add(left);
                        newTrapezoids.Add(top);
                        newTrapezoids.Add(bottom);

                        // replace trapezoid node by (left) segment point node
                        oldTrapezoids[c].Value = seg.Point1;
                        // add appropriate children
                        Node segNode = new Node(seg);
                        oldTrapezoids[c].LeftChild = left.AssocNode;
                        oldTrapezoids[c].RightChild = segNode;
                        segNode.LeftChild = top.AssocNode;
                        segNode.RightChild = bottom.AssocNode;
                    } else
                    {
                        // segment start point coincides with left point
                        if (onSegment(old.Bottom, old.LeftPoint) && onSegment(old.Top, old.LeftPoint))
                        {
                            // trilateral trapezoid
                            top = new Trapezoid(old.Top, seg, seg.Point1, Vector2.one, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null);
                            bottom = new Trapezoid(seg, old.Bottom, seg.Point1, Vector2.one, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null);
                        } else if (onSegment(old.Bottom, old.LeftPoint))
                        {
                            // left point is on 'bottom' -> 'top' gets 'old' neighbors
                            top = new Trapezoid(old.Top, seg, seg.Point1, Vector2.one, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null);
                            bottom = new Trapezoid(seg, old.Bottom, seg.Point1, Vector2.one, null, null, null, null); // no 'left' neighbors

                            // update old neighbors
                            if (old.UpperLeftNeighbor != null)
                            {
                                old.UpperLeftNeighbor.UpperRightNeighbor = top;
                                old.UpperLeftNeighbor.LowerRightNeighbor = top;
                            }
                            if (old.LowerLeftNeighbor != null)
                            {
                                old.LowerLeftNeighbor.UpperRightNeighbor = top;
                                old.LowerLeftNeighbor.LowerRightNeighbor = top;
                            }
                        } else if (onSegment(old.Top, old.LeftPoint))
                        {
                            // left point is on 'top' -> 'bottom' gets 'old' neighbors
                            top = new Trapezoid(old.Top, seg, seg.Point1, Vector2.one, null, null, null, null);
                            bottom = new Trapezoid(seg, old.Bottom, seg.Point1, Vector2.one, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null); // no 'left' neighbors

                            // update old neighbors
                            if (old.UpperLeftNeighbor != null)
                            {
                                old.UpperLeftNeighbor.UpperRightNeighbor = bottom;
                                old.UpperLeftNeighbor.LowerRightNeighbor = bottom;
                            }
                            if (old.LowerLeftNeighbor != null)
                            {
                                old.LowerLeftNeighbor.UpperRightNeighbor = bottom;
                                old.LowerLeftNeighbor.LowerRightNeighbor = bottom;
                            }
                        } else
                        {
                            // left point is in 'middle'
                            top = new Trapezoid(old.Top, seg, seg.Point1, Vector2.one, old.UpperLeftNeighbor, old.UpperLeftNeighbor, null, null);
                            bottom = new Trapezoid(seg, old.Bottom, seg.Point1, Vector2.one, old.LowerLeftNeighbor, old.LowerLeftNeighbor, null, null);

                            // update old neighbors
                            if (old.UpperLeftNeighbor != null)
                            {
                                old.UpperLeftNeighbor.UpperRightNeighbor = top;
                                old.UpperLeftNeighbor.LowerRightNeighbor = top;
                            }
                            if (old.LowerLeftNeighbor != null)
                            {
                                old.LowerLeftNeighbor.UpperRightNeighbor = bottom;
                                old.LowerLeftNeighbor.LowerRightNeighbor = bottom;
                            }
                        }

                        // add created trapezoids
                        newTrapezoids.Add(top);
                        newTrapezoids.Add(bottom);

                        // replace trapezoid node by segment node
                        oldTrapezoids[c].Value = seg;
                        // add appropriate children
                        oldTrapezoids[c].LeftChild = top.AssocNode;
                        oldTrapezoids[c].RightChild = bottom.AssocNode;
                    }
                    
                }

                if (c == oldTrapezoids.Count - 1)
                {
                    // last trapezoid
                    if (seg.Point2.x < old.RightPoint.x)
                    {
                        // segment ends in 'middle' of trapezoid
                        Trapezoid right = new Trapezoid(old.Top, old.Bottom, seg.Point2, old.RightPoint, top, bottom, old.UpperRightNeighbor, old.LowerRightNeighbor);

                        // 'finish' top and bottom
                        top.RightPoint = seg.Point2;
                        bottom.RightPoint = seg.Point2;
                        top.UpperRightNeighbor = right;
                        top.LowerRightNeighbor = right;
                        bottom.UpperRightNeighbor = right;
                        bottom.LowerRightNeighbor = right;

                        // update old neighbors
                        if (old.UpperRightNeighbor != null)
                        {
                            old.UpperRightNeighbor.UpperLeftNeighbor = right;
                            old.UpperRightNeighbor.LowerLeftNeighbor = right;
                        }
                        if (old.LowerRightNeighbor != null)
                        {
                            old.LowerRightNeighbor.UpperLeftNeighbor = right;
                            old.LowerRightNeighbor.LowerLeftNeighbor = right;
                        }

                        // add created trapezoid
                        newTrapezoids.Add(right);

                        if (c != 0)
                        {
                            // replace trapezoid node by (right) segment point node
                            oldTrapezoids[c].Value = seg.Point2;
                            //add appropriate children
                            Node segNode = new Node(seg);
                            oldTrapezoids[c].RightChild = right.AssocNode;
                            oldTrapezoids[c].LeftChild = segNode;
                            segNode.LeftChild = top.AssocNode;
                            segNode.RightChild = bottom.AssocNode;
                        } else
                        {
                            // end trapezoid is the same as start trapezoid
                            //add appropriate children
                            if (old.LeftPoint.x < seg.Point1.x)
                            {
                                // segment starts in 'middle' of old trapezoid
                                Node rightp = new Node(seg.Point2);
                                Node segNode = new Node(seg);
                                oldTrapezoids[c].RightChild = rightp;
                                rightp.RightChild = right.AssocNode;
                                rightp.LeftChild = segNode;
                                segNode.LeftChild = top.AssocNode;
                                segNode.RightChild = bottom.AssocNode;
                            } else
                            {
                                // segment starts in the leftpoint of the old trapezoid
                                // replace trapezoid node by (right) segment point node
                                oldTrapezoids[c].Value = seg.Point2;
                                Node segNode = new Node(seg);
                                oldTrapezoids[c].RightChild = right.AssocNode;
                                oldTrapezoids[c].LeftChild = segNode;
                                segNode.LeftChild = top.AssocNode;
                                segNode.RightChild = bottom.AssocNode;
                            }
                        }
                    } else
                    {
                        // segment end coincides with right point
                        // 'finish' top and bottom
                        top.RightPoint = seg.Point2;
                        bottom.RightPoint = seg.Point2;

                        // segment start point coincides with left point
                        if (onSegment(old.Bottom, seg.Point2) && onSegment(old.Top, seg.Point2))
                        {
                            // trilateral trapezoid
                            top.UpperRightNeighbor = old.UpperRightNeighbor;
                            top.LowerRightNeighbor = old.LowerRightNeighbor;
                            bottom.UpperRightNeighbor = old.UpperRightNeighbor;
                            bottom.LowerRightNeighbor = old.LowerRightNeighbor;
                        }
                        else if (onSegment(old.Bottom, seg.Point2))
                        {
                            // right point is on 'bottom' -> 'top' gets 'old' neighbors
                            top.UpperRightNeighbor = old.UpperRightNeighbor;
                            top.LowerRightNeighbor = old.LowerRightNeighbor;

                            // update old neighbors
                            if (old.UpperRightNeighbor != null)
                            {
                                old.UpperRightNeighbor.UpperLeftNeighbor = top;
                                old.UpperRightNeighbor.LowerLeftNeighbor = top;
                            }
                            if (old.LowerRightNeighbor != null)
                            {
                                old.LowerRightNeighbor.UpperLeftNeighbor = top;
                                old.LowerRightNeighbor.LowerLeftNeighbor = top;
                            }
                        }
                        else if (onSegment(old.Top, seg.Point2))
                        {
                            
                            // right point is on 'top' -> 'bottom' gets 'old' neighbors
                            bottom.UpperRightNeighbor = old.UpperRightNeighbor;
                            bottom.LowerRightNeighbor = old.LowerRightNeighbor;

                            // update old neighbors
                            if (old.UpperRightNeighbor != null)
                            {
                                old.UpperRightNeighbor.UpperLeftNeighbor = bottom;
                                old.UpperRightNeighbor.LowerLeftNeighbor = bottom;
                            }
                            if (old.LowerRightNeighbor != null)
                            {
                                old.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                                old.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                            }
                        }
                        else
                        {
                            // right point is in 'middle'
                            top.UpperRightNeighbor = old.UpperRightNeighbor;
                            top.LowerRightNeighbor = old.UpperRightNeighbor;
                            bottom.UpperRightNeighbor = old.LowerRightNeighbor;
                            bottom.LowerRightNeighbor = old.LowerRightNeighbor;

                            // update old neighbors
                            if (old.UpperRightNeighbor != null)
                            {
                                old.UpperRightNeighbor.UpperLeftNeighbor = top;
                                old.UpperRightNeighbor.LowerLeftNeighbor = top;
                            }
                            if (old.LowerRightNeighbor != null)
                            {
                                old.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                                old.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                            }
                        }

                        if (c != 0)
                        {
                            // replace trapezoid node by segment node
                            oldTrapezoids[c].Value = seg;
                            // add appropriate children
                            oldTrapezoids[c].LeftChild = top.AssocNode;
                            oldTrapezoids[c].RightChild = bottom.AssocNode;
                        }
                        // if start trapezoid is the same as end trapezoid,
                        // the appropriate nodes were already added in the previous step
                    }
                }

                if (c > 0 && c < oldTrapezoids.Count - 1)
                {
                    // intermediary trapezoid
                    // update tree
                    Debug.Log("FOUND OOOONE:  " + seg.Segment);
                    oldTrapezoids[c].Value = seg;
                    oldTrapezoids[c].LeftChild = top.AssocNode;
                    oldTrapezoids[c].RightChild = bottom.AssocNode;
                    // prepare next top/bottom
                    if (old.Top.IsRightOf(old.RightPoint) && !old.Bottom.IsRightOf(old.RightPoint) && !onSegment(old.Top, old.RightPoint) && !onSegment(old.Bottom, old.RightPoint))
                    {
                        Debug.Log("Yuuup im here");
                        // right point is in 'middle'
                        if (!seg.IsRightOf(old.RightPoint))
                        {
                            //seg is above right point
                            // prepare next 'bottom' trapezoid
                            bottom.RightPoint = old.RightPoint;
                            Trapezoid newBottom = new Trapezoid(seg, old.UpperRightNeighbor.Bottom, old.RightPoint, Vector2.one, bottom, bottom, null, null);
                            bottom.UpperRightNeighbor = newBottom;
                            bottom.LowerRightNeighbor = old.LowerRightNeighbor;

                            bottom = newBottom;
                            // add created trapezoid
                            newTrapezoids.Add(bottom);

                            // add old neighbors
                            if (old.LowerRightNeighbor != null)
                            {
                                old.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                                old.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                            }
                        }
                        else
                        {
                            // seg is below right point
                            // prepare next 'top' trapezoid
                            top.RightPoint = old.RightPoint;
                            Trapezoid newTop = new Trapezoid(old.LowerRightNeighbor.Top, seg, old.RightPoint, Vector2.one, top, top, null, null);
                            top.UpperRightNeighbor = old.UpperRightNeighbor;
                            top.LowerRightNeighbor = newTop;

                            top = newTop;
                            // add created trapezoid
                            newTrapezoids.Add(top);

                            // add old neighbors
                            if (old.UpperRightNeighbor != null)
                            {
                                old.UpperRightNeighbor.UpperLeftNeighbor = top;
                                old.UpperRightNeighbor.LowerLeftNeighbor = top;
                            }
                        }
                    }
                    else
                    {
                        if (seg.IsRightOf(old.RightPoint))
                        {
                            // right point of current trapezoid is above segment
                            top.RightPoint = old.RightPoint;

                            // prepare next 'top' trapezoid
                            Trapezoid newTop = new Trapezoid(old.Top, seg, old.RightPoint, Vector2.one, top, top, null, null);
                            top.UpperRightNeighbor = newTop;
                            top.LowerRightNeighbor = newTop;

                            top = newTop;
                            // add created trapezoid
                            newTrapezoids.Add(top);
                        }
                        else
                        {
                            // right point of current trapezoid is below segment
                            bottom.RightPoint = old.RightPoint;

                            // prepare next 'bottom' trapezoid
                            Trapezoid newBottom = new Trapezoid(seg, old.Bottom, old.RightPoint, Vector2.one, bottom, bottom, null, null);
                            bottom.UpperRightNeighbor = newBottom;
                            bottom.LowerRightNeighbor = newBottom;

                            bottom = newBottom;
                            // add created trapezoid
                            newTrapezoids.Add(bottom);
                        }
                    }
                }
            }

            return newTrapezoids;
        }
    }
}
