using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RmpFPS1.Utility;
using RmpFPS1.GameObjects;

namespace RmpFPS1.Pathfinding
{
    public class Grid
    {
        public struct Point
        {
            public float X;
            public float Z;
            public Point(float X, float Z)
            {
                this.X = X;
                this.Z = Z;
            }
        }
        private int[, ,] layout3d;
        public int[,] layout2d;
        Vector3 nodeSizes;
        int nodeCount;
        GraphicsDevice device;
        Camera camera;
        Point worldExtents;
        Vector3 worldExtentsY;
        public Grid(GraphicsDevice device, Camera camera)
        {
            this.device = device;
            this.camera = camera;
            LoadEntireGrid();
        }
        public Grid(GraphicsDevice device, Camera camera, int[,] gridMap)
        {
            this.device = device;
            this.camera = camera;
            LoadGridFromMap(gridMap);
        }
        public Pathfinder.Point FindOnGrid(Vector3 position)
        {
            for (int x = 0; x <= nodeCount - 1; x++)
            {
                for (int z = 0; z <= nodeCount - 1; z++)
                {
                    if (Math.Abs(-worldExtents.X + nodeSizes.X / 2 + x * nodeSizes.X - position.X) > nodeSizes.X / 2)
                        continue;
                    if (Math.Abs(-worldExtents.Z + nodeSizes.Z / 2 + z * nodeSizes.Z - position.Z) > nodeSizes.Z / 2)
                        continue;
                    return new Pathfinder.Point(x, z);
                }
            }
            return new Pathfinder.Point(-1, -1);
        }
        public Vector3 FindInWorld(Pathfinder.Point point)
        {
            float X = -worldExtents.X + nodeSizes.X / 2 + point.X * nodeSizes.X;    
            float Z = -worldExtents.Z + nodeSizes.Z / 2 + point.Z * nodeSizes.Z;

            return new Vector3(X, 0, Z);
        }
        public int[,] GetGroundNodes()
        {
            int[,] answer = new int[nodeCount, nodeCount];
            for (int x = 0; x <= nodeCount - 1; x++)
            {
                for (int z = 0; z <= nodeCount - 1; z++)
                {
                    answer[x, z] = layout3d[x, 0, z];
                }
            }
            return answer;
        }
        public void DrawNodes()
        {
            for(int x = 0; x <= nodeCount - 1; x++)
            {
                for (int z = 0; z <= nodeCount - 1; z++)
                {
                    if (layout2d[x, z] == 1)
                    {
                        BoxRenderer.Render(
                            new AABB(
                                new Vector3((-worldExtents.X + nodeSizes.X / 2 + nodeSizes.X * x - 10),
                                    (nodeSizes.Y / 2 - 10),
                                    (-worldExtents.Z + nodeSizes.Z / 2 + nodeSizes.Z * z - 10)),
                                new Vector3((-worldExtents.X + nodeSizes.X / 2 + nodeSizes.X * x + 10),
                                    (nodeSizes.Y / 2 + 10),
                                    (-worldExtents.Z + nodeSizes.Z / 2 + nodeSizes.Z * z + 10))),
                            device,
                            camera.view,
                            camera.projection,
                            Color.Red);
                    }
                    else
                        BoxRenderer.Render(
                            new AABB(
                                new Vector3((-worldExtents.X + nodeSizes.X / 2 + nodeSizes.X * x - 10),
                                    (nodeSizes.Y / 2 - 10),
                                    (-worldExtents.Z + nodeSizes.Z / 2 + nodeSizes.Z * z - 10)),
                                new Vector3((-worldExtents.X + nodeSizes.X / 2 + nodeSizes.X * x + 10),
                                    (nodeSizes.Y / 2 + 10),
                                    (-worldExtents.Z + nodeSizes.Z / 2 + nodeSizes.Z * z + 10))),
                            device,
                            camera.view,
                            camera.projection,
                            Color.White);
                    
                }
            }
        }
        public void DrawNodesY(){
            for(int x = 0; x <= nodeCount - 1; x++)
            {
                for (int y = 0; y <= nodeCount - 1; y++)
                {
                    for (int z = 0; z <= nodeCount - 1; z++)
                    {
                        if (layout3d[x, y, z] == 1)
                        {
                            BoxRenderer.Render(
                                new AABB(
                                    new Vector3((-worldExtentsY.X + nodeSizes.X / 2 + nodeSizes.X * x - 10),
                                        (-worldExtentsY.Y + nodeSizes.Y / 2 + nodeSizes.Y * y - 10),
                                        (-worldExtentsY.Z + nodeSizes.Z / 2 + nodeSizes.Z * z - 10)),
                                    new Vector3((-worldExtents.X + nodeSizes.X / 2 + nodeSizes.X * x + 10),
                                        (-worldExtentsY.Y + nodeSizes.Y / 2 + nodeSizes.Y * y + 10),
                                        (-worldExtentsY.Z + nodeSizes.Z / 2 + nodeSizes.Z * z + 10))),
                                device,
                                camera.view,
                                camera.projection,
                                Color.Violet);
                        }
                    }
                }
            }
        }
        public void LoadGridFromMap(int[,] mapGrid)
        {
            layout2d = new int[mapGrid.GetUpperBound(0), mapGrid.GetUpperBound(1)];
            for (int x = 0; x <= nodeCount - 1; x++)
            {
                for (int z = 0; z <= nodeCount - 1; z++)
                {
                    layout2d[x, z] = mapGrid[x, z];
                }
            }
        }
        public void LoadEntireGrid()
        {
            GlobalVariables.nodeCountAndSizes nodeCountAndDistance = GameObjectManager.Octree.CountNodesAndDistance();
            nodeCount = (int)nodeCountAndDistance.nodes;
            nodeSizes = new Vector3(nodeCountAndDistance.X, nodeCountAndDistance.Y, nodeCountAndDistance.Z);
            
            worldExtents = new Point(nodeCount * nodeSizes.X / 2, nodeCount * nodeSizes.Z / 2);
            layout2d = new int[nodeCount, nodeCount];
            //Console.Out.WriteLine(nodeCount);
            for(int x = 0; x <= nodeCount - 1; x++)
            {
                for (int z = 0; z <= nodeCount - 1; z++)
                {
                    int walkable = GameObjectManager.Octree.SurroundingMapObjects(new Vector3(-worldExtents.X + nodeSizes.X / 2 + nodeSizes.X * x, nodeSizes.Y / 2, -worldExtents.Z + nodeSizes.Z / 2 + nodeSizes.Z * z));
                    layout2d[x, z] = walkable; //walkable = 1 or 0 depending on if there is an object with mass in the maxLevel octreeNode that the pathfinding node is in
                }
                
            }
        }
        public void LoadEntireGridY()
        {
            GlobalVariables.nodeCountAndSizes nodeCountAndDistance = GameObjectManager.Octree.CountNodesAndDistance();
            nodeCount = (int)nodeCountAndDistance.nodes;
            nodeSizes = new Vector3(nodeCountAndDistance.X, nodeCountAndDistance.Y, nodeCountAndDistance.Z);

            worldExtentsY = new Vector3(nodeCount * nodeSizes.X / 2, nodeCount * nodeSizes.Y / 2, nodeCount * nodeSizes.Y / 2);
            layout3d = new int[nodeCount, nodeCount, nodeCount];
            for (int x = 0; x <= nodeCount - 1; x++)
            {
                for (int y = 0; y <= nodeCount - 1; y++)
                {
                    for (int z = 0; z <= nodeCount - 1; z++)
                    {
                        int walkable = GameObjectManager.Octree.SurroundingMapObjects(new Vector3(-worldExtentsY.X + nodeSizes.X / 2 + nodeSizes.X * x, -worldExtentsY.Y + nodeSizes.Y / 2 + nodeSizes.Y * y, -worldExtentsY.Z + nodeSizes.Z / 2 + nodeSizes.Z * z));
                        layout3d[x, y, z] = walkable; //walkable = 1 or 0 depending on if there is an object with mass in the maxLevel octreeNode that the pathfinding node is in
                    }
                }
            }
        }
    }
}
