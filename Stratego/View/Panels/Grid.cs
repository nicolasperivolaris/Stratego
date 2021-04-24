using Stratego.Model;
using Stratego.Model.Panels;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Stratego.View
{
    public class Grid : TableLayoutPanel
    {
        public Color _focused = Color.Aqua;
        protected Tile _lastClicked;
        public virtual Tile Selected { get; set; }
        private event EventHandler TileClick;

        private bool _selectable = false;

        public bool Selectable
        {
            get { return _selectable; }
            set
            {
                _selectable = value;
                if (!_selectable) Selected = null;
            }
        }

        public Grid(EventHandler tileClickListener)
        {
            this.TileClick = tileClickListener;
        }

        protected void OnTileClick(object sender, ActionEvent e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                if (Selectable) Selected = (Tile)e.Object;
                TileClick(this, e);
            });

        }
    }
}
