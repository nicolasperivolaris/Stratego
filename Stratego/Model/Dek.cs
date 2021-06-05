using Stratego.Model.Pieces;
using Stratego.Model.Tiles;
using System;
using System.Collections.Generic;

namespace Stratego.Model
{
    public class Dek : Dictionary<Model.Type, Tile>
    {
        public Dek(PieceFactory factory)
        {
            foreach (var piece in factory.GetNewPiecesSet())
            {
                Tile tile = new Tile(piece.Value, new System.Drawing.Point(0, (int)piece.Key));

                Add(piece.Key, tile);
            }
        }
    }
}
