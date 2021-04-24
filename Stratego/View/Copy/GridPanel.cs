using Stratego.Controler;
using Stratego.Model;
using Stratego.Model.Panels;
using Stratego.Model.Pieces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace Stratego.View.copy
{
    public class GridPanel : TableLayoutPanel, IObservable<ActionEvent>
    {
        public Tile LastSelectedTile { get; private set; }
        private Queue<IObserver<ActionEvent>> _observers;
        public event EventHandler OnTileClick;

        public GridPanel(EventHandler TileClickListener): base()
        {
            OnTileClick = TileClickListener;
            _observers = new Queue<IObserver<ActionEvent>>();
            CreateMap(Properties.Resources.pattern);
            SetStyle(ControlStyles.AllPaintingInWmPaint |
             ControlStyles.OptimizedDoubleBuffer |
             ControlStyles.UserPaint, true);

            Enter += OnFocusEnter;
            Leave += OnFocusLeave;
        }

        #region Event
        private void OnFocusEnter(object sender, EventArgs e)
        {
            foreach (Tile tile in this.Controls)
            {
                if (tile.Focused)
                    LastSelectedTile = tile;
            }
        }

        private void OnFocusLeave(object sender, EventArgs e)
        {
            LastSelectedTile = null;
        }

        private void TileClick(object sender, EventArgs e)
        {
            LastSelectedTile = (Tile)sender;
        } 
        #endregion

        #region Creation
        private void CreateMap(string pattern)
        {

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(pattern);
            XmlNode map = doc.DocumentElement;

            ColumnCount = Int32.Parse(map.Attributes["columns"].Value);
            RowCount = Int32.Parse(map.Attributes["rows"].Value);

            Populate(map);
        }

        private void Populate(XmlNode map)
        {
            foreach (XmlNode row in map.ChildNodes)
            {
                int i = Int32.Parse(row.Attributes["i"].Value);

                this.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / ColumnCount));

                foreach (XmlNode cell in row.ChildNodes)
                {
                    int j = Int32.Parse(cell.Attributes["j"].Value);
                    this.RowStyles.Add(new RowStyle(SizeType.Percent, 40F / RowCount));
                    int owner = row.Attributes["owner"] != null ? Convert.ToInt32(row.Attributes["owner"].Value) : -1;
                    Tile tile = GetNewTile(cell, i, j, owner);
                    var piece = cell.Attributes["piece"];
                    //if (piece != null)
                    //    tile.Piece = PieceFactory.InstanceOf(piece.Value);
                    this.Controls.Add(tile, j, i);
                }
            }
        }

        private Tile GetNewTile(XmlNode cell, int row, int column, int owner)
        {
            Tile result;
            if (cell.Attributes["accessible"].Value.Equals("true", StringComparison.InvariantCultureIgnoreCase))
            {
                result = new WalkableTile(row, column);
                result.ShowQuantity(false);
                if (owner >= 0)
                    ((WalkableTile)result).Owner = Program.;
                result.Click += OnTileClick;
                result.Click += TileClick;
            }
            else
                result = new HoleTile(row, column);

            return result;
        } 
        #endregion
        
        private void Notifie(ActionEvent action)
        {
            foreach (IObserver<ActionEvent> observer in _observers)
            {
                observer.OnNext(action);
            }
        }

        public void MovePiece(Tile from, Tile to)
        {
            if (to.IsEmpty())
            {
                to.Piece = from.Piece;
            }
            else
            {
                if (from.Piece.TryKill(to.Piece))
                    to.Piece = from.Piece;
            }

            from.Remove();
        }

        public IDisposable Subscribe(IObserver<ActionEvent> observer)
        {
            _observers.Enqueue(observer);
            return null;
        }
    }
}
