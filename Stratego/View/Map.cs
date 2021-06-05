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
        public event EventHandler WaitForPlayerChange;
        public event EventHandler<StringEventArgs> JointDialogSucceed;
        public event EventHandler<TileEventArgs> TileClicked;


        public Map(Player[] players, Grid grid)
        {
            InitializeComponent();
            Players = players;
            Grid = new GridPanel(players, grid)
            {
                Dock = DockStyle.Fill,
                Name = "grid"
            };
            Grid.TileClicked += OnGridTileClicked;
            DekPanel = new DekPanel()
            {
                Dock = DockStyle.Fill,
                Name = "dekPanel"
            };
            DekPanel.TileClicked += OnDekTileClicked;


            Controls.Find("content", false)[0].Controls.Add(Grid);
            Controls.Find("content", false)[0].Controls.Add(DekPanel);
        }

        public void OnChatMessage(object sender, StringEventArgs msg)
        {
            chatBox.Control.BeginInvoke((MethodInvoker)delegate ()
            {
                chatBox.Text = msg.Data;
            });
        }

        private void OnGridTileClicked(object sender, TileEventArgs e)
        {
            if(e.ActionType == ActionType.TileClick
                && DekPanel.SelectedTile != null
                && Grid.SelectedTile is ViewWalkableTile selected) 
            {
                e.ActionType = ActionType.FromDekToGrid;
                e.Object = new Move()
                {
                    From = DekPanel.SelectedTile.Tile,
                    To = Grid.SelectedTile.Tile
                };
                Grid.ResetSelection();
            }
            TileClicked(sender, e);
        }

        private void OnDekTileClicked(object sender, TileEventArgs e)
        {

        }

        private void OnStart()
        {
            //Todo start
        }

        private void PlayerPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerDialog dia = new PlayerDialog(Players[Program.PLAYER]);
            dia.ShowDialog(this);
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
            if (editor.Checked)
            {
                Grid.BackColor = GridPanel.DefaultBackColor;
                foreach (Control control in Grid.Controls)
                {
                    if (control is ViewWalkableTile tile)
                        tile.SetOwnerColor(false);
                }
            }
            else
            {
                Grid.BackColor = Color.LightGreen;
                foreach (Control control in Grid.Controls)
                {
                    if (control is ViewWalkableTile tile)
                        tile.SetOwnerColor(false);
                }
            }

            editor.Checked = !editor.Checked;
            EditorModeChange(sender, e);
        }

        internal void SetModeEditor(bool activated)
        {
            foreach (Control tile in Grid.Controls)
            {
                if (tile is ViewWalkableTile walkTile) walkTile.SetOwnerColor(activated);
            }
        }

        private void WaitForPlayersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = ((ToolStripMenuItem)sender);
            if (item.Checked)
            {
                item.Text = "Wait for player";
            }
            else
            {
                item.Text = "Stop waiting";
            }

            item.Checked = !item.Checked;
            WaitForPlayerChange(sender, e);
        }

        private void JointToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConnectDialog connectDialog = new ConnectDialog();

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (connectDialog.ShowDialog(this) == DialogResult.OK)
                JointDialogSucceed(sender, new StringEventArgs(connectDialog.Controls["addressBox"].Text));

            connectDialog.Dispose();
        }
    }
}
