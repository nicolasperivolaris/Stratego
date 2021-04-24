namespace Stratego.Model
{
    public class HoleTile : Tile
    {
        public HoleTile(int row, int column) : base(row, column)
        {
            this.BackColor = System.Drawing.SystemColors.HotTrack;
        }
    }
}
