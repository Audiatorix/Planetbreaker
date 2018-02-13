using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Planetbreaker.Utilities;
using Planetbreaker.Attacks;
using Planetbreaker.Enemies;
using PlanetbreakerCrossPlatform;

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
        Menu mainMenu, pauseMenu;
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

            var mainMenuActions = new Dictionary<String, Menu.MenuAction>
            {
                {
                    "Start", () =>
                    {
                        state = GameState.InGame;
                        return true;
                    }
                },
                {
                    "Instructions", () =>
                    {
                        state = GameState.Instructions;
                        return true;
                    }
                },
                {
                    "Exit", () =>
                    {
                        Exit();
                        return false;
                    }
                }
            };
            mainMenu = new Menu(200, graphics.PreferredBackBufferWidth, font, mainMenuActions);

            var pauseMenuActions = new Dictionary<String, Menu.MenuAction>
            {
                {
                    "Resume", () =>
                    {
                        state = GameState.InGame;
                        return true;
                    }
                },
                {
                    "Main Menu", () =>
                    {
                        state = GameState.MainMenu;
                        return true;
                    }
                }
            };
            pauseMenu = new Menu(400, graphics.PreferredBackBufferWidth, font, pauseMenuActions);
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState ks = Keyboard.GetState();

            switch (state)
            {
                case GameState.MainMenu:
                    mainMenu.Update(ks);
                    break;
                case GameState.Instructions:
                    UpdateInstructions(ks);
                    break;
                case GameState.InGame:
                    UpdateInGame(ks);
                    break;
                case GameState.Paused:
                    pauseMenu.Update(ks);
                    break;
            }

            base.Update(gameTime);
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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            switch (state)
            {
                case GameState.MainMenu:
                    spriteBatch.Draw(logo,
                        new Vector2(graphics.PreferredBackBufferWidth / 2 - logo.Width / 2, 50),
                        Color.White);
                    mainMenu.Draw(spriteBatch);
                    break;
                case GameState.Instructions:
                    DrawInstructions();
                    break;
                case GameState.InGame:
                    DrawGame();
                    break;
                case GameState.Paused:
                    pauseMenu.Draw(spriteBatch);
                    break;
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawInstructions()
        {
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
    }
}
