using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using RmpFPS1.Utility;
using RmpFPS1.GameObjects;
using RmpFPS1.GameObjects.MapObjects;
using RmpFPS1.Octree;
using RmpFPS1.Pathfinding;
using System.IO;

namespace RmpFPS1.GameObjects
{
    public class GameObjectManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public static List<GameObject> models = new List<GameObject>(); 
        public static List<GameObject> mapObjects = new List<GameObject>();
        int mapChangeCount = 0;
        Player player;
        Ground ground;
        int winGap = 0;
        float playerShotCooldown = 0.3f;
        float playerShotTimer = 0.0f;
        float enemyShotCooldown = 1f;
        float enemyShotTimer = 0.0f;
        float elapsed;
        Texture2D crosshairTexture;
        SpriteFont Arial;
        Game1 game;
        Camera camera;
        bool firstRun = true;
        public GameState currentGameState = new GameState();
        public static Octree.Octree Octree;
        public static Grid Grid;
        public static Pathfinder Pathfinder;
        int delayTimer = 0;
        int enemyCount = 0;
        int startingEnemyCount = 0;
        public Random rnd;
        Texture2D explosionTexture;
        Texture2D explosionColorsTexture;
        Effect explosionEffect;
        public int[,] mapGrid = new int[4000 / 100, 4000 / 100];
        bool isTextMap = true;

        public GameObjectManager(Game1 game, Camera camera)
            : base(game)
        {
            this.game = game;
            this.camera = camera;
            rnd = new Random();
            Octree = new Octree.Octree(new AABB(new Vector3(-2000, 0, -2000), 
                new Vector3(2000, 4000, 2000)),
                5, 
                game.GraphicsDevice,
                camera);
            //loads map from txt. comment out and uncomment addwalls() and addplatforms() in loadcontent to load the map normally and thereby see all features that dont quite work in txt loaded map
            //loadMap();
        }
        public override void Initialize()
        {
            currentGameState = GameState.InGame;
            base.Initialize();
        }
        private void loadMap()
        {
            int x = -2000;
            int z = -2000;
            int y = 500;
            int py = 0;
            string[] mapData = File.ReadAllLines(@"Content/Map/mapInfo.txt");
            foreach ( string data in mapData)
            {
               
                char[] characters = data.ToCharArray();
                x = -2000;
                foreach (char c in characters)
                {
                    Vector3 loca;
                    if(c.Equals('3'))
                    {
                        loca = new Vector3(x, py, z);
                        py+=100;
                    }
                    else
                    {
                        loca = new Vector3(x, y, z);
                        int X = (x + 2000) / 100;
                        int Z = (z + 2000) / 200;
                        mapGrid[X, Z] = 1;
                    }
                    createObstacles(c, loca);
                    x += 100;
                }
                    z += 200;
            }    
        }

        private void createObstacles(char character, Vector3 loca)
        {
            Matrix defaultScale = Matrix.CreateScale(2f, 2f, 2f);
            Matrix platScale = Matrix.CreateScale(2f, 0.2f, 2f);
            Matrix zBoundaryScale = Matrix.CreateScale(2f, 20f, 2f);
            Matrix xBoundaryScale = Matrix.CreateScale(2f, 20f, 2f);
            Vector3 ObVec = new Vector3(0, -400, 0);
            Matrix defaultRot = Matrix.Identity;
            switch (character)
            {
                case '0': // 0 is null
                    break;
                case '1':// 1 is wall
                    {
                    Cube cube = new Cube(
                    Game.Content.Load<Model>(@"Models/Objects/Wall"),
                    loca,
                    xBoundaryScale,
                    defaultRot);
                    GlobalVariables.gameObjects.Add(cube);
                        Octree.Add(cube); 
                        break;
                    }
                case '2': // 2 is obstacles
                    {
                        Cube cube = new Cube(
                                            Game.Content.Load<Model>(@"Models/Objects/Wall"),
                                            loca + ObVec,
                                            defaultScale,
                                            defaultRot);
                        GlobalVariables.gameObjects.Add(cube);
                        Octree.Add(cube);
                        break;
                    }
                    
                case '3': // 3 is platform
                    {
                        Cube cube = new Cube(
                            Game.Content.Load<Model>(@"Models/Objects/Platform"),
                            loca,
                            platScale,
                            defaultRot);
                        GlobalVariables.gameObjects.Add(cube);
                        Octree.Add(cube); 
                        break;
                    }
                    
                default:
                    break;
            }
        }
       
