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
    namespace Stats
    {
        public class Stats
        {
            private int score, level, goal;
            private double fallTimeout;
            private PreviewTetrimino next, hold;
            private SpriteFont font;

            public int Score
            {
                get { return score; }
                set { score = value; }
            }

            public int Level
            {
                get { return level; }
            }

            public PreviewTetrimino Hold
            {
                get { return hold; }
            }

            public PreviewTetrimino Next
            {
                get { return next; }
            }

            public double FallTimeout
            {
                get { return fallTimeout; }
            }

            public int Goal
            {
                get { return goal; }
                set
                {
                    goal = value;
                    if (value == 0) SetLevel(level + 1);
                }
            }

            public Stats(ContentManager content)
            {
                next = new PreviewTetrimino("Next", 20, 130, content);
                hold = new PreviewTetrimino("Hold", 20, 250, content);
                font = content.Load<SpriteFont>("small");
                SetLevel(1);
            }

            private void SetLevel(int newLevel)
            {
                level = newLevel;
                fallTimeout = Tetrimino.TIME_0 * Math.Pow(6.0f / 7.0f, newLevel - 1);
                Goal = (int)(10 * Math.Pow(3.0f / 2.0f, level - 1));
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.DrawString(font, "Score: " + score, new Vector2(20, 20), Color.Black);
                spriteBatch.DrawString(font, "Level: " + level, new Vector2(20, 50), Color.Black);
                spriteBatch.DrawString(font, "Goal: " + goal, new Vector2(20, 80), Color.Black);

                next.Draw(spriteBatch);
                hold.Draw(spriteBatch);
            }
        }
    }
}
