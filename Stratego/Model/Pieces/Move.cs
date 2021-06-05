using Stratego.Model.Tiles;
using System;

namespace Stratego.Model
{
    public class Move
    {
        public Tile From { get; set; }

        public Tile To { get; set; }

        public Move() { }

        public int Lenght()
        {
            return Math.Abs(To.Coordinate.X - From.Coordinate.X) + Math.Abs(To.Coordinate.Y - From.Coordinate.Y);
        }
    }
}
