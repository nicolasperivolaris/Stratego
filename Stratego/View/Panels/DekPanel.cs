using Stratego.Model;
using Stratego.Model.Pieces;
using Stratego.Model.Tiles;
using Stratego.Utils;
using Stratego.View.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Stratego.View
{
    public class DekPanel : ViewGrid
    {
        public List<Dek> Deks = new List<Dek>();
        private ViewTile _SelectedTile;
        public override ViewTile SelectedTile
        {
            get { return _SelectedTile; }
            set
            {
                if (_SelectedTile != null) _SelectedTile.BackColor = _SelectedTile.Tile.Piece.Player.Color;
                if (value != null)
                {
                    _SelectedTile = value;
                    _SelectedTile.BackColor = _focused;
                }
            }
        }

        public DekPanel() : base()
        {
            ColumnCount = 0;
            RowStyles.Clear();
            ColumnStyles.Clear();

            
        }

        public void AddPlayer(Player player)
        {
            if (RowStyles.Count == 0)
            {
                for (int i = 0; i < player.Dek.Count; i++)
                {
                    RowStyles.Add(new RowStyle(SizeType.Percent, 100 / player.Dek.Count));
                }
                RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
            }

            Dek dek = player.Dek;
            player.PieceFactory.AmountChanged += OnAmountChanged;
            int row = 0;
            ColumnCount++;
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            foreach (var iTile in dek)
            {
                int iRow = row % (Enum.GetNames(typeof(Model.Type)).Length - 1);
                Add(GetNewViewTile(iTile.Value), ColumnCount-1, iRow);
                row++;
            }
            Controls.Add(new Label(), 0, 12);
            Deks.Add(dek);
        }

        private ViewTile GetNewViewTile(Tile value)
        {
            ViewWalkableTile tile = new ViewWalkableTile
            {
                Tile = value,
                BackColor = value.Owner.Color
            };
            return tile;
        }

        public void OnAmountChanged(object sender, Piece piece)
        {
            ViewWalkableTile tile = Controls.OfType<ViewWalkableTile>().Single(view => (view.Tile.Piece.Player == piece.Player && view.Tile.Piece.Equals(piece)));
            
            Invoke((MethodInvoker)delegate { tile.UpdateView(); });
        }

        protected override void OnClick(object sender, TileEventArgs eventArgs)
        {
        }
    }
}
