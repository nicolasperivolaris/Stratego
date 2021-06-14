using System;
using System.Drawing;
using System.Windows.Forms;
using Stratego.Model;
using Stratego.Model.Tiles;
using Stratego.Sockets.Network;
using Stratego.Utils;
using Stratego.View.Tiles;

namespace Stratego.View
{
    public partial class Map : Form
    {
        private readonly Player[] Players;
        public GridPanel Grid { get; }
        public DekPanel DekPanel { get; }


        public event EventHandler EditorModeChange;
        public event EventHandler<StringEventArgs> JointDialogSucceed;
        public event EventHandler<MoveEventArgs> MovedPiece;
        public event EventHandler<StringEventArgs> MessageSent;
        public event EventHandler StartButton;


        public Map(Player[] players, Grid grid)
        {
            InitializeComponent();
            Players = players;
            Grid = new GridPanel(players, grid)
            {
                Dock = DockStyle.Fill,
                Name = "grid"
            };
            Grid.TileClicked += OnTileClicked;
            Grid.MovedPiece += OnMovedPiece;
            DekPanel = new DekPanel()
            {
                Dock = DockStyle.Fill,
                Name = "dekPanel"
            };

            Controls.Find("content", false)[0].Controls.Add(Grid);
            Controls.Find("content", false)[0].Controls.Add(DekPanel);
        }

        private void OnMovedPiece(object sender, MoveEventArgs e)
        {
            MovedPiece(sender, e);
        }

        public void OnMessageReceived(object sender, StringEventArgs msg)
        {
            chatBox.Control.BeginInvoke((MethodInvoker)delegate ()
            {
                chatBox.Text = ((Player)sender).Name + ((Player)sender).Number + " : " + msg.Data;
            });
        }

        private void OnTileClicked(object sender, TileEventArgs e)
        {
            if (DekPanel.Selectable && DekPanel.SelectedTile != null) //so player try to move a piece from the dek to the grid
            {
                MoveEventArgs move = new MoveEventArgs
                {
                    ActionType = ActionType.FromDekToGrid,
                    Action = new Move()
                    {
                        From = DekPanel.SelectedTile.Tile,
                        To = Grid.SelectedTile.Tile
                    },
                    Sender = Players[Program.PLAYER]
                };
                Grid.ResetSelection();
                MovedPiece?.Invoke(sender, move);
            }
        }

        private void PlayerPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerDialog dia = new PlayerDialog(Players[Program.PLAYER]);
            dia.ShowDialog(this);
            foreach (Control c in DekPanel.Controls)
            {
                if (c is ViewTile tile)
                    tile.UpdateView();
            }
        }

        public void SetPlayersAwaiting(bool await)
        {
            if (await)
            {
                toolStripProgressBar = new ToolStripProgressBar
                {
                    Name = "awaiting",
                    Style = ProgressBarStyle.Marquee,
                    Visible = true,
                    Dock = DockStyle.Left
                };
                menuStrip1.Items.Add(toolStripProgressBar);
            }
            else
            {
                menuStrip1.Items.Remove(toolStripProgressBar);
                toolStripProgressBar.Dispose();
            }
        }

        private void EditorModeClick(object sender, EventArgs e)
        {
            ToolStripMenuItem editor = ((ToolStripMenuItem)sender);
            editor.Checked = !editor.Checked;
            EditorModeChange?.Invoke(sender, e);
        }

        internal void SetModeEditor(bool activated)
        {
            foreach (Control tile in Grid.Controls)
            {
                if (tile is ViewWalkableTile walkTile) walkTile.SetOwnerColor(activated);
            }
            Grid.BackColor = activated ? Color.LightGreen : Grid.BackColor = GridPanel.DefaultBackColor;
            DekPanel.Selectable = activated;
            Grid.PiecesCanMove = !activated;
        }

        private void ChatBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '\r')
            {
                MessageSent.Invoke(this, new StringEventArgs(chatBox.Text));
                chatBox.Clear();
            }
        }

        private void ChatBox_Enter(object sender, EventArgs e)
        {
            chatBox.Clear();
        }

        public void ShowConnectionDialog(object sender, EventArgs e)
        {
            ConnectDialog connectDialog = new ConnectDialog();

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (connectDialog.ShowDialog(this) == DialogResult.OK)
                JointDialogSucceed?.Invoke(this, new StringEventArgs(connectDialog.Controls["addressBox"].Text));
            else
            {
                MessageBox.Show("You must be connected to a server to play", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                connectDialog.ShowDialog();
            }
            connectDialog.Dispose();
        }

        public void ShowConnectionError()
        {
            MessageBox.Show("You must be connected");
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartButton.Invoke(this, EventArgs.Empty);
        }
    }
}
