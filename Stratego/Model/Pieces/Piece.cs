using Stratego.Model.Tiles;
using System;
using System.Collections.Generic;

namespace Stratego.Model
{
    public class Piece : IEquatable<Piece>
    {
        public String Name { get; set; }

        public Player Player { get; set; }

        public int MaxAmount { get; set; }

        public Type Type { get; set; }

        public Piece() { }

        public Piece(Type type, Player player)
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

        public bool IsFree(Tile tile)
        {
            if (tile.Accessible &&
                (tile.Piece == null ||
                !tile.Piece.Player.Equals(Player)))
                return true;
            else return false;
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
            if (IsPossible(new Move() { From = from, To = to }))
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
                from.Piece = null;
                return true;
            }
            else
                return false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Piece);
        }

        public bool Equals(Piece other)
        {
            return other != null &&
                   EqualityComparer<Player>.Default.Equals(Player, other.Player) &&
                   Type == other.Type;
        }

        public override int GetHashCode()
        {
            int hashCode = 969200013;
            hashCode = hashCode * -1521134295 + EqualityComparer<Player>.Default.GetHashCode(Player);
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            return hashCode;
        }
    }

}
