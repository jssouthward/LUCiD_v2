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
        Camera camera;
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
        string[,] level;
        Dictionary<string, string> key = new Dictionary<string, string>();
        int darkX;
        int darkY;
        Texture2D background, darkness, lightmask;
        //health bar
        Texture2D healthTexture;

        //lucidity bar
        Texture2D lucidityTexture;

        //lighting
        RenderTarget2D mainScene;
        RenderTarget2D lightMask;

        public GameScreen(GameLoop game, SpriteBatch spriteBatch, string name)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            loadLevel(name);
            controls = new Controls();
            camera = new Camera(game.GraphicsDevice.Viewport);
            player1.LoadContent(game.Content);
            darkness = game.Content.Load<Texture2D>("darkness");
            lightmask = game.Content.Load<Texture2D>("lightmask");
            background = game.Content.Load<Texture2D>("darkwoods");
            healthTexture = game.Content.Load<Texture2D>("health");
            lucidityTexture = game.Content.Load<Texture2D>("lucidity");
            var pp = game.GraphicsDevice.PresentationParameters;
            mainScene = new RenderTarget2D(game.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
            lightMask = new RenderTarget2D(game.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight);
        }

        public void Update(GameTime gameTime)
        {
            controls.Update();
            camera.Update(gameTime, player1);
            player1.testblocks = blocks;
            player1.powerTest = powerList;
            player1.monsterTest = monsterList;
            player1.warpTest = warpList;
            player1.Update(controls, gameTime);

            darkX = player1.getX() - 134; //150-16 offset for player
            darkY = player1.getY() - 118; //150-32

            if (player1.health <= 0)
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

            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.transform);
            spriteBatch.Begin();

            spriteBatch.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);
            
            if (player1.endOfLevel == true)
            {
                game.LevelComplete();
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

            spriteBatch.Draw(healthTexture, new Rectangle(20, 20, (int)player1.health, 20), Color.White);
            spriteBatch.Draw(lucidityTexture, new Rectangle(140, 20, (int)player1.lucidity, 20), Color.White);

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
                //spriteBatch.Draw(lightmask, pos, Color.White);
            }
            foreach (Lucidity shot in shotList)
            {
                var pos2 = new Rectangle(shot.getX() - 40, shot.getY() - 40, 100, 100);
                spriteBatch.Draw(lightmask, pos2, Color.White);
            }
            if (player1.lucidity < 33)
            {
                spriteBatch.Draw(lightmask, new Rectangle(darkX + 100, darkY + 100, 100, 100), Color.White);
            }
            if (player1.lucidity < 67 && player1.lucidity >= 33)
            {
                spriteBatch.Draw(lightmask, new Rectangle(darkX + 50, darkY + 50, 200, 200), Color.White);
            }
            if (player1.lucidity >= 67)
            {
                spriteBatch.Draw(lightmask, new Rectangle(darkX, darkY, 300, 300), Color.White);
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

            //hud.Draw(spriteBatch);
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
                        monsterTemp = new Monster(20 * j, 20 * i, 40, 40);
                        monsterTemp.LoadContent(game.Content);
                        monsterList.Add(monsterTemp);
                    }
                    if (level[i, j].Equals("U"))
                    {
                        powerTemp = new Powerup(20 * j, 20 * i, 20, 20);
                        powerTemp.LoadContent(game.Content);
                        powerList.Add(powerTemp);
                    }
                }
            }
        }
    }
}
