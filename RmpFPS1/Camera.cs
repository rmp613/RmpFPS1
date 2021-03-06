﻿using System;
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

        public float speed;
        public float xAngle;
        public float yAngle;
        float YrotationSpeed;
        float XrotationSpeed;
        public float yaw = 0;
        public float pitch = 0;
        public Matrix rotation;

        public Matrix view { get; set; }
        public Matrix projection { get; set; }
        Vector3 target;
        public Camera(Game game, Vector3 position, Vector3 target, float speed)
            : base(game)
        {
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
               (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height,
                .00001f, 10000f);
            cameraPos = position;
            this.speed = speed;
            YrotationSpeed = speed;
            XrotationSpeed = speed;
            this.target = target;
            rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);

            screenCenter = new Vector2(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height) / 2;
        }
        
        public void UpdateCamera(float yaw, float pitch, Vector3 position)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw);
            
            Vector3 cameraRotatedTarget = Vector3.Transform(target, cameraRotation);
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
            rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);

            UpdateCamera(yaw, pitch, cameraPos);

            previousMouseState = currentMouseState;
            base.Update(gameTime);
        }

    }
}