        private void AddPlatforms()
        {
            Matrix platScale = Matrix.CreateScale(2.5f, 0.2f, 2.5f);
            Matrix platRotation = Matrix.Identity;
            GlobalVariables.gameObjects.Add(new Cube(
                 Game.Content.Load<Model>(@"Models/Objects/Platform"),
                 new Vector3(-900, 100, -900),
                 platScale,
                 platRotation));
            GlobalVariables.gameObjects.Add(new Cube(
                 Game.Content.Load<Model>(@"Models/Objects/Platform"),
                 new Vector3(-700, 200, -700),
                 platScale,
                 platRotation));
            GlobalVariables.gameObjects.Add(new Cube(
                 Game.Content.Load<Model>(@"Models/Objects/Platform"),
                 new Vector3(-500, 300, -500),
                 platScale,
                 platRotation));
            GlobalVariables.gameObjects.Add(new Cube(
                  Game.Content.Load<Model>(@"Models/Objects/Platform"),
                  new Vector3(-300, 400, -300),
                 platScale,
                 platRotation));
            GlobalVariables.gameObjects.Add(new Cube(
                  Game.Content.Load<Model>(@"Models/Objects/Platform"),
                  new Vector3(-100, 500, -100),
                 platScale,
                 platRotation));
            GlobalVariables.gameObjects.Add(new Cube(
                   Game.Content.Load<Model>(@"Models/Objects/Platform"),
                   new Vector3(100, 600, 100),
                  platScale,
                  platRotation));
            GlobalVariables.gameObjects.Add(new Cube(
                  Game.Content.Load<Model>(@"Models/Objects/Platform"),
                  new Vector3(300, 700, 300),
                 platScale,
                 platRotation));
            GlobalVariables.gameObjects.Add(new Cube(
                  Game.Content.Load<Model>(@"Models/Objects/Platform"),
                  new Vector3(500, 800, 500),
                 platScale,
                 platRotation));
            GlobalVariables.gameObjects.Add(new Cube(
                  Game.Content.Load<Model>(@"Models/Objects/Platform"),
                  new Vector3(700, 900, 700),
                 platScale,
                 platRotation));
            GlobalVariables.gameObjects.Add(new Cube(
                  Game.Content.Load<Model>(@"Models/Objects/Platform"),
                  new Vector3(900, 1000, 900),
                 platScale,
                 platRotation));
            GlobalVariables.gameObjects.Add(new Cube(
                  Game.Content.Load<Model>(@"Models/Objects/Platform"),
                  new Vector3(-900, 800, 900),
                 platScale,
                 platRotation));
            GlobalVariables.gameObjects.Add(new Cube(
                   Game.Content.Load<Model>(@"Models/Objects/Platform"),
                   new Vector3(900, 600, -900),
                  platScale,
                  platRotation));
            GlobalVariables.gameObjects.Add(new Cube(
                  Game.Content.Load<Model>(@"Models/Objects/Platform"),
                  new Vector3(-900, 400, -900),
                 platScale,
                 platRotation));
        }
        private void AddWalls()
        {
            Matrix defaultScale = Matrix.CreateScale(0.4f, 3f, 9f);
            Matrix defaultScaleZ = Matrix.CreateScale(9f, 3f, 0.4f);
            Matrix zBoundaryScale = Matrix.CreateScale(2f, 20f, 40f);
            Matrix xBoundaryScale = Matrix.CreateScale(40f, 16f, 2f);
            Matrix defaultRot = Matrix.Identity;
            GlobalVariables.gameObjects.Add(new Cube(
                    Game.Content.Load<Model>(@"Models/Objects/Wall"),
                    new Vector3(2000, 500, 0),
                    zBoundaryScale,
                    defaultRot)); 
            GlobalVariables.gameObjects.Add(new Cube(
                    Game.Content.Load<Model>(@"Models/Objects/Wall"),
                    new Vector3(-2000, 500, 0),
                    zBoundaryScale,
                    defaultRot)); 
            GlobalVariables.gameObjects.Add(new Cube(
                    Game.Content.Load<Model>(@"Models/Objects/Wall"),
                    new Vector3(0, 500, 2000),
                    xBoundaryScale,
                    defaultRot)); 
            GlobalVariables.gameObjects.Add(new Cube(
                    Game.Content.Load<Model>(@"Models/Objects/Wall"),
                    new Vector3(0, 500, -2000),
                    xBoundaryScale,
                    defaultRot));
            GlobalVariables.gameObjects.Add(new Cube(
                 Game.Content.Load<Model>(@"Models/Objects/Wall"),
                 new Vector3(100, 100, -300),
                 defaultScale,
                 defaultRot));
            GlobalVariables.gameObjects.Add(new Cube(
                 Game.Content.Load<Model>(@"Models/Objects/Wall"),
                 new Vector3(900, 100, -300),
                 defaultScale,
                 defaultRot));
            GlobalVariables.gameObjects.Add(new Cube(
                 Game.Content.Load<Model>(@"Models/Objects/Wall"),
                 new Vector3(450, 100, 300),
                 defaultScaleZ,
                 defaultRot));
            GlobalVariables.gameObjects.Add(new Cube(
                 Game.Content.Load<Model>(@"Models/Objects/Wall"),
                 new Vector3(1500, 100, 1100),
                 defaultScaleZ,
                 defaultRot));
            //GlobalVariables.gameObjects.Add(new Cube(
            //      Game.Content.Load<Model>(@"Models/Objects/Cube"),
            //      new Vector3(600, 100, 600),
            //     boundaryScale,
            //     xRot));
            //GlobalVariables.gameObjects.Add(new Cube(
            //      Game.Content.Load<Model>(@"Models/Objects/Cube"),
            //      new Vector3(400, 200, 600),
            //     boundaryScale,
            //     xRot));
            //GlobalVariables.gameObjects.Add(new Cube(
            //      Game.Content.Load<Model>(@"Models/Objects/Cube"),
            //      new Vector3(300, 150, -500),
            //     defaultScale,
            //     xRot));
            //GlobalVariables.gameObjects.Add(new Cube(
            //       Game.Content.Load<Model>(@"Models/Objects/Cube"),
            //       new Vector3(400, 0, -300),
            //     defaultScale,
            //     xRot));
        }
        protected override void LoadContent()
        {

            SpriteBatch spriteBatch = new SpriteBatch(game.GraphicsDevice);
            crosshairTexture = Game.Content.Load<Texture2D>(@"textures\crosshair");
            Arial = Game.Content.Load<SpriteFont>(@"SpriteFonts\Courier New");
            ground = new Ground(Game.Content.Load<Model>(@"Models/Ground/Ground"));
            player = new Player(
                  Game.Content.Load<Model>(@"Models/Objects/Cube"),
                  game.GraphicsDevice,
                  camera,
                  game,
                  this);
            Gun gun = new Gun(Game.Content.Load<Model>(@"Models/Objects/scar-h"),
                player, player.position, player.direction);
            
            //enemyManager = new EnemyManager(
            //    Game.Content.Load<Model>(@"Models/Objects/Cube"),
            //    Game,
            //    player);
            //trexList.Add(new Tank(Game.Content.Load<Model>(@"Models/Npcs/Tank/tank"),
            //    player,
            //    new Vector3(900, 100, 900),
            //    this));
            //trexList.Add(new Tank(Game.Content.Load<Model>(@"Models/Npcs/Tank/tank"),
            //     player,
            //     new Vector3(1500, 100, 1700),
            //    this));
            //trexList.Add(new Tank(Game.Content.Load<Model>(@"Models/Npcs/Tank/tank"),
            //    player,
            //    new Vector3(-1800, 100, 900),
            //    this));
            //trexList.Add(new Tank(Game.Content.Load<Model>(@"Models/Npcs/Tank/tank"),
            //    player,
            //    new Vector3(-900, 100, 1700),
            //    this));
            //AddPlatforms();
            //AddWalls();
            GlobalVariables.gameObjects.Add(player);
            GlobalVariables.gameObjects.Add(gun);

            //foreach(Tank trex in trexList){
            //    GlobalVariables.gameObjects.Add(trex);
            //}
            
            base.LoadContent();
        }
        private void FireCube()
        {
            if (playerShotTimer >= playerShotCooldown)
            {
                Projectile playerProj = new Projectile(Game.Content.Load<Model>(@"Models/Objects/Projectile"),
                    player.position + new Vector3(0, 50, 0) + player.direction,
                    player.direction,
                    this);
                playerProj.type = GameObject.ObjectType.PlayerProjectile;
                GlobalVariables.gameObjects.Add(playerProj);
                playerShotTimer = 0f;
            }
        }
        
