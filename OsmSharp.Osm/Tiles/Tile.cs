using OsmSharp.Math.Geo;
using OsmSharp.Math.Geo.Projections;
using OsmSharp.Math.Primitives;
using OsmSharp.Units.Angle;
using System;
using System.Collections.Generic;

namespace OsmSharp.Osm.Tiles
{
    /// <summary>
    /// Represents a tile.
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// Holds the id.
        /// </summary>
        private ulong _id;

        /// <summary>
        /// Creates a new tile from a given id.
        /// </summary>
        /// <param name="id"></param>
        public Tile(ulong id)
        {
            _id = id;

            Tile tile = Tile.CalculateTile(id);
            this.X = tile.X;
            this.Y = tile.Y;
            this.Zoom = tile.Zoom;
        }

        /// <summary>
        /// Creates a new tile.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        public Tile(int x, int y, int zoom)
        {
            this.X = x;
            this.Y = y;
            this.Zoom = zoom;

            _id = Tile.CalculateTileId(zoom, x, y);
        }

        /// <summary>
        /// The X position of the tile.
        /// </summary>
        public int X { get; private set; }

        /// <summary>
        /// The Y position of the tile.
        /// </summary>
        public int Y { get; private set; }

        /// <summary>
        /// The zoom level for this tile.
        /// </summary>
        public int Zoom { get; private set; }

        /// <summary>
        /// Gets the parent tile.
        /// </summary>
        public Tile Parent
        {
            get
            {
                return new Tile(this.X / 2, this.Y / 2, this.Zoom - 1);
            }
        }

