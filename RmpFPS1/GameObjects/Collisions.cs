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
    static public class Collisions
    {
        static public GameObject ground;
        static public void CheckCollisions(GameObject gameObject)
        {
            List<GameObject> surroundingObjects = new List<GameObject>();
            GameObjectManager.Octree.SurroundingObjects(surroundingObjects, gameObject);

            if (gameObject.aabb.Intersects(ground.aabb)) gameObject.Impulse(ground);

            if (surroundingObjects == null) { return; }

            for (int j = 0; j <= surroundingObjects.Count() - 1; j++)
            {
                //Console.Out.WriteLine("player: " + gameObject.aabb.Min + gameObject.aabb.Max + "platform: " + surroundingObjects[j].aabb.Min + surroundingObjects[j].aabb.Max);
                if (gameObject.aabb.Instersects(surroundingObjects[j].aabb))
                {
                    gameObject.Impulse(surroundingObjects[j]);
                }
            }
        }
        //static public void CheckGlobalCollisions()
        //{
        //    for (int i = 0; i <= GlobalVariables.gameObjects.Count() - 1; i++)
        //    {
        //        List<GameObject> surroundingObjects = GameObjectManager.Octree.SurroundingObjects(GlobalVariables.gameObjects[i]);
        //        if (surroundingObjects == null) return;
        //        for (int j = 0; j <= surroundingObjects.Count() - 1; j++)
        //        {
        //            if (GlobalVariables.gameObjects[i].aabb.Instersects(surroundingObjects[j].aabb))
        //            {
        //                GlobalVariables.gameObjects[i].Impulse(surroundingObjects[j]);
        //            }
        //        }
        //    }
        //}
    }
}