using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using System.Text;

namespace LUCiD
{
    public class GameScreen
    {
        GameLoop game;
        SpriteBatch spriteBatch;
        Player player1;
        Controls controls;
        Moving movingTemp; // generic moving monster used for filling the list
        Jumping jumpingTemp;
        Stationary stationaryTemp;
        SpikeR spikeRTemp;
        SpikeL spikeLTemp;
        SpikeB spikeBTemp;
        SpikeT spikeTTemp;
        List<Block> blocks;
        List<Moving> movingList = new List<Moving>();
        List<Jumping> jumpingList = new List<Jumping>();
        List<Stationary> stationaryList = new List<Stationary>();
        List<SpikeR> spikeRList = new List<SpikeR>();
        List<SpikeL> spikeLList = new List<SpikeL>();
        List<SpikeB> spikeBList = new List<SpikeB>();
        List<SpikeT> spikeTList = new List<SpikeT>();
        List<Powerup> powerList = new List<Powerup>();
        List<Lucidity> shotList = new List<Lucidity>();
        List<Warp> warpList = new List<Warp>();
        Powerup powerTemp;
        Warp warpTemp;
        string[,] level;
        Dictionary<string, string> key = new Dictionary<string, string>();
        int darkX;
        int darkY;
        Texture2D background, darkness, lightmask, warpmask, tut1, tut2, tut3, tut4, tut5, tut6;
        HUD hud;
        //lighting
        RenderTarget2D mainScene;
        RenderTarget2D lightMask;
        Timer timer;
        SpriteFont font;

