namespace Stratego.Model
{
    public class Demineur : Movable
    {
        public Demineur(Player player) : base(Type.demineur, player) { }

        public override bool TryKill(Piece defense)
        {
            if (defense.Type == Type.bomb)
            {
                return true;
            }
            else return base.TryKill(defense);
        }
    }
}
