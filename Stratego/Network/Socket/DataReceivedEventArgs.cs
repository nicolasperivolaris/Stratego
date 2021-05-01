using Stratego.Model;
using Stratego.Utils;
using System;

namespace Stratego.Sockets.Network
{
    public class StringEventArgs : EventArgs
    {
        public String Data { get; set; }

        public StringEventArgs(String data)
        {
            this.Data = data;
        }

        public override String ToString()
        {
            return Data;
        }
    }

    public class PlayerEventArgs : EventArgs
    {
        public Player Player { get; set; }
        public Flag Flag{ get; set; }

        public PlayerEventArgs(Player player, Flag flag)
        {
            Player = player;
            Flag = flag;
        }
    }
}
