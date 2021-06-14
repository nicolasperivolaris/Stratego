using Stratego.Model;
using Stratego.Model.Tiles;
using Stratego.Network;
using Stratego.Sockets.Network;
using Stratego.View;
using Stratego.View.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Type = Stratego.Model.Type;

namespace Stratego.Utils
{
    public class GameControler
    {
        private enum Mode
        {
            Normal, Editor, Run,
            PlayerAwaiting, Server, Client
        }

        private readonly NetworkController NetworkController;
        private readonly Map Map;
        private readonly Grid Grid;
        public Player[] Players
        {
            get; private set;
        }
        public int PlayerQuantity { get; private set; }

        private Mode _statusMode;
        private Mode _multiMode;

        public GameControler(Player[] tab)
        {
            Players = tab;

            Grid = CreateMap(Properties.Resources.pattern2);
            PlayerQuantity = 2;

            _multiMode = _statusMode = Mode.Normal;

            Map = new Map(tab, Grid);
            Map.EditorModeChange += EditorMode;
            Map.WaitForPlayerChange += WaitForPlayer;
            Map.JointDialogSucceed += JointPartner;
            Map.TileClicked += OnTileClick;
            Map.MovedPiece += OnMove;
            Map.MessageSent += OnMessageSent;
            Map.Grid.Selectable = false;
            Map.Grid.Error = "Connect to server first";

            Map.DekPanel.AddPlayer(Players[Program.PLAYER]);

            NetworkController = new NetworkController();
        }

        private void OnMessageSent(object sender, StringEventArgs e)
        {
            NetworkController.Send(e.Data);
        }

        public void Start()
        {
            Application.Run(Map);
        }

        private void AddPlayerPieces(Player player)
        {
            Map.Invoke((MethodInvoker)delegate 
            {
                Map.DekPanel.AddPlayer(player);
            });
        }

        // Important events for other players
        #region actions

        private void OnTileClick(object sender, TileEventArgs actionEvent)
        {
            if (sender == Map.DekPanel) Map.Grid.ResetSelection();
            switch (_statusMode)
            {
                case Mode.Normal:
                    break;
                case Mode.Editor:
                    break;
                case Mode.Run:
                    break;
                case Mode.PlayerAwaiting:
                    break;
                default:
                    break;
            }
        }
        private void OnMove(object sender, MoveEventArgs actionEvent)
        {
            bool validAction = false;
            Move originalMove = (Move)actionEvent.Action.Clone(); //keep copy before changing the grid
            switch (_statusMode)
            {
                case Mode.Normal:
                    if (actionEvent.ActionType == ActionType.Move)
                    {
                        validAction = MovePiece(actionEvent.Action);
                    }
                    break;
                case Mode.Editor:
                    if (sender == Map.Grid
                        && actionEvent.ActionType == ActionType.FromDekToGrid
                        && (actionEvent.Action).From.Piece.Player == Players[Program.PLAYER])
                    {
                        validAction = SetOnGrid(actionEvent.Action);
                    }
                    break;
                case Mode.Run:
                    break;
                case Mode.PlayerAwaiting:
                    break;
                default:
                    break;
            }
            if (validAction)
            {
                actionEvent.Action = originalMove;
                SendToPartner(actionEvent);
            }
        }

        private void SendToPartner(MoveEventArgs move)
        {
            ActionSerializer actionSerializer = new ActionSerializer()
            {
                ActionType = move.ActionType,
                Move = move.Action,
                Player = Players[Program.PLAYER]
            };
            SendToPartner(actionSerializer);
        }

        private void SendToPartner(ActionSerializer action)
        {
            NetworkController.Send(action);
        }

        private bool SetOnGrid(Move move)
        {
            if (move.To.Piece != null) move.To.Piece.ToFactory();
            move.To.Piece = move.From.Piece.Player.PieceFactory.CountedInstanceOf(move.From.Piece.Type);
            return move.To.Piece != null;
        }

        private bool MovePiece(Move move)
        {
            Piece toMove = move.From.Piece;
            if (toMove.IsPossible(move))
            {
                toMove.Move(move.From, move.To);
                return true;
            }
            else
                return false;
        }
        private void SetEditorMode(bool activate)
        {
            if (activate)
            {
                _statusMode = Mode.Editor;
                Map.SetModeEditor(true);
            }
            else
            {
                _statusMode = Mode.Normal;
                Map.SetModeEditor(false);
            }
        }

        #endregion

        #region buttons
        private void EditorMode(object sender, EventArgs e)
        {
            if (_statusMode == Mode.Editor || _statusMode == Mode.Normal)
            {
                SetEditorMode(_statusMode == Mode.Normal);//If normal => editor
                SendToPartner(new ActionSerializer()
                {
                    ActionType = _statusMode == Mode.Editor ? ActionType.EditorMode : ActionType.NormalMode,
                    Player = Players[Program.PLAYER]
                });
            }
        }

