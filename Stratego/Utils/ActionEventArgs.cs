using Stratego.Model;
using System.Xml.Serialization;

namespace Stratego.Utils
{
    [XmlInclude(typeof(Piece))]
    [XmlInclude(typeof(Player))]
    [XmlInclude(typeof(Move))]
    public class ActionEventArgs<TAction, TSender> : System.EventArgs
    {
        public ActionType ActionType { get; set; }
        public TSender Sender { get; set; }
        public TAction Action { get; set; }
    }

    public enum ActionType
    {
        NormalMode, EditorMode, WaitForPlayer,
        TileClick, FromDekToGrid, FromGridToDek, 
        Move
    }
}
