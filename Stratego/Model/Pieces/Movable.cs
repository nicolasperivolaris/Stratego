using Stratego.Model;

namespace Stratego
{
    public class Movable : Piece
    {
        public Movable() { }
        public Movable(Type type, Player player) : base(type, player) { }

        public override bool IsPossible(Move move)
        {
            if (move.Lenght() == 1 && IsFree(move.To))
                return true;
            else return false;

        }
    }
}