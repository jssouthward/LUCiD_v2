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
        Monster monster1;
        Warp stageEnd;
        List<Block> blocks;
        List<Monster> monsterList = new List<Monster>();
        Controls controls;
        string[,] level;
        Dictionary<string, string> key = new Dictionary<string, string>();
        Texture2D largeDark, mediumDark, smallDark;
        Texture2D background;
        Lucidity playerShot;
        int darkX;
        int darkY;

        Texture2D texture;

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

            player1 = new Player(100, 400, 32, 64);
            monster1 = new Monster(800, 100, 64, 64);
            monsterList.Add(monster1);
            playerShot = new Lucidity(100, 100, 32, 32, 1);
            playerShot.monsters = monsterList;
            base.Initialize();

            Joystick.Init();
            Console.WriteLine("Number of joysticks: " + Sdl.SDL_NumJoysticks());
            controls = new Controls();

            //health
            texture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.Red });

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
            monster1.LoadContent(this.Content);
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
            player1.Update(controls, gameTime);

            monster1.testblocks = blocks;
            monster1.Update(controls, gameTime);

            player1.shot = playerShot;
            player1.shot.Update(controls, gameTime);

            darkX = player1.getX() - 2484; //2500-16 offset for player
            darkY = player1.getY() - 2468; // 2500-32

           


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
            spriteBatch.Draw(texture, new Rectangle(1, 1, 20, 5), Color.Red);
            player1.Draw(spriteBatch);
            if (monster1.dead == false)
            {
                monster1.Draw(spriteBatch);
            }
            if (player1.shot.spent == false)
            {
                player1.shot.Draw(spriteBatch);
            }
            
            foreach (Block block in blocks)
            {
                block.Draw(spriteBatch);
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

