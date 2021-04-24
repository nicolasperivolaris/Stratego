using Stratego.Controler;
using Stratego.Controler.Network;
using Stratego.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Stratego.View.copy
{
    public partial class Map : Form
    {
        private NetworkManager NetworkManager;
        private GridPanel Grid { get; set; }
        private DekPanel DekPanel { get; set; }
        public Dictionary<int, Player> Players { get; private set; }

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
            Grid = new GridPanel(ActionTileClick)
            {
                Dock = DockStyle.Fill,
                Name = "grid"
            };
            Grid.Enter += Panel_Enter;

            DekPanel = new DekPanel(players.Length)
            {
                Dock = DockStyle.Fill,
                Name = "dekPanel"
            };
            DekPanel.Enter += Panel_Enter;

            Controls.Find("content", false)[0].Controls.Add(Grid);
            Controls.Find("content", false)[0].Controls.Add(DekPanel);

            _multiMode = _statusMode = Mode.Normal;
            
            foreach (Player player in Players.Values)
            {
                AddPlayerPieces(player);
            }
        }

        private void Panel_Enter(object sender, EventArgs e)
        {
            Tile selectedTile = null;
            foreach (Control tile in ((Control)sender).Controls)
            {
                if (tile.Focused) selectedTile = (Tile)tile;
            }
            if (selectedTile != null)
            {
                ActionGotFocus(sender, selectedTile);
            }
        }

        private void ActionGotFocus(object sender, Tile focusedTile)
        {
            if (_statusMode == Mode.Editor)
            {
                if (sender == Grid)
                {
                    if (DekPanel.LastSelectedTile != null
                        && focusedTile is WalkableTile
                        && ((WalkableTile)focusedTile).Owner)
                    {
                        if (!focusedTile.IsEmpty())
                        {
                            Players[Program.PLAYER].PieceFactory.PutPieceBack(focusedTile.Piece);
                            DekPanel.UpdateDekWith(focusedTile.Piece);
                            focusedTile.Piece = null;
                        }
                        Piece piece = Players[Program.PLAYER].PieceFactory.CountedInstanceOf(DekPanel.LastSelectedTile.Piece.Type);
                        if (piece != null)
                        {
                            SetOnGrid(piece);
                            DekPanel.UpdateDekWith(piece);

                        }
                        DekPanel.LastSelectedTile.Focus();
                    }
                }

                if (sender == DekPanel) ;

            }
        }

        public void SetOnGrid(Piece piece)
        {
            Grid.LastSelectedTile.Piece = piece;
        }

        public void AddPlayerPieces(Player player)
        {
            DekPanel.AddPlayer(player);
            ActiveControl = DekPanel.Controls[0];
        }

        private void ActionTileClick(object senderTile, EventArgs e)
        {
            if (((Tile)senderTile).Parent == Grid && (_statusMode == Mode.Normal || _statusMode == Mode.Run))
            {
                if (senderTile == Grid.LastSelectedTile)
                    return;

                Tile last = Grid.LastSelectedTile;

                if (last != null && last.IsEmpty())
                {
                    return;
                }

                if (last != null)
                {// a tile was selected       
                    Move move = new Move
                    {
                        From = last,
                        To = ((Tile)senderTile)
                    };

                    if (!last.IsEmpty()
                        && last.Piece is Movable
                        && ((Movable)last.Piece).IsPossible(move))
                    {// there is a piece to move on the last tile
                        Grid.MovePiece(last, ((Tile)senderTile)); //make the move
                    }
                }
            }
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
                    if (control is WalkableTile && ((WalkableTile)control).Owner)
                        control.BackColor = Color.Yellow;
                }
            }
            else if (_statusMode == Mode.Normal)
            {
                _statusMode = Mode.Editor;
                Grid.BackColor = Color.LightGreen;
                foreach (Control control in Grid.Controls)
                {
                    if (control is WalkableTile && ((WalkableTile)control).Owner)
                        control.BackColor = Color.Green;
                }
            }

            editor.Checked = !editor.Checked;
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
            Player player = new Player()
            {
                Name = "Player " + Players.Count,
                Number = Players.Count,
                Address = (IPAddress)sender,
                Color = Color.IndianRed
            };
            Players.Add(player.Number, player);
            BeginInvoke((MethodInvoker)delegate ()
           {
                AddPlayerPieces(player);
           });


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
                        SetOnGrid(piece);
                        if (piece != null)
                            DekPanel.UpdateDekWith(piece);

                        DekPanel.LastSelectedTile.Focus();
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
                NetworkManager.DataReceived += OnDataReceived;
                NetworkManager.PartnerArrival += OnPartnerArrival;
            }

            connectDialog.Dispose();
        }

        public enum Mode
        {
            Normal, Editor, Run, PlayerAwaiting
        }

        public class MoveSerializer
        {
            public Player Player;
            public Point from = new Point();
            public Point to = new Point();

            public MoveSerializer() { }

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

    }
}
