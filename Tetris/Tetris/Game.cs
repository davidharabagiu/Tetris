using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Tetris
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        public const int WINDOW_WIDTH = 482,
            WINDOW_HEIGHT = 608;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameArea gameArea;
        private Stats.Stats stats;
        private GameControl gameControl;
        private SoundControl soundControl;
        private GameUI.MenuScreen activeMenu;
        private KeyboardState keyState, prevKeyState;

        public GameArea GameField
        {
            get { return gameArea; }
            set { gameArea = value; }
        }
        public Stats.Stats Stats
        {
            get { return stats; }
            set { stats = value; }
        }
        public GameUI.MenuScreen ActiveMenu
        {
            get { return activeMenu; }
            set { activeMenu = value; }
        }
        public SoundControl SoundControl
        {
            get { return soundControl; }
        }

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            if (Settings.Default.Fullscreen) graphics.IsFullScreen = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            soundControl = new SoundControl(Content);
            gameControl = new GameControl(this, graphics);
            gameControl.MainMenu();
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            prevKeyState = keyState;
            keyState = Keyboard.GetState();

            if (activeMenu == null)
            {
                gameArea.Update(gameControl, gameTime, keyState, prevKeyState);
                if (keyState.IsKeyDown(Keys.Escape) && prevKeyState.IsKeyUp(Keys.Escape))
                    gameControl.PauseGame();
            }
            else activeMenu.Update(gameControl, keyState, prevKeyState);
            prevKeyState = keyState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();

            if (activeMenu == null)
            {
                gameArea.Draw(spriteBatch);
                stats.Draw(spriteBatch);
            }
            else activeMenu.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
