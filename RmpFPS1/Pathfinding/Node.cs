using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RmpFPS1.Pathfinding
{
    public class Node
    {
        public Node parent;
        public int X, Z;
        public float F, G, H;
        public Node(int x, int z, float f, float g, float h)
        {
            X = x;
            Z = z;
            F = f;
            G = g;
            H = h;
        }
        public Node(int x, int z)
        {
            X = x;
            Z = z;
            F = 0;
            G = 0;
            H = 0;
        }
        public Node()
        {
            X = 0;
            Z = 0;
            F = 0;
            G = 0;
            H = 0;
        }
        internal static float GetTraversalCost(Pathfinder.Point location, Pathfinder.Point otherLocation)
        {
            float deltaX = otherLocation.X - location.X;
            float deltaY = otherLocation.Z - location.Z;
            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}