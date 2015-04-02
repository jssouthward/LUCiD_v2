using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace LUCiD
{
    class Lucidity : Sprite
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
        public List<Monster> monsters;
        public int direction;
        public bool spent = false;

        public Lucidity(int x, int y, int width, int height, int direction)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            grounded = false;
            moving = false;
            pushing = false;
            this.direction = direction;

            // Movement
            speed = 5;
            friction = .15;
            x_accel = 0;
            x_vel = -2;
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
            image = content.Load<Texture2D>("lucid.png");
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
        }

        public void Update(Controls controls, GameTime gameTime)
        {
            Move(controls);
            //Jump(controls, gameTime);
        }

        public void Move(Controls controls)
        {

            double playerFriction = pushing ? (friction * 3) : friction;
            x_vel = x_vel + x_accel * .10;
            movedX = Convert.ToInt32(x_vel);

            spriteX += movedX*this.direction;



            // Check up/down collisions, then left/right
            checkCollisions();

        }

        private void checkCollisions()
        {

            foreach (Monster monster in monsters)
            {
                Rectangle currMonster = new Rectangle(monster.getX(), monster.getY(), monster.getSpriteWidth(), monster.getSpriteHeight());
                Rectangle lucidBox = new Rectangle(this.spriteX, this.spriteY, this.spriteWidth, this.spriteHeight);

                Rectangle rect = Rectangle.Intersect(lucidBox, currMonster);
                if (!rect.IsEmpty && !this.spent)
                {
                    monster.dead = true;
                    this.spent = true;
                    break;
                }
            } 
 
        }

        private void Jump(Controls controls, GameTime gameTime)
        {

        }
    }
}
