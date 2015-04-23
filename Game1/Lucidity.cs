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
    class Lucidity : Sprite
    {
        //private bool moving;
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
        public List<Moving> movings;
        public List<Stationary> stationaries;
        public int direction;
        public bool spent = false;

        public Lucidity(int x, int y, int width, int height, int direction)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = 20;
            this.spriteHeight = 20;
            grounded = false;
            //moving = false;
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
            image = content.Load<Texture2D>("lucid");
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

            foreach (Moving moving in movings)
            {
                Rectangle currMonster = new Rectangle(moving.getX(), moving.getY(), moving.getSpriteWidth(), moving.getSpriteHeight());
                Rectangle lucidBox = new Rectangle(this.spriteX, this.spriteY, this.spriteWidth, this.spriteHeight);

                Rectangle rect = Rectangle.Intersect(lucidBox, currMonster);
                if (!rect.IsEmpty && !this.spent)
                {
                    moving.dead = true;
                    this.spent = true;
                    break;
                }
            }

            foreach (Stationary stationary in stationaries)
            {
                Rectangle currMonster = new Rectangle(stationary.getX(), stationary.getY(), stationary.getSpriteWidth(), stationary.getSpriteHeight());
                Rectangle lucidBox = new Rectangle(this.spriteX, this.spriteY, this.spriteWidth, this.spriteHeight);

                Rectangle rect = Rectangle.Intersect(lucidBox, currMonster);
                if (!rect.IsEmpty && !this.spent)
                {
                    stationary.dead = true;
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
