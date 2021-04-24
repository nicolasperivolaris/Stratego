using System.Windows.Forms;

namespace Stratego.Model.Panels
{
    public class ActionEvent : System.EventArgs
    {
        public ActionType ActionType { get; set; }
        public Control Sender { get; set; }
        public Control Object { get; set; }
    }

    public enum ActionType
    {
        EditorMode, TileClick, WaitForPlayer
    }
}
