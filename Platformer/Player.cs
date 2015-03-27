using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

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
        public Lucidity shot;
        private int currDirection = 1;
        public bool fired = false;
        
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

        public int getX(){
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
            image = content.Load<Texture2D>("dude.png");
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
        }

		public void Update(Controls controls, GameTime gameTime)
		{
			Move (controls);
			Jump (controls, gameTime);

            if (controls.onPress(Keys.X, Buttons.LeftShoulder))
            {
                shot.setX(this.getX());
                shot.setY(this.getY());
                shot.direction = currDirection;
                shot.spent = false;
            }
            
		}

		public void Move(Controls controls)
		{

			// Sideways Acceleration
			if (controls.onPress(Keys.Right, Buttons.DPadRight)) 
            {
                x_accel += speed;
                currDirection = -1;
            }
				
			else if (controls.onRelease(Keys.Right, Buttons.DPadRight))
				x_accel -= speed;

            if (controls.onPress(Keys.Left, Buttons.DPadLeft))
            {
                x_accel -= speed;
                currDirection = 1;
            }

            else if (controls.onRelease(Keys.Left, Buttons.DPadLeft))
                x_accel += speed;

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
			if (spriteY >= 800)
				grounded = true;
			else
				grounded = false;
           
            // TODO need to handle case where 1x1 corner
            foreach (Block block in testblocks)
            {
                Rectangle currBlock = new Rectangle(block.getX(),block.getY(),block.getSpriteWidth(),block.getSpriteHeight());
                Rectangle playerBox = new Rectangle(this.spriteX, this.spriteY, this.spriteWidth, this.spriteHeight);

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
                if (rect.Height > rect.Width && this.spriteX+1 > rect.X)
                {
                    //side collision with player on the right
                    this.spriteX += rect.Width;
                    x_vel = 0;
                    //Console.WriteLine("collision");
                }
                if (rect.Height < rect.Width && this.spriteY < rect.Y)
                {
                    // floor collision
                    this.spriteY -= rect.Height;
                    y_vel = 0;
                }
                if (rect.Height < rect.Width && this.spriteY+1 > rect.Y)
                {
                    // cieling collision
                    this.spriteY += rect.Height;
                    y_vel = 0;
                }
            } 
		}

		private void Jump(Controls controls, GameTime gameTime)
		{
			// Jump on button press
			if (controls.onPress(Keys.Space, Buttons.A) && grounded)
			{
				y_vel = -17;
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
