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
    public class GameControl
    {
        private Game game;
        private GraphicsDeviceManager graphics;

        public GraphicsDevice graphicsDevice
        {
            get { return game.GraphicsDevice; }
        }

        public GameControl(Game game, GraphicsDeviceManager graphics)
        {
            this.game = game;
            this.graphics = graphics;
        }

        public void NewGame()
        {
            game.Stats = new Stats.Stats(game.Content);
            game.GameField = new GameArea(game.Stats, game.Content, game.SoundControl);
            game.ActiveMenu = null;
            if (Settings.Default.Music) game.SoundControl.Music.Play();
        }

        public void MainMenu()
        {
            game.ActiveMenu = new GameUI.MenuScreen(GameUI.MenuScreenType.Title, String.Empty, game.Content, game.GraphicsDevice, game.SoundControl);
            if (Settings.Default.Music) game.SoundControl.Music.Stop();
        }

        public void ResumeGame()
        {
            game.ActiveMenu = null;
            if (Settings.Default.Music) game.SoundControl.Music.Pause();
            game.SoundControl.PlayOptionSelect();
        }

        public void SettingsMenu()
        {
            game.ActiveMenu = new GameUI.MenuScreen(GameUI.MenuScreenType.Settings, String.Empty, game.Content, game.GraphicsDevice, game.SoundControl);
        }

        public void PauseGame()
        {
            game.ActiveMenu = new GameUI.MenuScreen(GameUI.MenuScreenType.Pause, String.Empty, game.Content, game.GraphicsDevice, game.SoundControl);
            if (Settings.Default.Music) game.SoundControl.Music.Pause();
            game.SoundControl.PlayOptionSelect();
        }

        public void GameOver()
        {
            game.ActiveMenu = new GameUI.MenuScreen(GameUI.MenuScreenType.GameOver, game.Stats.Score.ToString() + " " + game.Stats.Level, game.Content, game.GraphicsDevice, game.SoundControl);
            if (Settings.Default.Music) game.SoundControl.Music.Stop();
            game.SoundControl.PlayOptionSelect();
        }

        public void ToggleFullscreen(bool fullScreen)
        {
            graphics.IsFullScreen = fullScreen;
            graphics.ApplyChanges();
        }

        public void ExitApplication()
        {
            game.Exit();
        }
    }
}
