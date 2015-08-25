using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Osm.Tiles
{
    /// <summary>
    /// Holds an index of tiles for a given tile range.
    /// </summary>
    public class TileRangeIndex
    {
        /// <summary>
        /// Holds the tile range this index is for.
        /// </summary>
        private TileRange _range;

        /// <summary>
        /// Creates a new tile range index.
        /// </summary>
        /// <param name="tileRange"></param>
        public TileRangeIndex(TileRange tileRange)
        {
            _range = tileRange;
        }

        /// <summary>
        /// Adds a new tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public void Add(Tile tile)
        {
            if (tile.Zoom < _range.Zoom && _range.Zoom - tile.Zoom < 3)
            { // add this tile to all the subtiles.
                var subTiles = tile.GetSubTiles(_range.Zoom);
                foreach (var subTile in subTiles)
                {
                    if (_range.Contains(subTile))
                    { // ok, this tile is in this range.
                        this.Add(subTile.Id, tile);
                    }
                }
            }
            else if (tile.Zoom > _range.Zoom && tile.Zoom - _range.Zoom < 3)
            { // find the tile that this subtile belongs to.
                foreach (var rangedTile in _range)
                {
                    if (rangedTile.Overlaps(tile))
                    { // add this this tile here.
                        this.Add(rangedTile.Id, tile);
                        // this tile can only belong in one place.
                        return;
                    }
                }
            }
            if (_range.Contains(tile))
            { // this tile is already the correct zoom.
                this.Add(tile.Id, tile);
            }
        }

        /// <summary>
        /// Chooses the best tile(s) for the given tile.
        /// </summary>
        /// <param name="tile">The tile to search tiles for.</param>
        /// <param name="higherFirst">Choose tiles with a higher zoom level first, otherwise choose lower first.</param>
        /// <returns></returns>
        public IEnumerable<Tile> ChooseBest(Tile tile, bool higherFirst)
        {
            var tiles = new List<Tile>(this.Get(tile.Id));
            tiles.Sort(delegate(Tile x, Tile y)
            {
                return TileRangeIndex.TileWeight(tile.Zoom, x.Zoom, higherFirst).CompareTo(
                    TileRangeIndex.TileWeight(tile.Zoom, y.Zoom, higherFirst));
            });

            return tiles;
        }

        #region Tiles Index Management

        /// <summary>
        /// Holds the tiles index.
        /// </summary>
        private Dictionary<ulong, HashSet<Tile>> _tilesIndex = new Dictionary<ulong, HashSet<Tile>>();

        /// <summary>
        /// Calculates the tile sorting function.
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="x"></param>
        /// <param name="highestFirst"></param>
        /// <returns></returns>
        public static int TileWeight(int zoom, int x, bool highestFirst)
        {
            if (highestFirst)
            {
                if (zoom >= x) { return zoom - x; }
                else { return 100 - (x - zoom); }
            }
            else
            {
                if (zoom <= x) { return x - zoom; }
                else { return 100 + (zoom - x); }
            }
        }

        /// <summary>
        /// Adds the given tile as the best tile for the given tileId.
        /// </summary>
        /// <param name="tileId"></param>
        /// <param name="tile"></param>
        private void Add(ulong tileId, Tile tile)
        {
            HashSet<Tile> tilesSet;
            if (!_tilesIndex.TryGetValue(tileId, out tilesSet))
            { // create the tiles set.
                tilesSet = new HashSet<Tile>();
                _tilesIndex.Add(tileId, tilesSet);
            }
            tilesSet.Add(tile);
        }

        /// <summary>
        /// Returns the tiles for the given tile id.
        /// </summary>
        /// <param name="tileId"></param>
        /// <returns></returns>
        private IEnumerable<Tile> Get(ulong tileId)
        {
            HashSet<Tile> tilesSet;
            if (!_tilesIndex.TryGetValue(tileId, out tilesSet))
            { // tileset not there!
                tilesSet = new HashSet<Tile>();
            }
            return tilesSet;
        }

        #endregion
    }
}
