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
    public class PreviewTetrimino
    {
        const int WIDTH = 100,
            HEIGHT = 100;

        private Texture2D texture;
        private SpriteFont font;
        private Vector2 size, position;
        private Color color;
        private string title;
        private byte[,] shape;

        public Vector2 Size
        {
            get { return size; }
        }

        public PreviewTetrimino(string title, int x, int y, ContentManager content)
        {
            texture = content.Load<Texture2D>("block");
            font = content.Load<SpriteFont>("small");
            position = new Vector2(x, y);
            this.title = title;
        }

        public void Set(int type)
        {
            switch ((int)type)
            {
                case 0:
                    {
                        shape = new byte[4, 1];
                        size = new Vector2(4, 1);
                        shape[0, 0] = shape[1, 0] = shape[2, 0] = shape[3, 0] = 1;
                        break;
                    }
                case 1:
                    {
                        shape = new byte[3, 2];
                        size = new Vector2(3, 2);
                        shape[0, 0] = shape[0, 1] = shape[1, 1] = shape[2, 1] = 1;
                        break;
                    }
                case 2:
                    {
                        shape = new byte[3, 2];
                        size = new Vector2(3, 2);
                        shape[0, 1] = shape[1, 1] = shape[2, 1] = shape[2, 0] = 1;
                        break;
                    }
                case 3:
                    {
                        shape = new byte[2, 2];
                        size = new Vector2(2, 2);
                        shape[0, 0] = shape[1, 0] = shape[0, 1] = shape[1, 1] = 1;
                        break;
                    }
                case 4:
                    {
                        shape = new byte[3, 2];
                        size = new Vector2(3, 2);
                        shape[1, 0] = shape[2, 0] = shape[0, 1] = shape[1, 1] = 1;
                        break;
                    }
                case 5:
                    {
                        shape = new byte[3, 2];
                        size = new Vector2(3, 2);
                        shape[1, 0] = shape[0, 1] = shape[1, 1] = shape[2, 1] = 1;
                        break;
                    }
                case 6:
                    {
                        shape = new byte[3, 2];
                        size = new Vector2(3, 2);
                        shape[0, 0] = shape[1, 0] = shape[1, 1] = shape[2, 1] = 1;
                        break;
                    }
            }
            color = Tetrimino.GetColor(type);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (shape != null)
            {
                spriteBatch.DrawString(font, title, new Vector2(position.X + (WIDTH - font.MeasureString(title).X) / 2, position.Y), Color.Black);
                for (int i = 0; i < (int)size.X; i++)
                    for (int j = 0; j < (int)size.Y; j++)
                        if (shape[i, j] == 1)
                            spriteBatch.Draw(texture, new Vector2((int)position.X + (WIDTH - (int)size.X * 32) / 2 + i * 32, (int)position.Y + 20 + (HEIGHT - 20 - (int)size.Y * 32) / 2 + j * 32), color);
            }
        }
    }
}