        /// <summary>
        /// Returns a hashcode for this tile position.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^
                   this.Y.GetHashCode() ^
                   this.Zoom.GetHashCode();
        }

        /// <summary>
        /// Returns true if the given object represents the same tile.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = (obj as Tile);
            if (other != null)
            {
                return other.X == this.X &&
                    other.Y == this.Y &&
                    other.Zoom == this.Zoom;
            }
            return false;
        }

        /// <summary>
        /// Returns a description for this tile.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}x-{1}y@{2}z", this.X, this.Y, this.Zoom);
        }

        /// <summary>
        /// Returns the top left corner.
        /// </summary>
        public GeoCoordinate TopLeft
        {
            get
            {
                var n = System.Math.PI - ((2.0 * System.Math.PI * (double)this.Y) / System.Math.Pow(2.0, (double)this.Zoom));

                var longitude = (double)(((double)this.X / System.Math.Pow(2.0, (double)this.Zoom) * 360.0) - 180.0);
                var latitude = (double)(180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(n)));

                return new GeoCoordinate(latitude, longitude);
            }
        }

        /// <summary>
        /// Returns the bottom right corner.
        /// </summary>
        public GeoCoordinate BottomRight
        {
            get
            {
                var n = System.Math.PI - ((2.0 * System.Math.PI * (this.Y + 1)) / System.Math.Pow(2.0, this.Zoom));

                var longitude = (double)(((this.X + 1) / System.Math.Pow(2.0, this.Zoom) * 360.0) - 180.0);
                var latitude = (double)(180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(n)));

                return new GeoCoordinate(latitude, longitude);
            }
        }

        /// <summary>
        /// Returns the bounding box for this tile.
        /// </summary>
        public GeoCoordinateBox Box
        {
            get
            {
                // calculate the tiles bounding box and set its properties.
                var topLeft = this.TopLeft;
                var bottomRight = this.BottomRight;

                return new GeoCoordinateBox(topLeft, bottomRight);
            }
        }

        /// <summary>
        /// Returns the 4 subtiles.
        /// </summary>
        /// <returns></returns>
        public TileRange SubTiles
        {
            get
            {
                return new TileRange(2 * this.X,
                    2 * this.Y,
                    2 * this.X + 1,
                    2 * this.Y + 1,
                    this.Zoom + 1);
            }
        }

        /// <summary>
        /// Returns the subtiles of this tile at the given zoom.
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public TileRange GetSubTiles(int zoom)
        {
            if (this.Zoom > zoom) { throw new ArgumentOutOfRangeException("zoom", "Subtiles can only be calculated for higher zooms."); }

            if (this.Zoom == zoom)
            { // just return a range of one tile.
                return new TileRange(this.X, this.Y, this.X, this.Y, this.Zoom);
            }

            var factor = 1 << (zoom - this.Zoom);

            return new TileRange(
                this.X * factor,
                this.Y * factor,
                this.X * factor + factor - 1,
                this.Y * factor + factor - 1,
                zoom);
        }

        /// <summary>
        /// Returns true if this tile overlaps the given tile.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public bool Overlaps(Tile tile)
        {
            if (tile == null) { throw new ArgumentNullException("tile"); }

            if (tile.Zoom == this.Zoom)
            { // only overlaps when identical.
                return tile.Equals(this);
            }
            else if (tile.Zoom > this.Zoom)
            { // the zoom is bigger.
                var range = this.GetSubTiles(tile.Zoom);
                return range.Contains(tile);
            }
            return false;
        }

        /// <summary>
        /// Returns true if this tile is completely overlapped by tiles in the given collection.
        /// </summary>
        /// <param name="tiles"></param>
        /// <returns></returns>
        public bool IsOverlappedBy(IEnumerable<Tile> tiles)
        {
            var foundTiles = new Dictionary<int, HashSet<Tile>>();
            foreach (Tile tile in tiles)
            {
                if (tile.Zoom <= this.Zoom)
                { // check regular overlaps.
                    if (tile.Overlaps(this))
                    { // ok, this collection overlaps this tile.
                        return true;
                    }
                }
                else
                { // tile is at a higher zoom level but several tiles combined can still overlap this one.
                    HashSet<Tile> found;
                    if (!foundTiles.TryGetValue(tile.Zoom, out found))
                    { // create new hashset.
                        found = new HashSet<Tile>();
                        foundTiles.Add(tile.Zoom, found);
                    }
                    found.Add(tile);
                }
            }

            // ok still no conclusive answer, check found tiles.
            foreach (var foundTilePair in foundTiles)
            {
                var subtiles = this.GetSubTiles(foundTilePair.Key);
                int count = 0;
                foreach (var foundTile in foundTilePair.Value)
                {
                    if (subtiles.Contains(foundTile))
                    { // the tile is in the subtiles collection.
                        count++;
                    }
                }
                if (subtiles.Count == foundTilePair.Value.Count)
                { // if the match is exact all subtiles are covered.
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Calculates the tile id of the tile at position (0, 0) for the given zoom.
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        private static ulong CalculateTileId(int zoom)
        {
            if (zoom == 0)
            { // zoom level 0: {0}.
                return 0;
            }
            else if (zoom == 1)
            {
                return 1;
            }
            else if (zoom == 2)
            {
                return 5;
            }
            else if(zoom == 3)
            {
                return 21;
            }
            else if (zoom == 4)
            {
                return 85;
            }
            else if (zoom == 5)
            {
                return 341;
            }
            else if (zoom == 6)
            {
                return 1365;
            }
            else if (zoom == 7)
            {
                return 5461;
            }
            else if (zoom == 8)
            {
                return 21845;
            }
            else if (zoom == 9)
            {
                return 87381;
            }
            else if (zoom == 10)
            {
                return 349525;
            }
            else if (zoom == 11)
            {
                return 1398101;
            }
            else if (zoom == 12)
            {
                return 5592405;
            }
            else if (zoom == 13)
            {
                return 22369621;
            }
            else if (zoom == 14)
            {
                return 89478485;
            }
            else if (zoom == 15)
            {
                return 357913941;
            }
            else if (zoom == 16)
            {
                return 1431655765;
            }
            else if (zoom == 17)
            {
                return 5726623061;
            }
            else if (zoom == 18)
            {
                return 22906492245;
            }

            ulong size = (ulong)System.Math.Pow(2, 2 * (zoom - 1));
            var tileId = Tile.CalculateTileId(zoom - 1) + size;
            return tileId;
        }

        /// <summary>
        /// Calculates the tile id of the tile at position (x, y) for the given zoom.
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static ulong CalculateTileId(int zoom, int x, int y)
        {
            ulong id = Tile.CalculateTileId(zoom);
            long width = (long)System.Math.Pow(2, zoom);
            return id + (ulong)x + (ulong)(y * width);
        }

        /// <summary>
        /// Calculate the tile given the id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static Tile CalculateTile(ulong id)
        {
            // find out the zoom level first.
            int zoom = 0;
            if (id > 0)
            { // only if the id is at least at zoom level 1.
                while (id >= Tile.CalculateTileId(zoom))
                {
                    // move to the next zoom level and keep searching.
                    zoom++;
                }
                zoom--;
            }

            // calculate the x-y.
            ulong local = id - Tile.CalculateTileId(zoom);
            ulong width = (ulong)System.Math.Pow(2, zoom);
            int x = (int)(local % width);
            int y = (int)(local / width);

            return new Tile(x, y, zoom);
        }

        /// <summary>
        /// Returns the id of this tile.
        /// </summary>
        public ulong Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Returns true if this tile is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (this.X >= 0 &&
                    this.Y >= 0 &&
                    this.Zoom >= 0)
                { // some are negative.
                    var size = System.Math.Pow(2, this.Zoom);
                    return this.X < size && this.Y < size;
                }
                return false;
            }
        }

        #region Conversion Functions

        /// <summary>
        /// Returns the tile at the given location at the given zoom.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static Tile CreateAroundLocation(double latitude, double longitude, int zoom)
        {
            int n = (int)System.Math.Floor(System.Math.Pow(2, zoom));

            Radian rad = new Degree(latitude);

            int x = (int)(((longitude + 180.0f) / 360.0f) * (double)n);
            int y = (int)(
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);

            return new Tile(x, y, zoom);
        }

        /// <summary>
        /// Returns the tile at the given location at the given zoom.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static Tile CreateAroundLocation(GeoCoordinate location, int zoom)
        {
            return Tile.CreateAroundLocation(location.Latitude, location.Longitude, zoom);
        }

        /// <summary>
        /// Inverts the X-coordinate.
        /// </summary>
        /// <returns></returns>
        public Tile InvertX()
        {
            int n = (int)System.Math.Floor(System.Math.Pow(2, this.Zoom));

            return new Tile(n - this.X - 1, this.Y, this.Zoom);
        }

        /// <summary>
        /// Inverts the Y-coordinate.
        /// </summary>
        /// <returns></returns>
        public Tile InvertY()
        {
            int n = (int)System.Math.Floor(System.Math.Pow(2, this.Zoom));

            return new Tile(this.X, n - this.Y - 1, this.Zoom);
        }

        /// <summary>
        /// Converts this tile definition into a projected box.
        /// </summary>
        /// <param name="projection"></param>
        /// <returns></returns>
        public BoxF2D ToBox(IProjection projection)
        {
            double left = projection.LongitudeToX(this.TopLeft[0]);
            double right = projection.LongitudeToX(this.BottomRight[0]);
            double bottom = projection.LatitudeToY(this.BottomRight[1]);
            double top = projection.LatitudeToY(this.TopLeft[1]);

            return new BoxF2D(left, bottom, right, top);
        }

        #endregion
    }
}