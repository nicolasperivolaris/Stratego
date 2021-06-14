using System.Drawing;
using System.Windows.Forms;

namespace Stratego.Model
{
    public class ViewWalkableTile : ViewTile
    {
        public ViewWalkableTile() : base()
        {
            BackColor = GetDefaultColor();
        }

        protected override Color GetDefaultColor()
        {
            return Color.Yellow;
        }
    }
}
