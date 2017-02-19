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
        public enum MenuScreenType
        {
            Title,
            Pause,
            GameOver,
            Settings,
            Credits,
        }

        public class MenuScreen
        {
            private Texture2D texture;
            private SpriteFont largeFont, smallFont;
            private List<MenuOption> options;
            private MenuScreenType type;
            private SoundControl soundControl;
            private string title;
            private int selectedOption;
            private string extraLabel;

            private int SelectedOption
            {
                get { return selectedOption; }
                set
                {
                    options[selectedOption].Selected = false;
                    options[value].Selected = true;
                    selectedOption = value;
                }
            }

            public MenuScreen(MenuScreenType type, string extraLabelData, ContentManager content, GraphicsDevice graphicsDevice, SoundControl soundControl)
            {
                texture = CreateTexture(graphicsDevice);
                largeFont = content.Load<SpriteFont>("large");
                smallFont = content.Load<SpriteFont>("small");
                options = GetOptionList(type, content);

                //set title
                switch (type)
                {
                    case MenuScreenType.Title: { title = "Tetris"; break; }
                    case MenuScreenType.Settings: { title = "Settings"; break; }
                    case MenuScreenType.Credits: { title = "Credits"; break; }
                    case MenuScreenType.Pause: { title = "Pause"; break; }
                    case MenuScreenType.GameOver: { title = "Game Over"; break; }
                }

                //set locations for options
                for (int i = 0; i < options.Count; i++)
                {
                    if (i == 0)
                    {
                        int totalHeight = 0;
                        foreach (MenuOption option in options)
                            totalHeight += option.Height;
                        totalHeight += (options.Count - 1) * 10;
                        options[i].Position = new Vector2(options[i].Position.X, (Game.WINDOW_HEIGHT - totalHeight) / 2);
                    }
                    else
                        options[i].Position = new Vector2(options[i].Position.X, options[i - 1].Position.Y + options[i - 1].Height + 10);
                }

                //extra label
                if (type == MenuScreenType.GameOver)
                {
                    extraLabel += "Score: " + extraLabelData.Substring(0, extraLabelData.IndexOf(' '));
                    extraLabel += "\nLevel: " + extraLabelData.Substring(extraLabelData.IndexOf(' ') + 1);
                }
                else extraLabel = extraLabelData;

                this.type = type;
                this.soundControl = soundControl;
                SelectedOption = 0;
            }

            public void Update(GameControl gameControl, KeyboardState keyState, KeyboardState prevKeyState)
            {
                int selectedOption = SelectedOption;
                if (keyState.IsKeyDown(Keys.Down) && prevKeyState.IsKeyUp(Keys.Down))
                    selectedOption++;
                else if (keyState.IsKeyDown(Keys.Up) && prevKeyState.IsKeyUp(Keys.Up))
                    selectedOption--;
                if (selectedOption == options.Count) selectedOption = 0;
                else if (selectedOption < 0) selectedOption = options.Count - 1;
                if (selectedOption != SelectedOption)
                {
                    SelectedOption = selectedOption;
                    soundControl.PlayOptionHightlight();
                }

                if (keyState.IsKeyDown(Keys.Enter) && prevKeyState.IsKeyUp(Keys.Enter))
                {
                    options[selectedOption].DoAction(gameControl);
                    soundControl.PlayOptionSelect();
                }

                if (title == "Pause" && keyState.IsKeyDown(Keys.Escape) && prevKeyState.IsKeyUp(Keys.Escape))
                    gameControl.ResumeGame();
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(texture, Vector2.Zero, Color.White);
                spriteBatch.DrawString(largeFont, title, new Vector2((texture.Width - largeFont.MeasureString(title).X) / 2, 100), Color.White);
                if (extraLabel != String.Empty)
                    spriteBatch.DrawString(smallFont, extraLabel, new Vector2((texture.Width - smallFont.MeasureString(extraLabel).X) / 2, 110 + largeFont.MeasureString(title).Y), Color.White);
                foreach (MenuOption option in options) option.Draw(spriteBatch);
            }

            private static Texture2D CreateTexture(GraphicsDevice graphicsDevice)
            {
                Texture2D texture = new Texture2D(graphicsDevice, Game.WINDOW_WIDTH, Game.WINDOW_HEIGHT, false, SurfaceFormat.Color);
                Color[] colors = new Color[Game.WINDOW_WIDTH * Game.WINDOW_HEIGHT];
                for (int i = 0; i < colors.Length; i++) colors[i] = Color.Black;
                texture.SetData(colors);
                return texture;
            }

            private static List<MenuOption> GetOptionList(MenuScreenType menuScreenType, ContentManager content)
            {
                List<MenuOption> options = new List<MenuOption>();
                switch (menuScreenType)
                {
                    case MenuScreenType.Title:
                        {
                            options.Add(new MenuOption(MenuOptionType.StartGame, "Start Game", content));
                            options.Add(new MenuOption(MenuOptionType.SettingLink, "Settings", content));
                            options.Add(new MenuOption(MenuOptionType.CreditsLink, "Credits", content));
                            options.Add(new MenuOption(MenuOptionType.ExitApplication, "Exit", content));
                            break;
                        }
                    case MenuScreenType.Settings:
                        {
                            options.Add(new MenuOption(MenuOptionType.SoundToggle, "Sound ", content));
                            options.Add(new MenuOption(MenuOptionType.MusicToggle, "Music ", content));
                            options.Add(new MenuOption(MenuOptionType.FullScreenToggle, "Full Screen ", content));
                            options.Add(new MenuOption(MenuOptionType.ParticlesToggle, "Particles ", content));
                            options.Add(new MenuOption(MenuOptionType.TitleScreenLink, "Back", content));
                            break;
                        }
                    case MenuScreenType.Credits:
                        {
                            options.Add(new MenuOption(MenuOptionType.TitleScreenLink, "Back", content));
                            break;
                        }
                    case MenuScreenType.Pause:
                        {
                            options.Add(new MenuOption(MenuOptionType.ResumeGame, "Resume Game", content));
                            options.Add(new MenuOption(MenuOptionType.StartGame, "Restart Game", content));
                            options.Add(new MenuOption(MenuOptionType.TitleScreenLink, "Back to Menu", content));
                            break;
                        }
                    case MenuScreenType.GameOver:
                        {
                            options.Add(new MenuOption(MenuOptionType.StartGame, "Try Again", content));
                            options.Add(new MenuOption(MenuOptionType.TitleScreenLink, "Back to Menu", content));
                            break;
                        }
                }
                return options;
            }
        }
    }
}
