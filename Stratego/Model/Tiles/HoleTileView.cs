namespace Stratego.Model
{
    public class HoleTileView : TileView
    {
        public HoleTileView(int row, int column) : base(row, column)
        {
            this.BackColor = System.Drawing.SystemColors.HotTrack;
        }
    }
}
