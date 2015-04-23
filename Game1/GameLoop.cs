using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Linq;
using System.Text;

namespace LUCiD
{

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameLoop : Game
    {
        enum Screen
        {
            StartScreen,
            GameScreen,
            GameOverScreen,
            LevelCompleteScreen
        }
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        StartScreen startScreen;
        GameScreen gameScreen;
        GameOverScreen gameOverScreen;
        LevelCompleteScreen levelCompleteScreen;
        Screen currentScreen;
        SoundEffect song;
        SoundEffectInstance mySong;
        string name;
        public int level;

        public GameLoop()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 720;   // set this value to the desired height of your window
            graphics.ApplyChanges();
            IsMouseVisible = true;
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
            //controls = new Controls();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            startScreen = new StartScreen(this);
            currentScreen = Screen.StartScreen;

            //song = this.Content.Load<SoundEffect>("DST-Arch-Delerium_wav");
            //mySong = song.CreateInstance();
            //mySong.IsLooped = true;
            //mySong.Play();

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            // TODO: Add your update logic here

            MouseState mouse = Mouse.GetState();

            switch (currentScreen)
            {
                case Screen.StartScreen:
                    if (startScreen != null)
                        startScreen.Update();
                    break;
                case Screen.GameScreen:
                    if (gameScreen != null)
                        gameScreen.Update(gameTime);
                    break;
                case Screen.GameOverScreen:
                    if (gameOverScreen != null)
                        gameOverScreen.Update();
                    break;
                case Screen.LevelCompleteScreen:
                    if (levelCompleteScreen != null)
                        levelCompleteScreen.Update();
                    break;
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            switch (currentScreen)
            {
                case Screen.StartScreen:
                    if (startScreen != null)
                        startScreen.Draw(spriteBatch);
                    break;
                case Screen.GameScreen:
                    if (gameScreen != null)
                        gameScreen.Draw(spriteBatch);
                    break;
                case Screen.GameOverScreen:
                    if (gameOverScreen != null)
                        gameOverScreen.Draw(spriteBatch);
                    break;
                case Screen.LevelCompleteScreen:
                    if (levelCompleteScreen != null)
                        levelCompleteScreen.Draw(spriteBatch);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void StartGame()
        {
            level = 0;
            name = "Levels/level0.txt";
            gameScreen = new GameScreen(this, spriteBatch, name);
            currentScreen = Screen.GameScreen;

            startScreen = null;
            gameOverScreen = null;
            levelCompleteScreen = null;
        }
        public void GameOver()
        {
            gameOverScreen = new GameOverScreen(this);
            currentScreen = Screen.GameOverScreen;

            startScreen = null;
            gameScreen = null;
            levelCompleteScreen = null;
        }
        public void LevelComplete()
        {
            levelCompleteScreen = new LevelCompleteScreen(this);
            currentScreen = Screen.LevelCompleteScreen;

            startScreen = null;
            gameScreen = null;
            gameOverScreen = null;
        }
        public void NextLevel()
        {
            level++;
            name = "Levels/level" + level + ".txt";
            gameScreen = new GameScreen(this, spriteBatch, name);
            currentScreen = Screen.GameScreen;

            startScreen = null;
            gameOverScreen = null;
            levelCompleteScreen = null;
        }
    }
}
