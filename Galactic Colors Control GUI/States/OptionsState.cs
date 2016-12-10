﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyMonoGame.GUI;
using System.Threading;

namespace Galactic_Colors_Control_GUI.States
{
    public class OptionsState : State
    {
        private bool locked = false;

        public override void Draw(SpriteBatch spritebatch)
        {
            Game.singleton.background.Draw(spritebatch);
            Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4), Game.singleton.multilang.GetWord("GCC", Game.singleton.config.lang), Game.singleton.GUI.content.GetFont("title"), new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
            if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 75, Game.singleton.ScreenHeight / 2 + 20, 150, 40), Game.singleton.GUI.content.GetBox("Default"), Game.singleton.multilang.IDToLang(Game.singleton.config.lang),Game.singleton.GUI.content.GetFont("basic"), new MyMonoGame.Colors(Color.LightGray, Color.White)))
            {
                Game.singleton.GUI.ResetFocus();
                ChangeLang();
            }
            if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 75, Game.singleton.ScreenHeight / 2 + 70, 150, 40), Game.singleton.GUI.content.GetBox("Default"), Game.singleton.multilang.GetWord("Back", Game.singleton.config.lang),Game.singleton.GUI.content.GetFont("basic"), new MyMonoGame.Colors(Color.LightGray, Color.White)))
            {
                if (!locked)
                {
                    locked = true;
                    Game.singleton.GUI.ResetFocus();
                    Game.singleton.config.Save();
                    new Thread(() =>
                    {
                        while (!Utilities.DoubleTo(ref Game.singleton.background.speedX, 1, 0.1)) { Thread.Sleep(20); }
                        Game.singleton.gameState = new MainMenuState();
                    }).Start();
                }
            }
        }

        public override void Update()
        {
            Game.singleton.background.Update();
        }

        private void ChangeLang()
        {
            if (Game.singleton.config.lang < Game.singleton.multilang.langsCount - 1)
            {
                Game.singleton.config.lang++;
            }
            else
            {
                Game.singleton.config.lang = 0;
            }
        }
    }
}