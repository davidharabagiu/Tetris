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
    public class SoundControl
    {
        SoundEffect music, optionHighlight, optionSelect, move, dropSoft, touchGround, dropHard, clear1, clear2, clear3, clear4;
        SoundEffectInstance musicInstance;

        public SoundControl(ContentManager content)
        {
            music = content.Load<SoundEffect>("music");
            optionHighlight = content.Load<SoundEffect>("option_highlight");
            optionSelect = content.Load<SoundEffect>("option_select");
            move = content.Load<SoundEffect>("move");
            dropSoft = content.Load<SoundEffect>("drop_soft");
            touchGround = content.Load<SoundEffect>("touch_ground");
            dropHard = content.Load<SoundEffect>("drop_hard");
            clear1 = content.Load<SoundEffect>("clear1");
            clear2 = content.Load<SoundEffect>("clear2");
            clear3 = content.Load<SoundEffect>("clear3");
            clear4 = content.Load<SoundEffect>("clear4");
            musicInstance = music.CreateInstance();
            musicInstance.Volume = 0.2f;
            musicInstance.IsLooped = true;
        }

        public SoundEffectInstance Music
        {
            get { return musicInstance; }
        }

        public void PlayOptionHightlight()
        {
            if (Settings.Default.Sound)
                optionHighlight.Play();
        }

        public void PlayOptionSelect()
        {
            if (Settings.Default.Sound)
                optionSelect.Play();
        }

        public void PlayMoveTetrimino()
        {
            if (Settings.Default.Sound)
                move.Play();
        }

        public void PlayDropSoft()
        {
            if (Settings.Default.Sound)
                dropSoft.Play();
        }

        public void PlayTouchGround()
        {
            if (Settings.Default.Sound)
                touchGround.Play();
        }

        public void PlayDropHard()
        {
            if (Settings.Default.Sound)
                dropHard.Play();
        }

        public void PlayClear1()
        {
            if (Settings.Default.Sound)
                clear1.Play();
        }

        public void PlayClear2()
        {
            if (Settings.Default.Sound)
                clear2.Play();
        }

        public void PlayClear3()
        {
            if (Settings.Default.Sound)
                clear3.Play();
        }

        public void PlayClear4()
        {
            if (Settings.Default.Sound)
                clear4.Play();
        }

    }
}
