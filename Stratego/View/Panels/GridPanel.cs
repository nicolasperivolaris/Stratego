using Stratego.Model;
using Stratego.Model.Panels;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace Stratego.View
{
    public class GridPanel : Grid
    {
        public Dictionary<int, Player> Players { get; }
        public GridPanel(EventHandler tileClickListener, Dictionary<int, Player> players) : base(tileClickListener)
        {
            Players = players;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
             ControlStyles.OptimizedDoubleBuffer |
             ControlStyles.UserPaint, true);
            Selectable = true;
        }

        #region Creation
        public void CreateMap(string pattern)
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
                if (row.Name != "row") continue;

                int i = Int32.Parse(row.Attributes["i"].Value);
                ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / ColumnCount));

                foreach (XmlNode cell in row.ChildNodes)
                {
                    int j = Int32.Parse(cell.Attributes["j"].Value);
                    this.RowStyles.Add(new RowStyle(SizeType.Percent, 40F / RowCount));
                    int owner = row.Attributes["owner"] != null ? Convert.ToInt32(row.Attributes["owner"].Value) : -1;
                    Tile tile = GetNewTile(cell, i, j, owner);
                    var piece = cell.Attributes["piece"];
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
                    ((WalkableTile)result).Owner = Players[owner];
                result.Click += OnClick;
            }
            else
                result = new HoleTile(row, column);

            return result;
        }
        #endregion

        private void OnClick(object sender, EventArgs e)
        {
            bool? moved = Selected?.Piece?.Move(Selected, (Tile)sender);

            ActionEventArgs click;

            if (moved == true)
            {
                Move move = new Move() { From = Selected, To = (Tile)sender };
                click = new ActionEventArgs()
                {
                    ActionType = ActionType.Move,
                    Object = move,
                    Sender = this
                };
            }
            else
            {
                click = new ActionEventArgs()
                {
                    ActionType = ActionType.TileClick,
                    Object = (Tile)sender,
                    Sender = this
                };
            }
            OnTileClick(this, click);
        }


    }
}
