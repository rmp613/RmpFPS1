using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace RmpFPS1.GameObjects.MapObjects
{
    public class Cube : GameObject
    {
        Matrix scale = Matrix.Identity;
        Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        Matrix startMin;
        Matrix startMax;

        public Cube(Model model, Vector3 position, Matrix scale, Matrix rotation)
            : base(model)
        {
            this.scale = scale;
            this.rotation = rotation;
            this.position = position;
            translation.Translation = position;
            MeshModel(GetWorld());
            startMin = aabb.MatrixMin;
            startMax = aabb.MatrixMax;
            aabb.MatrixMin = startMin * GetWorld();
            aabb.MatrixMax = startMax * GetWorld();
            GameObjectManager.Octree.Add(this);
        }
        public override void Update(GameTime gameTime)
        {
        }
        
        protected override Matrix GetWorld()
        {
            return scale * rotation * translation;
        }
    }
}