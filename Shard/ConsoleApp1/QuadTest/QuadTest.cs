using Pinball;
using Shard.Shard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shard
{
    internal class QuadTest : Game
    {
        public override void initialize()
        {
            Display display = Bootstrap.getDisplay();
            int width = display.getWidth();
            int height = display.getHeight();
            int x = 1;
            int y = 1;
            QuadTree qt = new(x, y, (int)width, (int)height);
            Box b = new Box(400, 400, 40, 40);
            qt.Insert(b);
            Box b1 = new Box(100, 100, 20, 20);
            qt.Insert(b1);
            Box b2 = new Box(200, 200, 20, 20);
            qt.Insert(b2);
            Box b3 = new Box(800, 600, 20, 20);
            qt.Insert(b3);
            Box b4 = new Box(400, 400, 30, 30);
            qt.Insert(b4);
            var intersections = qt.findAllIntersections();

            drawBoxes(qt);
        }

        public void drawBoxes(QuadTree qt)
        {
            var boxes = traverseQuadTree(qt);
            foreach (var b in boxes)
            {
                PinballRectangle r = new("", (int)b.Left, (int)b.Top, (int)b.Width, (int)b.Height);
            }
        }

        // basically bfs
        public List<Box> traverseQuadTree(QuadTree qt)
        {
            var boxes = new List<Box>();
            var nodes = new List<QNode>();
            var currNode = qt.Root;
            while(currNode != null)
            {
                boxes.AddRange(currNode.boxes);
                if (currNode.children is not null)
                {
                    foreach(var child in currNode.children)
                    {
                        if (child.boxes.Count > 0)
                            nodes.Add(child);
                    }
                }

                if (nodes.Count > 0)
                {
                    currNode = nodes[nodes.Count - 1];
                    nodes.RemoveAt(nodes.Count - 1);
                } 
                else
                {
                    currNode = null;
                }
            }
            return boxes;
        }

        public override void update()
        {
        }
    }
}
