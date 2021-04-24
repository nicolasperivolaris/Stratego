using System.Drawing;
using System.Windows.Forms;

namespace Stratego.Model
{
    public abstract class Tile : Button
    {
        private Label powerLl;
        private Label quantityLl;

        public Point Coordinate { get; set; }

        public Piece _piece;

        public Piece Piece
        {
            get
            {
                return _piece;
            }
            set
            {
                _piece = value;
                if (_piece != null)
                {
                    Text = _piece.Type.ToString();
                    this.powerLl.Text = ((int)Piece.Type).ToString();
                    this.quantityLl.Text = (Piece.MaxAmount - Piece.Player.PieceFactory.GetCount(Piece)).ToString();
                    this.powerLl.Show();
                }
                else
                {
                    Text = "";
                    this.powerLl.Hide();
                }
            }
        }

        public Tile(int row, int column) : base()
        {
            Coordinate = new Point(row, column);
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            InitializeComponent();
        }

        public bool IsEmpty() { return Piece == null; }

        public void Remove()
        {
            Piece.ToFactory();
            Piece = null;
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
