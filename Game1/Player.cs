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
    class Player : Sprite
    {
        private bool moving;
        private bool grounded;
        private int speed;
        private int x_accel;
        private double friction;
        public double x_vel;
        public double y_vel;
        public int movedX;
        private bool pushing;
        public double gravity = .5;
        public int maxFallSpeed = 10;
        private int jumpPoint = 0;
        public List<Block> testblocks;
        public List<Powerup> powerTest;
        public List<Moving> movingTest;
        public List<Stationary> stationaryTest;
        public List<Warp> warpTest;
        public List<Lucidity> shotTest = new List<Lucidity>();
        public int currDirection = 1;
        public bool fired = false;
        public int lucidity = 100;
        public double health = 100;
        public bool endOfLevel = false;
        public bool moved = false;

        //animation
        // the elapsed amount of time the frame has been shown for
        float time;
        // duration of time to show each frame
        float frameTime = 0.15f;
        // an index of the current frame being shown
        int frameIndex;
        // total number of frames in our spritesheet
        const int totalFrames = 3;
        // define the size of our animation frame
        int frameHeight = 128;
        int frameWidth = 85;
        Rectangle source;

        public Player(int x, int y, int width, int height)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            grounded = false;
            moving = false;
            pushing = false;

            // Movement
            speed = 5;
            friction = .15;
            x_accel = 0;
            x_vel = 0;
            y_vel = 0;
            movedX = 0;
        }

        public int getX()
        {
            return spriteX;
        }
        public int getY()
        {
            return spriteY;
        }
        public void setX(int x)
        {
            spriteX = x;
        }
        public void setY(int y)
        {
            spriteY = y;
        }

        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>("character");
        }

        public void Draw(SpriteBatch sb)
        {
            if (!moved && (currDirection == 1))
            {
                sb.Draw(image, new Rectangle(spriteX, spriteY, frameWidth, frameHeight), new Rectangle(0, 0, frameWidth, frameHeight), Color.White,
                0.0f, new Vector2(frameWidth / 2.0f, frameHeight), SpriteEffects.None, 0.0f);
            }
            else if (!moved && (currDirection == -1))
            {
                sb.Draw(image, new Rectangle(spriteX, spriteY, frameWidth, frameHeight), new Rectangle(frameWidth * 3, frameHeight, frameWidth, frameHeight), Color.White,
                0.0f, new Vector2(frameWidth / 2.0f, frameHeight), SpriteEffects.None, 0.0f);
            }
            else if (moved && (currDirection == 1))
            {
                sb.Draw(image, new Rectangle(spriteX, spriteY, frameWidth, frameHeight), source, Color.White,
                0.0f, new Vector2(frameWidth / 2.0f, frameHeight), SpriteEffects.None, 0.0f);
            }
            else if (moved && (currDirection == -1))
            {
                sb.Draw(image, new Rectangle(spriteX, spriteY, frameWidth, frameHeight), source, Color.White,
                0.0f, new Vector2(frameWidth / 2.0f, frameHeight), SpriteEffects.None, 0.0f);
            }
        }

        public void Update(Controls controls, GameTime gameTime)
        {
            // Process elapsed time
            if (moved)
            {
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                while (time > frameTime)
                {
                    // Play the next frame in the SpriteSheet
                    frameIndex++;
                    // reset elapsed time
                    time = 0f;
                }
                if (frameIndex > totalFrames) frameIndex = 1;
                if (currDirection == 1)
                {
                    source = new Rectangle(frameIndex * frameWidth, 0, frameWidth, frameHeight);
                }
                else if (currDirection == -1)
                {
                    source = new Rectangle(frameIndex * frameWidth, frameHeight, frameWidth, frameHeight);
                }
            }
            Move(controls);
            Jump(controls, gameTime);
        }

        public void Move(Controls controls)
        {
            // Sideways Acceleration
            if (controls.onPress(Keys.Right, Buttons.DPadRight))
            {
                x_accel += speed;
                currDirection = -1;
                moved = true;
                // source = new Rectangle(frameIndex * frameWidth, frameHeight, frameWidth, frameHeight);
            }

            else if (controls.onRelease(Keys.Right, Buttons.DPadRight))
            {
                x_accel -= speed;
                moved = false;
            }

            if (controls.onPress(Keys.Left, Buttons.DPadLeft))
            {
                x_accel -= speed;
                currDirection = 1;
                moved = true;
            }

            else if (controls.onRelease(Keys.Left, Buttons.DPadLeft))
            {
                x_accel += speed;
                moved = false;
            }

            double playerFriction = pushing ? (friction * 3) : friction;
            x_vel = x_vel * (1 - playerFriction) + x_accel * .10;
            movedX = Convert.ToInt32(x_vel);
            spriteX += movedX;

            // Gravity
            //TODO got rid of grounded
            if (!grounded)
            {
                y_vel += gravity;
                if (y_vel > maxFallSpeed)
                    y_vel = maxFallSpeed;
                spriteY += Convert.ToInt32(y_vel);
            }
            else
            {
                y_vel = 1;
            }

            grounded = false;

            // Check up/down collisions, then left/right
            checkCollisions();
        }

        private void checkCollisions()
        {
            // TODO need to remove this except for dying mechanic
            /*if (spriteY >= 800)
                grounded = true;
            else
                grounded = false;
             * */

            Rectangle playerBox = new Rectangle(this.spriteX, this.spriteY, this.spriteWidth, this.spriteHeight);

            foreach (Powerup power in powerTest)
            {
                Rectangle currPower = new Rectangle(power.getX(), power.getY(), power.getSpriteWidth(), power.getSpriteHeight());

                Rectangle rectPower = Rectangle.Intersect(playerBox, currPower);
                if (!rectPower.IsEmpty && power.collected == false)
                {
                    power.collected = true;
                    if (this.lucidity < 100)
                    {
                        this.lucidity += 20;
                    }
                }
            }

            foreach (Warp warp in warpTest)
            {
                Rectangle currPower = new Rectangle(warp.getX(), warp.getY(), warp.getSpriteWidth(), warp.getSpriteHeight());

                Rectangle rectPower = Rectangle.Intersect(playerBox, currPower);
                if (!rectPower.IsEmpty && (warp.getX() - warp.getSpriteWidth() / 2 + this.spriteWidth + 2) <= this.spriteX && this.spriteX <= warp.getX() + (warp.getSpriteWidth()) / 2 - this.spriteWidth / 2 + 10)
                {
                    this.endOfLevel = true;
                }
            }

            foreach (Moving moving in movingTest)
            {
                Rectangle currPower = new Rectangle(moving.getX(), moving.getY(), moving.getSpriteWidth(), moving.getSpriteHeight());

                Rectangle rectPower = Rectangle.Intersect(playerBox, currPower);
                if (!rectPower.IsEmpty && moving.dead == false)
                {
                    this.health -= 0.3;
                }
            }

            foreach (Stationary stationary in stationaryTest)
            {
                Rectangle currPower = new Rectangle(stationary.getX(), stationary.getY(), stationary.getSpriteWidth(), stationary.getSpriteHeight());

                Rectangle rectPower = Rectangle.Intersect(playerBox, currPower);
                if (!rectPower.IsEmpty && stationary.dead == false)
                {
                    this.health -= 0.3;
                }
            }

            // TODO need to handle case where 1x1 corner
            foreach (Block block in testblocks)
            {
                Rectangle currBlock = new Rectangle(block.getX(), block.getY(), block.getSpriteWidth(), block.getSpriteHeight());
                Rectangle rect = Rectangle.Intersect(playerBox, currBlock);
                if (!rect.IsEmpty && rect.Width > rect.Height && this.spriteY < rect.Y)
                {
                    grounded = true;
                }

                if (rect.Height > rect.Width && this.spriteX < rect.X)
                {
                    // side collision with player on the left
                    this.spriteX -= rect.Width;
                    x_vel = 0;
                }
                if (rect.Height > rect.Width && this.spriteX + 1 > rect.X)
                {
                    //side collision with player on the right
                    this.spriteX += rect.Width;
                    x_vel = 0;
                    //Console.WriteLine("collision");
                }
                if (rect.Height < rect.Width && this.spriteY < rect.Y)
                {
                    // floor collision
                    this.spriteY -= rect.Height - 1;
                    y_vel = 0;
                }
                if (rect.Height < rect.Width && this.spriteY + 1 > rect.Y)
                {
                    // cieling collision
                    this.spriteY += rect.Height + 1;
                    y_vel = 0;
                }
            }
        }

        private void Jump(Controls controls, GameTime gameTime)
        {
            // Jump on button press
            if (controls.onPress(Keys.Space, Buttons.A) && grounded)
            {
                y_vel = -13;
                jumpPoint = (int)(gameTime.TotalGameTime.TotalMilliseconds);
                grounded = false;
            }

            // Cut jump short on button release
            else if (controls.onRelease(Keys.Space, Buttons.A) && y_vel < 0)
            {
                y_vel /= 2;
            }
        }
    }
}
