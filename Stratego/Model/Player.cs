using Stratego.Model.Pieces;
using System;
using System.Drawing;

namespace Stratego.Model
{
    public class Player
    {
        public String Name { get; set; }
        public int Number { get; set; }
        public System.Net.IPAddress Address { get; set; }
        public Color Color { get; set; }
        public PieceFactory PieceFactory { get; set; }

        public Player()
        {
            PieceFactory = new PieceFactory(this);
        }
    }
}
