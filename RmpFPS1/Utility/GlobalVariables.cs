using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace RmpFPS1.Utility
{
    public static class GlobalVariables
    {
        public static int score = 0;
        public static bool youWin = false;
        public static bool Debug = false;
        public static int count = 0;
        public static List<GameObjects.GameObject> gameObjects = new List<GameObjects.GameObject>();
        public struct nodeCountAndSizes
        {
            public float X, Y, Z;
            public int nodes;
            public nodeCountAndSizes(float X, float Y, float Z, int nodes)
            {
                this.X = X;
                this.Y = Y;
                this.Z = Z;
                this.nodes = nodes;
            }
            
        }
    }
}
