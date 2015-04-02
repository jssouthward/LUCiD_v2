#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Tao.Sdl;
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
        Powerup powerTemp;
        Controls controls;
        string[,] level;
        Dictionary<string, string> key = new Dictionary<string, string>();
        Texture2D largeDark, mediumDark, smallDark;
        Texture2D background;
        Lucidity playerShot;
        int darkX;
        int darkY;


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

            
           
            playerShot = new Lucidity(100, 100, 32, 32, 1);
            playerShot.monsters = monsterList;
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
                    if(level[i,j].Equals("X"))
                    {
                        //Console.Write(level[i, j] + ",");
                        Block temp = new Block(32 * j, 32 * i, 32, 32, "gray.png");
                        blocks.Add(temp);
                    }
                    if (level[i, j].Equals("W"))
                    {
                        stageEnd = new Warp(32 * j, 32 * i, 64, 64);
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
            
            playerShot.LoadContent(this.Content);
            foreach (Block block in blocks)
            {
                block.LoadContent(this.Content);
            }
            stageEnd.LoadContent(this.Content);
            largeDark = Content.Load<Texture2D>("300x300.png");
            mediumDark = Content.Load<Texture2D>("200x200.png");
            smallDark = Content.Load<Texture2D>("100x100.png");
            background = Content.Load<Texture2D>("darkwoods.png");

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
            //set our keyboardstate tracker update can change the gamestate on every cycle
            controls.Update();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //Up, down, left, right affect the coordinates of the sprite

            player1.testblocks = blocks;
            player1.powerTest = powerList;
            player1.Update(controls, gameTime);

           
            player1.shot = playerShot;
            player1.shot.Update(controls, gameTime);

            darkX = player1.getX() - 2484; //2500-16 offset for player
            darkY = player1.getY() - 2468; // 2500-32


            foreach (Monster monster in monsterList)
            {
                monster.testblocks = blocks;
                monster.Update(controls, gameTime);
            }
            monsterList.RemoveAll(monster => monster.dead == true);

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
            spriteBatch.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);
            player1.Draw(spriteBatch);

            foreach (Monster monster in monsterList)
            {
                if (monster.dead == false)
                {
                    monster.Draw(spriteBatch);
                }
            }   
   
           
            if (player1.shot.spent == false)
            {
                player1.shot.Draw(spriteBatch);
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

            stageEnd.Draw(spriteBatch);

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

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

}

