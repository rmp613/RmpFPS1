using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using RmpFPS1.GameObjects;
using RmpFPS1.Utility;

namespace RmpFPS1.Octree
{
    public class Octree
    {
        private OctreeNode _RootNode;

        private int _MaxDepth;
        GraphicsDevice device;
        Camera camera;
        public Octree(AABB bounds, 
            int maxDepth, 
            GraphicsDevice device,
            Camera camera)
        {
            this.device = device;
            this.camera = camera;
            _MaxDepth = maxDepth;
            _RootNode = new OctreeNode(bounds, 0, maxDepth, bounds, device, camera); //bounds must make a cube for pathfinding
        }

        public void Add(GameObject gameObject)
        {
            _RootNode.Add(gameObject);
        }

        public void Add(GameObject gameObject, bool enabled)
        {
            _RootNode.Add(gameObject);
            //_RootNode.GetGameObject(gameObject).Enabled = enabled;
        }

        public void UpdatePosition(GameObject gameObject)
        {
            //var enabled = _RootNode.GetGameObject(gameObject).Enabled;
            _RootNode.Remove(gameObject);
            _RootNode.Add(gameObject);
           // _RootNode.GetGameObject(gameObject).Enabled = enabled;
        }
        public List<GameObject> SurroundingObjects(List<GameObject> returnObjects, GameObject centralObj)
        {
            return _RootNode.SurroundingObjects(returnObjects, centralObj);
        }
        public int SurroundingMapObjects(Vector3 position)
        {
            return _RootNode.SurroundingMapObjects(position, 0);
        }
        public void UpdateEnabled(GameObject gameObject, bool enabled)
        {
           // _RootNode.GetGameObject(gameObject).Enabled = enabled;
        }

        public void Remove(GameObject gameObject)
        {
            _RootNode.Remove(gameObject);
        }
        private OctreeNode MakeNode(AABB bounds, 
            int level,
            int maxDepth)
        {
            return new OctreeNode(bounds, level, maxDepth, bounds, device, camera);
        }
        public void Clear()
        {
            _RootNode = MakeNode(_RootNode.Bounds, 0, _MaxDepth);
        }

        public int CountObjects()
        {
            return _RootNode.CountObjects(0);
        }
        //public int CountNodes()
        //{
        //    return _RootNode.CountNodes(0);
        //}
        public GlobalVariables.nodeCountAndSizes CountNodesAndDistance()
        {
            int nodes = (int)Math.Pow(2, _MaxDepth);//something somewhere, maybe here, is making there only 1quarter walkable/unwalkable nodes
            float xSize = _RootNode.Bounds.Max.X - _RootNode.Bounds.Min.X;
            float ySize = _RootNode.Bounds.Max.Y - _RootNode.Bounds.Min.Y;
            float zSize = _RootNode.Bounds.Max.Z - _RootNode.Bounds.Min.Z;
            float xNodeSize = xSize / nodes;
            float yNodeSize = ySize / nodes;
            float zNodeSize = zSize / nodes;
            return new GlobalVariables.nodeCountAndSizes(xNodeSize, yNodeSize, zNodeSize, nodes);
        }
        public int CountObjectsInRoot()
        {
            return _RootNode.CountObjectsInRoot();
        }
      
        public void Debug()
        {
            _RootNode.Debug();
        }
    }
}
