namespace Stratego.Model.Pieces
{
    public class Spy : Movable
    {
        public Spy(){ }
        public Spy(Player player) : base(Type.espion, player) { }

        public override bool TryKill(Piece defense)
        {
            if (defense.Type == Type.marechal)
            {
                return true;
            }
            else return base.TryKill(defense);
        }

        public override bool IsPossible(Move move)
        {//spy can move unlimited distance in only one direction
            int x = move.From.Coordinate.X - move.To.Coordinate.X;
            int y = move.From.Coordinate.Y - move.To.Coordinate.Y;

            return (x == 0 || y == 0) && IsFree(move.To);
        }
    }
}
