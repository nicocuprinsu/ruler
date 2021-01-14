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

        public Node Search(Vector2 query)
        {
            Node currentNode = RootNode;
            while (true)
            {
                if (currentNode.Value is Trapezoid)
                {
                    Trapezoid res = (Trapezoid)currentNode.Value;
                    return currentNode;
                } else if (currentNode.Value is Vector2)
                {
                    if (query.x < ((Vector2) currentNode.Value).x)
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
                        currentNode = currentNode.LeftChild;
                    }
                    else
                    {
                        currentNode = currentNode.RightChild;
                    }
                }
            }
        }

        public bool onSegment(CountryLineSegment seg, Vector2 point)
        {
            return seg.Segment.IsEndpoint(point);
        }

        /// <summary>
        /// Updates old trapezoids to new trapezoids cut by input segment.
        /// Requires seg.Point1.x < seg.Point2.x and no intersections
        /// </summary>
        /// <param name="oldTrapezoids"></param>
        /// <param name="seg"></param>
        /// <returns></returns>
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
                                old.UpperLeftNeighbor.UpperRightNeighbor = top;
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

                        if (old.UpperLeftNeighbor.UpperRightNeighbor == old.UpperLeftNeighbor.LowerRightNeighbor)
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

                            old.LowerLeftNeighbor.UpperRightNeighbor.UpperLeftNeighbor = top;
                            old.LowerLeftNeighbor.UpperRightNeighbor.LowerLeftNeighbor = top;
                        }

                        // add new top
                        top = newTop;
                        newTrapezoids.Add(top);

                        // end trapezoid, so add rightp to 'bottom'
                        bottom.RightPoint = seg.Point2;
                    }
                    else
                    {
                        // old leftp is below segment ==> 'bottom' is finished
                        Trapezoid newBottom = new Trapezoid(seg, old.Bottom, old.LeftPoint, seg.Point2, bottom, bottom, null, null);
                        if (old.UpperLeftNeighbor != old.LowerLeftNeighbor)
                        {
                            newBottom.LowerLeftNeighbor = old.LowerLeftNeighbor;
                        }

                        bottom.RightPoint = old.LeftPoint;
                        bottom.UpperRightNeighbor = newBottom;

                        if (old.LowerLeftNeighbor.LowerRightNeighbor == old.LowerLeftNeighbor.UpperRightNeighbor)
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

                            old.LowerLeftNeighbor.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                            old.LowerLeftNeighbor.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                        }

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

                        if (old.UpperLeftNeighbor.UpperRightNeighbor == old.UpperLeftNeighbor.LowerRightNeighbor)
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

                            old.LowerLeftNeighbor.UpperRightNeighbor.UpperLeftNeighbor = top;
                            old.LowerLeftNeighbor.UpperRightNeighbor.LowerLeftNeighbor = top;
                        }

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

                        if (old.LowerLeftNeighbor.LowerRightNeighbor == old.LowerLeftNeighbor.UpperRightNeighbor)
                        {
                            bottom.LowerRightNeighbor = newBottom;

                            old.LowerLeftNeighbor.LowerRightNeighbor = newBottom;
                            old.LowerLeftNeighbor.UpperRightNeighbor = newBottom;
                        }
                        else
                        {
                            // account for splitting into 'higher' trapezoid
                            bottom.LowerRightNeighbor = old.LowerLeftNeighbor.LowerRightNeighbor;

                            old.LowerLeftNeighbor.LowerRightNeighbor.UpperLeftNeighbor = bottom;
                            old.LowerLeftNeighbor.LowerRightNeighbor.LowerLeftNeighbor = bottom;
                        }

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
    }
}
