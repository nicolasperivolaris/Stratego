using Stratego.Model;
using Stratego.Model.Pieces;
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
            Normal, Editor, Run, WaitTurn, Finished,
            StartAwaiting, OtherPlayerWaitForStart
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
            Map.JointDialogSucceed += JointPartner;
            Map.Grid.TileClicked += OnTileClick;
            Map.DekPanel.TileClicked += OnTileClick;
            Map.MovedPiece += OnMove;
            Map.MessageSent += OnMessageSent;
            Map.StartButton += TryStart;

            Map.Grid.Selectable = false;
            Map.DekPanel.AddPlayer(GetPlayer());

            GetPlayer().PieceFactory.AmountChanged += CheckFlagTaken;

            NetworkController = new NetworkController(Players);
        }

        private void EndGame(bool Victory)
        {
            if(Victory)
            {
                MessageBox.Show("You've defeated him !", "Win", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                _statusMode = _statusMode = Mode.Finished;
            }
            else
            {
                MessageBox.Show("Your flag is taken", "Defeat", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                _statusMode = _statusMode = Mode.Finished;
            }
        }

        

        public void Start()
        {
            Application.Run(Map);
        }

        private void StartBattle()
        {
            SetEditorMode(false);

            Map.OnMessageReceived(GetEnemy(), new StringEventArgs("Open the fire !"));
            _statusMode = Mode.Run;
        }

        private void AddPlayerPieces(Player player)
        {
            Map.Invoke((MethodInvoker)delegate
            {
                Map.DekPanel.AddPlayer(player);
            });
            GetEnemy().PieceFactory.AmountChanged += CheckFlagTaken;
        }



        private Player GetEnemy()
        {
            return Players[Program.ENEMY];
        }



        private Tile GetGridTile(Player partner, Point coord)
        {
            if (partner.Equals(GetEnemy())) // adapt for all enemies
            {
                int X = Grid.XSize - 1 - coord.X;
                int Y = Grid.YSize - 1 - coord.Y;
                return Grid.Get(new Point(X, Y));
            }
            return null;
        }

        private Player GetPlayer()
        {
            return Players[Program.PLAYER];
        }

        private void SendToPartner(MoveEventArgs move)
        {
            ActionSerializer actionSerializer = new ActionSerializer()
            {
                ActionType = move.ActionType,
                Move = move.Action,
                Player = GetPlayer()
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
            if (move.To.Owner.Equals(move.From.Piece.Player))
                move.To.Piece = move.From.Piece.Player.PieceFactory.CountedInstanceOf(move.From.Piece.Type);
            return move.To.Piece != null;
        }

        private void HideEnemyZone(bool activate)
        {
            foreach (Control c in Map.Grid.Controls)
            {
                if (c is ViewWalkableTile tile && tile.Tile.Owner == GetEnemy())
                {
                    if (activate) tile.HideContent(true);
                    else if (tile.Tile.Piece == null || tile.Tile.Piece.Player != GetEnemy()) tile.HideContent(false);//unhide all except enemy pieces
                }
            }
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
                HideEnemyZone(true);
            }
            else
            {
                _statusMode = Mode.Normal;
                Map.SetModeEditor(false);
                HideEnemyZone(false);
            }
        }

        #region buttons
        private void TryStart(object sender, EventArgs e)
        {
            ActionSerializer action = new ActionSerializer
            {
                ActionType = ActionType.StartRequired
            };
            NetworkController.Send(action);
            if (_multiMode != Mode.OtherPlayerWaitForStart)
            {
                _statusMode = Mode.StartAwaiting;
                _multiMode = Mode.WaitTurn;
                Map.Grid.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                StartBattle();
                _multiMode = Mode.WaitTurn;
                Map.Grid.BackColor = System.Drawing.Color.Red;
            }
        }

        private void OnMessageSent(object sender, StringEventArgs e)
        {
            NetworkController.Send(e.Data);
        }

        private void EditorMode(object sender, EventArgs e)
        {
            if (NetworkController.IsDisconnected())
            {
                Map.ShowConnectionError();
                return;
            }

            if (_statusMode == Mode.Editor || _statusMode == Mode.Normal)
            {
                SetEditorMode(_statusMode == Mode.Normal);//If normal => editor
                SendToPartner(new ActionSerializer()
                {
                    ActionType = _statusMode == Mode.Editor ? ActionType.EditorMode : ActionType.NormalMode,
                    Player = GetPlayer()
                });
            }
        }


        private void JointPartner(object sender, StringEventArgs e)
        {
            if (NetworkController.IsConnected())
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
                NetworkController.StartAsClient(address, GetPlayer());
                Map.Grid.Selectable = true;
            }
            catch (SocketException)
            {
                ErrorDialog dialog = new ErrorDialog();
                dialog.ShowDialog();
                Map.ShowConnectionDialog(this, EventArgs.Empty);
            }
            NetworkController.PlayerConnection += OnPlayerConnection;
            NetworkController.Message += Map.OnMessageReceived;
            NetworkController.Action += OnPartnerAction;
            NetworkController.PlayerLeave += OnPlayerLeave;
            NetworkController.PlayerNumberReceived += OnPlayerNumberReceived;
            NetworkController.ConnectionError += OnConnectionError;
        }


        #endregion

        #region events

        private void CheckFlagTaken(object sender, Piece piece)
        {
            if (piece.Type == Type.drapeau &&
                (_statusMode == Mode.Run ||
                _statusMode == Mode.WaitTurn))
            {
                if (piece.Player.Equals(GetPlayer())) EndGame(false);
                else EndGame(true);
            }
        }

        private void OnConnectionError(object sender, EventArgs e)
        {
            MessageBox.Show("Connection error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void OnPlayerNumberReceived(object sender, StringEventArgs e)
        {
            GetPlayer().Number = Int32.Parse(e.Data);
        }
        private void OnPlayerLeave(object sender, PlayerEventArgs e)
        {
            MessageBox.Show("Player disconnected... You win !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            _statusMode = Mode.Finished;
        }
        private void OnPlayerConnection(object sender, PlayerEventArgs e)
        {
            GetEnemy().Number = e.Player.Number;
            GetEnemy().Name = e.Player.Name;
            AddPlayerPieces(GetEnemy());
        } 
        
        private void OnPartnerAction(object sender, ActionSerializer action)
        {
            Player partner = (Player)sender;
            switch (action.ActionType)
            {
                case ActionType.NormalMode:
                    SetEditorMode(false);
                    break;
                case ActionType.EditorMode:
                    SetEditorMode(true);
                    break;
                case ActionType.StartRequired:
                    if (_statusMode != Mode.StartAwaiting)
                    {
                        _multiMode = Mode.OtherPlayerWaitForStart;
                        Map.OnMessageReceived(sender, new StringEventArgs("Want to start..."));
                    }
                    else
                    {
                        StartBattle();
                        _multiMode = Mode.Run;
                        Map.Grid.BackColor = System.Drawing.Color.LightSeaGreen;
                    }
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
                    Move move = new Move
                    {
                        From = GetGridTile(action.Player, action.Move.From.Coordinate),
                        To = GetGridTile(action.Player, action.Move.To.Coordinate)
                    };
                    Map.Grid.GetViewTile(move.From.Coordinate).HideContent(false);
                    MovePiece(move);
                    if (Grid.Get(move.To.Coordinate).Piece.Player != GetPlayer())
                        Map.Grid.GetViewTile(move.To.Coordinate).HideContent(true);
                    if (_statusMode != Mode.Finished)
                    {
                        _statusMode = _multiMode = Mode.Run;
                        Map.Grid.BackColor = System.Drawing.Color.LightSeaGreen;
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnTileClick(object sender, TileEventArgs actionEvent)
        {
            if (NetworkController.IsDisconnected())
            {
                Map.ShowConnectionError();
                Map.ShowConnectionDialog(this, EventArgs.Empty);
            }
            if (GetEnemy().Socket == null)
                MessageBox.Show("Wait for another player", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (sender == Map.DekPanel) Map.Grid.ResetSelection();
            switch (_statusMode)
            {
                case Mode.Normal:
                    MessageBox.Show("Place your pieces by clicking \"Set up army\" or select start", "Advice", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Mode.Editor:
                    break;
                case Mode.Finished:
                    MessageBox.Show("Restart the game.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Mode.StartAwaiting:
                    MessageBox.Show("Wait for the start of the game.", "Advice", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                default:
                    break;
            }
            switch (_multiMode)
            {
                case Mode.WaitTurn:
                    MessageBox.Show("Wait, the other player has to play.", "Advice", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case Mode.OtherPlayerWaitForStart:
                    MessageBox.Show("Hurry up, the other player wants to play.", "Advice", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                case Mode.Run:
                    if (actionEvent.ActionType == ActionType.Move
                        && actionEvent.Action.From.Piece.Player == GetPlayer())
                    {
                        validAction = MovePiece(actionEvent.Action);
                        if (validAction)
                        {
                            Map.Grid.BackColor = System.Drawing.Color.Red;
                            _statusMode = Mode.WaitTurn;

                            if (validAction && Grid.Get(actionEvent.Action.To.Coordinate).Piece.Player.Equals(GetPlayer())) //unhide if the tile was hiden
                                Map.Grid.GetViewTile(actionEvent.Action.To.Coordinate).HideContent(false);
                        }
                    }
                    break;
                case Mode.Editor:
                    if (sender == Map.Grid
                        && actionEvent.ActionType == ActionType.FromDekToGrid
                        && (actionEvent.Action).From.Piece.Player == GetPlayer())
                    {
                        validAction = SetOnGrid(actionEvent.Action);
                    }
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

        #endregion

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
