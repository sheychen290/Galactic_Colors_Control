﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyMonoGame.GUI;
using System.Threading;
using System;
using Galactic_Colors_Control_Common.Protocol;
using Galactic_Colors_Control_Common;
using System.Collections.Generic;

namespace Galactic_Colors_Control_GUI.States
{
    public class PartyState : State
    {
        public struct Party
        {
            public int id;
            public string text;

            public Party(int ID, string TEXT)
            {
                id = ID;
                text = TEXT;
            }
        }

        private string password;
        private int page = 1;
        private List<Party> parties = new List<Party>();
        private Message message;
        private int id = -1;

        private bool locked = false;
        private bool showLoading = false;
        private bool showOKMessage = false;

        public PartyState()
        {
            UpdateParty();
        }

        public override void Draw(SpriteBatch spritebatch)
        {
            Game.singleton.background.Draw(spritebatch);
            Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4), Game.singleton.multilang.Get("GCC", Game.singleton.config.lang), Game.singleton.fonts.title, new MyMonoGame.Colors(Color.White), Manager.textAlign.centerCenter);
            if (showLoading)
            {
                Game.singleton.GUI.Box(new Rectangle(Game.singleton.ScreenWidth / 2 - 150, Game.singleton.ScreenHeight / 4 + 50, 300, 50), Game.singleton.buttonsSprites[0]);
                Game.singleton.GUI.Label(new Rectangle(Game.singleton.ScreenWidth / 2 - 150, Game.singleton.ScreenHeight / 4 + 50, 300, 50), Game.singleton.multilang.Get("Loading", Game.singleton.config.lang), Game.singleton.fonts.basic);
            }
            else
            {
                if (showOKMessage)
                {
                    Game.singleton.GUI.Box(new Rectangle(Game.singleton.ScreenWidth / 2 - 150, Game.singleton.ScreenHeight / 4 + 50, 300, 150), Game.singleton.buttonsSprites[0]);
                    Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4 + 60), message.title, Game.singleton.fonts.basic, null, Manager.textAlign.bottomCenter);
                    Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 4 + 100), message.text, Game.singleton.fonts.small, null, Manager.textAlign.bottomCenter);
                    if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 140, Game.singleton.ScreenHeight / 4 + 150, 280, 40), Game.singleton.buttonsSprites[0], Game.singleton.multilang.Get("OK", Game.singleton.config.lang), Game.singleton.fonts.basic)) { Game.singleton.GUI.ResetFocus(); showOKMessage = false; }
                }
                else
                {
                    Game.singleton.GUI.Box(new Rectangle(Game.singleton.ScreenWidth / 2 - 150, Game.singleton.ScreenHeight / 2 - 300, 300, 600), Game.singleton.buttonsSprites[0]);
                    if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 140, Game.singleton.ScreenHeight / 2 - 290, 100, 40), Game.singleton.buttonsSprites[0] ,Game.singleton.multilang.Get("Update", Game.singleton.config.lang), Game.singleton.fonts.basic, new MyMonoGame.Colors(Color.LightGray, Color.White)))
                    {
                        if (!locked)
                        {
                            locked = true;
                            new Thread(UpdateParty).Start();
                        }
                    }
                    Game.singleton.GUI.TextField(new Rectangle(Game.singleton.ScreenWidth / 2 + 40, Game.singleton.ScreenHeight / 2 - 290, 100, 40), ref password, Game.singleton.fonts.basic, null, Manager.textAlign.centerCenter, Game.singleton.multilang.Get("Password", Game.singleton.config.lang));
                    if (parties.Count > 0) {
                        if (parties.Count > 10) {
                            //TODO page change
                        }
                        for (int i = (page - 1) * 10; i < page * 10 && i < parties.Count; i++)
                        {
                            if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 100, Game.singleton.ScreenHeight / 2 - 250 + i*50, 200, 40), Game.singleton.buttonsSprites[0], parties[i].text, Game.singleton.fonts.basic, new MyMonoGame.Colors(Color.LightGray, Color.White)))
                            {
                                locked = true;
                                id = parties[i].id;
                                new Thread(PartyJoin).Start();
                            }
                        }
                    }
                    else
                    {
                        Game.singleton.GUI.Label(new MyMonoGame.Vector(Game.singleton.ScreenWidth / 2, Game.singleton.ScreenHeight / 2 - 240), Game.singleton.multilang.Get("AnyParty", Game.singleton.config.lang), Game.singleton.fonts.basic, null, Manager.textAlign.centerCenter);
                    }
                    if (Game.singleton.GUI.Button(new Rectangle(Game.singleton.ScreenWidth / 2 - 75, Game.singleton.ScreenHeight / 2 + 250, 150, 40), Game.singleton.buttonsSprites[0], Game.singleton.multilang.Get("Back", Game.singleton.config.lang), Game.singleton.fonts.basic, new MyMonoGame.Colors(Color.LightGray, Color.White)))
                    {
                        if (!locked)
                        {
                            locked = true;
                            Game.singleton.GUI.ResetFocus();
                            Game.singleton.client.ExitHost();
                            new Thread(() =>
                            {
                                while (!Utilities.DoubleTo(ref Game.singleton.background.speedX, 1, 0.1)) { Thread.Sleep(20); }
                                Game.singleton.gameState = new MainMenuState();
                            }).Start();
                        }
                    }
                }
            }
        }

        private void UpdateParty()
        {
            showLoading = true;
            page = 1;
            ResultData res = Game.singleton.client.Request(new string[2] { "party", "list" });
            if (res.type == ResultTypes.OK) {
                parties.Clear();
                foreach (string str in res.result)
                {
                    string[] data = str.Split(new char[1] { ':' }, 2);
                    int id = -1;
                    if (int.TryParse(data[0], out id))
                    {
                        parties.Add(new Party(id, data[1]));
                    }
                }
            }
            else
            {
                parties = new List<Party>();
            }
            showLoading = false;
            locked = false;
        }

        private void PartyJoin()
        {
            showLoading = true;
            if (id != -1)
            {
                string[] request = password != null ? new string[4] { "party", "join", id.ToString() , password } : new string[3] { "party", "join", id.ToString() };
                ResultData res = Game.singleton.client.Request(request);
                if (res.type == ResultTypes.OK)
                {
                    Game.singleton.logger.Write("Join party " + id.ToString(), Logger.logType.info);
                    Game.singleton.gameState = new GameState();
                }
                else
                {
                    Game.singleton.logger.Write("Join error " + res.result, Logger.logType.error);
                    message.title = Game.singleton.multilang.Get("Error", Game.singleton.config.lang);
                    message.text = Common.ArrayToString(res.result);
                    showOKMessage = true;
                }
            }
            showLoading = false;
            locked = false;
        }

        private void PartyCreate()
        {
            //TODO 
        }

        public override void Update()
        {
            Game.singleton.background.Update();
        }
    }
}