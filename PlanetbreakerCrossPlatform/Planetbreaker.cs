using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Planetbreaker.Utilities;
using Planetbreaker.Attacks;
using Planetbreaker.Enemies;


namespace Planetbreaker
{
    public class Planetbreaker : Game
    {
        private enum GameState
        {
            MainMenu,
            Instructions,
            InGame,
            Paused,
            GameOver
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        GameState state;
        int menuSelection = 0;
        Texture2D logo;

        Player player;
        Point playerStartPos;
        ParallaxingBG[] backgrounds = new ParallaxingBG[3];
        List<LivingGameEntity> asteroids = new List<LivingGameEntity>();
        List<Attack> activeAttacks = new List<Attack>();
        List<Enemy> activeEnemies = new List<Enemy>();

        public Planetbreaker()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            state = GameState.MainMenu;


        }

        protected override void Initialize()
        {
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 680;
            graphics.PreferredBackBufferHeight = 1080;
            graphics.ApplyChanges();

            IsFixedTimeStep = true;


            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            backgrounds[0] = new ParallaxingBG(
                Content.Load<Texture2D>("Background/Farthest"),
                graphics.PreferredBackBufferHeight,
                30);
            backgrounds[1] = new ParallaxingBG(
                Content.Load<Texture2D>("Background/Middle"),
                graphics.PreferredBackBufferHeight,
                7);
            backgrounds[2] = new ParallaxingBG(
                Content.Load<Texture2D>("Background/Closest"),
                graphics.PreferredBackBufferHeight,
                1);

            Texture2D playerShip = Content.Load<Texture2D>("Ships/Player");
            playerStartPos = new Point(
                graphics.PreferredBackBufferWidth / 2 - playerShip.Width / 2,
                graphics.PreferredBackBufferHeight - playerShip.Height - 15);
            IHitbox playerHB = new RectHitbox(
                playerStartPos.X, playerStartPos.Y,
                playerShip.Width,
                playerShip.Height);
            player = new Player(playerHB, playerShip, playerShip, playerShip);

            Gunfire.Texture = Content.Load<Texture2D>("Attacks/Bullet");
            PhotonTorpedo.Texture = Content.Load<Texture2D>("Attacks/Torpedo");
            //Missile.Texture = Content.Load<Texture2D>("Attacks/Missile");

            Texture2D bft = Content.Load<Texture2D>("Ships/BasicFighter");
            BasicFighter.SetTextures(bft, bft, bft);

            logo = Content.Load<Texture2D>("Art/Logo");
            font = Content.Load<SpriteFont>("Fonts/Consolas");
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        private int keypressTicker = 1;
        protected override void Update(GameTime gameTime)
        {
            if (keypressTicker > 0) --keypressTicker;

            KeyboardState ks = Keyboard.GetState();

            switch (state)
            {
                case GameState.MainMenu:
                    if (keypressTicker == 0)
                    {
                        if (UpdateMainMenu(ks)) keypressTicker = 2;
                    }
                    break;
                case GameState.Instructions:
                    UpdateInstructions(ks);
                    break;
                case GameState.InGame:
                    UpdateInGame(ks);
                    break;
                case GameState.Paused:
                    if (keypressTicker == 0)
                    {
                        if (UpdatePauseMenu(ks)) keypressTicker = 2;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        private bool UpdateMainMenu(KeyboardState ks)
        {
            if (ks.IsKeyDown(Keys.Enter))
            {
                switch (menuSelection)
                {
                    case 0:
                        // TODO load the level
                        state = GameState.InGame;
                        break;
                    case 1:
                        menuSelection = 0;
                        state = GameState.Instructions;
                        break;
                }
                return true;
            }
            else if (ks.IsKeyDown(Keys.Down))
            {
                if (menuSelection < 1) ++menuSelection;
                return true;
            }
            else if (ks.IsKeyDown(Keys.Up))
            {
                if (menuSelection > 0) --menuSelection;
                return true;
            }
            return false;
        }

        private void UpdateInstructions(KeyboardState ks)
        {
            if (ks.IsKeyDown(Keys.Escape))
            {
                state = GameState.MainMenu;
            }
        }

        Random r = new Random();
        private void UpdateInGame(KeyboardState ks)
        {
            if (ks.IsKeyDown(Keys.Escape))
            {
                state = GameState.Paused;
                return;
            }

            foreach (ParallaxingBG bg in backgrounds)
            {
                bg.Update();
            }

            bool shouldFilterAttacks = false;
            foreach (Attack a in activeAttacks)
            {
                bool shouldFilterEnemies = false;
                foreach (Enemy e in activeEnemies)
                {
                    if (a.CollidesWith(e))
                    {
                        e.Damage(a.DamageType, a.Power);
                        shouldFilterEnemies = true;
                        shouldFilterAttacks = true;
                    }
                }
                if (shouldFilterEnemies) activeEnemies = activeEnemies.Where(e => !e.ShouldDie).ToList();

                bool shouldFilterAsteroids = false;
                foreach (LivingGameEntity e in asteroids)
                {
                    if (a.CollidesWith(e))
                    {
                        e.Damage(a.DamageType, a.Power);
                        shouldFilterAsteroids = true;
                        shouldFilterAttacks = true;
                    }
                }
                if (shouldFilterAsteroids) asteroids = asteroids.Where(ast => !ast.ShouldDie).ToList();

                if (a.CollidesWith(player))
                {
                    player.Damage(a.DamageType, a.Power);
                    if (player.ShouldDie)
                    {
                        state = GameState.GameOver;
                        activeEnemies.Clear();
                        asteroids.Clear();
                        activeAttacks.Clear();
                        player.Area.MoveTo(playerStartPos.X, playerStartPos.Y);
                        return;
                    }
                    shouldFilterAttacks = true;
                }
            }
            if (shouldFilterAttacks) activeAttacks = activeAttacks.Where(a => !a.ShouldDie).ToList();

            if (r.NextDouble() > 0.99)
            {
                activeEnemies.Add(new BasicFighter(
                    new Point(r.Next(graphics.PreferredBackBufferWidth), 0),
                    player.Center()));
            }

            player.Update(ks, graphics.PreferredBackBufferWidth, ref activeAttacks);

            activeAttacks.ForEach(a => a.Update());
            activeEnemies.ForEach(e => e.Update());
        }

        private bool UpdatePauseMenu(KeyboardState ks)
        {
            return false;
        }

        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            switch (state)
            {
                case GameState.MainMenu:
                    DrawMainMenu();
                    break;
                case GameState.Instructions:
                    DrawInstructions();
                    break;
                case GameState.InGame:
                    DrawGame();
                    break;
                case GameState.Paused:
                    DrawPauseMenu();
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawMainMenu()
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Draw(logo, 
                new Vector2(graphics.PreferredBackBufferWidth / 2 - logo.Width / 2, 50),
                Color.White);
            DrawCenteredString("Start", 200, menuSelection == 0);
            DrawCenteredString("Instructions", 230, menuSelection == 1);
        }

        private void DrawInstructions()
        {
            GraphicsDevice.Clear(Color.Black);

            DrawCenteredString("Instructions", 50, true);
            DrawCenteredString("Move:             < >\nMain Cannon:       Z\nPhoton Torpedoes:  X", 100);
            DrawCenteredString("ESC to exit back to main menu...", 300);
        }

        private void DrawCenteredString(String str, int y, bool bright = false)
        {
            spriteBatch.DrawString(font, str,
                new Vector2(graphics.PreferredBackBufferWidth / 2 - font.MeasureString(str).X / 2, y),
                bright ? Color.White : Color.Gray);
        }

        private void DrawGame()
        {
            GraphicsDevice.Clear(Color.Black);

            foreach (ParallaxingBG bg in backgrounds)
            {
                bg.Draw(spriteBatch);
            }
            foreach (Enemy e in activeEnemies)
            {
                e.Draw(spriteBatch);
            }
            player.Draw(spriteBatch);
            activeAttacks.ForEach(a => a.Draw(spriteBatch));
        }

        private void DrawPauseMenu()
        {

        }
    }
}
