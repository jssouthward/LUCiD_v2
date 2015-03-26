﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LUCiD
{
    class Block : Sprite
    {
        string color;

         public Block(int x, int y, int width, int height, string color)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.color = color;
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
        public int getSpriteWidth()
        {
            return spriteWidth;
        }
        public int getSpriteHeight()
        {
            return spriteHeight;
        }

        public void LoadContent(ContentManager content)
        {
            image = content.Load<Texture2D>("gray.png");
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
        }

		public void Update(Controls controls, GameTime gameTime)
		{

		}

		public void Move(Controls controls)
		{

		}

		private void checkYCollisions()
		{

		}

		private void Jump(Controls controls, GameTime gameTime)
		{

		}
    }
}
