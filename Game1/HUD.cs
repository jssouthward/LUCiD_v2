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
    public class HUD
    {
        private Vector2 scorePos = new Vector2(20, 10);

        public SpriteFont Font { get; set; }

        public int Score { get; set; }

        public HUD()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the Score in the top-left of screen
            spriteBatch.DrawString(
                Font,                           // SpriteFont
                "Score: " + Score.ToString(),   // Text
                scorePos,                       // Position
                Color.White);                   // Tint
        }
    }
}
