using Stratego.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategoServer.Games
{
    public class Game
    {
        public string Name {get; set;}
        public readonly int MaxPlayer;
        public List<Player> Players {get; set;}
        public Tile[] Grid { get; set; }

        public Game(int maxPlayer, String name)
        {
            Name = name;
            MaxPlayer = maxPlayer;
            Players = new List<Player>();
        }

        public void Add(Player player)
        {
            Players.Add(player);
        }

        public bool AcceptPlayers()
        {
            return Players.Count < MaxPlayer;
        }
    }
}
