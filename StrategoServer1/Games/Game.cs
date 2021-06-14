using Stratego.Model;
using Stratego.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StrategoServer.Games
{
    public class Game
    {
        public string Name {get; set;}
        public int MaxPlayers { get; set; }
        public ObservableCollection<Player> Players {get; set;}
        public Tile[] Grid { get; set; }
        public Label[] LabelsGrid { get; set; }

        public Game(int maxPlayers, String name)
        {
            Name = name;
            MaxPlayers = maxPlayers;
            Players = new ObservableCollection<Player>();
        }

        public void Add(Player player)
        {
            Players.Add(player);
        }

        public bool IsFull()
        {
            return Players.Count >= MaxPlayers;
        }
    }
}
