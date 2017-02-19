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
    public class BlockSlot
    {
        private Rectangle rectangle;
        private Block block;

        public Block Block
        {
            get { return block; }
            set
            {
                block = value;
                if (block != null) block.Rectangle = rectangle;
            }
        }

        public BlockSlot(int x, int y, int offsetX, int offsetY)
        {
            rectangle = new Rectangle(offsetX + x * 32, offsetY + y * 32, 32, 32);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Block != null) Block.Draw(spriteBatch);
        }
    }
}
