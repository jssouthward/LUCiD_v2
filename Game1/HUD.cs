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
    class HUD
    {
        GameLoop game;
        Player player;
        Texture2D healthTexture, lucidityTexture, header;
        public HUD(GameLoop game, Player player1)
        {
            this.game = game;
            player = player1;
            healthTexture = game.Content.Load<Texture2D>("health");
            lucidityTexture = game.Content.Load<Texture2D>("lucidity");
            header = game.Content.Load<Texture2D>("header");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(header, new Rectangle(0, 0, 1500, 52), Color.White);
            spriteBatch.Draw(healthTexture, new Rectangle(100, 16, (int)player.health, 20), Color.White);
            spriteBatch.Draw(lucidityTexture, new Rectangle(220, 16, (int)player.lucidity, 20), Color.White);
            spriteBatch.End(); 
        }
    }
}
