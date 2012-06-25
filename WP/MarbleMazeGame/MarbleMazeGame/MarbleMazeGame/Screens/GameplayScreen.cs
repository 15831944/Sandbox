using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameStateManagement;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Devices;

namespace MarbleMazeGame
{
    class GameplayScreen : GameScreen
    {
        Maze maze;
        Marble marble;
        Camera camera;
        readonly float angularVelocity = MathHelper.ToRadians(1.5f);
        Vector3? accelerometerState = Vector3.Zero;
        bool gameOver = false;
        LinkedListNode<Vector3> lastCheackpointNode;
        SpriteFont timeFont;
        TimeSpan gameTime;
        bool startScreen = true;
        TimeSpan startScreenTime = TimeSpan.FromSeconds(4);

        public bool IsActive { get; set; }

        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);

            EnabledGestures = GestureType.Tap;
        }

        public override void LoadContent()
        {
            //LoadAssets();
            timeFont = ScreenManager.Game.Content.Load<SpriteFont>(@"Fonts\MenuFont");
            Accelerometer.Initialize();
            base.LoadContent();
        }

        public void LoadAssets()
        {
            InitializeCamera();
            InitializeMaze();
            InitializeMarble();
        }

        private void InitializeCamera()
        {
            // Create the camera
            camera = new Camera(ScreenManager.Game, ScreenManager.GraphicsDevice);
            camera.Initialize();
        }

        private void InitializeMaze()
        {
            maze = new Maze(ScreenManager.Game)
            {
                Position = Vector3.Zero,
                Camera = camera
            };

            maze.Initialize();

            // Save the last checkpoint
            lastCheackpointNode = maze.Checkpoints.First;
        }

        private void InitializeMarble()
        {
            marble = new Marble(ScreenManager.Game as MarbleMazeGame)
            {
                Position = maze.StartPoistion,
                Camera = camera,
                Maze = maze
            };
            marble.Initialize();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (IsActive && !gameOver)
            {
                if (!startScreen)
                {
                    // Calculate the time from the start of the game
                    this.gameTime += gameTime.ElapsedGameTime;

                    CheckFallInPit();
                    UpdateLastCheackpoint();
                }

                // Update all the component of the game
                maze.Update(gameTime);
                marble.Update(gameTime);
                camera.Update(gameTime);

                CheckGameFinish();

                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            }
            if (startScreen)
            {
                if (startScreenTime.Ticks > 0)
                {
                    startScreenTime -= gameTime.ElapsedGameTime;
                }
                else
                {
                    startScreen = false;
                }
            }
        }

        private void UpdateLastCheackpoint()
        {
            BoundingSphere marblePosition = marble.BoundingSphereTransformed;

            var tmp = lastCheackpointNode;
            while (tmp.Next != null)
            {
                // If the marble is close to a checkpoint save the checkpoint
                if (Math.Abs(Vector3.Distance(marblePosition.Center, tmp.Next.Value))
                    <= marblePosition.Radius * 3)
                {
                    AudioManager.PlaySound("checkpoint");
                    lastCheackpointNode = tmp.Next;
                    return;
                }
                tmp = tmp.Next;
            }
        }

        private void CheckFallInPit()
        {
            if (marble.Position.Y < -150)
            {
                marble.Position = lastCheackpointNode.Value;
                maze.Rotation = Vector3.Zero;
                marble.Acceleration = Vector3.Zero;
                marble.Velocity = Vector3.Zero;
            }
        }

        private void CheckGameFinish()
        {
            BoundingSphere marblePosition = marble.BoundingSphereTransformed;

            if (Math.Abs(Vector3.Distance(marblePosition.Center, maze.End)) <=
                marblePosition.Radius * 3)
            {
                gameOver = true;
                return;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.Black);
            ScreenManager.SpriteBatch.Begin();
            if (startScreen)
            {
                DrawStartGame(gameTime);
            }
            if (IsActive)
            {
                // Draw the elapsed time
                ScreenManager.SpriteBatch.DrawString(timeFont,
                    String.Format("{0:00}:{1:00}", this.gameTime.Minutes,
                    this.gameTime.Seconds), new Vector2(20, 20),
                    Color.YellowGreen);

                // Drawing sprites changes some render states around, which don't
                // play nicely with 3d models. 
                // In particular, we need to enable the depth buffer.
                DepthStencilState depthStensilState =
                    new DepthStencilState() { DepthBufferEnable = true };
                ScreenManager.GraphicsDevice.DepthStencilState = depthStensilState;

                // Draw all the game components
                maze.Draw(gameTime);
                marble.Draw(gameTime);
            }
            if (gameOver)
            {
                AudioManager.StopSounds();
                DrawEndGame(gameTime);
            }

            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);

        }

        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.IsPauseGame(null))
            {
                if (!gameOver)
                    PauseCurrentGame();
                else
                    FinishCurrentGame();
            }

            if (IsActive && !startScreen)
            {
                if (input.Gestures.Count > 0)
                {
                    GestureSample sample = input.Gestures[0];
                    if (sample.GestureType == GestureType.Tap)
                    {
                        if (gameOver)
                            FinishCurrentGame();
                    }
                }


                if (!gameOver)
                {
                    // Rotate the maze according to accelerometer data
                    Vector3 currentAccelerometerState =
                            Accelerometer.GetState().Acceleration;


                    if (Microsoft.Devices.Environment.DeviceType == DeviceType.Device)
                    {
                        //Change the velocity according to acceleration reading
                        maze.Rotation.Z = (float)Math.Round(MathHelper.ToRadians(
                            currentAccelerometerState.Y * 30), 2);
                        maze.Rotation.X = -(float)Math.Round(MathHelper.ToRadians(
                            currentAccelerometerState.X * 30), 2);
                    }
                    else if (Microsoft.Devices.Environment.DeviceType ==
                        DeviceType.Emulator)
                    {
                        Vector3 Rotation = Vector3.Zero;

                        if (currentAccelerometerState.X != 0)
                        {
                            if (currentAccelerometerState.X > 0)
                                Rotation += new Vector3(0, 0, -angularVelocity);
                            else
                                Rotation += new Vector3(0, 0, angularVelocity);
                        }

                        if (currentAccelerometerState.Y != 0)
                        {
                            if (currentAccelerometerState.Y > 0)
                                Rotation += new Vector3(-angularVelocity, 0, 0);
                            else
                                Rotation += new Vector3(angularVelocity, 0, 0);
                        }

                        // Limit the rotation of the maze to 30 degrees
                        maze.Rotation.X =
                            MathHelper.Clamp(maze.Rotation.X + Rotation.X,
                            MathHelper.ToRadians(-30), MathHelper.ToRadians(30));

                        maze.Rotation.Z =
                            MathHelper.Clamp(maze.Rotation.Z + Rotation.Z,
                            MathHelper.ToRadians(-30), MathHelper.ToRadians(30));

                    }
                }
            }
        }


        internal void Restart()
        {
            marble.Position = maze.StartPoistion;
            marble.Velocity = Vector3.Zero;
            marble.Acceleration = Vector3.Zero;
            maze.Rotation = Vector3.Zero;
            IsActive = true;
            gameOver = false;
            gameTime = TimeSpan.Zero;
            startScreen = true;
            startScreenTime = TimeSpan.FromSeconds(4);
            lastCheackpointNode = maze.Checkpoints.First;
        }

        private void PauseCurrentGame()
        {
            IsActive = false;
            // Pause the sounds
            AudioManager.PauseResumeSounds(false);

            ScreenManager.AddScreen(new BackgroundScreen(), null);
            ScreenManager.AddScreen(new PauseScreen(), null);
        }

        private void FinishCurrentGame()
        {
            IsActive = false;

            foreach (GameScreen screen in ScreenManager.GetScreens())
                screen.ExitScreen();

            if (HighScoreScreen.IsInHighscores(gameTime))
            {
                // Show the device's keyboard
                Guide.BeginShowKeyboardInput(PlayerIndex.One,
                "Player Name", "Enter your name (max 15 characters)", "Player",
                (r) =>
                {
                    string playerName = Guide.EndShowKeyboardInput(r);

                    if (playerName != null && playerName.Length > 15)
                        playerName = playerName.Substring(0, 15);

                    HighScoreScreen.PutHighScore(playerName, gameTime);

                    ScreenManager.AddScreen(new BackgroundScreen(), null);
                    ScreenManager.AddScreen(new HighScoreScreen(), null);

                }, null);
                return;
            }

            ScreenManager.AddScreen(new BackgroundScreen(), null);
            ScreenManager.AddScreen(new HighScoreScreen(), null);
        }

        private void DrawEndGame(GameTime gameTime)
        {
            string text = HighScoreScreen.IsInHighscores(this.gameTime) ?
                "    You got a High Score!" : "          Game Over";
            text += "\nTouch the screen to continue";
            Vector2 size = timeFont.MeasureString(text);
            Vector2 textPosition = (new
                Vector2(ScreenManager.GraphicsDevice.Viewport.Width,
                ScreenManager.GraphicsDevice.Viewport.Height) - size) / 2f;

            ScreenManager.SpriteBatch.DrawString(timeFont, text,
                textPosition, Color.White);
        }

        private void DrawStartGame(GameTime gameTime)
        {
            string text = (startScreenTime.Seconds == 0) ? "Go!" :
                startScreenTime.Seconds.ToString();
            Vector2 size = timeFont.MeasureString(text);
            Vector2 textPosition = (new
                Vector2(ScreenManager.GraphicsDevice.Viewport.Width,
                ScreenManager.GraphicsDevice.Viewport.Height) - size) / 2f;

            ScreenManager.SpriteBatch.DrawString(timeFont, text, textPosition,
            Color.White);
        }


    }

}
