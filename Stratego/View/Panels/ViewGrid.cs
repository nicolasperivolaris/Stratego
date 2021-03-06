using Stratego.Model;
using Stratego.Model.Tiles;
using Stratego.Utils;
using Stratego.View.Tiles;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Stratego.View
{
    public abstract class ViewGrid : TableLayoutPanel
    {
        public Color _focused = Color.Aqua;

        public virtual ViewTile SelectedTile { get; set; }
        public event EventHandler<TileEventArgs> TileClicked;
        public String Error;

        private bool _selectable = false;
        
        public bool Selectable
        {
            get { return _selectable; }
            set
            {
                _selectable = value;
                if (!_selectable) ResetSelection();
            }
        }

        public ViewGrid()
        {
        }

        protected void Add(ViewTile tile, int column, int row)
        {
            tile.Click += OnClick;
            Controls.Add(tile, column, row);
        }

        protected void Add(ViewTile tile)
        {
            Add(tile, tile.Tile.Coordinate.Y, tile.Tile.Coordinate.X);
        }

        public void ResetSelection()
        {
            SelectedTile = null;
        }

        protected abstract void OnClick(object sender, TileEventArgs eventArgs);

        protected virtual void OnClick(object sender, EventArgs e)
        {            
            if (sender is ViewWalkableTile tile)
                SelectedTile = tile;

            TileEventArgs eventArgs = new TileEventArgs()
            {
                ActionType = ActionType.TileClick,
                Action = ((ViewTile)sender).Tile,
                Sender = this
            };

            TileClicked?.Invoke(this, eventArgs);
            OnClick(this, eventArgs);
        }
    }
}
