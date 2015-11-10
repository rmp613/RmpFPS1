using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace RmpFPS1.Utility
{
    public static class BoxRenderer //from: https://gist.github.com/demonixis/4003211
    {
        #region Fields

        static VertexPositionColor[] verts = new VertexPositionColor[8];
        static short[] indices = new short[]
	{
		0, 1,
		1, 2,
		2, 3,
		3, 0,
		0, 4,
		1, 5,
		2, 6,
		3, 7,
		4, 5,
		5, 6,
		6, 7,
		7, 4,
	};

        static BasicEffect effect;
        static VertexDeclaration vertDecl;
        #endregion

        /// <summary>
        /// Renders the bounding box for debugging purposes.
        /// </summary>
        /// <param name="box">The box to render.</param>
        /// <param name="graphicsDevice">The graphics device to use when rendering.</param>
        /// <param name="view">The current view matrix.</param>
        /// <param name="projection">The current projection matrix.</param>
        /// <param name="color">The color to use drawing the lines of the box.</param>
        public static void Render(
            AABB box,
            GraphicsDevice graphicsDevice,
            Matrix view,
            Matrix projection,
            Color color)
        {
            
            if (effect == null)
            {
                effect = new BasicEffect(graphicsDevice);
                effect.VertexColorEnabled = true;
                effect.LightingEnabled = false;
            }

            Vector3[] corners = box.getCorners();
            for (int i = 0; i < 8; i++)
            {
                verts[i].Position = corners[i];
                verts[i].Color = color;
            }

            effect.View = view;
            effect.Projection = projection;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    verts,
                    0,
                    8,
                    indices,
                    0,
                    indices.Length / 2);
            }
            
        }
        public static void Render(
           Vector3 center,
           float width,
           GraphicsDevice graphicsDevice,
           Matrix view,
           Matrix projection,
           Color color)
        {

            if (effect == null)
            {
                effect = new BasicEffect(graphicsDevice);
                effect.VertexColorEnabled = true;
                effect.LightingEnabled = false;
            }

            Vector3[] corners = GetCorners(center, width);
            for (int i = 0; i < 8; i++)
            {
                verts[i].Position = corners[i];
                verts[i].Color = color;
            }

            effect.View = view;
            effect.Projection = projection;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                graphicsDevice.DrawUserIndexedPrimitives(
                    PrimitiveType.LineList,
                    verts,
                    0,
                    8,
                    indices,
                    0,
                    indices.Length / 2);
            }

        }
        static Vector3[] GetCorners(Vector3 center, float width)
        {
            Vector3[] corners = new Vector3[8];
            corners[0] = new Vector3(center.X - width, //frontTopLeft
                center.Y + width, center.Z + width);
            corners[1] = new Vector3(center.X + width,
                center.Y + width, center.Z + width);
            corners[2] = new Vector3(center.X + width,
                center.Y - width, center.Z + width);
            corners[3] = new Vector3(center.X - width, //frontBotLeft
                center.Y - width, center.Z + width);
            corners[4] = new Vector3(center.X - width,
                center.Y + width, center.Z - width);
            corners[5] = new Vector3(center.X + width,
                center.Y + width, center.Z - width);
            corners[6] = new Vector3(center.X + width,
                center.Y - width, center.Z - width);
            corners[7] = new Vector3(center.X - width,
                center.Y - width, center.Z - width);
            return corners;
        }
    }
}
