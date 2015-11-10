using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RmpFPS1.Pathfinding
{
    public class Pathfinder
    {
        public struct Point
        {

            public int X;
            public int Z;
            public Point(int X, int Z)
            {
                this.X = X;
                this.Z = Z;
            }

        }
        private int hEstimate = 2;
        private int[,] grid = null;
        private List<Node> closed = new List<Node>();
        private List<Node> openList = new List<Node>();
        private Queue<Node> open = new Queue<Node>(new CompareNode());
        private float heuristic;
        public Pathfinder(int[,] grid)
        {
            if (grid == null)
                throw new Exception("Grid is null");
            this.grid = grid;
        }
        public List<Node> FindPath(int x, int z, int ex, int ez)
        {
            return FindPath(new Pathfinder.Point(x, z), new Pathfinder.Point(ex, ez));
        }
        public List<Node> FindPath(Point start, Point end)
        {
            bool found = false;
            Node parentNode = new Node(start.X, start.Z, 0 + hEstimate, 0, hEstimate);
            int gridBoundX = grid.GetUpperBound(0);
            int gridBoundZ = grid.GetUpperBound(1);
            open.Clear();
            closed.Clear();
            int[,] direction = new int[8, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { 1, -1 }, { 1, 1 }, { -1, 1 }, { -1, -1 } };
            open.Push(parentNode);
            
            while(open.Count > 0 && !found){
                
                parentNode = open.Pop();
                if (parentNode.X == end.X && parentNode.Z == end.Z)
                {
                    closed.Add(parentNode);
                    found = true;
                    break;
                } 
                for (int i = 0; i < 8; i++)
                {
                    Node newNode = new Node(parentNode.X + direction[i, 0], parentNode.Z + direction[i, 1]);
                    if (newNode.X < 0 || newNode.Z < 0 || newNode.X >= gridBoundX || newNode.Z >= gridBoundZ || grid[newNode.X, newNode.Z] == 1)
                    {
                        if (newNode.X == end.X && newNode.Z == end.Z)
                        {
                            closed.Add(newNode);
                            found = true;
                            break;
                        }
                        continue;
                    }
                    float newG = parentNode.G + Node.GetTraversalCost(new Pathfinder.Point(newNode.X, newNode.Z), new Pathfinder.Point(parentNode.X, parentNode.Z));
                    if (newG == parentNode.G)
                    {
                        continue;
                    }
                    int foundInOpenIndex = -1;
                    for (int j = 0; j < open.Count; j++)
                    {
                        if (open[j].X == newNode.X && open[j].Z == newNode.Z)
                        {
                            foundInOpenIndex = j;
                            break;
                        }
                    }
                    if (foundInOpenIndex != -1 && open[foundInOpenIndex].G <= newG)
                        continue;

                    int foundInCloseIndex = -1;
                    for (int j = 0; j < closed.Count; j++)
                    {
                        if (closed[j].X == newNode.X && closed[j].Z == newNode.Z)
                        {
                            foundInCloseIndex = j;
                            break;
                        }
                    }
                    if (foundInCloseIndex != -1 && closed[foundInCloseIndex].G <= newG)
                        continue;
                    heuristic = hEstimate * (Math.Abs(newNode.X - end.X) + Math.Abs(newNode.Z - end.Z));
                    newNode.parent = parentNode;
                    newNode.G = newG;
                    newNode.H = heuristic;
                    newNode.F = newNode.G + newNode.H;
                    open.Push(newNode);
                }
                closed.Add(parentNode);
            }
            if (found)
            {
                Node node = closed[closed.Count - 1];
                for (int i = closed.Count - 1; i >= 0; i--)
                {
                    if (node.parent != null && node.parent.X == closed[i].X && node.parent.Z == closed[i].Z || i == closed.Count - 1)
                    {
                        node = closed[i];
                    }
                    else
                    {
                        closed.RemoveAt(i);
                    }
                }
                return closed;
            }
            return null;
        }
        ////public List<PathFinderNode> FindPath(Point start, Point end)
        ////{
        ////    PathFinderNode parentNode;
        ////    bool found = false;
        ////    int gridX = grid.GetUpperBound(0);
        ////    int gridZ = grid.GetUpperBound(1);
        ////    int[,] direction = new int[8, 2] { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { 1, -1 }, { 1, 1 }, { -1, 1 }, { -1, -1 } };


        ////}
        internal class CompareNode : IComparer<Node>
        {
            public int Compare(Node x, Node z)
            {
                if (x.F > z.F)
                    return 1;
                else if (x.F < z.F)
                    return -1;
                return 0;
            }
        }

    }
}