        public GameScreen(GameLoop game, SpriteBatch spriteBatch, string name)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            loadLevel(name);
            controls = new Controls();
            timer = new Timer();
            player1.LoadContent(game.Content);
            darkness = game.Content.Load<Texture2D>("darkness");
            lightmask = game.Content.Load<Texture2D>("lightmask");
            warpmask = game.Content.Load<Texture2D>("warpmask");
            background = game.Content.Load<Texture2D>("darkwoods");
            tut1 = game.Content.Load<Texture2D>("tut1");
            tut2 = game.Content.Load<Texture2D>("tut2");
            tut3 = game.Content.Load<Texture2D>("tut3");
            tut4 = game.Content.Load<Texture2D>("tut4");
            tut5 = game.Content.Load<Texture2D>("tut5");
            tut6 = game.Content.Load<Texture2D>("tut6");
            font = game.Content.Load<SpriteFont>("Font");
            hud = new HUD(game, player1);
            var pp = game.GraphicsDevice.PresentationParameters;
            mainScene = new RenderTarget2D(game.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            lightMask = new RenderTarget2D(game.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        public void Update(GameTime gameTime)
        {
            if (timer.isRunning == false)
            {
                timer.start(60);
            }
            else
            {
                timer.checkTime(game, gameTime);
            }
            controls.Update();
            player1.testblocks = blocks;
            player1.powerTest = powerList;
            player1.movingTest = movingList;
            player1.jumpingTest = jumpingList;
            player1.stationaryTest = stationaryList;
            player1.spikeRTest = spikeRList;
            player1.spikeLTest = spikeLList;
            player1.spikeBTest = spikeBList;
            player1.spikeTTest = spikeTList;
            player1.warpTest = warpList;
            player1.Update(controls, gameTime);

            //darkX = player1.getX() - 134; //150-16 offset for player
            //darkY = player1.getY() - 118; //150-32
            darkX = player1.getX() - 120;
            darkY = player1.getY() - 120;

            if (player1.health <= 0)
            {
                game.GameOver();
            }

            if (player1.getY() > 780)
            {
                game.GameOver();
            }

            if (controls.onPress(Keys.X, Buttons.LeftShoulder) && player1.lucidity > 5)
            {
                Lucidity shotTemp = new Lucidity(player1.getX(), player1.getY(), 32, 32, player1.currDirection);
                shotTemp.LoadContent(game.Content);
                this.shotList.Add(shotTemp);
                player1.lucidity -= 5;
            }

            foreach (Lucidity lucid in shotList)
            {
                lucid.movings = this.movingList;
                lucid.stationaries = this.stationaryList;
                lucid.jumpings = this.jumpingList;
                lucid.Update(controls, gameTime);
            }
            shotList.RemoveAll(lucid => lucid.spent == true);

            foreach (Moving moving in movingList)
            {
                moving.testblocks = blocks;
                moving.Update(controls, gameTime);
            }
            movingList.RemoveAll(moving => moving.dead == true);

            foreach (Jumping jumping in jumpingList)
            {
                jumping.testblocks = blocks;
                jumping.Update(controls, gameTime);
            }
            jumpingList.RemoveAll(jumping => jumping.dead == true);

            foreach (Stationary stationary in stationaryList)
            {
                stationary.testblocks = blocks;
                stationary.Update(controls, gameTime);
            }
            stationaryList.RemoveAll(stationary => stationary.dead == true);

            foreach (Powerup power in powerList)
            {
                power.Update(controls, gameTime);
            }
            powerList.RemoveAll(powerups => powerups.collected == true);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.End();

            game.GraphicsDevice.SetRenderTarget(mainScene);
            //game.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);
            
            if (player1.endOfLevel == true)
            {
                game.LevelComplete();
            }

            foreach (Moving moving in movingList)
            {
                if (moving.dead == false)
                {
                    moving.Draw(spriteBatch);
                }
            }

            foreach (Jumping jumping in jumpingList)
            {
                if (jumping.dead == false)
                {
                    jumping.Draw(spriteBatch);
                }
            }

            foreach (Stationary stationary in stationaryList)
            {
                if (stationary.dead == false)
                {
                    stationary.Draw(spriteBatch);
                }
            }

            foreach (SpikeR spikeR in spikeRList)
            {
                spikeR.Draw(spriteBatch);
            }

            foreach (SpikeL spikeL in spikeLList)
            {
                spikeL.Draw(spriteBatch);
            }

            foreach (SpikeB spikeB in spikeBList)
            {
                spikeB.Draw(spriteBatch);
            }

            foreach (SpikeT spikeT in spikeTList)
            {
                spikeT.Draw(spriteBatch);
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

            if (game.level == 0)
            {
                spriteBatch.Draw(tut1, new Rectangle(30, 550, 137, 91), Color.White);
                spriteBatch.Draw(tut2, new Rectangle(180, 400, 137, 91), Color.White);
                spriteBatch.Draw(tut3, new Rectangle(340, 550, 137, 91), Color.White);
                spriteBatch.Draw(tut4, new Rectangle(540, 370, 137, 91), Color.White);
                spriteBatch.Draw(tut5, new Rectangle(800, 550, 137, 91), Color.White);
                spriteBatch.Draw(tut6, new Rectangle(1050, 550, 137, 91), Color.White);
            }

            spriteBatch.End();
            game.GraphicsDevice.SetRenderTarget(null);

            game.GraphicsDevice.SetRenderTarget(lightMask);
            //game.GraphicsDevice.Clear(Color.Black);

            // Create a Black Background
            //spriteBatch.Begin();
            //spriteBatch.Draw(darkness, new Vector2(0, 0), new Rectangle(0, 0, 1280, 720), Color.White);
            //spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive);

            // Draw out lightmask
            player1.Draw(spriteBatch);
            foreach (Powerup power in powerList)
            {
                var pos = new Rectangle(power.getX() - 40, power.getY() - 40, 100, 100);
                spriteBatch.Draw(lightmask, pos, Color.White);
            }
            foreach (Warp warp in warpList)
            {
                var pos = new Rectangle(warp.getX() - 25, warp.getY() - 50, 100, 100);
                spriteBatch.Draw(warpmask, pos, Color.White);
            }
            foreach (Lucidity shot in shotList)
            {
                var pos2 = new Rectangle(shot.getX() - 40, shot.getY() - 40, 100, 100);
                spriteBatch.Draw(lightmask, pos2, Color.White);
            }
            if (player1.lucidity <= 5)
            {
                //spriteBatch.Draw(lightmask, new Rectangle(darkX, darkY, 0, 0), Color.White);
            }
            else if (player1.lucidity > 5 && player1.lucidity < 40)
            {
                spriteBatch.Draw(lightmask, new Rectangle(darkX + 75, darkY + 75, 150, 150), Color.White);
            }
            else if (player1.lucidity >= 40 && player1.lucidity < 60)
            {
                spriteBatch.Draw(lightmask, new Rectangle(darkX + 35, darkY + 35, 200, 200), Color.White);
            }
            else if (player1.lucidity >= 60 && player1.lucidity < 80)
            {
                spriteBatch.Draw(lightmask, new Rectangle(darkX + 15, darkY + 15, 250, 250), Color.White);
            }
            else if (player1.lucidity >= 80)
            {
                spriteBatch.Draw(lightmask, new Rectangle(darkX - 5, darkY - 5, 300, 300), Color.White);
            }
            spriteBatch.End();

            game.GraphicsDevice.SetRenderTarget(null);

            //Draw everything to screen w/ blendstate
            //game.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Immediate, null);
            spriteBatch.Draw(mainScene, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            BlendState multiply = new BlendState();
            multiply.ColorBlendFunction = BlendFunction.Add;
            multiply.ColorSourceBlend = Blend.DestinationColor;
            multiply.ColorDestinationBlend = Blend.Zero;

            spriteBatch.Begin(SpriteSortMode.Immediate, multiply);
            spriteBatch.Draw(lightMask, new Vector2(0, 0), Color.White);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, null);
            player1.Draw(spriteBatch);
            hud.Draw(spriteBatch);
            spriteBatch.End();

            //Draw timer
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Level " + game.level.ToString(), new Vector2(20, 18), Color.White);
            if (!timer.isFinished)
            {
                spriteBatch.DrawString(font, timer.displayClock, new Vector2(350, 18), Color.White);
            }
            spriteBatch.End();
        }
        public void loadLevel(string name)
        {
            //Read in the level
            string[] lines = System.IO.File.ReadAllLines(name);
            string[] keylines = System.IO.File.ReadAllLines("Levels/key.txt");
            int numRows = lines.Length;
            int numCols = lines[0].Length;
            int realnumCols = lines[0].Split(new Char[] { ' ' }).Length; //removes the whitespace
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
                    if (level[i, j].Equals("X"))
                    {
                        Block temp = new Block(25 * j, 25 * i, 25, 25, game);
                        blocks.Add(temp);
                    }
                    if (level[i, j].Equals("W"))
                    {
                        warpTemp = new Warp(25 * j, 25 * i, 50, 50);
                        warpTemp.LoadContent(game.Content);
                        warpList.Add(warpTemp);
                    }
                    if (level[i, j].Equals("P"))
                    {
                        player1 = new Player(20 * j, 20 * i, 20, 40);
                    }
                    if (level[i, j].Equals("M"))
                    {
                        movingTemp = new Moving(20 * j, 20 * i, 40, 40);
                        movingTemp.LoadContent(game.Content);
                        movingList.Add(movingTemp);
                    }
                    if (level[i, j].Equals("J"))
                    {
                        jumpingTemp = new Jumping(20 * j, 20 * i, 40, 40);
                        jumpingTemp.LoadContent(game.Content);
                        jumpingList.Add(jumpingTemp);
                    }
                    if (level[i, j].Equals("S"))
                    {
                        stationaryTemp = new Stationary(25 * j, 25 * i, 40, 40);
                        stationaryTemp.LoadContent(game.Content);
                        stationaryList.Add(stationaryTemp);
                    }
                    if (level[i, j].Equals("R"))
                    {
                        spikeRTemp = new SpikeR(25 * j, 25 * i, 25, 25);
                        spikeRTemp.LoadContent(game.Content);
                        spikeRList.Add(spikeRTemp);
                    }
                    if (level[i, j].Equals("L"))
                    {
                        spikeLTemp = new SpikeL(25 * j, 25 * i, 25, 25);
                        spikeLTemp.LoadContent(game.Content);
                        spikeLList.Add(spikeLTemp);
                    }
                    if (level[i, j].Equals("B"))
                    {
                        spikeBTemp = new SpikeB(25 * j, 25 * i, 25, 25);
                        spikeBTemp.LoadContent(game.Content);
                        spikeBList.Add(spikeBTemp);
                    }
                    if (level[i, j].Equals("T"))
                    {
                        spikeTTemp = new SpikeT(25 * j, 25 * i, 25, 25);
                        spikeTTemp.LoadContent(game.Content);
                        spikeTList.Add(spikeTTemp);
                    }
                    if (level[i, j].Equals("U"))
                    {
                        powerTemp = new Powerup(25 * j, 25 * i, 20, 20);
                        powerTemp.LoadContent(game.Content);
                        powerList.Add(powerTemp);
                    }
                }
            }
        }
    }
}
