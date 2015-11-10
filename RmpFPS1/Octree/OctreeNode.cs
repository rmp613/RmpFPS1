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
    class OctreeNode
    {

        private bool _IsLeaf;
        private int _Level;
        private int _MaxLevel;
        private List<GameObject> _GameObjects;
        private OctreeNode[] _Children;
        private AABB _ActualBounds; 
        private GraphicsDevice device;
        private Camera camera;
        public ReadOnlyCollection<GameObject> Contents { get; private set; }

        public AABB Bounds { get; private set; }
        AABB OctreeBounds;
        public OctreeNode(AABB bounds, 
            int level, 
            int maxLevel,
            AABB OctreeBounds,
            GraphicsDevice device,
            Camera camera)
        {
            _Level = level;
            _MaxLevel = maxLevel;
            _IsLeaf = true;
            _Children = new OctreeNode[8];
            _GameObjects = new List<GameObject>(1);
            _ActualBounds = bounds;
            this.Bounds = bounds;
            this.Contents = _GameObjects.AsReadOnly();
            this.device = device;
            this.camera = camera;
            this.OctreeBounds = OctreeBounds;
        }
        public List<GameObject> SurroundingObjects(List<GameObject> returnObjects, GameObject centralObj)
        {
           
            if (_IsLeaf && _GameObjects.Contains(centralObj) && _GameObjects.Count() > 1)
            {
                foreach (GameObject obj in _GameObjects)
                {
                    if (!centralObj.Equals(obj)) returnObjects.Add(obj);
                }
            }
            else if (!_IsLeaf)
            {
                for (int i = 0; i < 8; i++)
                {
                    _Children[i].SurroundingObjects(returnObjects, centralObj);
                }
            }
            return returnObjects;
        }
        public int SurroundingMapObjects(Vector3 position, int answer)
        {

            if (_IsLeaf && _Level == _MaxLevel && Bounds.Contains(position) && _GameObjects.Count() > 0)
            {
                foreach (GameObject obj in _GameObjects)
                {
                    if (obj.type == GameObject.ObjectType.Map) answer = 1;
                }
            }
            else if (_IsLeaf && _MaxLevel != _Level)
            {
                addChildren();
                for (int i = 0; i < 8; i++)
                {
                    answer += _Children[i].SurroundingMapObjects(position, answer);
                }
            }
            else if (!_IsLeaf )
            {
                for (int i = 0; i < 8; i++)
                {
                    answer += _Children[i].SurroundingMapObjects(position, answer);
                }
            }
            if (answer > 1) answer = 1;
            return answer;
        }
        
        public void Debug()
        {
            Color[] color = new Color[7];
            color[0] = Color.Red;
            color[1] = Color.Blue;
            color[2] = Color.Yellow;
            color[3] = Color.Green;
            color[4] = Color.Orange;
            color[5] = Color.Beige;
            color[6] = Color.Gold;
            Utility.BoxRenderer.Render(Bounds, device, camera.view, camera.projection, color[_Level]);

            if (!_IsLeaf)
            {
                for (int j = 0; j < 8; j++)
                {
                    _Children[j].Debug();
                }
            }
        }
        
        public int CountObjectsInRoot()
        {
            return _GameObjects.Count();
        }
        public int CountObjects(int answerSoFar)
        {
            int answer = answerSoFar;
            if (_IsLeaf)
            {
                answer += _GameObjects.Count();
            }
            else
            {
                for (var j = 0; j < 8; j++)
                {
                    answer = _Children[j].CountObjects(answer);
                }
            }
            return answer;
        }
        public int CountNodes(int answerSoFar)
        {
            int answer = answerSoFar;
            if (_IsLeaf)
            {
                answer++;
            }
            else
            {
                for (var j = 0; j < 8; j++)
                {
                    answer += _Children[j].CountNodes(answer);
                }
            }
            return answer;
        }
        public void addChildren()
        {
            
            _IsLeaf = false;

            Vector3 ex = this.Bounds.Extents;

            _Children[0] = new OctreeNode(new AABB(this.Bounds.Min, this.Bounds.Center), _Level + 1, _MaxLevel, OctreeBounds, device, camera);
            _Children[1] = new OctreeNode(new AABB(this.Bounds.Min + new Vector3(0, 0, ex.Z), this.Bounds.Center + new Vector3(0, 0, ex.Z)), _Level + 1, _MaxLevel, OctreeBounds, device, camera);
            _Children[2] = new OctreeNode(new AABB(this.Bounds.Min + new Vector3(0, ex.Y, 0), this.Bounds.Center + new Vector3(0, ex.Y, 0)), _Level + 1, _MaxLevel, OctreeBounds, device, camera);
            _Children[3] = new OctreeNode(new AABB(this.Bounds.Min + new Vector3(0, ex.Y, ex.Z), this.Bounds.Center + new Vector3(0, ex.Y, ex.Z)), _Level + 1, _MaxLevel, OctreeBounds, device, camera);
            _Children[4] = new OctreeNode(new AABB(this.Bounds.Min + new Vector3(ex.X, 0, 0), this.Bounds.Center + new Vector3(ex.X, 0, 0)), _Level + 1, _MaxLevel, OctreeBounds, device, camera);
            _Children[5] = new OctreeNode(new AABB(this.Bounds.Min + new Vector3(ex.X, 0, ex.Z), this.Bounds.Center + new Vector3(ex.X, 0, ex.Z)), _Level + 1, _MaxLevel, OctreeBounds, device, camera);
            _Children[6] = new OctreeNode(new AABB(this.Bounds.Min + new Vector3(ex.X, ex.Y, 0), this.Bounds.Center + new Vector3(ex.X, ex.Y, 0)), _Level + 1, _MaxLevel, OctreeBounds, device, camera);
            _Children[7] = new OctreeNode(new AABB(this.Bounds.Center, this.Bounds.Max), _Level + 1, _MaxLevel, OctreeBounds, device, camera);

            // move any existing items
            for (int k = 0; k <= _GameObjects.Count() - 1; k++)
            {
                GameObject i = _GameObjects[k];
                if (!i.aabb.Center.Equals(this.Bounds.Center))
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (_Children[j].Intersects(i.aabb))
                            _Children[j].Add(i);
                    }

                    _GameObjects.Remove(i);
                }
            }
            
        }
        public void Add(GameObject obj)
        {
            if (!OctreeBounds.Intersects(obj.aabb))
                return;
            else if ((_IsLeaf && _GameObjects.Count == 0) || _Level == _MaxLevel)
            {
                _GameObjects.Add(obj);
            }
            else
            {
                if (obj.aabb.Center.Equals(this.Bounds.Center) || (_GameObjects.Count > 0 && _GameObjects.TrueForAll(i => obj.aabb.Center.Equals(i.aabb.Center))))
                {
                    _GameObjects.Add(obj);
                }
                else
                {
                    if (_IsLeaf)
                    {
                        addChildren();
                    }
                    // add new item into child
                    for (var j = 0; j < 8; j++)
                    {
                        if (_Children[j].Intersects(obj.aabb))
                            _Children[j].Add(obj);
                    }
                }
            }
        }
        public void RemoveChildren()
        {
            int emptyChildren = 0;
            if (!_IsLeaf)
            {
                for (var i = 0; i < 8; i++)
                {
                    if (_Children[i].IsEmpty())
                    {
                        emptyChildren++;
                    }
                }
            }

            if (emptyChildren == 8)
            {
                _IsLeaf = true;
                _Children = new OctreeNode[8];
            }
        }
        public void Remove(GameObject gameObject)
        {
            _GameObjects.Remove(gameObject);

            if (!_IsLeaf)
            {
                int emptyChildren = 0;
                bool hasFloater = true;
                List<GameObject> floaters = null;

                for (var i = 0; i < 8; i++)
                {
                    _Children[i].Remove(gameObject);

                    if (_Children[i].IsEmpty())
                    {
                        emptyChildren++;
                    }
                    else if (_Children[i].IsLeaf())
                    {
                        if (floaters == null)
                        {
                            floaters = _Children[i].Contents.ToList();
                        }
                        else if (!floaters.Equals(_Children[i].Contents.ToList()))
                        {
                            hasFloater = false;
                        }
                    }
                    else
                    {
                        hasFloater = false;
                    }
                }

                if (emptyChildren == 8)
                {
                    _IsLeaf = true;
                    _Children = new OctreeNode[8];
                }
                else if (hasFloater && _GameObjects.Count == 0)
                {
                    foreach (GameObject obj in floaters)
                        _GameObjects.Add(obj);

                    _IsLeaf = true;
                    _Children = new OctreeNode[8];
                }
            }
        }
        public GameObject GetGameObject(GameObject gameObject)
        {
            GameObject match = _GameObjects.FirstOrDefault(c => c == gameObject);

            if (match != null)
                return match;
            else if (!_IsLeaf)
            {
                for (var i = 0; i < 8; i++)
                {
                    match = _Children[i].GetGameObject(gameObject);

                    if (match != null)
                        return match;
                }
            }

            return null;
        }
        public bool IsEmpty()
        {
            return _IsLeaf && _GameObjects.Count == 0;
        }

        public bool IsLeaf()
        {
            return _IsLeaf;
        }

        public bool Intersects(AABB bounds)
        {
            return _ActualBounds.Intersects(bounds);
        }

    }
}