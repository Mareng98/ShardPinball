using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Shard
{
    /*
     * There are many ways to optimize this implementation of a quadtree. For example,
     * in terms of memory efficiency we shouldn't use large collections like List/ArrayList etc. for containers that barely contain any elements.
     * 
     * We also don't store all nodes on the leaves (because this leads to slower insert/removal). 
     * It is more efficient however, since we will always know where intersections might happen and only have to look at leaves.
     * 
     * One problem that might arise is that a very large bounding box will be intersect several leaves.
     */
    internal class QuadTree
    {
        int maxDepth;
        int maxThreshold;

        private QNode root;
        public readonly Box globalBox;

        internal QNode Root { get => root; set => root = value; }

        public QuadTree(int x, int y, int width, int height, int depth = 8, int threshold = 16)
        {
            maxDepth = depth;
            maxThreshold = threshold;
            root = new QNode();
            globalBox = new Box(x, y, width, height);
        } 



        public bool isLeaf(QNode node)
        {
            return node.children is null;
        }

        public Box computeBox(Box box, NodeDirection dir)
        {
            var origin = box.GetTopLeft();
            var childSize = box.Size() / 2.0f;

            var width = childSize.X;
            var height = childSize.Y;

            switch(dir)
            {
                case NodeDirection.NorthWest:
                    return new Box(origin.X, origin.Y, width, height);
                case NodeDirection.NorthEast:
                    return new Box(origin.X + childSize.X, origin.Y, width, height);
                case NodeDirection.SouthWest:
                    return new Box(origin.X, origin.Y + height, width, height);
                case NodeDirection.SouthEast:
                    var v = origin + childSize;
                    return new Box(v.X, v.Y, width, height);
                default:
                    Debug.Log("ERROR: invalid child index");
                    return null;
            }
        }
        public NodeDirection findQuadrant(Box nodeBox, Box toBeInserted)
        {
            var center = nodeBox.GetCenter();
            // Implies we're in the "west" quadrants

            if (toBeInserted.GetRight() < center.X)
            {
                if (toBeInserted.GetBottom() < center.Y)
                {
                    return NodeDirection.NorthWest;
                }
                else if (toBeInserted.Top >= center.Y)
                {
                    return NodeDirection.SouthWest;
                }
                else
                {
                    return NodeDirection.NotInQuadrant;
                }
            }

            // implies we're in the "east" section of the quadrants
            else if (toBeInserted.Left >= center.X)
            {
                if (toBeInserted.GetBottom() < center.Y)
                {
                    return NodeDirection.NorthEast;
                } 
                else if (toBeInserted.Top >= center.Y)
                {
                    return NodeDirection.SouthEast;
                }
                else
                {
                    return NodeDirection.NotInQuadrant;
                }
            }
            else
            {
                return NodeDirection.NotInQuadrant;
            }
        }

        public void split(QNode node, Box box)
        {
            Debug.Assert(node != null);
            Debug.Assert(isLeaf(node));
            node.children = new QNode[4];

            for (int i = 0; i < node.children.Length; i++)
            {
                node.children[i] = new();
            }

            // decide where nodes should be moved (from parent to child.)
            var newBoxes = new List<Box>();
            for (int i = 0; i < node.boxes.Count; i++)
            {
                var quadrant = findQuadrant(box, node.boxes[i]); 
                if (quadrant != NodeDirection.NotInQuadrant)
                {
                    node.children[(int)quadrant].boxes.Add(node.boxes[i]);
                }
                else
                {
                    Console.WriteLine("added to parent box: ", box.ToString());
                    newBoxes.Add(node.boxes[i]); 
                }
            }
            node.boxes = newBoxes;
        }

        public void Insert(Box box)
        {
            // we start at depth 0
            Insert(root, 0, globalBox, box);
        }

        private void Insert(QNode node, int depth, Box box, Box boxToBeInserted)
        {
            Debug.Assert(node != null);
            if (isLeaf(node))
            {
                // If it's a leaf and there are less than 16 nodes (our threshold) then insert. 
                if (depth >= maxDepth || node.boxes.Count < 1)
                {
                    Console.WriteLine(boxToBeInserted.ToString());
                    node.boxes.Add(boxToBeInserted);
                }
                else
                {
                    // if it's not a leaf, split and retry insertion.
                    split(node, box);
                    Insert(node, depth, box, boxToBeInserted);
                }
            }
            // if it is not a leaf, we have to check if it is fully contained in one of the 4 children
            else
            {
                NodeDirection quad = findQuadrant(box, boxToBeInserted);
                if (quad != NodeDirection.NotInQuadrant)
                {
                    QNode child = node.children[ (int) quad];
                    Insert(child, depth + 1, computeBox(box, quad), boxToBeInserted); 
                }
                else
                {
                    node.boxes.Add(boxToBeInserted);
                }
            }
        }

        // queries possible intersections given a box with the rest of the tree.
        // this will return pairwise intersections, meaning if there is an intersection (A, B) then there will also be an intersection
        // (B, A). Use findPairWiseIntersections for an optimized query.
        public List<Box> query(Box box)
        {
            return internalQuery(root, globalBox, box);
        }
        
        private List<Box> internalQuery(QNode node, Box box, Box queryBox)
        {
            Debug.Assert(node != null);
            List<Box> intersections = new();             
            
            for (int i = 0; i < node.boxes.Count; i++)
            {
                if (queryBox.Intersects(node.boxes[i]))
                {
                    intersections.Add(node.boxes[i]);
                }
            }

            if (!isLeaf(node))
            {
                for (int i = 0; i < node.children.Length; i++)
                {
                    // check every quadrant
                    var cBox = computeBox(box, (NodeDirection)i);
                    if (queryBox.Intersects(cBox))
                    {
                        internalQuery(node.children[i], cBox, queryBox);
                    }
                }
            }
            return intersections;
        }

        // given a quad tree, find all its intersecting boxes.
        public List<Tuple<Box, Box>> findAllIntersections()
        {
            List<Tuple<Box, Box>> intersections = new();
            internalFindAllIntersections(root, intersections);
            return intersections;
        }

        public void internalFindAllIntersections(QNode node, List<Tuple<Box, Box>> intersections)
        {
            for (int i = 0; i < node.boxes.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    if (node.boxes[i].Intersects(node.boxes[j]))
                    {
                        intersections.Add(new Tuple<Box, Box>(node.boxes[i], node.boxes[j]));
                    }
                }
            }

            if (!isLeaf(node))
            {
                for (int i = 0; i < node.children.Length; i++)
                {
                    for (int j = 0; j < node.boxes.Count; j++)
                    {
                        findIntersectingChildren(node.children[i], node.boxes[j], intersections);
                    }
                }

                foreach (var child in  node.children)
                {
                    internalFindAllIntersections(child, intersections);
                }
            }
        }

        public void findIntersectingChildren(QNode node, Box box, List<Tuple<Box, Box>> intersections)
        {
            foreach (var b in node.boxes)
            {
                if (box.Intersects(b))
                {
                    intersections.Add(new Tuple<Box, Box>(box, b));
                }
            }

            if (!isLeaf(node))
            {
                foreach (var child in node.children)
                {
                    findIntersectingChildren(child, box, intersections);
                }
            }
        }
    }

    // we can also calculate this on the fly. Probably promotes better memory efficiency.
    // this class isn't generic because c# generics suck :)
    // sure we can use constraints but they suck as well (we could also propagate it to run-time by casting to dynamic for certain math ops but what's the
    // point of generics then) :)
    class Box
    {
        // (top, left) represents the "topLeft" corner of the bounding box.
        private float top;
        private float left;
        private float width;
        private float height;

        public Box(float top, float left, float width, float height)
        {
            this.top = top;
            this.left = left;
            this.width = width;
            this.height = height;
        }
        
        public float GetRight()
        {
            return left + width;
        }

        public float Top { get => top; set => top = value; }
        public float Left { get => left; set => left = value; }
        public float Width { get => width; set => width = value; }
        public float Height { get => height; set => height = value; }

        public float GetBottom()
        {
            return top + height;
        }

        public Vector2 GetTopLeft()
        {
            return new Vector2(left, top);
        }

        public Vector2 GetCenter()
        {
            return new Vector2(left + width / 2, top + height / 2);
        }

        public Vector2 Size()
        {
            return new Vector2(width, height);
        }
         
        public bool Contains(Box boundingBox)
        {
            return left <= boundingBox.left &&
                GetRight() <= boundingBox.GetRight() &&
                top <= boundingBox.top && GetBottom() <= boundingBox.GetBottom();
        }

        public bool Intersects(Box boundingBox)
        {
            // De Morgan? :))
            return !(left >= boundingBox.GetRight() || GetRight() <= boundingBox.left ||
                top >= boundingBox.GetBottom() || GetBottom() <= boundingBox.top);
        }

        public override string ToString()
        {
            return string.Format("Top: {0}, Left: {1}, Width: {2}, Height: {3}", top, left, width, height);
        }
    }

    class QNode
    {
        public QNode[] children;
        // this is an optimization since it is inefficient to have 1 node store 1 element (in our case a Box), but in theory
        // a quad tree only has 1 element per node.
        // another optimization (due to locality and cache misses) could be to store all nodes in one list (in the tree) and only store indices per node.
        // this is easier but hypothetically less inefficient (according to above statement).
        public List<Box> boxes = new();
    }

    enum NodeDirection
    {
        NorthWest,
        NorthEast,
        SouthWest,
        SouthEast,
        NotInQuadrant 
    }
}
