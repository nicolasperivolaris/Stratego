using Stratego.Controler.Network;
using Stratego.Model;
using System;
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
            Grid = new GridPanel(OnClick, Players)
            {
                Dock = DockStyle.Fill,
                Name = "grid"
            };
            Grid.CreateMap(Properties.Resources.pattern2);
            PlayerAmount = 2;

            DekPanel = new DekPanel(OnClick)
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
        }

        private void OnClick(object sender, EventArgs e)
        {
            if (sender == DekPanel) Grid.Selected = null;
            switch (_statusMode)
            {
                case Mode.Normal:
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

        private void SetOnGrid(Piece piece, Tile tile)
        {
            if (tile.Piece != null) tile.Piece.ToFactory();
            tile.Piece = piece.Player.PieceFactory.CountedInstanceOf(piece.Type);
        }

        private void AddPlayerPieces(Player player)
        {
            DekPanel.AddPlayer(player);
            //ActiveControl = DekPanel.Controls[0]; 
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
                    if (control is WalkableTile && ((WalkableTile)control).Owner == Players[Program.PLAYER])
                        control.BackColor = Color.Green;
                }

                DekPanel.Selectable = true;
            }

            editor.Checked = !editor.Checked;
        }


        #region multiplayer
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

                NetworkManager = new Server(new MoveSerializer().GetSize());
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
                MoveSerializer m = MoveSerializer.TryDeserialize(e.ToString());

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

                this.NetworkManager = new Client(address, new MoveSerializer().GetSize());
                this.NetworkManager.Connect();
                _multiMode = Mode.Client;
                NetworkManager.DataReceived += OnDataReceived;
                NetworkManager.PartnerArrival += OnPartnerArrival;
            }

            connectDialog.Dispose();
        }

        public class MoveSerializer
        {
            public Player Player;
            public Point from = new Point();
            public Point to = new Point();

            public MoveSerializer()
            {
            }

            public int GetSize()
            {
                return Serialize().Length;
            }

            public String Serialize()
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MoveSerializer));
                using (TextWriter tw = new StringWriter())
                {
                    serializer.Serialize(tw, this);
                    return tw.ToString();
                }
            }

            public static MoveSerializer TryDeserialize(String data)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MoveSerializer));

                try
                {
                    using (System.Xml.XmlReader reader = System.Xml.XmlReader.Create(data))
                    {
                        if (serializer.CanDeserialize(reader))
                        {
                            MoveSerializer result = (MoveSerializer)serializer.Deserialize(reader);
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

        #endregion

    }
}
