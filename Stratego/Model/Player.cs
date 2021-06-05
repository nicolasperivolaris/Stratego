using Stratego.Model.Pieces;
using System;
using System.Drawing;
using System.Xml.Serialization;

namespace Stratego.Model
{
    public class Player
    {
        public String Name { get; set; }
        public int Number { get; set; }
        public Color Color { get; set; }
        [XmlIgnoreAttribute]
        public System.Net.IPAddress Address { get; set; }
        [XmlIgnoreAttribute]
        public PieceFactory PieceFactory { get; set; }
        [XmlIgnoreAttribute]
        public Dek Dek { get; set; }

        public Player()
        {
            PieceFactory = new PieceFactory(this);
            Dek = new Dek(PieceFactory);
        }
    }
}
