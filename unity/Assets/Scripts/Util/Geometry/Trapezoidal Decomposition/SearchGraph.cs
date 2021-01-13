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

        public Node Search(Vector2 query)
        {
            Node currentNode = RootNode;
            while (true)
            {
                if (currentNode.Value is Trapezoid)
                {
                    Debug.Log(currentNode.Value);
                    return currentNode;
                } else if (currentNode.Value is Vector2)
                {
                    Debug.Log(currentNode.Value);
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
                    Debug.Log(((CountryLineSegment)currentNode.Value).Segment);
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
                    oldTrapezoids[c].Value = seg;
                    oldTrapezoids[c].LeftChild = top.AssocNode;
                    oldTrapezoids[c].RightChild = bottom.AssocNode;

                    // prepare next top/bottom
                    if (old.Top.IsRightOf(old.RightPoint) && !old.Bottom.IsRightOf(old.RightPoint))
                    {
                        Debug.Log("Bayaaya");
                        // right point is in 'middle'
                        if (!seg.IsRightOf(old.RightPoint))
                        {
                            //seg is above right point
                            // prepare next 'bottom' trapezoid
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
                        if (!seg.IsRightOf(old.RightPoint))
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

        public List<Trapezoid> UpdateOld(List<Node> oldTrapezoids, CountryLineSegment seg)
        {
            List<Trapezoid> newTrapezoids = new List<Trapezoid>();
            Vector2 emptyVector = new Vector2();
            if (oldTrapezoids[0].Value is Trapezoid)
            {
                Debug.Log("We have a trapezoid");
            } else
            {
                Debug.Log("Not a trapezoid");
            }
            Trapezoid top = (Trapezoid)oldTrapezoids[0].Value;
            Trapezoid bottom = (Trapezoid)oldTrapezoids[0].Value;

            for (int c = 0; c < oldTrapezoids.Count; c++)
            {
                // old trapezoid to be replaced by new trapezoids
                Trapezoid old = (Trapezoid)oldTrapezoids[c].Value;

                if (oldTrapezoids.Count == 1)
                {
                    Trapezoid left;
                    if (old.LeftPoint.x < seg.Point1.x)
                    {
                        left = new Trapezoid(old.Top, old.Bottom, old.LeftPoint, seg.Point1, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null);
                        top = new Trapezoid(old.Top, seg, seg.Point1, seg.Point2, left, left, null, null);
                        bottom = new Trapezoid(seg, old.Bottom, seg.Point1, seg.Point2, left, left, null, null);
                        left.UpperRightNeighbor = top;
                        left.LowerRightNeighbor = bottom;

                        // update old neighbors
                        if (old.UpperLeftNeighbor != null)
                        {
                            if (old.UpperLeftNeighbor.UpperRightNeighbor == old)
                                old.UpperLeftNeighbor.UpperRightNeighbor = left;
                            if (old.UpperLeftNeighbor.LowerRightNeighbor == old)
                                old.UpperLeftNeighbor.LowerRightNeighbor = left;
                        }
                        if (old.LowerLeftNeighbor != null)
                        {
                            if (old.LowerLeftNeighbor.UpperRightNeighbor == old)
                                old.LowerLeftNeighbor.UpperRightNeighbor = left;
                            if (old.LowerLeftNeighbor.LowerRightNeighbor == old)
                                old.LowerLeftNeighbor.LowerRightNeighbor = left;
                        }
                    }
                    else
                    {
                        // three-point trapezoid
                        top = new Trapezoid(old.Top, seg, old.LeftPoint, seg.Point2, old.UpperLeftNeighbor, null, null, null);
                        bottom = new Trapezoid(seg, old.Bottom, old.LeftPoint, seg.Point2, top, old.LowerLeftNeighbor, null, null);
                        top.LowerLeftNeighbor = bottom;
                    }

                    if (seg.Point2.x < old.RightPoint.x)
                    {
                        Trapezoid right = new Trapezoid(old.Top, old.Bottom, seg.Point2, old.RightPoint, top, bottom, old.UpperRightNeighbor, old.LowerRightNeighbor);
                        top.UpperRightNeighbor = right;
                        top.LowerRightNeighbor = right;
                        bottom.UpperRightNeighbor = right;
                        bottom.LowerRightNeighbor = right;
                    }

                }
                else if (c == 0)
                {
                    Trapezoid left;
                    // left trapezoid
                    if (old.LeftPoint.x < seg.Point1.x)
                    {
                        left = new Trapezoid(old.Top, old.Bottom, old.LeftPoint, seg.Point1, old.UpperLeftNeighbor, old.LowerLeftNeighbor, null, null);
                        // maybe here?
                        //old.UpperLeftNeighbor.UpperRightNeighbor = left;
                        //old.UpperLeftNeighbor.LowerRightNeighbor = left;
                        //old.LowerLeftNeighbor.UpperRightNeighbor = left;
                        //old.LowerLeftNeighbor.LowerRightNeighbor = left;
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

                    // maybe actually here
                    old.UpperLeftNeighbor.UpperRightNeighbor = top;
                    old.UpperLeftNeighbor.LowerRightNeighbor = bottom;
                    old.LowerLeftNeighbor.UpperRightNeighbor = top;
                    old.LowerLeftNeighbor.LowerRightNeighbor = bottom;

                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // CHANGE OLD'S NEIGHBOR'S NEIGHBOR'S TO TOP/BOTTOM
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                    // maybe actually here
                    //if (old.UpperLeftNeighbor != null)
                    //    old.UpperLeftNeighbor.UpperRightNeighbor = top;
                    //if (old.UpperLeftNeighbor != null)
                    //    old.UpperLeftNeighbor.LowerRightNeighbor = bottom;
                    //if (old.LowerLeftNeighbor != null)
                    //    old.LowerLeftNeighbor.UpperRightNeighbor = top;
                    //if (old.LowerLeftNeighbor != null)
                    //    old.LowerLeftNeighbor.LowerRightNeighbor = bottom;

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
                else if (c == oldTrapezoids.Count - 1)
                {
                    // right trapezoid
                    Trapezoid right;
                    if (old.RightPoint.x < seg.Point2.x)
                    {
                        right = new Trapezoid(old.Top, old.Bottom, seg.Point2, old.RightPoint, null, null, old.UpperRightNeighbor, old.LowerRightNeighbor);

                        // MUST CHECK CASE WHERE LEFTPOINT IS ON SEGMENT
                    }
                    else
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

                    // CHANGE HERE
                    if (old.UpperRightNeighbor != null)
                        old.UpperRightNeighbor.UpperLeftNeighbor = right;
                    if (old.UpperRightNeighbor != null)
                        old.UpperRightNeighbor.UpperLeftNeighbor = right;
                    if (old.LowerRightNeighbor != null)
                        old.LowerRightNeighbor.UpperLeftNeighbor = right;
                    if (old.LowerRightNeighbor != null)
                        old.LowerRightNeighbor.LowerLeftNeighbor = right;

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
