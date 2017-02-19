using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Tetris
{
    public class Block
    {
        private Texture2D texture;
        private Rectangle rectangle;
        private Color color;
        private bool falling;

        public Rectangle Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }
        public bool Falling
        {
            get { return falling; }
            set { falling = value; }
        }

        public Block(Color color, ContentManager content)
        {
            texture = content.Load<Texture2D>("block");
            rectangle = new Rectangle();
            falling = true;
            this.color = color;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, color);
        }
    }
}