        private void FireCube(Vector3 position, Vector3 direction)
        {
            if (enemyShotTimer >= enemyShotCooldown)
            {
                Projectile enemyProj = new Projectile(Game.Content.Load<Model>(@"Models/Objects/ammo"),
                    position + new Vector3(0, 50, 0) + direction,
                    direction,
                    this);
                enemyProj.type = GameObject.ObjectType.EnemyProjectile;
                GlobalVariables.gameObjects.Add(enemyProj);
                enemyShotTimer = 0f;
            }
        }
        private void delayBuildGrid()
        {
            if (delayTimer == 1)
            {
                Grid = new Grid(game.GraphicsDevice, camera);
            }
            if (delayTimer < 4)
            {
                delayTimer++;
            }
            if (Grid != null && GlobalVariables.Debug && Octree != null)
            {
                Grid.DrawNodes();
                Octree.Debug();
            }
            if (delayTimer == 2)
            {
                Pathfinder = new Pathfinder(Grid.layout2d);
                //foreach (Tank tRex in trexList)
                //{
                //    tRex.pathfinder = Pathfinder;
                //    tRex.grid = Grid;
                //}
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            enemyCount = 0;
            elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            delayBuildGrid();
            //if (Keyboard.GetState().IsKeyDown(Keys.M) && mapChangeCount > 1000)
            //{
            //    mapChangeCount = 0;
            //    foreach (GameObject mapObj in GlobalVariables.gameObjects)
            //    {
            //        if (mapObj.type == GameObject.ObjectType.Map)
            //        {
            //            Console.Out.WriteLine("delete")
            //            GlobalVariables.gameObjects.Remove(mapObj);
            //        }
            //    }
            //    if (isTextMap)
            //    {
            //        AddPlatforms();
            //        AddWalls();
            //    }
            //    else
            //        loadMap();
            //}
            //    mapChangeCount++;
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                FireCube();
            }
            for (int i = 0; i <= GlobalVariables.gameObjects.Count()-1; i++)
            {
                GameObject obj = GlobalVariables.gameObjects[i];
                if (!obj.IsActive)
                {
                    Octree.Remove(obj);
                    GlobalVariables.gameObjects.Remove(obj);
                }
                else obj.Update(gameTime);
                if (obj.type == GameObject.ObjectType.Enemy)
                {
                    //if (((Tank)obj).State == Tank.TankState.Pursue)
                    //{
                    //    Vector3 playerHeightAim = (player.position - ((Tank)obj).position);
                    //    playerHeightAim.Normalize();
                    //    playerHeightAim.Y -= 0.1f;
                    //    FireCube(obj.position + new Vector3(0, 34, 0), ((Tank)obj).direction * new Vector3(1, 0, 1) + playerHeightAim);
                    //}
                    enemyCount++;
                }
                if (firstRun) startingEnemyCount = enemyCount;
                GlobalVariables.score = startingEnemyCount - enemyCount;
            }
            if (playerShotTimer < playerShotCooldown)
                playerShotTimer += elapsed;
            if (enemyShotTimer < enemyShotCooldown)
                enemyShotTimer += elapsed;
            //if (enemyCount == 0) winGap++;
            if (winGap > 50)
            {
                currentGameState = GameState.GameOver;
                GlobalVariables.youWin = true;
            }
            firstRun = false;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (GameObject obj in GlobalVariables.gameObjects)
            {
                obj.Draw(game.GraphicsDevice, game.camera);
            }
            ground.Draw(game.GraphicsDevice, game.camera);
            
            SpriteBatch spriteBatch = new SpriteBatch(game.GraphicsDevice);
            spriteBatch.Begin();
            spriteBatch.Draw(crosshairTexture,
                new Vector2((game.Window.ClientBounds.Width / 2)
                    - (crosshairTexture.Width / 2),
                    (game.Window.ClientBounds.Height / 2)
                    - (crosshairTexture.Height / 2)),
                    Color.White);
            if (GlobalVariables.Debug)
            {
                spriteBatch.DrawString(Arial, "Velocity: " +
                    player.Velocity.ToString(),
                    new Vector2(game.Window.ClientBounds.Width - 600, 0),
                    Color.Black,
                    0, Vector2.Zero, new Vector2(0.6f, 0.6f), SpriteEffects.None, 0);
                spriteBatch.DrawString(Arial, "Acceleration: " +
                 player.Acceleration.ToString(),
                 new Vector2(game.Window.ClientBounds.Width - 600, 20),
                 Color.Black,
                 0, Vector2.Zero, new Vector2(0.6f, 0.6f), SpriteEffects.None, 0);
                spriteBatch.DrawString(Arial, "Pos: " +
                     player.position.ToString(),
                     new Vector2(game.Window.ClientBounds.Width - 600, 40),
                     Color.Black,
                     0, Vector2.Zero, new Vector2(0.6f, 0.6f), SpriteEffects.None, 0);
                spriteBatch.DrawString(Arial, "Box: " +
                     "Max: " + player.aabb.Max + "Min: " + player.aabb.Min,
                     new Vector2(game.Window.ClientBounds.Width - 800, 60),
                     Color.Black,
                     0, Vector2.Zero, new Vector2(0.6f, 0.6f), SpriteEffects.None, 0);
            }
            spriteBatch.DrawString(Arial, "Score: " +
                GlobalVariables.score + "/" + startingEnemyCount, 
                new Vector2(0, 0), Color.Blue);
            spriteBatch.DrawString(Arial, "Health: " +
                player.health + "/" + player.maxHealth,
                new Vector2(0, game.Window.ClientBounds.Height - 40),
                Color.Red);
            
            spriteBatch.End();
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            if (currentGameState == GameState.GameOver && !GlobalVariables.youWin)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
                spriteBatch.Begin();
                spriteBatch.DrawString(Arial, "Your Score Was: " +
                     GlobalVariables.score + "/" + startingEnemyCount, new Vector2(0, 0), Color.Red);
                spriteBatch.DrawString(Arial, "Game Over!", 
                    new Vector2(-100 + GraphicsDevice.Viewport.Width / 2, 
                        GraphicsDevice.Viewport.Height / 2), Color.Red);
                spriteBatch.End();
            }
            else if (currentGameState == GameState.GameOver)
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);
                spriteBatch.Begin();
                spriteBatch.DrawString(Arial, "Your Score Was: " +
                     GlobalVariables.score + "/" + startingEnemyCount, new Vector2(0, 0), Color.Red);
                spriteBatch.DrawString(Arial, "You Win!", 
                    new Vector2(-100 + GraphicsDevice.Viewport.Width / 2, 
                        GraphicsDevice.Viewport.Height / 2), Color.Red);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
