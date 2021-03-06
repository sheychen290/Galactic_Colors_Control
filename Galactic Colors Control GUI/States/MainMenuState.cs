﻿using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MyMonoGame.GUI;
using System.Reflection;
using Galactic_Colors_Control;
using System.Threading;
using Galactic_Colors_Control_Common;

namespace Galactic_Colors_Control_GUI.States
{
    public class MainMenuState : State
    {
        public static Texture2D logoSprite;
        private bool locked = false;

        public override void Draw(SpriteBatch spritebatch)
        {
            Game.singleton.background.Draw(spritebatch);
            Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4), Game.singleton.multilang.Get("GCC", Game.singleton.config.lang), Game.singleton.fonts.title, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
            Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4 + 40), Game.singleton.multilang.Get("GUI", Game.singleton.config.lang) + " " + Assembly.GetEntryAssembly().GetName().Version.ToString(), Game.singleton.fonts.basic, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
            if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth - 64, Game.singleton.ScreenHeight - 74, 64, 64), logoSprite)) { System.Diagnostics.Process.Start("https://sheychen.shost.ca/"); }
            if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 75, Game.singleton.ScreenHeight / 2 - 30, 150, 40), Game.singleton.buttonsSprites[0], Game.singleton.multilang.Get("Play", Game.singleton.config.lang), Game.singleton.fonts.basic, new MyMonoGame.Colors(Color.White, Color.Green)))
            {
                if (!locked)
                {
                    locked = true;
                    Game.singleton.GUI.ResetFocus();
                    Game.singleton.client = new Client();
                    new Thread(() =>
                    {
                        while (!Utilities.DoubleTo(ref Game.singleton.background.speedX, 5, 0.1)) { Thread.Sleep(20); }
                        Game.singleton.gameState = new ConnectState();
                    }).Start();
                }
            }
            if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 75, Game.singleton.ScreenHeight / 2 + 20, 150, 40), Game.singleton.buttonsSprites[0], Game.singleton.multilang.Get("Options", Game.singleton.config.lang), Game.singleton.fonts.basic, new MyMonoGame.Colors(Color.White, Color.Blue))) {
                Game.singleton.GUI.ResetFocus();
                Game.singleton.gameState = new OptionsState();
            }
            if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 75, Game.singleton.ScreenHeight / 2 + 70, 150, 40), Game.singleton.buttonsSprites[0], Game.singleton.multilang.Get("Exit", Game.singleton.config.lang), Game.singleton.fonts.basic, new MyMonoGame.Colors(Color.White, Color.Red)))
            {
                if (!locked)
                {
                    locked = true;
                    Game.singleton.GUI.ResetFocus();
                    Game.singleton.logger.Write("Game exit", Logger.logType.warm);
                    Game.singleton.gameState = new TitleState();
                    new Thread(() =>
                    {
                        while (!Utilities.DoubleTo(ref Game.singleton.background.speedX, 0, 0.1)) { Thread.Sleep(50); }
                        Game.singleton.logger.Join();
                        Game.singleton.Exit();
                    }).Start();
                }
            }
        }

        public override void Update()
        {
            Game.singleton.background.Update();
        }
    }
}
