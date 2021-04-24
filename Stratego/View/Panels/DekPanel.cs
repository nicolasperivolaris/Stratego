using Stratego.Model;
using Stratego.Model.Panels;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Stratego.View
{
    public class DekPanel : Grid
    {
        public Dictionary<Player, Dek> Deks = new Dictionary<Player, Dek>();

        public override Tile Selected
        {
            get { return _lastClicked; }
            set
            {
                if (_lastClicked != null) _lastClicked.BackColor = _lastClicked.Piece.Player.Color;
                if (value != null)
                {
                    _lastClicked = value;
                    _lastClicked.BackColor = _focused;
                }
            }
        }

        public DekPanel(EventHandler TileClickListener) : base(TileClickListener)
        {
            ColumnCount = 1;
            RowStyles.Clear();
            ColumnStyles.Clear();
        }

        public void AddPlayer(Player player)
        {
            Dek dek = new Dek(player, OnTileClick);
            player.PieceFactory.AmountChanged += OnAmountChanged;
            int row = 0;
            ColumnCount++;
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            foreach (var iTile in dek)
            {
                int iRow = row % (Enum.GetNames(typeof(Model.Type)).Length - 1);
                Controls.Add(iTile.Value, player.Number, iRow);

                RowStyles.Add(new RowStyle(SizeType.Percent, 10));
                row++;
            }
            Deks.Add(player, dek);
        }

        public void OnTileClick(object sender, EventArgs e)
        {
            ActionEvent click = new ActionEvent()
            {
                ActionType = ActionType.TileClick,
                Object = (Tile)sender,
                Sender = this
            };

            base.OnTileClick(this, click);
        }

        private void OnAmountChanged(object pieceFactory, Piece piece)
        {
            Deks[piece.Player][piece.Type].Invoke((MethodInvoker)delegate { Deks[piece.Player][piece.Type].Piece = piece; });
        }

        public void UpdateDekWith(Piece piece)
        {
            Deks[piece.Player][piece.Type].Piece = piece;
        }
    }


}
