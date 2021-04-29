using System;

namespace Stratego.Model
{
    public class Piece
    {
        public String Name { get; internal set; }

        public Player Player { get; internal set; }

        public int MaxAmount { get; internal set; }

        public Type Type { get; private set; }

        internal Piece(Type type, Player player)
        {
            Type = type;
            Player = player;
        }

        public int GetAmount()
        {
            return Player.PieceFactory.GetCount(Type);
        }

        public virtual bool TryKill(Piece defence)
        {
            if (Type > defence.Type)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool IsPossible(Move move)
        {
            return false;
        }

        public void ToFactory()
        {
            Player.PieceFactory.PutPieceBack(this);
        }

        public bool Move(Tile from, Tile to)
        {
            if (from.Piece.IsPossible(new Model.Move() { From = from, To = to }))
            {
                if (to.IsEmpty())
                {
                    to.Piece = from.Piece;
                }
                else
                {
                    if (from.Piece.TryKill(to.Piece))
                    {
                        to.Remove();
                        to.Piece = from.Piece;
                    }
                }
                from.Remove();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
