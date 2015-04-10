#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.Threading;
using Tao.Sdl;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
#endregion

namespace LUCiD
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class LUCiD : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player player1;
        Monster monsterTemp; // generic monster used for filling the list
        Warp stageEnd;
        List<Block> blocks;
        List<Monster> monsterList = new List<Monster>();
        List<Monster> temp = new List<Monster>(); //temporary monster list
        List<Powerup> powerList = new List<Powerup>();
        List<Lucidity> shotList = new List<Lucidity>();
        List<Warp> warpList = new List<Warp>();
        Powerup powerTemp;
        Warp warpTemp;
        Controls controls;
        string[,] level;
        Dictionary<string, string> key = new Dictionary<string, string>();
        Texture2D largeDark, mediumDark, smallDark;
        Texture2D background;
        int darkX;
        int darkY;

        //health bar
        Texture2D healthTexture;
        Rectangle healthRectangle;

        //lucidity bar
        Texture2D lucidityTexture;
        Rectangle lucidityRectangle;

        //*** Menu shit start ***//
        private Texture2D title, startButton, exitButton, resumeButton, loadingScreen, deathScreen;
        private Vector2 startButtonPosition, exitButtonPosition, resumeButtonPosition;
        private Thread backgroundThread;
        private bool isLoading = false;
        MouseState mouseState;
        MouseState previousMouseState;
        GameState gameState;

        enum GameState
        {
            StartMenu,
            Loading,
            Playing,
            Paused,
            Win,
            Dead
        }
        //*** Menu shit end ***//

        public LUCiD()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 720;   // set this value to the desired height of your window
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //*** Menu shit start ***//
            //enable the mousepointer
            IsMouseVisible = true;

            //set the position of the buttons
            startButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 50, 400);
            exitButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 50, 450);

            //set the gamestate to start menu
            gameState = GameState.StartMenu;

            //get the mouse state
            mouseState = Mouse.GetState();
            previousMouseState = mouseState;
            //*** Menu shit end ***//

            base.Initialize();

            Joystick.Init();
            Console.WriteLine("Number of joysticks: " + Sdl.SDL_NumJoysticks());
            controls = new Controls();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Read in the level
            string[] lines = System.IO.File.ReadAllLines("Levels/level001.txt");
            string[] keylines = System.IO.File.ReadAllLines("Levels/key001.txt");
            int numRows = lines.Length;
            int numCols = lines[0].Length;
            int realnumCols = lines[0].Split(new Char[] { ' ' }).Length; //removes the whitespace
            //Console.Write("rows: " + numRows + " cols: " + realnumCols);
            blocks = new List<Block>();
            level = new string[numRows, realnumCols]; // number of rows by number of columns
            int levelindex = 0;

            foreach (string line in lines)
            {
                string[] split = line.Split(new Char[] { ' ' });

                for (int i = 0; i < split.Length; i++)
                {
                    if (!split[i].Equals(" "))
                    {
                        level[levelindex, i] = split[i].Trim();
                    }
                }
                levelindex++;
            }

            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < realnumCols; j++)
                {
                    if (level[i,j].Equals("X"))
                    {
                        //Console.Write(level[i, j] + ",");
                        Block temp = new Block(32 * j, 32 * i, 32, 32, "gray.png");
                        blocks.Add(temp);
                    }
                    if (level[i, j].Equals("W"))
                    {
                        warpTemp = new Warp(32 * j, 32 * i, 64, 64);
                        warpTemp.LoadContent(this.Content);
                        warpList.Add(warpTemp);
                    }
                    if (level[i, j].Equals("P"))
                    {
                        player1 = new Player(32*j, 32*i, 32, 64);
                    }
                    if (level[i, j].Equals("M"))
                    {
                        monsterTemp = new Monster(32*j, 32*i, 64, 64);
                        monsterTemp.LoadContent(this.Content);
                        monsterList.Add(monsterTemp);
                    }
                    if (level[i, j].Equals("U"))
                    {
                        powerTemp = new Powerup(32 * j, 32 * i, 32, 32);
                        powerTemp.LoadContent(this.Content);
                        powerList.Add(powerTemp);
                    }
                }
                //Console.Write("\n");
            }

            foreach (string keyline in keylines)
            {
                string[] split = keyline.Split(new Char[] { ',' });
                key.Add(split[0], split[1]);

                Console.WriteLine("\t" + split[0] + ": " + key[split[0]]);
            }

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            player1.LoadContent(this.Content);
            

            foreach (Block block in blocks)
            {
                block.LoadContent(this.Content);
            }

            foreach (Lucidity lucid in player1.shotTest)
            {
                lucid.LoadContent(this.Content);
            }

            largeDark = Content.Load<Texture2D>("300x300.png");
            mediumDark = Content.Load<Texture2D>("200x200.png");
            smallDark = Content.Load<Texture2D>("100x100.png");
            background = Content.Load<Texture2D>("darkwoods.png");
            healthTexture = Content.Load<Texture2D>("health.png");
            lucidityTexture = Content.Load<Texture2D>("lucidity.png");

            //*** Menu shit start ***//
            //load the buttonimages into the content pipeline
            title = Content.Load<Texture2D>(@"title");
            startButton = Content.Load<Texture2D>(@"start");
            exitButton = Content.Load<Texture2D>(@"exit");

            //load the loading screen
            loadingScreen = Content.Load<Texture2D>(@"loading");
            //*** Menu shit end ***//

            // This block should play the song on repeat, but it seems to not work
            // with the file format. I think it has to do with the content pipeline, but
            // I can't get that to work on my computer. See the content pipeline tutorial on
            // piazza for more information.
            /*
            Song song = Content.Load<Song>("DST-Arch-Delerium_wav.wav");
            MediaPlayer.Play(song);
            MediaPlayer.IsRepeating = true;
             */

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            //*** Menu shit start ***//
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }
                
            //load the game when needed
            //isLoading bool is to prevent the LoadGame method from being called 60 times a seconds
            if (gameState == GameState.Loading && !isLoading)
            {
                //set backgroundthread
                backgroundThread = new Thread(LoadGame);
                isLoading = true;

                //start backgroundthread
                backgroundThread.Start();
            }
            //wait for mouseclick
            mouseState = Mouse.GetState();
            if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                MouseClicked(mouseState.X, mouseState.Y);
            }

            previousMouseState = mouseState;

            if (gameState == GameState.Playing && isLoading)
            {
                LoadGame();
                isLoading = false;
            }
            //*** Menu shit end ***//

            //set our keyboardstate tracker update can change the gamestate on every cycle
            controls.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            //check the pausebutton
            if (gameState == GameState.Playing)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.P))
                {
                    gameState = GameState.Paused;
                }
            }

            // TODO: Add your update logic here
            //Up, down, left, right affect the coordinates of the sprite
            player1.testblocks = blocks;
            player1.powerTest = powerList;
            player1.monsterTest = monsterList;
            player1.warpTest = warpList;
            player1.Update(controls, gameTime);
           

            darkX = player1.getX() - 2484; //2500-16 offset for player
            darkY = player1.getY() - 2468; // 2500-32

            healthRectangle = new Rectangle(20, 20, player1.health, 20);
            lucidityRectangle = new Rectangle(140, 20, player1.lucidity, 20);
            //insert here the player losing health update
            //if monster.intersect(player1)   player.health -= 10

            if (player1.health <= 0)
            {
                gameState = GameState.Dead;
            }

            if (gameState == GameState.Playing) //Pause monsters if not playing
            {

                if (controls.onPress(Keys.X, Buttons.LeftShoulder) && player1.lucidity > 5)
                {
                    Lucidity shotTemp = new Lucidity(player1.getX(), player1.getY(), 32, 32, player1.currDirection);
                    shotTemp.LoadContent(this.Content);
                    this.shotList.Add(shotTemp);
                    player1.lucidity -= 5;
                }


                foreach (Lucidity lucid in shotList)
                {
                    lucid.monsters = this.monsterList;
                    lucid.Update(controls, gameTime);
                }
                shotList.RemoveAll(lucid => lucid.spent == true);

                foreach (Monster monster in monsterList)
                {
                    monster.testblocks = blocks;
                    monster.Update(controls, gameTime);
                }
                monsterList.RemoveAll(monster => monster.dead == true);
            }

            foreach (Powerup power in powerList)
            {
                power.Update(controls, gameTime);
            }
            powerList.RemoveAll(powerups => powerups.collected == true);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            //*** Menu shit start ***/
            //draw the start menu
        if (gameState == GameState.StartMenu)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.Draw(title, new Rectangle(490, 180, 280, 120), Color.White);
            spriteBatch.Draw(startButton, startButtonPosition, Color.White);
            spriteBatch.Draw(exitButton, exitButtonPosition, Color.White);
        }

        if (gameState == GameState.Dead)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.Draw(deathScreen, new Rectangle(440, 180, 380, 100), Color.White);
            spriteBatch.Draw(startButton, startButtonPosition, Color.White);
            spriteBatch.Draw(exitButton, exitButtonPosition, Color.White);
        }

        //show the loading screen when needed
        if (gameState == GameState.Loading)
        {
           spriteBatch.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);
           spriteBatch.Draw(loadingScreen, new Vector2((GraphicsDevice.Viewport.Width / 2) - (loadingScreen.Width / 2), (GraphicsDevice.Viewport.Height / 2) - (loadingScreen.Height / 2)), Color.White);
        }

        if (gameState == GameState.Win)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.Draw(title, new Rectangle(490, 180, 280, 120), Color.White);
        }
        
        //draw the the game when playing
        if (gameState == GameState.Playing) //Draw game if playing
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);
            player1.Draw(spriteBatch);

            if (player1.endOfLevel == true)
            {
                gameState = GameState.Win;
            }

            foreach (Monster monster in monsterList)
            {
                if (monster.dead == false)
                {
                    monster.Draw(spriteBatch);
                }
            }

            foreach (Lucidity shot in shotList)
            {
                if (shot.spent == false)
                {
                    shot.Draw(spriteBatch);
                }
            }
            

            foreach (Block block in blocks)
            {
                block.Draw(spriteBatch);
            }

            foreach (Powerup power in powerList)
            {
                if (power.collected == false)
                {
                    power.Draw(spriteBatch);
                }
            }

            foreach (Warp warp in warpList)
            {
                warp.Draw(spriteBatch);
            }

            if (player1.lucidity < 33)
            {
                spriteBatch.Draw(smallDark, new Rectangle(darkX, darkY, 5000, 5000), Color.White);
            }
            if (player1.lucidity < 67 && player1.lucidity >= 33)
            {
                spriteBatch.Draw(mediumDark, new Rectangle(darkX, darkY, 5000, 5000), Color.White);
            }
            if (player1.lucidity >= 67)
            {
                spriteBatch.Draw(largeDark, new Rectangle(darkX, darkY, 5000, 5000), Color.White);
            }

            spriteBatch.Draw(healthTexture, healthRectangle, Color.White);
            spriteBatch.Draw(lucidityTexture, lucidityRectangle, Color.White);
        }

        //draw the pause screen
        if (gameState == GameState.Paused)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.Draw(resumeButton, resumeButtonPosition, Color.White);
        }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void LoadGame()
        {
            //load the game images into the content pipeline
            resumeButton = Content.Load<Texture2D>(@"resume");
            resumeButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - (resumeButton.Width / 2), (GraphicsDevice.Viewport.Height / 2) - (resumeButton.Height / 2));

            //since this will go to fast for this demo's purpose, wait for 3 seconds
            //Thread.Sleep(3000);

            //start playing
            gameState = GameState.Playing;
            isLoading = false;
        }

        void MouseClicked(int x, int y)
        {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);

            //check the startmenu
            if (gameState == GameState.StartMenu)
            {
                Rectangle startButtonRect = new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, 100, 20);
                Rectangle exitButtonRect = new Rectangle((int)exitButtonPosition.X, (int)exitButtonPosition.Y, 100, 20);

                if (mouseClickRect.Intersects(startButtonRect)) //player clicked start button
                {
                    gameState = GameState.Loading;
                    isLoading = false;
                }
                else if (mouseClickRect.Intersects(exitButtonRect)) //player clicked exit button
                {
                    Exit();
                }
            }

            if (gameState == GameState.Dead)
            {
                Rectangle restartButtonRect = new Rectangle((int)startButtonPosition.X, (int)startButtonPosition.Y, 100, 20);
                Rectangle exitButtonRect = new Rectangle((int)exitButtonPosition.X, (int)exitButtonPosition.Y, 100, 20);

                if (mouseClickRect.Intersects(restartButtonRect)) //Not working part. Rectangle seems to not be drawn
                {
                    gameState = GameState.Loading;
                    isLoading = false;
                }
                else if (mouseClickRect.Intersects(exitButtonRect)) //player clicked exit button
                {
                    Exit();
                }
            }

            //check the resumebutton
            if (gameState == GameState.Paused)
            {
                Rectangle resumeButtonRect = new Rectangle((int)resumeButtonPosition.X, (int)resumeButtonPosition.Y, 100, 20);

                if (mouseClickRect.Intersects(resumeButtonRect))
                {
                    gameState = GameState.Playing;
                }
            }
        }
    }
}

