using Stratego.Network;
using Stratego.Model;
using Stratego.Model.Panels;
using Stratego.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using Stratego.Sockets.Network;

namespace Stratego.View
{
    public partial class Map : Form
    {
        private enum Mode
        {
            Normal, Editor, Run, 
            PlayerAwaiting, Server, Client
        }

        private NetworkController NetworkController;
        private GridPanel Grid
        {
            get; set;
        }
        private DekPanel DekPanel
        {
            get; set;
        }
        public Dictionary<int, Player> Players
        {
            get; private set;
        }

        public Stack<Move> MovesHistory
        {
            get;private set;
        }

        public int PlayerAmount { get; private set; }

        private Mode _statusMode;
        private Mode _multiMode;

        public Map(Player[] Tab)
        {
            Players = new Dictionary<int, Player>();
            InitializeComponent();
            foreach (Player player in Tab)
            {
                if(player != null)Players.Add(player.Number, player);
            }
            Grid = new GridPanel(OnTileClick, Players)
            {
                Dock = DockStyle.Fill,
                Name = "grid"
            };
            Grid.CreateMap(Properties.Resources.pattern2);
            PlayerAmount = 2;

            DekPanel = new DekPanel(OnTileClick)
            {
                Dock = DockStyle.Fill,
                Name = "dekPanel"
            };

            Controls.Find("content", false)[0].Controls.Add(Grid);
            Controls.Find("content", false)[0].Controls.Add(DekPanel);

            _multiMode = _statusMode = Mode.Normal;

            foreach (Player player in Players.Values)
            {
                AddPlayerPieces(player);
            }

            MovesHistory = new Stack<Move>();
        }

        private void OnChatMessage(object sender, StringEventArgs msg)
        {
            chatBox.Control.BeginInvoke((MethodInvoker)delegate ()
            {
                chatBox.Text = msg.Data;
            });
        }

        private void OnTileClick(object sender, EventArgs e)
        {
            ActionEventArgs actionEvent = (ActionEventArgs)e;
            if (sender == DekPanel) Grid.Selected = null;
            switch (_statusMode)
            {
                case Mode.Normal:
                    if (actionEvent.ActionType == ActionType.Move)
                    {
                        OnMove(actionEvent);
                        MovesHistory.Push((Move)actionEvent.Object);
                    }
                    break;
                case Mode.Editor:
                    if (sender == Grid
                    && DekPanel.Selected != null
                    && Grid.Selected is WalkableTile selected
                    && selected.Owner == DekPanel.Selected.Piece.Player)
                    {
                        SetOnGrid(DekPanel.Selected.Piece, Grid.Selected);
                        Grid.Selected = null;
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
        private void AddPlayerPieces(Player player)
        {
            DekPanel.AddPlayer(player);
        }

        // Important events for other players
        #region actions

        private void SetOnGrid(Piece piece, Tile endTile)
        {
            if (endTile.Piece != null) endTile.Piece.ToFactory();
            endTile.Piece = piece.Player.PieceFactory.CountedInstanceOf(piece.Type);
        }

        private void OnMove(ActionEventArgs e)
        {
            //Todo send info to clients
            ActionSerializer actionSerializer = new ActionSerializer()
            {
                ActionType = e.ActionType,
                from = ((Move)e.Object).From.Coordinate,
                to = ((Move)e.Object).To.Coordinate
            };
            NetworkController.Send(actionSerializer);
        }

        private void OnStart()
        {
            //Todo start
        }

        private void EditorModeClick(object sender, EventArgs e)
        {
            ToolStripMenuItem editor = ((ToolStripMenuItem)sender);
            if (_statusMode == Mode.Editor)
            {
                _statusMode = Mode.Normal;
                Grid.BackColor = GridPanel.DefaultBackColor;
                foreach (Control control in Grid.Controls)
                {
                    if (control is WalkableTile tile && tile.Owner == Players[Program.PLAYER])
                        control.BackColor = Color.Yellow;
                }
                DekPanel.Selectable = false;
            }
            else if (_statusMode == Mode.Normal)
            {
                _statusMode = Mode.Editor;
                Grid.BackColor = Color.LightGreen;
                foreach (Control control in Grid.Controls)
                {
                    if (control is WalkableTile tile 
                        && tile.Owner == Players[Program.PLAYER])
                        control.BackColor = Color.Green;
                }

                DekPanel.Selectable = true;
            }

            editor.Checked = !editor.Checked;
        }

        #endregion
        private void WaitForPlayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_multiMode == Mode.PlayerAwaiting)
            {
                menuStrip1.Items.Remove(toolStripProgressBar);
                toolStripProgressBar.Dispose();
                NetworkController.StopWaiting();
                ((ToolStripMenuItem)sender).Text = "Wait for player";
                _multiMode = Mode.Normal;
            }
            else
            {
                toolStripProgressBar = new ToolStripProgressBar
                {
                    Name = "awaiting",
                    Style = ProgressBarStyle.Marquee,
                    Visible = true,
                    Dock = DockStyle.Left
                };
                menuStrip1.Items.Add(toolStripProgressBar);

                NetworkController.StartAsServer();
                NetworkController.Message += OnChatMessage;

                _multiMode = Mode.PlayerAwaiting;
                ((ToolStripMenuItem)sender).Text = "Stop waiting";
            }
        }


        private void JointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectDialog connectDialog = new ConnectDialog();

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (connectDialog.ShowDialog(this) == DialogResult.OK)
            {
                IPAddress address;
                try
                {
                    address = IPAddress.Parse(connectDialog.Controls["addressBox"].Text);
                }
                catch
                {
                    address = IPAddress.Loopback; //default : (debug only)
                }
                NetworkController = new NetworkController();
                NetworkController.StartAsClient(address, Players[Program.PLAYER]);
                NetworkController.PlayerConnection += OnPlayerConnection;
                NetworkController.Message += OnChatMessage;
                NetworkController.Action += OnPartnerAction;
            }

            connectDialog.Dispose();
        }

        private void OnPlayerConnection(object sender, PlayerEventArgs e)
        {
            Player p = new Player
            {
                Color = e.Player.Color,
                Number = e.Player.Number
            };

            Players.Add(Program.ENEMI, p);
            AddPlayerPieces(p);
        }

        private void OnPartnerAction(object sender, ActionEventArgs action)
        {
            if (_statusMode == Mode.Editor)
            {
                Player player = (Player) sender;
                Model.Type pieceType = (Model.Type)((Move)action.Object).From.Coordinate.Y;
                Piece piece = player.PieceFactory.CountedInstanceOf(pieceType);
                SetOnGrid(piece, (Tile)Grid.GetChildAtPoint(((Move)action.Object).To.Coordinate));
                if (piece != null)
                    DekPanel.UpdateDekWith(piece);

                DekPanel.Selected.Focus();
            }
        }

        private void PlayerPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerDialog dia = new PlayerDialog(Players[Program.PLAYER]);
            dia.ShowDialog(this);
        }
    }
}
