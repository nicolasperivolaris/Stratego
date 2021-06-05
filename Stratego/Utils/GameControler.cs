using Stratego.Model;
using Stratego.Model.Tiles;
using Stratego.Network;
using Stratego.Sockets.Network;
using Stratego.View;
using Stratego.View.Tiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Stratego.Utils
{
    public class GameControler
    {
        private enum Mode
        {
            Normal, Editor, Run,
            PlayerAwaiting, Server, Client
        }

        private NetworkController NetworkController;
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

            foreach (Player player in Players.Where(p => p!=null))
            {
                Map.DekPanel.AddPlayer(player);
            }

            NetworkController = new NetworkController();
        }

        public void Start()
        {
            Application.Run(Map);
        }

        private void AddPlayerPieces(Player player)
        {
            Map.DekPanel.AddPlayer(player);
        }

        // Important events for other players
        #region actions

        private void OnTileClick(object sender, TileEventArgs actionEvent)
        {
            if (sender == Map.DekPanel) Map.Grid.ResetSelection();
            switch (_statusMode)
            {
                case Mode.Normal:
                    if (actionEvent.ActionType == ActionType.Move)
                    {
                        OnMove(actionEvent);
                    }
                    break;
                case Mode.Editor:
                    if (sender == Map.Grid 
                        && actionEvent.ActionType == ActionType.FromDekToGrid
                        && ((Move)actionEvent.Object).From.Piece.Player == Players[Program.PLAYER])
                    {
                        Move move = (Move)actionEvent.Object;
                        SetOnGrid(move.From.Piece, move.To);

                    }
                    break;
                case Mode.Run:
                    break;
                case Mode.PlayerAwaiting:
                    break;
                default:
                    break;
            }
        }

        private void SetOnGrid(Piece piece, Tile tile)
        {
            if (tile.Piece != null) tile.Piece.ToFactory();
            tile.Piece = piece.Player.PieceFactory.CountedInstanceOf(piece.Type);
        }

        private void OnMove(ActionEventArgs e)
        {
            ActionSerializer actionSerializer = new ActionSerializer()
            {
                ActionType = e.ActionType,
                Move = (Move)e.Object,
                Player = Players[Program.PLAYER]
            };
            NetworkController.Send(actionSerializer);
        }

        #endregion

        #region buttons
        private void EditorMode(object sender, EventArgs e)
        {
            if (_statusMode == Mode.Editor)
            {
                _statusMode = Mode.Normal;
                Map.DekPanel.Selectable = false;
            }
            else if (_statusMode == Mode.Normal)
            {
                _statusMode = Mode.Editor;
                Map.DekPanel.Selectable = true;
                Map.SetModeEditor(true);
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
                NetworkController.Message += Map.OnChatMessage;
                _multiMode = Mode.PlayerAwaiting;
            }
        }


        private void JointPartner(object sender, StringEventArgs e)
        {
            IPAddress address;
            try
            {
                address = IPAddress.Parse(e.Data);
            }
            catch
            {
                address = IPAddress.Loopback; //default : (debug only)
            }
            NetworkController.StartAsClient(address, Players[Program.PLAYER]);
            NetworkController.PlayerConnection += OnPlayerConnection;
            NetworkController.Message += Map.OnChatMessage;
            NetworkController.Action += OnPartnerAction;
        }

        #endregion

        private void OnPlayerConnection(object sender, PlayerEventArgs e)
        {
            Player p = new Player
            {
                Color = e.Player.Color,
                Number = e.Player.Number
            };

            Players[Program.ENEMI] = p;
            AddPlayerPieces(p);
        }

        private void OnPartnerAction(object sender, ActionEventArgs action)
        {
            if (_statusMode == Mode.Editor)
            {
                Player player = (Player)sender;
                Model.Type pieceType = (Model.Type)((Move)action.Object).From.Coordinate.Y;
                Piece piece = player.PieceFactory.CountedInstanceOf(pieceType);
                SetOnGrid(piece, ((Move)action.Object).To);
                if (piece != null)
                    Map.DekPanel.OnAmountChanged(this, piece);

                Map.DekPanel.SelectedTile.Focus();
            }
        }

        #region Creation
        

        private Grid CreateMap(string pattern)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(pattern);
            XmlNode map = doc.DocumentElement;

            Grid grid = new Grid(Players,Int32.Parse(map.Attributes["columns"].Value),Int32.Parse(map.Attributes["rows"].Value));

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
