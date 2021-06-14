using Stratego.Model.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratego.Model
{
    public class Grid
    {
        public Player[] Players { get; }
        public Tile[] TileGrid { get; }
        public readonly int XSize;
        public readonly int YSize;

        public Grid(Player[] players, int xSize, int ySize)
        {
            Players = players;
            TileGrid = new Tile[xSize * ySize];
            this.XSize = xSize;
            this.YSize = ySize;
        }

        public void Set(Tile tile)
        {
            TileGrid[tile.Coordinate.X + tile.Coordinate.Y * XSize] = tile;
        }

        public Tile Get(Point p)
        {
            return TileGrid[p.X + p.Y * XSize];
        }
    }
}
