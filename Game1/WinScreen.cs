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
    public class WinScreen
    {
        Texture2D background, youwon, again, exitButton;
        Vector2 againButtonPosition, exitButtonPosition;
        MouseState mouseState;
        MouseState previousMouseState;
        GameLoop game;

        public WinScreen(GameLoop game)
        {
            this.game = game;
            background = game.Content.Load<Texture2D>("darkwoods");
            youwon = game.Content.Load<Texture2D>("youwon");
            //again = game.Content.Load<Texture2D>("again");
            exitButton = game.Content.Load<Texture2D>("exit");

            //set the position of the buttons
            //againButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2) - 50, 400);
            exitButtonPosition = new Vector2((game.GraphicsDevice.Viewport.Width / 2) - 50, 450);
        }

        public void Update()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                game.Exit();
            }

            mouseState = Mouse.GetState();
            if (previousMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                MouseClicked(mouseState.X, mouseState.Y);
            }

            previousMouseState = mouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);
            spriteBatch.Draw(youwon, new Rectangle(400, 100, 400, 170), Color.White);
            //spriteBatch.Draw(again, againButtonPosition, Color.White);
            spriteBatch.Draw(exitButton, exitButtonPosition, Color.White);
        }
        void MouseClicked(int x, int y)
        {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 10, 10);

            //Rectangle againButtonRect = new Rectangle((int)againButtonPosition.X, (int)againButtonPosition.Y, 100, 20);
            Rectangle exitButtonRect = new Rectangle((int)exitButtonPosition.X, (int)exitButtonPosition.Y, 100, 20);

            //if (mouseClickRect.Intersects(againButtonRect)) //player clicked start button
            //{
            //    game.level = 0;
            //    game.StartGame();
            //}
            if (mouseClickRect.Intersects(exitButtonRect)) //player clicked exit button
            {
                game.Exit();
            }
        }
    }
}