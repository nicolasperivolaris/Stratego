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
        public List<Player> Players {get; set;}
        public Tile[] Grid { get; set; }
        
    }
}
