using Stratego.Model.Pieces;
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;

namespace Stratego.Model 
{
    public class Player : IEquatable<Player>
    {
        public String Name { get; set; }
        public int Number { get; set; }
        public Color Color { get; set; }
        [XmlIgnoreAttribute]
        public Socket Socket { get; set; }
        [XmlIgnoreAttribute]
        public System.Net.IPAddress Address {get{ return Socket == null ? IPAddress.Loopback: ((IPEndPoint)Socket.RemoteEndPoint).Address; } }
        [XmlIgnoreAttribute]
        public PieceFactory PieceFactory { get; set; }
        [XmlIgnoreAttribute]
        public Dek Dek { get; set; }

        public bool OnLine { get; set; }

        public Player()
        {
            PieceFactory = new PieceFactory(this);
            Dek = new Dek(PieceFactory);
        }

        public override string ToString()
        {
            return Name + " "+ Number + ":" + Address;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Player);
        }

        public bool Equals(Player other)
        {
            return other != null &&
                   Number == other.Number;
        }

        public override int GetHashCode()
        {
            return 187193536 + Number.GetHashCode();
        }
    }
}
