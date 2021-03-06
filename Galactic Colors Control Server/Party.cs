﻿using System.Collections.Generic;
using System.Net.Sockets;

namespace Galactic_Colors_Control_Server
{
    public class Party
    {
        public string name = "";
        private string password = "";
        public int size = 0;
        public bool open = false;
        private string owner = "";
        public bool isPrivate { get { return password != ""; } }

        public Party(string Name, int Size, string Owner)
        {
            name = Name;
            size = Size;
            owner = Owner;
        }

        public bool IsOwner(string name)
        {
            return owner == name;
        }

        public bool TestPassword(string pass)
        {
            if (isPrivate)
            {
                return (password == pass);
            }
            else
            {
                return true;
            }
        }

        public bool SetPassword(string newPass, string oldPass)
        {
            if (TestPassword(oldPass))
            {
                password = newPass;
                return true;
            }
            else
            {
                return false;
            }
        }

        public int count
        {
            get
            {
                return clients.Count;
            }
        }

        public List<Socket> clients
        {
            get
            {
                List<Socket> list = new List<Socket>();
                foreach (Socket soc in Program.clients.Keys)
                {
                    if (Program.clients[soc].party == this) { list.Add(soc); }
                }
                return list;
            }
        }
    }
}