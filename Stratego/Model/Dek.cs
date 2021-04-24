using System;
using System.Collections.Generic;

namespace Stratego.Model
{
    public class Dek : Dictionary<Model.Type, Tile>
    {
        public Dek(Player player, EventHandler OnTileClick)
        {
            var set = player.PieceFactory.GetNewPiecesSet();
            foreach (var piece in set)
            {
                Tile tile = new WalkableTile(-1, -1)
                {
                    Piece = piece.Value,
                    BackColor = player.Color
                };

                tile.Click += OnTileClick;

                Add(piece.Key, tile);
            }
        }
    }
}
