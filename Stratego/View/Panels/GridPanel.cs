using Stratego.Model;
using Stratego.Model.Tiles;
using Stratego.Utils;
using Stratego.View.Tiles;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace Stratego.View
{
    public class GridPanel : ViewGrid
    {
        public Player[] Players { get; }
        public Grid Grid { get; set; }

        private Move CurrentMove = new Move();
        public bool PiecesCanMove { get; set; }

        public event EventHandler<MoveEventArgs> MovedPiece;

        public GridPanel() { }

        public GridPanel(Player[] players, Grid grid) : base()
        {
            Players = players;
            Grid = grid;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
             ControlStyles.OptimizedDoubleBuffer |
             ControlStyles.UserPaint, true);
            Selectable = true;

            int sideLength = (int)Math.Sqrt(grid.TileGrid.Length);
            ColumnCount = sideLength;
            RowCount = sideLength;

            RowStyles.Clear();
            ColumnStyles.Clear();
            Controls.Clear();
            for (int i = 0; i < sideLength; i++)
            {
                RowStyles.Add(new RowStyle(SizeType.Percent, 100/RowCount));
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / ColumnCount));
            }

            foreach (var tile in grid.TileGrid)
            {
                Add(GetNewGridTile(tile));
            }

            RowStyles.Add(new RowStyle(SizeType.Absolute, 0));
            Controls.Add(new Label());
        }


        private ViewTile GetNewGridTile(Tile tile)
        {
            ViewTile result;
            if (tile.Accessible)
            {
                result = new ViewWalkableTile();
                result.ShowQuantity(false);
            }
            else
                result = new ViewHoleTile();

            result.Tile = tile;
            return result;
        }


        protected override void OnClick(object sender, TileEventArgs e)
        {
            Tile clicked = e.Action;
            if (CurrentMove.From?.Piece == null) //is there already a tile selected to begin a move ?
            {
                if(!clicked.IsEmpty()) CurrentMove.From = clicked ;
            }
            else
            {
                if (PiecesCanMove)
                {
                    CurrentMove.To = clicked;
                    MoveEventArgs move = new MoveEventArgs();
                    move.ActionType = ActionType.Move;
                    move.Action = CurrentMove;
                    move.Sender = Players[Program.PLAYER];
                    MovedPiece?.Invoke(this, move);
                    CurrentMove.From = CurrentMove.To; // keep the last tile selected as begin of the next move
                }
            }
        }
    }
}
