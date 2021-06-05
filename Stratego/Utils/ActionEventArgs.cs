namespace Stratego.Utils
{
    public class ActionEventArgs : System.EventArgs
    {
        public ActionType ActionType { get; set; }
        public object Sender { get; set; }
        public object Object { get; set; }
    }

    public enum ActionType
    {
        NormalMode, EditorMode, WaitForPlayer,
        TileClick, FromDekToGrid, FromGridToDek, 
        Move
    }
}
