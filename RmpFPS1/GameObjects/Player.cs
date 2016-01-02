using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace RmpFPS1.GameObjects
{
    public class Player : GameObject
    {
        public Vector3 playerPos;
        public Vector3 playerDir { get; protected set; }
        private Vector3 pastPos;
        private Matrix startMin;
        private Matrix startMax;
        private Vector3 maxVelocity = new Vector3(200, 200, 200);
        private Vector3 defaultMaxVelocity = new Vector3(200, 200, 200);
        private Vector3 sprintMaxVelocity = new Vector3(350, 200, 350);
        private Vector3 crouchMaxVelocity = new Vector3(100, 200, 100);
        Vector3 xzVector = new Vector3(1, 0, 1);
        Vector3 cameraUp;
        public Vector3 Velocity;
        MouseState currentMouse;
        float debugCooldown = 0;

        public Vector3 Acceleration = new Vector3(0, 0, 0);
        float maxAcceleration = 1000;
        float accelerationFactor = 700;
        float deccelerationFactor = 0.7f;
        float defaultDeccelerationFactor = 0.7f;
        float friction = 0.9f;
        float defaultFriction = 0.9f;
        float sprintFriction = 0.95f;
        float crouchFriction = 0.85f;

        float jumpHeight = 500;
        float defaultMaxAcceleration = 1000;
        float time = 0;
        float gravity = -9.8f / 10;
        public float playerHeight;
        public float cameraHeight;
        float defaultPlayerHeight;
        public int health = 3;
        public int maxHealth = 3;

        bool crouching = false;
        public bool isHit = false;
        bool jumping = false;
        bool jumpPressed = false;
        bool sprinting = false;
        bool xzInput = false;
        bool isColliding = false;
        bool feetColliding = false;
        Matrix scale = Matrix.Identity;
        Matrix normalScale = Matrix.CreateScale(0.25f, 1, 0.25f);
        Matrix crouchingScale = Matrix.CreateScale(0.25f, 0.5f, 0.25f);
        public List<AABB> playerBoxes = new List<AABB>();
        Matrix translation = Matrix.Identity;
        Matrix rotation = Matrix.Identity;
        Matrix[] transforms;
        public Camera camera { get; protected set; }
        MouseState prevMouseState;
        Game game;
        GameObjectManager gameObjectManager;
        SoundEffectInstance moveSound;
        SoundEffect deathSound;
        SoundEffectInstance hitSound;
        List<GameObject> hitBy = new List<GameObject>();

        public Player(Model model, 
            GraphicsDevice device, 
            Camera cam, 
            Game game,
            GameObjectManager gameObjectManager)
            : base(model)
        {
            this.gameObjectManager = gameObjectManager;
            this.game = game;
            scale = normalScale;
            cameraUp = Vector3.Up;
            type = ObjectType.Player;
            camera = ((Game1)game).camera;

            position = camera.cameraPos;
            playerDir = camera.cameraDir;
            camera.cameraPos = position;
            translation.Translation = position;
            Mouse.SetPosition(game.Window.ClientBounds.Width / 2,
                game.Window.ClientBounds.Height / 2);
            prevMouseState = Mouse.GetState();
            MeshModel(GetWorld());
            startMin = aabb.MatrixMin;
            startMax = aabb.MatrixMax;
            aabb.MatrixMin = startMin * GetWorld();
            aabb.MatrixMax = startMax * GetWorld();
            defaultPlayerHeight = startMax.Translation.Y - startMin.Translation.Y;
            playerHeight = defaultPlayerHeight;
            GameObjectManager.Octree.Add(this);
        }
        
        
        public override void Impulse(GameObject gameObject)
        {
            isColliding = false;
            Vector3 impulse = aabb.minimumTranslation(gameObject.aabb);

            if (gameObject.type != ObjectType.EnemyProjectile && gameObject.type != ObjectType.PlayerProjectile)
            {
                position += impulse;
                if (aabb.CollisionNormal == AABB.Normals[3] && !jumpPressed)
                {
                    Velocity *= new Vector3(1, aabb.ChangeVelocity().Y, 1);
                    jumping = false;
                    feetColliding = true;
                } 
                Velocity *= new Vector3(aabb.ChangeVelocity().X, 1, aabb.ChangeVelocity().Z);
                Acceleration *= new Vector3(aabb.ChangeVelocity().X, 1, aabb.ChangeVelocity().Z);
                
                isColliding = true;
            }
            if (gameObject.type == ObjectType.EnemyProjectile || gameObject.type == ObjectType.Enemy)
            {
                bool alreadyHitBy = false;
                foreach (GameObject obj in hitBy)
                {
                    if (obj.Equals(gameObject))
                        alreadyHitBy = true;
                }
                if (!alreadyHitBy)
                {
                    health--;
                    hitBy.Add(gameObject);
                }
                if (gameObject.type == ObjectType.EnemyProjectile)
                {
                    gameObject.IsActive = false;
                    Utility.GlobalVariables.gameObjects.Remove(gameObject);
                    GameObjectManager.Octree.Remove(gameObject);
                }
            }
        }

        public void EulerIntergrate(float deltaTime)
        {
            position += Velocity * deltaTime;
            Velocity += Acceleration * deltaTime * accelerationFactor;

           
        }
        private void addAcceleration(float time)
        {
            Crouch();
            Sprint();

            EulerIntergrate(time);
            if (feetColliding)
            {
                Velocity.X *= friction;
                Velocity.Z *= friction;
            }
            else
            {
                Acceleration.Y = gravity;
                Acceleration.X = 0;
                Acceleration.Z = 0;
            }
           
            Acceleration.X *= deccelerationFactor;
            Acceleration.Z *= deccelerationFactor;
            
            if (Acceleration.X < 0.01f && Acceleration.X > -0.01f)
                Acceleration.X = 0;
            if (Acceleration.Y < 0.01f && Acceleration.Y > -0.01f)
                Acceleration.Y = 0;
            if (Acceleration.Z < 0.01f && Acceleration.Z > -0.01f)
                Acceleration.Z = 0;
        }
        private void Crouch()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                crouching = true;
                if (scale.Scale.Y > crouchingScale.Scale.Y)
                {
                    scale.M22 -= 0.1f;
                }
                else scale = crouchingScale;
                if (playerHeight > defaultPlayerHeight / 2)
                {
                    playerHeight -= 10;
                }
                else playerHeight = defaultPlayerHeight / 2;
                friction = crouchFriction;
            }
            else
            {
                crouching = false;
                if (scale.Scale.Y < normalScale.Scale.Y)
                {
                    scale.M22 += 0.1f;
                }
                if (playerHeight < defaultPlayerHeight)
                {
                    playerHeight += 10;
                }
                else playerHeight = defaultPlayerHeight;
                friction = defaultFriction;
            }
        }

        private void Sprint()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.LeftShift) && !crouching)
            {
                sprinting = true;
                friction = sprintFriction;
            }
            else if (!Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.LeftShift) && !jumping && !crouching)
                friction = defaultFriction;
        }
        private void Input(float time)
        {
            debugCooldown += time;
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                 Acceleration += playerDir * xzVector;
                
                //else Acceleration += playerDir * xzVector / 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                 Acceleration -= playerDir * xzVector;
                
                //else Acceleration -= playerDir * xzVector / 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                
                    Acceleration += Vector3.Cross(cameraUp, playerDir) * xzVector;
                
                //else Acceleration += Vector3.Cross(cameraUp, playerDir) * xzVector / 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                
                    Acceleration -= Vector3.Cross(cameraUp, playerDir) * xzVector;
                
                //else Acceleration -= Vector3.Cross(cameraUp, playerDir) * xzVector / 2;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space) && (!jumping || Utility.GlobalVariables.Debug))
            {
                jumpPressed = true;
                jumping = true;
                if (!Utility.GlobalVariables.Debug)
                    Velocity.Y += 300;
                else
                    Velocity.Y += 50;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.K) && debugCooldown > 0.5)
            {
                debugCooldown = 0;
                if (Utility.GlobalVariables.Debug)
                    Utility.GlobalVariables.Debug = false;
                else
                    Utility.GlobalVariables.Debug = true;
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            currentMouse = Mouse.GetState();
            playerDir = camera.cameraDir;
            hitBy.Clear();
            isHit = false;
            time = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000;

            Input(time);
            addAcceleration(time);
            feetColliding = false;
            jumpPressed = false;
            rotation = Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), (float)Math.Atan2(playerDir.X, playerDir.Z));

            camera.cameraPos = position + new Vector3(0, playerHeight / 2, 0);

            if (health <= 0)
            {
                IsActive = false;
                gameObjectManager.currentGameState = GameState.GameOver;
            }
            translation.Translation = position;
            aabb.MatrixMin = startMin * scale * translation;
            aabb.MatrixMax = startMax * scale * translation;
            GameObjectManager.Octree.UpdatePosition(this);
            Collisions.CheckCollisions(this);
            base.Update(gameTime);
        }
        public override void Draw(GraphicsDevice device, Camera camera) //dont draw player cause no point atm (1st person)
        {
        }
        protected override Matrix GetWorld()
        {
            return scale * rotation * translation;
        }
    }
}

