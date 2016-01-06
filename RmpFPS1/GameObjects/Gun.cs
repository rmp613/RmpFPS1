using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RmpFPS1.Utility;
using RmpFPS1.GameObjects;
using RmpFPS1.GameObjects.MapObjects;
using RmpFPS1.Octree;

namespace RmpFPS1.GameObjects
{
    public class Gun : GameObject
    {
        Player player;
        Matrix rotation = Matrix.Identity;
        Matrix scale = Matrix.CreateScale(1);
        Matrix translation = Matrix.Identity;
        public Gun(Model model,
            Player player,
            Vector3 position,
            Vector3 direction)
            : base(model)
        {
            base.position = position;
            this.player = player;
            base.direction = direction;
            rotation = player.rotation;
            translation.Translation = player.position;
        }
        public override void Update(GameTime gameTime)
        {
            //position = new Vector3(0, 50, 0);
            //rotation = Matrix.Identity;  + new Vector3(0, player.playerHeight/2, 0);new Vector3(50, 0, 0), rotation) + V
            //translation = Matrix.Identity;
            rotation = player.rotation;
            position = player.position + Vector3.Transform(new Vector3(0, 0, 100), rotation);
            translation.Translation = position;

            direction = player.direction;
            Console.Out.WriteLine("gunpos:" + position + "playerpos: " + player.position);
            base.Update(gameTime);
        }
        protected override Matrix GetWorld()
        {
            return scale * rotation * translation;
        }
        
    }
}
