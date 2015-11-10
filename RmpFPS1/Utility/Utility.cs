using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace RmpFPS1.Utility
{
    public static class Utility
    {
        public static float QuaternionToEulerY(Quaternion q)
        {
            if (q.X * q.Y + q.Z * q.W != 0.5f)
                return (float)Math.Atan2(2 * q.Y * q.W - 2 * q.X * q.Z, 1 - 2 * Math.Pow(q.Y, 2) - 2 * Math.Pow(q.Z, 2));
            else if (q.X * q.Y + q.Z * q.W == 0.5f)
                return 2 * (float)Math.Atan2(q.X, q.W);
            else
                return -2 * (float)Math.Atan2(q.X, q.W);
        }
    }
}
