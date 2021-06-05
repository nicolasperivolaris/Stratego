using Stratego.Model.Tiles;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Stratego.Model
{
    public abstract class ViewTile : Button
    {
        private Label powerLl;
        private Label quantityLl;
        private Tile _tile;

        public Tile Tile
        {
            get { return _tile; }
            set { 
                _tile = value;
                value.PieceChanged += OnPieceChanged;
                UpdateView();
            }
        }

        public ViewTile() : base()
        {
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            InitializeComponent();
        }

        private void OnPieceChanged(object sender, EventArgs e)
        {
            UpdateView();
        }

        public void UpdateView()
        {
            if (Tile.Piece != null)
            {
                Text = Tile.Piece.Type.ToString();
                this.powerLl.Text = ((int)Tile.Piece.Type).ToString();
                this.quantityLl.Text = (Tile.Piece.MaxAmount - Tile.Piece.Player.PieceFactory.GetCount(Tile.Piece)).ToString();
                this.powerLl.Show();
            }
            else
            {
                Text = "";
                this.powerLl.Hide();
            }
        }

        public void ShowQuantity(bool b)
        {
            quantityLl.Visible = b;
        }

        private void InitializeComponent()
        {
            this.powerLl = new System.Windows.Forms.Label();
            this.quantityLl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // powerLl
            // 
            this.powerLl.AutoSize = true;
            this.powerLl.BackColor = System.Drawing.Color.Transparent;
            this.powerLl.Dock = System.Windows.Forms.DockStyle.Left;
            this.powerLl.Location = new System.Drawing.Point(0, 0);
            this.powerLl.Name = "powerLl";
            this.powerLl.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.powerLl.Size = new System.Drawing.Size(2, 21);
            this.powerLl.TabIndex = 0;
            this.powerLl.Hide();
            // 
            // quantityLl
            // 
            this.quantityLl.AutoSize = true;
            this.quantityLl.BackColor = System.Drawing.Color.Transparent;
            this.quantityLl.Dock = DockStyle.Right;
            this.quantityLl.Location = new System.Drawing.Point(0, 0);
            this.quantityLl.Name = "quantityLl";
            this.quantityLl.Padding = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.quantityLl.Size = new System.Drawing.Size(2, 21);
            this.quantityLl.TabIndex = 0;
            // 
            // Tile
            // 
            this.Controls.Add(this.powerLl);
            this.Controls.Add(this.quantityLl);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
