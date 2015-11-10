using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RmpFPS1
{
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public Vector3 cameraPos;
        public Vector3 cameraDir;
        public Vector3 cameraLookAt;
        Vector3 cameraFinalTarget;
        Vector2 screenCenter;

        private MouseState currentMouseState;
        private MouseState previousMouseState;

        public float cameraSpeed;
        public float xAngle;
        public float yAngle;
        float YrotationSpeed;
        float XrotationSpeed;
        float yaw = 0;
        float pitch = 0;

        public Matrix view { get; set; }
        public Matrix projection { get; set; }

        public Camera(Game game, Vector3 position, Vector3 target, Vector3 rotation, float speed)
            : base(game)
        {
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
               (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height,
                1, 6000);
            cameraPos = position;
            YrotationSpeed = 0.2f;
            XrotationSpeed = 0.2f;

            screenCenter = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height) / 2;
        }
        
        public void UpdateCamera(float yaw, float pitch, Vector3 position)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw);
            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            cameraDir = cameraRotatedTarget;
            cameraFinalTarget = position + cameraRotatedTarget;

            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedUpVector = Vector3.Transform(Vector3.Up, cameraRotation);

            view = Matrix.CreateLookAt(position, cameraFinalTarget, cameraRotatedUpVector);
        }
        
        private void HandleMouse(GameTime gameTime)
        {
            currentMouseState = Mouse.GetState();
            float amount = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            if (currentMouseState.X != screenCenter.X)
            {
                yaw -= YrotationSpeed * (currentMouseState.X - screenCenter.X) * amount;
            }
            if (currentMouseState.Y != screenCenter.Y)
            {
                pitch -= XrotationSpeed * (currentMouseState.Y - screenCenter.Y) * amount;
                if (pitch > MathHelper.ToRadians(90))
                    pitch = MathHelper.ToRadians(90);
                if (pitch < MathHelper.ToRadians(-50))
                    pitch = MathHelper.ToRadians(-50);
            }
            try
            {
                Mouse.SetPosition((int)screenCenter.X, (int)screenCenter.Y);
            }
            catch { }
        }
        public override void Update(GameTime gameTime)
        {
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;

            currentMouseState = Mouse.GetState();
            HandleMouse(gameTime);
            UpdateCamera(yaw, pitch, cameraPos);

            previousMouseState = currentMouseState;
            base.Update(gameTime);
        }
    }
}