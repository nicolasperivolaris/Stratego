using System.Windows.Forms;

namespace Stratego.Model.Panels
{
    public class ActionEvent : System.EventArgs
    {
        public ActionType ActionType { get; set; }
        public Control Sender { get; set; }
        public object Object { get; set; }
    }

    public enum ActionType
    {
        NormalMode, EditorMode, WaitForPlayer,
        TileClick, FromDekToGrid, FromGridToDek, Move
    }
}
