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
    public class GameArea
    {
        public const int WIDTH = 10,
            HEIGHT = 18,
            X = 145,
            Y = 16,
            OFFSET_X = -3,
            OFFSET_Y = -3;

        private Texture2D texture;
        private Vector2 bgPosition;
        private Tetrimino tetrimino;
        private ContentManager content;
        private SoundControl soundControl;
        private BlockSlot[,] slots;
        private Stats.Stats stats;
        private int next;
        private int? hold;
        private bool canHold;

        public BlockSlot[,] Slots
        {
            get { return slots; }
        }

        public GameArea(Stats.Stats stats, ContentManager content, SoundControl soundControl)
        {
            texture = content.Load<Texture2D>("field");
            bgPosition = new Vector2(X + OFFSET_X, Y + OFFSET_Y);
            slots = new BlockSlot[WIDTH, HEIGHT];
            this.content = content;
            this.stats = stats;

            for (int i = 0; i < WIDTH; i++)
                for (int j = 0; j < HEIGHT; j++)
                    slots[i, j] = new BlockSlot(i, j, X, Y);

            tetrimino = new Tetrimino(Tetrimino.GenerateType(null), content, soundControl);
            PrepareNextTetrimino();
            canHold = true;
            this.soundControl = soundControl;
        }

        public void Update(GameControl gameControl, GameTime gameTime, KeyboardState keyState, KeyboardState prevKeyState)
        {
            tetrimino.Update(slots, stats, gameTime, keyState, prevKeyState);

            if (!tetrimino.Falling)
            {
                //Game over
                if (tetrimino.Position.Y < 0)
                    gameControl.GameOver();

                //Detach blocks
                foreach (Block block in tetrimino.Blocks)
                    if (block != null)
                        block.Falling = false;

                //Clear any complete lines
                int completeLines = 0;
                for (int i = 0; i < HEIGHT; i++)
                {
                    bool completeLine = true;
                    for (int j = 0; j < WIDTH; j++)
                    {
                        if (slots[j, i].Block == null)
                        {
                            completeLine = false;
                            break;
                        }
                    }
                    if (completeLine)
                    {
                        for (int j = 0; j < WIDTH; j++)
                            slots[j, i].Block = null;
                        for (int k = 0; k < WIDTH; k++)
                        {
                            for (int l = i - 1; l >= 0; l--)
                            {
                                if (slots[k, l].Block != null)
                                {
                                    if (!slots[k, l].Block.Falling)
                                    {
                                        slots[k, l + 1].Block = slots[k, l].Block;
                                        slots[k, l].Block = null;
                                    }
                                }
                            }
                        }
                        stats.Goal--;
                        completeLines++;
                    }
                }
                switch (completeLines)
                {
                    case 1: { stats.Score += 100; soundControl.PlayClear1(); break; }
                    case 2: { stats.Score += 300; soundControl.PlayClear2(); break; }
                    case 3: { stats.Score += 500; soundControl.PlayClear3(); break; }
                    case 4: { stats.Score += 800; soundControl.PlayClear4(); break; }
                }
                
                //New tetrimino
                tetrimino = new Tetrimino(next, content, soundControl);
                PrepareNextTetrimino();
                canHold = true;
            }

            //Hold
            if (keyState.IsKeyDown(Keys.LeftShift) && prevKeyState.IsKeyUp(Keys.LeftShift))
            {
                int currentType = tetrimino.Type;
                if (canHold)
                {
                    for (int i = 0; i < 4; i++)
                        for (int j = 0; j < 4; j++)
                            if ((int)tetrimino.Position.Y + j >= 0 && (int)tetrimino.Position.X + i >= 0 && (int)tetrimino.Position.X + i < WIDTH && (int)tetrimino.Position.Y + j < HEIGHT)
                                if (tetrimino.Blocks[i, j] != null)
                                    slots[(int)tetrimino.Position.X + i, (int)tetrimino.Position.Y + j].Block = null;
                    if (hold == null)
                    {
                        tetrimino = new Tetrimino(next, content, soundControl);
                        PrepareNextTetrimino();
                    }
                    else tetrimino = new Tetrimino((int)hold, content, soundControl);
                    hold = currentType;
                    stats.Hold.Set((int)hold);
                    canHold = false;
                    soundControl.PlayDropSoft();
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, bgPosition, Color.White);
            foreach (BlockSlot blockSlot in slots)
                blockSlot.Draw(spriteBatch);
        }

        private void PrepareNextTetrimino()
        {
            next = Tetrimino.GenerateType(tetrimino.Type);
            stats.Next.Set(next);
        }
    }
}
