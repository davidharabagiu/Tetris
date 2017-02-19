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
    namespace GameUI
    {
        enum MenuOptionType
        {
            StartGame,
            ResumeGame,
            TitleScreenLink,
            SettingLink,
            CreditsLink,
            ExitApplication,
            SoundToggle,
            MusicToggle,
            FullScreenToggle,
            ParticlesToggle
        }

        class MenuOption
        {
            private SpriteFont font;
            private Vector2 position;
            private MenuOptionType type;
            private string text;
            private bool selected, toggle;

            public bool Selected
            {
                get { return selected; }
                set { selected = value; }
            }

            public Vector2 Position
            {
                get { return position; }
                set { position = value; }
            }

            public int Height
            {
                get { return (int)font.MeasureString(text).Y; }
            }

            private string ToggleText
            {
                get
                {
                    string s = String.Empty;
                    if ((int)type > 5)
                    {
                        if (toggle) s = "[ON]";
                        else s = "[OFF]";
                    }
                    return s;
                }
            }

            private bool Setting
            {
                get
                {
                    switch (type)
                    {
                        case MenuOptionType.SoundToggle: { return Settings.Default.Sound; }
                        case MenuOptionType.MusicToggle: { return Settings.Default.Music; }
                        case MenuOptionType.ParticlesToggle: { return Settings.Default.Particles; }
                        case MenuOptionType.FullScreenToggle: { return Settings.Default.Fullscreen; }
                        default: { return false; }
                    }
                }
                set
                {
                    switch (type)
                    {
                        case MenuOptionType.SoundToggle: { Settings.Default.Sound = value; break;  }
                        case MenuOptionType.MusicToggle: { Settings.Default.Music = value; break; }
                        case MenuOptionType.ParticlesToggle: { Settings.Default.Particles = value; break; }
                        case MenuOptionType.FullScreenToggle: { Settings.Default.Fullscreen = value; break; }
                    }
                    Settings.Default.Save();
                }
            }

            public MenuOption(MenuOptionType type, string text, ContentManager content)
            {
                font = content.Load<SpriteFont>("small");
                position = new Vector2();
                this.type = type;
                this.text = text;
                toggle = Setting;
                UpdateX();
            }

            public void DoAction(GameControl gameControl)
            {
                if ((int)type > 5)
                {
                    toggle = !toggle;
                    Setting = toggle;
                    UpdateX();
                }

                switch (type)
                {
                    case MenuOptionType.StartGame:
                        {
                            gameControl.NewGame();
                            break;
                        }
                    case MenuOptionType.TitleScreenLink:
                        {
                            gameControl.MainMenu();
                            break;
                        }
                    case MenuOptionType.ExitApplication:
                        {
                            gameControl.ExitApplication();
                            break;
                        }
                    case MenuOptionType.ResumeGame:
                        {
                            gameControl.ResumeGame();
                            break;
                        }
                    case MenuOptionType.SettingLink:
                        {
                            gameControl.SettingsMenu();
                            break;
                        }
                    case MenuOptionType.FullScreenToggle:
                        {
                            gameControl.ToggleFullscreen(toggle);
                            break;
                        }
                }
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                if (selected)
                    spriteBatch.DrawString(font, "> " + text + ToggleText, new Vector2(position.X - font.MeasureString("> ").X, position.Y), Color.White);
                else
                    spriteBatch.DrawString(font, text + ToggleText, position, Color.White);
            }

            private void UpdateX()
            {
                position.X = (Game.WINDOW_WIDTH - font.MeasureString(text + ToggleText).X) / 2;
            }
        }
    } 
}
