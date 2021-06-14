using Stratego.Model.Pieces;
using Stratego.Model.Tiles;
using System;
using System.Xml.Serialization;

namespace Stratego.Model
{

    [XmlInclude(typeof(Movable))]
    [XmlInclude(typeof(Spy))]
    [XmlInclude(typeof(Demineur))]
    public class Move : ICloneable
    {
        public Tile From { get; set; }

        public Tile To { get; set; }

        public Move() { }

        public int Lenght()
        {
            return Math.Abs(To.Coordinate.X - From.Coordinate.X) + Math.Abs(To.Coordinate.Y - From.Coordinate.Y);
        }

        public object Clone()
        {
            return new Move
            {
                From = (Tile)this.From.Clone(),
                To = (Tile)this.To.Clone()
            };
        }
    }
}
