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
    class Menu
    {
        private bool initialized = false;
        private int _selectedIndex;
        Controls controls;
        private List<MenuItem> menuItems { get; set; }
        public int Count
        {
            get { return menuItems.Count; }
        }
        public string Title { get; set; }
        private int lastNavigated { get; set; }
        public int selectedIndex
        {
            get
            {
                return _selectedIndex;
            }
            protected set
            {
                if (value >= menuItems.Count || value < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                _selectedIndex = value;
            }
        }
        public MenuItem SelectedItem
        {
            get
            {
                return menuItems[selectedIndex];
            }
        }
        public Menu(string title)
        {
            menuItems = new List<MenuItem>();
            Title = title;
        }
        public virtual void AddMenuItem(string title, Action<Keys> action)
        {
            AddMenuItem(title, action, "");
        }

        public virtual void AddMenuItem(string title, Action<Keys> action, string description)
        {
            menuItems.Add(new MenuItem { Title = title, Description = description, Action = action });
            selectedIndex = 0;
        }
        public void DrawMenu(SpriteBatch batch, int screenWidth, SpriteFont font, int yPos, Vector2 descriptionPos, Color itemColor, Color selectedColor)
        {
            batch.DrawString(font, Title, new Vector2(screenWidth / 2 - font.MeasureString(Title).X / 2, yPos), Color.White);
            yPos += (int)font.MeasureString(Title).Y + 10;
            for (int i = 0; i < Count; i++)
            {
                Color color = itemColor;
                if (i == selectedIndex)
                {
                    color = selectedColor;
                }
                batch.DrawString(font, menuItems[i].Title, new Vector2(screenWidth / 2 - font.MeasureString(menuItems[i].Title).X / 2, yPos), color);
                yPos += (int)font.MeasureString(menuItems[i].Title).Y + 10;
            }
            batch.DrawString(font, menuItems[selectedIndex].Description, descriptionPos, selectedColor);
        }
        public void Navigate(KeyboardState keyboardState, GameTime gameTime)
        {
            if (!initialized)
            {
                lastNavigated = (int)gameTime.TotalGameTime.TotalMilliseconds;
                initialized = true;
            }
            if (gameTime.TotalGameTime.TotalMilliseconds - lastNavigated > 250)
            {
                if (keyboardState.IsKeyDown(Keys.Down) && selectedIndex < Count - 1)
                {
                    selectedIndex++;
                    lastNavigated = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
                if (keyboardState.IsKeyDown(Keys.Up) && selectedIndex > 0)
                {
                    selectedIndex--;
                    lastNavigated = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    SelectedItem.Action(Keys.Enter);
                    lastNavigated = (int)gameTime.TotalGameTime.TotalMilliseconds;
                }
            }
        }
    }
}
