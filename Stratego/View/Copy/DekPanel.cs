using Stratego.Controler;
using Stratego.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Stratego.View.copy
{
    public class DekPanel : System.Windows.Forms.TableLayoutPanel
    {
        public Dictionary<Player, Dek> Deks = new Dictionary<Player, Dek>();
        public Tile LastSelectedTile { get; set; }

        public DekPanel(int playerCount) : base()
        {
            ColumnCount = playerCount;

            RowStyles.Clear();
            ColumnStyles.Clear();
        }

        public void AddPlayer(Player player)
        {
            Dek dek = new Dek(player, null);
            int row = 0;
            ColumnCount++;
            ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            foreach (var iTile in dek)
            {
                int iRow = row % (Enum.GetNames(typeof(Model.Type)).Length -1);
                iTile.Value.GotFocus += TileSelected;
                Controls.Add(iTile.Value, player.Number, iRow);

                RowStyles.Add(new RowStyle(SizeType.Percent, 10));
                row++;
            }
            Deks.Add(player, dek);

        }

        private void TileSelected(object sender, EventArgs e) => LastSelectedTile = (Tile)sender;


        public void UpdateDekWith(Piece piece)
        {
            Deks[piece.Player][piece.Type].Piece = piece;
        }
    }

    
}