        private void WaitForPlayer(object sender, EventArgs e)
        {
            if (_multiMode == Mode.PlayerAwaiting)
            {
                NetworkController.StopWaiting();
                _multiMode = Mode.Normal;
            }
            else
            {
                NetworkController.StartAsServer();
                NetworkController.Message += Map.OnMessageReceived;
                _multiMode = Mode.PlayerAwaiting;
            }
        }


        private void JointPartner(object sender, StringEventArgs e)
        {
            if (NetworkController.Connected)
            {
                MessageBox.Show("Already connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            IPAddress address;
            try
            {
                address = IPAddress.Parse(e.Data);
            }
            catch
            {
                address = IPAddress.Loopback; //default : (debug only)
            }
            try
            {
                NetworkController.StartAsClient(address, Players[Program.PLAYER]);
                Map.Grid.Selectable = true;
            }
            catch (SocketException)
            {
                ErrorDialog dialog = new ErrorDialog();
                dialog.ShowDialog();
            }
            NetworkController.PlayerConnection += OnPlayerConnection;
            NetworkController.Message += Map.OnMessageReceived;
            NetworkController.Action += OnPartnerAction;
            NetworkController.PlayerLeave += OnPlayerLeave;
            NetworkController.PlayerNumberReceived += OnPlayerNumberReceived;
            NetworkController.ConnectionError += OnConnectionError;
        }

        private void OnConnectionError(object sender, EventArgs e)
        {
            MessageBox.Show("Connection error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void OnPlayerNumberReceived(object sender, StringEventArgs e)
        {
            Players[Program.PLAYER].Number = Int32.Parse(e.Data);
        }

        #endregion
        private void OnPlayerLeave(object sender, PlayerEventArgs e)
        {
            MessageBox.Show("Player disconnected... You win !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private void OnPlayerConnection(object sender, PlayerEventArgs e)
        {
            Players[Program.ENEMY].Number = e.Player.Number;
            Players[Program.ENEMY].Name = e.Player.Name;
            AddPlayerPieces(Players[Program.ENEMY]);
        }
        /*
        private void OnPartnerMove()
        {
            Player player = (Player)sender;
            Model.Type pieceType = (Model.Type)action.Move.From.Coordinate.Y;
            Piece piece = player.PieceFactory.CountedInstanceOf(pieceType);
            if (from.Piece != null) from.Piece.ToFactory();
            from.Piece = from.Piece.Player.PieceFactory.CountedInstanceOf(from.Piece.Type);
            if (piece != null)
                Map.DekPanel.OnAmountChanged(this, piece);

            Map.DekPanel.SelectedTile.Focus();
        }*/

        private void OnPartnerAction(object sender, ActionSerializer action)
        {
            switch (action.ActionType)
            {
                case ActionType.NormalMode:
                    SetEditorMode(false);
                    break;
                case ActionType.EditorMode:
                    SetEditorMode(true);
                    break;
                case ActionType.WaitForPlayer:
                    break;
                case ActionType.TileClick:
                    break;
                case ActionType.FromDekToGrid:
                    action.Move.From = action.Player.Dek[(Type)action.Move.From.Coordinate.Y];
                    action.Move.To = GetGridTile(action.Player, action.Move.To.Coordinate);
                    SetOnGrid(action.Move);
                    break;
                case ActionType.FromGridToDek:
                    break;
                case ActionType.Move:
                    break;
                default:
                    break;
            }
        }

        private Tile GetGridTile(Player partner, Point coord)
        {
            if (partner.Equals(Players[Program.ENEMY])) // adapt for all enemies
            {
                int X = Grid.XSize -1 - coord.X;
                int Y = Grid.YSize -1 - coord.Y;
                return Grid.Get(new Point(X, Y));
            }
            return null;
        }

        #region Creation


        private Grid CreateMap(string pattern)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(pattern);
            XmlNode map = doc.DocumentElement;

            Grid grid = new Grid(Players, Int32.Parse(map.Attributes["columns"].Value), Int32.Parse(map.Attributes["rows"].Value));

            Populate(map, grid);
            return grid;
        }

        private void Populate(XmlNode map, Grid grid)
        {
            foreach (XmlNode row in map.ChildNodes)
            {
                if (row.Name != "row") continue;

                int x = Int32.Parse(row.Attributes["i"].Value);

                foreach (XmlNode cell in row.ChildNodes)
                {
                    int y = Int32.Parse(cell.Attributes["j"].Value);
                    int owner = row.Attributes["owner"] != null ? Convert.ToInt32(row.Attributes["owner"].Value) : -1;
                    Tile tile = GetNewTile(cell, x, y, owner);
                    var piece = cell.Attributes["piece"];
                    grid.Set(tile);
                }
            }
        }

        private Tile GetNewTile(XmlNode cell, int x, int y, int owner)
        {
            Tile result;
            Point coordinate = new Point(x, y);
            if (cell.Attributes["accessible"].Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new Tile(true);
                if (owner >= 0)
                    result.Owner = Players[owner];
            }
            else
                result = new Tile(false);

            result.Coordinate = coordinate;

            return result;
        }

        #endregion
    }
}
