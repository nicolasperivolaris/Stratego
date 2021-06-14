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
        private bool hiden;

        public Tile Tile
        {
            get { return _tile; }
            set
            {
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
            Invoke((MethodInvoker)delegate ()
            {
                UpdateView();
            });
        }

        protected abstract Color GetDefaultColor();

        public void SetOwnerColor(bool state)
        {
            if (state && (Tile.Owner != null))
                BackColor = Tile.Owner.Color;
            else BackColor = Tile.Piece != null ? Tile.Piece.Player.Color : Color.Yellow;
        }
        public void HideContent(bool activate)
        {
            Invoke((MethodInvoker)delegate
            {
                hiden = activate;
                if (activate)
                {
                    powerLl.Hide();
                    quantityLl.Hide();
                    Text = "";
                }
                else
                {
                    if(Tile.Piece != null)
                    {
                        powerLl.Show();
                        Text = Tile.Piece?.Type.ToString();
                    }
                }
            });
        }

        public void UpdateView()
        {
            if (Tile.Piece != null)
            {
                this.powerLl.Text = ((int)Tile.Piece.Type).ToString();
                this.quantityLl.Text = (Tile.Piece.MaxAmount - Tile.Piece.Player.PieceFactory.GetCount(Tile.Piece)).ToString();
                BackColor = Tile.Piece.Player.Color;
                if (!hiden)
                {
                    Text = Tile.Piece.Type.ToString();
                    this.powerLl.Show();
                }
            }
            else
            {
                Text = "";
                this.powerLl.Hide();
                BackColor = GetDefaultColor();
            }
        }

        public void ShowQuantity(bool b)
        {
            quantityLl.Visible = b;
        }

        private void InitializeComponent()
        {
            this.powerLl = new Label();
            this.quantityLl = new Label();
            this.SuspendLayout();
            // 
            // powerLl
            // 
            this.powerLl.AutoSize = true;
            this.powerLl.BackColor = Color.Transparent;
            this.powerLl.Dock = DockStyle.Left;
            this.powerLl.Location = new System.Drawing.Point(0, 0);
            this.powerLl.Name = "powerLl";
            this.powerLl.Padding = new Padding(1, 3, 1, 1);
            this.powerLl.Size = new Size(2, 21);
            this.powerLl.TabIndex = 0;
            this.powerLl.Hide();
            // 
            // quantityLl
            // 
            this.quantityLl.AutoSize = true;
            this.quantityLl.BackColor = Color.Transparent;
            this.quantityLl.Dock = DockStyle.Right;
            this.quantityLl.Location = new System.Drawing.Point(0, 0);
            this.quantityLl.Name = "quantityLl";
            this.quantityLl.Padding = new Padding(1, 3, 1, 1);
            this.quantityLl.Size = new Size(2, 21);
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
