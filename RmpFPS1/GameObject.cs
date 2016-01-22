using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RmpFPS1.GameObjects
{
    public class GameObject
    {
        public enum ObjectType
        {
            Map,
            Player,
            PlayerProjectile,
            EnemyProjectile,
            Enemy
        };
        public Model model { get; protected set; }
        public Vector3 position = Vector3.Zero;
        public Vector3 direction = Vector3.Zero;
        public AABB aabb;
        public bool IsActive = true;
        public ObjectType type = ObjectType.Map;
        

        public GameObject(Model model)
        {
            this.model = model;
            MeshModel(GetWorld());
        }
        public virtual void Impulse(GameObject gameObject)
        {
            position += aabb.minimumTranslation(gameObject.aabb);
        }
        public virtual void MeshModel(Matrix world)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            // For each mesh of the model
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), Matrix.Identity);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // Create and return bounding box
            aabb = new AABB(min, max);
        }
        //public virtual void MeshModel()
        //{
        //    Matrix[] transforms = new Matrix[model.Bones.Count];
        //    model.CopyAbsoluteBoneTransformsTo(transforms);

        //    foreach (ModelMesh mesh in model.Meshes)
        //    {
        //        Matrix meshTransform = transforms[mesh.ParentBone.Index];
        //        aabb = BuildAABB(mesh, meshTransform);
        //    }
        //}
        //protected AABB BuildAABB(ModelMesh mesh, Matrix meshTransform)
        //{
        //    Vector3 meshMax = new Vector3(float.MinValue);
        //    Vector3 meshMin = new Vector3(float.MaxValue);
        //    foreach (ModelMeshPart part in mesh.MeshParts)
        //    {
        //        int stride = part.VertexBuffer.VertexDeclaration.VertexStride;
        //        VertexPositionNormalTexture[] vertexData = new VertexPositionNormalTexture[part.NumVertices];
        //        part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, stride);
        //        Vector3 vertPosition = new Vector3();
        //        for (int i = 0; i < vertexData.Length; i++)
        //        {
        //            vertPosition = vertexData[i].Position;
        //            meshMin = Vector3.Min(meshMin, vertPosition);
        //            meshMax = Vector3.Max(meshMax, vertPosition);
        //        }
        //    }
        //    meshMin = Vector3.Transform(meshMin, meshTransform);
        //    meshMax = Vector3.Transform(meshMax, meshTransform);

        //    AABB box = new AABB(meshMin, meshMax);

        //    return box;
        //}
        public virtual void Update(GameTime gameTime)
        {
            
        }

        public virtual void Draw(GraphicsDevice device, Camera camera)
        {
            
            Matrix[] transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = transforms[mesh.ParentBone.Index] * GetWorld();
                    effect.View = camera.view;
                    effect.Projection = camera.projection;
                    effect.LightingEnabled = true;
                    effect.EnableDefaultLighting();
                    effect.Alpha = 1;
                }
                mesh.Draw();
            }
            //if (Utility.GlobalVariables.Debug)
            //{
            //    Utility.BoxRenderer.Render(aabb, device, camera.view,
            //        camera.projection, Color.Blue);
            //}
        }

        protected virtual Matrix GetWorld()
        {
            return Matrix.Identity;
        }

    }
}
