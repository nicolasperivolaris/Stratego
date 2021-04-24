using Stratego.Model;

namespace Stratego
{
    public class Movable : Model.Piece
    {
        public Movable(Type type, Player player) : base(type, player) { }

        public override bool IsPossible(Move move)
        {
            if (move.Lenght() == 1)
                return true;
            else return false;

        }
    }
}