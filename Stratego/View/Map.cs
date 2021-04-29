using Stratego.Controler.Network;
using Stratego.Model;
using Stratego.Model.Panels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Stratego.View
{
    public partial class Map : Form
    {
        private enum Mode
        {
            Normal, Editor, Run, 
            PlayerAwaiting, Server, Client
        }

        private NetworkManager NetworkManager;
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
            get;set;
        }

        public int PlayerAmount { get; private set; }

        private Mode _statusMode;
        private Mode _multiMode;

        public Map(Player[] players)
        {
            Players = new Dictionary<int, Player>();
            InitializeComponent();
            foreach (Player player in players)
            {
                Players.Add(player.Number, player);
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

        private void OnTileClick(object sender, EventArgs e)
        {
            ActionEvent actionEvent = (ActionEvent)e;
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
            //ActiveControl = DekPanel.Controls[0]; 
        }

        // Important events for other players
        #region actions

        private void SetOnGrid(Piece piece, Tile endTile)
        {
            if (endTile.Piece != null) endTile.Piece.ToFactory();
            endTile.Piece = piece.Player.PieceFactory.CountedInstanceOf(piece.Type);
        }

        private void OnMove(ActionEvent e)
        {
            //Todo send info to clients
            ActionSerializer actionSerializer = new ActionSerializer()
            {
                ActionType = e.ActionType,
                from = ((Move)e.Object).From.Coordinate,
                to = ((Move)e.Object).To.Coordinate
            };
            Send(actionSerializer);
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

        #region Multiplayer management
        /// <summary>
        /// Send an action on the network
        /// </summary>
        /// <param name="serializer"></param>
        private void Send(ActionSerializer serializer)
        {
            NetworkManager.Send("action\n");
            NetworkManager.Send(serializer.Serialize());
        }

        /// <summary>
        /// Send a text message on the network
        /// </summary>
        /// <param name="s">Message</param>
        private void Send(String s)
        {
            NetworkManager.Send(s);
        }
        private void WaitForPlayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_multiMode == Mode.PlayerAwaiting)
            {
                menuStrip1.Items.Remove(toolStripProgressBar);
                toolStripProgressBar.Dispose();
                NetworkManager.CloseConnection();
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

                NetworkManager = new Server(new ActionSerializer().GetSize());
                NetworkManager.Connect();
                NetworkManager.DataReceived += OnDataReceived;
                NetworkManager.PartnerArrival += OnPartnerArrival;

                _multiMode = Mode.PlayerAwaiting;
                ((ToolStripMenuItem)sender).Text = "Stop waiting";
            }
        }

        private void OnPartnerArrival(object sender, EventArgs e)
        {
            var enumerator = Players.Values.GetEnumerator();
            bool newPartnerLinked = false;
            while (enumerator.MoveNext() && !newPartnerLinked)
            {
                if (enumerator.Current.Address == null)
                {
                    enumerator.Current.Address = (IPAddress)sender;
                    newPartnerLinked = true;
                }
            }
        }

        private void OnDataReceived(object sender, EventArgs e)
        {
            IPAddress address = (IPAddress)sender;

            if (e is StringEventArgs)
            {
                ActionSerializer m = ActionSerializer.TryDeserialize(e.ToString());

                if (m == null)
                {
                    chatBox.Control.BeginInvoke((MethodInvoker)delegate ()
                    {
                        chatBox.Text = ((StringEventArgs)e).Data;
                    });
                }
                else
                {
                    if (_statusMode == Mode.Editor)
                    {
                        Player player = Players[m.Player.Number];
                        Piece piece = player.PieceFactory.CountedInstanceOf((Model.Type)m.from.Y);
                        SetOnGrid(piece, (Tile)Grid.GetChildAtPoint(m.to));
                        if (piece != null)
                            DekPanel.UpdateDekWith(piece);

                        DekPanel.Selected.Focus();
                    }
                }
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
                    address = Dns.GetHostAddresses("localhost")[0];
                }

                this.NetworkManager = new Client(address, new ActionSerializer().GetSize());
                this.NetworkManager.Connect();
                _multiMode = Mode.Client;
                NetworkManager.DataReceived += OnDataReceived;
                NetworkManager.PartnerArrival += OnPartnerArrival;
            }

            connectDialog.Dispose();
        }


        #endregion
        public class ActionSerializer
        {
            public Player Player;
            public ActionType ActionType;
            public Point from = new Point();
            public Point to = new Point();

            public ActionSerializer()
            {
            }

            public ActionSerializer(Move move)
            {
                from = move.From.Coordinate;
                to = move.To.Coordinate;
            }

            public int GetSize()
            {
                return Serialize().Length;
            }

            public String Serialize()
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ActionSerializer));
                using (TextWriter tw = new StringWriter())
                {
                    serializer.Serialize(tw, this);
                    return tw.ToString();
                }
            }

            public static ActionSerializer TryDeserialize(String data)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ActionSerializer));

                try
                {
                    using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(data))
                    {
                        if (serializer.CanDeserialize(reader))
                        {
                            ActionSerializer result = (ActionSerializer)serializer.Deserialize(reader);
                            return result;
                        }

                    }
                }
                catch (ArgumentException)
                {
                    Console.WriteLine(data);
                }

                return null;
            }
        }


    }
}
