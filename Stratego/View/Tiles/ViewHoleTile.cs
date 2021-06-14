using System.Drawing;

namespace Stratego.Model
{
    public class ViewHoleTile : ViewTile
    {
        public ViewHoleTile() : base()
        {
            this.BackColor = GetDefaultColor();
        }

        protected override Color GetDefaultColor()
        {
            return SystemColors.HotTrack;
        }
    }
}
