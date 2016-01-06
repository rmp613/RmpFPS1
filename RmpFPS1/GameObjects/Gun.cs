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
    public class Gun : GameObject
    {
        Player player;
        Vector3 direction;
        Matrix rotation;
        public Gun(Model model,
            Player player,
            Vector3 position,
            Vector3 direction)
            : base(model)
        {
            base.position = position;
            this.player = player;
            this.direction = direction;
            rotation = player.rotation;
        }
        public override void Update(GameTime gameTime)
        {
            rotation = player.rotation;
            position = player.position + Vector3.Transform(new Vector3(0, 0, -50), rotation);
            direction = player.direction;
            base.Update(gameTime);
        }
        
    }
}
