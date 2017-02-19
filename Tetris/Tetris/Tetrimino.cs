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
    enum TetriminoType { i, j, l, o, s, t, z }

    class Tetrimino
    {
        public const int START_X = 3,
            START_Y = -2,
            TIME_0 = 750,
            TIME_1 = 50,
            TIME_2 = 300;

        private TetriminoType type;
        private Vector2 position;
        private Block[,] blocks;
        private ContentManager content;
        private SoundControl soundControl;
        private double[] times;
        private bool movingSideways, falling;

        public Vector2 Position { get { return position; } }
        public Block[,] Blocks { get { return blocks; } }
        public int Type { get { return (int)type; } }
        public bool Falling { get { return falling; } }

        public Tetrimino(int type, ContentManager content, SoundControl soundControl)
        {
            this.type = (TetriminoType)type;
            position = new Vector2(START_X, START_Y);
            blocks = new Block[4, 4];
            times = new double[3];
            falling = true;
            this.content = content;

            byte[,] shape = GetShape(type);
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (shape[i, j] == 1)
                        blocks[i, j] = new Block(GetColor(type), content);

            this.soundControl = soundControl;
        }

        public void Update(BlockSlot[,] slots, Stats.Stats stats, GameTime gameTime, KeyboardState kState, KeyboardState pkState)
        {
            if (falling)
            {
                //Update times
                double elapsedTime = gameTime.ElapsedGameTime.TotalMilliseconds;
                times[0] += elapsedTime;
                times[1] += elapsedTime;
                if (movingSideways && times[2] < TIME_2) times[2] += elapsedTime;
                else if (!movingSideways) times[2] = 0;

                //Clear blocks from game field
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                        if (position.X + i >= 0 && position.X + i < GameArea.WIDTH &&
                            position.Y + j >= 0 && position.Y + j < GameArea.HEIGHT)
                        {
                            if (slots[(int)position.X + i, (int)position.Y + j].Block != null)
                                if (slots[(int)position.X + i, (int)position.Y + j].Block.Falling)
                                    slots[(int)position.X + i, (int)position.Y + j].Block = null;
                        }
                }

                //Fall
                if (times[0] >= stats.FallTimeout)
                    MoveDown(slots, false);
                else if (kState.IsKeyDown(Keys.Down) && times[1] >= TIME_1)
                {
                    MoveDown(slots, true);
                    stats.Score++;
                }
                if (kState.IsKeyDown(Keys.Space) && pkState.IsKeyUp(Keys.Space))
                {
                    while (falling)
                    {
                        MoveDown(slots, false);
                        stats.Score += 2;
                    }
                    soundControl.PlayDropHard();
                }

                //Move sideways
                if (kState.IsKeyDown(Keys.Left) || kState.IsKeyDown(Keys.Right))
                    movingSideways = true;
                else
                    movingSideways = false;
                if ((kState.IsKeyDown(Keys.Left) && pkState.IsKeyUp(Keys.Left)) ||
                    (kState.IsKeyDown(Keys.Left) && times[2] >= TIME_2 && times[1] >= TIME_1))
                    MoveSideways(true, slots);
                else if ((kState.IsKeyDown(Keys.Right) && pkState.IsKeyUp(Keys.Right)) ||
                    (kState.IsKeyDown(Keys.Right) && times[2] >= TIME_2 && times[1] >= TIME_1))
                    MoveSideways(false, slots);

                //Rotate
                else if (kState.IsKeyDown(Keys.Up) && pkState.IsKeyUp(Keys.Up)) Rotate(true, slots);
                else if (kState.IsKeyDown(Keys.LeftControl) && pkState.IsKeyUp(Keys.LeftControl)) Rotate(false, slots);

                //Set blocks on game field
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                        if (position.X + i >= 0 && position.X + i < GameArea.WIDTH &&
                            position.Y + j >= 0 && position.Y + j < GameArea.HEIGHT)
                        {
                            if (slots[(int)position.X + i, (int)position.Y + j].Block == null)
                                slots[(int)position.X + i, (int)position.Y + j].Block = blocks[i, j];
                            else if (slots[(int)position.X + i, (int)position.Y + j].Block.Falling)
                                slots[(int)position.X + i, (int)position.Y + j].Block = blocks[i, j];
                        }
                }

                //Update times
                if (times[0] >= stats.FallTimeout) times[0] = 0;
                if (times[1] >= TIME_1) times[1] = 0;
            }
        }

        private void MoveSideways(bool left, BlockSlot[,] slots)
        {
            bool canMove = true;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (blocks[i, j] != null)
                    {
                        int limit;
                        if (left) limit = 0;
                        else limit = GameArea.WIDTH - 1;
                        if (position.X + i == limit)
                            canMove = false;
                        else
                        {
                            int nextSlotPosition;
                            bool nextSlotExists;
                            if (left)
                            {
                                nextSlotPosition = (int)position.X + i - 1;
                                nextSlotExists = nextSlotPosition >= limit;
                            }
                            else
                            {
                                nextSlotPosition = (int)position.X + i + 1;
                                nextSlotExists = nextSlotPosition <= limit;
                            }
                            if (nextSlotExists)
                            {
                                try
                                {
                                    if (slots[nextSlotPosition, (int)position.Y + j].Block != null)
                                        if (!slots[nextSlotPosition, (int)position.Y + j].Block.Falling)
                                            canMove = false;
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
            if (canMove)
            {
                if (left) position.X--;
                else position.X++;
                soundControl.PlayMoveTetrimino();
            }
        }

        private void MoveDown(BlockSlot[,] slots, bool forced)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (blocks[i, j] != null)
                    {
                        if (position.Y + j == GameArea.HEIGHT - 1)
                        {
                            falling = false;
                            soundControl.PlayTouchGround();
                            if (forced) soundControl.PlayDropSoft();
                        }
                        else
                        {
                            if (position.Y + j + 1 >= 0)
                                if (slots[(int)position.X + i, (int)position.Y + j + 1].Block != null)
                                    if (!slots[(int)position.X + i, (int)position.Y + j + 1].Block.Falling)
                                    {
                                        falling = false;
                                        soundControl.PlayTouchGround();
                                        if (forced) soundControl.PlayDropSoft();
                                    }
                        }
                    }
                }
            }
            if (falling)
            {
                position.Y++;
                if (forced) soundControl.PlayMoveTetrimino();
            }
        }

        private void Rotate(bool cw, BlockSlot[,] slots)
        {
            Block[,] newBlocks = new Block[4, 4];
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    if (cw) newBlocks[i, j] = blocks[j, 3 - i];
                    else newBlocks[i, j] = blocks[3 - j, i];
                }

            bool canRotate = true;

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    if (newBlocks[i, j] != null)
                    {
                        if (position.Y + j > GameArea.HEIGHT - 1) canRotate = false;
                        else
                        {
                            int offset = 0;
                            while (position.X + i + offset < 0)
                            {
                                offset++;
                                for (int a = 0; a < 4; a++)
                                    for (int b = 0; b < 4; b++)
                                        if (newBlocks[a, b] != null)
                                            if (slots[(int)position.X + a + offset + 1, (int)position.Y + b].Block != null)
                                                canRotate = false;
                            }
                            if (canRotate)
                            {
                                position.X += offset;
                                offset = 0;
                                while (position.X + i - offset > GameArea.WIDTH - 1)
                                {
                                    offset++;
                                    for (int a = 0; a < 4; a++)
                                        for (int b = 0; b < 4; b++)
                                            if (newBlocks[a, b] != null)
                                                if (slots[(int)position.X + a - offset - 1, (int)position.Y + b].Block != null)
                                                    canRotate = false;
                                }
                                if (canRotate)
                                {
                                    position.X -= offset;

                                    if (position.Y + j >= 0)
                                        if (slots[(int)position.X + i, (int)position.Y + j].Block != null)
                                            canRotate = false;
                                }
                            }
                        }
                    }

            if (canRotate)
            {
                blocks = newBlocks;
                soundControl.PlayMoveTetrimino();
            }
        }

        public static byte[,] GetShape(int type)
        {
            byte[,] shape = new byte[4, 4];

            switch ((int)type)
            {
                case 0:
                    {
                        shape[0, 1] = shape[1, 1] = shape[2, 1] = shape[3, 1] = 1;
                        break;
                    }
                case 1:
                    {
                        shape[1, 1] = shape[1, 2] = shape[2, 2] = shape[3, 2] = 1;
                        break;
                    }
                case 2:
                    {
                        shape[0, 2] = shape[1, 2] = shape[2, 2] = shape[2, 1] = 1;
                        break;
                    }
                case 3:
                    {
                        shape[1, 1] = shape[2, 1] = shape[1, 2] = shape[2, 2] = 1;
                        break;
                    }
                case 4:
                    {
                        shape[2, 1] = shape[3, 1] = shape[1, 2] = shape[2, 2] = 1;
                        break;
                    }
                case 5:
                    {
                        shape[2, 1] = shape[1, 2] = shape[2, 2] = shape[3, 2] = 1;
                        break;
                    }
                case 6:
                    {
                        shape[1, 1] = shape[2, 1] = shape[2, 2] = shape[3, 2] = 1;
                        break;
                    }
            }

            return shape;
        }

        public static Color GetColor(int type)
        {
            switch (type)
            {
                case 0: { return Color.Cyan; }
                case 1: { return Color.Blue; }
                case 2: { return Color.Orange; }
                case 3: { return Color.Yellow; }
                case 4: { return Color.Lime; }
                case 5: { return Color.Magenta; }
                default: { return Color.Red; }
            }
        }

        public static int GenerateType(int? exclude)
        {
            int type;
            Random r = new Random();
            if (exclude == null) type = r.Next(7);
            else do { type = r.Next(7); } while (type == exclude);
            return type;
        }
    }
}
