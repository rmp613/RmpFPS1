using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RmpFPS1.GameObjects.MapObjects
{
    public class Ground : GameObjects.GameObject
    {
        public Ground(Model model)
            : base(model)
        {
            MeshModel(GetWorld());
            aabb.Min += new Vector3(0, -10, 0);
            Collisions.ground = this;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GraphicsDevice device, Camera camera)
        {
            device.SamplerStates[0] = SamplerState.LinearWrap; //repeat
            base.Draw(device, camera);
        }
    }
}
