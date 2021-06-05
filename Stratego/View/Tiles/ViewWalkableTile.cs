using System.Drawing;

namespace Stratego.Model
{
    public class ViewWalkableTile : ViewTile
    {
        public ViewWalkableTile() : base()
        {
            BackColor = Color.Yellow;
        }

        public void SetOwnerColor(bool state)
        {
            if (state && (Tile.Owner != null))
                BackColor = Tile.Owner.Color;
            else BackColor = Color.Yellow;
        }
    }
}
