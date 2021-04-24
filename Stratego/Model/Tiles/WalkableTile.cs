using System.Drawing;

namespace Stratego.Model
{
    public class WalkableTile : Tile
    {
        private Player _player;

        ///<summary>Owner Player in editable mode</summary>
        public Player Owner { get; set; }

        public WalkableTile(int row, int column) : base(row, column)
        {
            BackColor = Color.Yellow;
        }

        public Player Player
        {
            get { return _player; }
            set
            {
                _player = value;
                BackColor = Player == null ? Color.Yellow : Player.Color;
            }
        }

    }
}
