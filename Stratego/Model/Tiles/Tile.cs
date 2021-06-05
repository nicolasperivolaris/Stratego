using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Stratego.Model.Tiles
{
    public class Tile
    {
        private Piece _Piece;
        [XmlIgnoreAttribute]
        public Piece Piece
        {
            get { return _Piece; }
            set 
            { 
                _Piece = value;
                PieceChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        public Player Owner { get; set; }

        public Point Coordinate { get; set; }
        public bool Accessible { get; }

        public event EventHandler PieceChanged;

        public Tile()
        {

        }
        public Tile(bool accessible)
        {
            this.Accessible = accessible;
        }

        public Tile(Piece piece, Point coordinate):this(true)
        {
            Piece = piece;
            Coordinate = coordinate;
        }

        public bool IsEmpty() { return Piece == null; }

        public void Remove()
        {
            Piece.ToFactory();
            Piece = null;
        }
    }
}
