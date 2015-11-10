using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using RmpFPS1.Utility;
using RmpFPS1.GameObjects;
using RmpFPS1.GameObjects.MapObjects;
using RmpFPS1.Octree;

namespace RmpFPS1.GameObjects
{
    public class Projectile : GameObject
    {
        public Vector3 Velocity;
        public Vector3 startPos;
        int speed = 1000;
        Matrix startMin;
        Matrix startMax;
        Matrix scale = Matrix.CreateScale(15, 15, 15);
        Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        GameObjectManager gameObjectManager;
        const int numExplosionParticles = 30;
        const int numExplosionSmokeParticles = 50;
        public Projectile(Model model, 
            Vector3 position, 
            Vector3 direction,
            GameObjectManager gameObjectManager)
            : base(model)
        {
            base.position = position;
            this.gameObjectManager = gameObjectManager;
            startPos = position;
            Velocity = direction;
            Velocity.Normalize();
            type = ObjectType.EnemyProjectile;
            
            translation.Translation = position;
            scale = Matrix.Identity;

            MeshModel(GetWorld());
            startMin = aabb.MatrixMin;
            startMax = aabb.MatrixMax;
            aabb.MatrixMin = startMin * scale * translation;
            aabb.MatrixMax = startMax * scale * translation;
            GameObjectManager.Octree.Add(this);
        }
        public override void Impulse(GameObject gameObject)
        {
            if (type == ObjectType.Map)
            {
                IsActive = false;
                Utility.GlobalVariables.gameObjects.Remove(this);
                GameObjectManager.Octree.Remove(this);
            }
            else if (type == ObjectType.Player)
            {
                Console.Out.WriteLine("hitplayer");
            }
        }
        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalMilliseconds/1000;

            position += (Velocity * elapsed * speed);
            translation.Translation = position;

            if (position.Y < 0 || Vector3.Distance(startPos, position) > 2000)
            {
                IsActive = false;
            }

            aabb.MatrixMin = startMin * scale * translation;
            aabb.MatrixMax = startMax * scale * translation;

            GameObjectManager.Octree.UpdatePosition(this);
            Collisions.CheckCollisions(this);
            base.Update(gameTime);
        }
       
        protected override Matrix GetWorld()
        {
            return scale * rotation * translation;
        }
    }
}
