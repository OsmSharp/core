// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using OsmSharp.Math.Geo;
using OsmSharp.Units.Angle;

namespace OsmSharp.Osm.Tiles
{
    /// <summary>
    /// Represents a range of tiles.
    /// </summary>
    public class TileRange : IEnumerable<Tile>
    {
        /// <summary>
        /// Creates a new tile range.
        /// </summary>
        /// <param name="xMin"></param>
        /// <param name="yMin"></param>
        /// <param name="xMax"></param>
        /// <param name="yMax"></param>
        /// <param name="zoom"></param>
        public TileRange(int xMin, int yMin, int xMax, int yMax, int zoom)
        {
            this.XMin = xMin;
            this.XMax = xMax;
            this.YMin = yMin;
            this.YMax = yMax;

            this.Zoom = zoom;
        }

        /// <summary>
        /// The minimum X of this range.
        /// </summary>
        public int XMin { get; private set; }

        /// <summary>
        /// The minimum Y of this range.
        /// </summary>
        public int YMin { get; private set; }

        /// <summary>
        /// The maximum X of this range.
        /// </summary>
        public int XMax { get; private set; }

        /// <summary>
        /// The maximum Y of this range.
        /// </summary>
        public int YMax { get; private set; }

        /// <summary>
        /// The zoom of this range.
        /// </summary>
        public int Zoom { get; private set; }

        /// <summary>
        /// Returns the number of tiles in this range.
        /// </summary>
        public int Count
        {
            get
            {
                return System.Math.Abs(this.XMax - this.XMin + 1) *
                    System.Math.Abs(this.YMax - this.YMin + 1);
            }
        }

        /// <summary>
        /// Returns true if the given tile exists in this range.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public bool Contains(Tile tile)
        {
            return this.XMax >= tile.X && this.XMin <= tile.X &&
                this.YMax >= tile.Y && this.YMin <= tile.Y;
        }

        #region Functions

        /// <summary>
        /// Returns true if the given tile lies at the border of this range.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public bool IsBorderAt(int x, int y, int zoom)
        {
            return ((x == this.XMin) || (x == this.XMax)
                || (y == this.YMin) || (y == this.YMin)) &&
                this.Zoom == zoom;
        }

        /// <summary>
        /// Returns true if the given tile lies at the border of this range.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public bool IsBorderAt(Tile tile)
        {
            return IsBorderAt(tile.X, tile.Y, tile.Zoom);
        }

        #endregion

        #region Conversion Functions

        /// <summary>
        /// Returns a tile range that encompasses the given bounding box at a given zoom level.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public static TileRange CreateAroundBoundingBox(GeoCoordinateBox box, int zoom)
        {
            int n = (int)System.Math.Floor(System.Math.Pow(2, zoom));

            Radian rad = new Degree(box.MaxLat);

            int x_tile_min = (int)(((box.MinLon + 180.0f) / 360.0f) * (double)n);
            int y_tile_min = (int)(
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);

            rad = new Degree(box.MinLat);
            int x_tile_max = (int)(((box.MaxLon + 180.0f) / 360.0f) * (double)n);
            int y_tile_max = (int)(
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);

            return new TileRange(x_tile_min, y_tile_min, x_tile_max, y_tile_max, zoom);
        }

        #endregion

        /// <summary>
        /// Returns en enumerator of tiles.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Tile> GetEnumerator()
        {
            return new TileRangeEnumerator(this);
        }

        /// <summary>
        /// Returns en enumerator of tiles.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Simple enumerator.
        /// </summary>
        private class TileRangeEnumerator : IEnumerator<Tile>
        {
            private TileRange _range;

            private Tile _current;

            public TileRangeEnumerator(TileRange range)
            {
                _range = range;
            }

            public Tile Current
            {
                get 
                {
                    return _current;
                }
            }

            public void Dispose()
            {
                _range = null;
            }

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                if (_current == null)
                {
                    _current = new Tile(_range.XMin, _range.YMin, _range.Zoom);
                    return true;
                }

                int x = _current.X;
                int y = _current.Y;

                if (x == _range.XMax)
                {
                    if (y == _range.YMax)
                    {
                        return false;
                    }
                    y++;
                    x = _range.XMin;
                }
                else
                {
                    x++;
                }
                _current = new Tile(x, y, _current.Zoom);
                return true;
            }

            public void Reset()
            {
                _current = null;
            }
        }

        /// <summary>
        /// Defines an enumerator that start at the center of the range and moves out in a spiral.
        /// </summary>
        public class TileRangeCenteredEnumerator : IEnumerator<Tile>
        {   
            /// <summary>
            /// Holds the range to enumerate.
            /// </summary>
            private TileRange _range;

            /// <summary>
            /// Holds the current tile.
            /// </summary>
            private Tile _current;

            /// <summary>
            /// Holds the enumerated tiles.
            /// </summary>
            private HashSet<Tile> _enumeratedTiles = new HashSet<Tile>();

            /// <summary>
            /// Creates the enumerator.
            /// </summary>
            /// <param name="range"></param>
            public TileRangeCenteredEnumerator(TileRange range)
            {
                _range = range;
            }

            /// <summary>
            /// Returns the current tile.
            /// </summary>
            public Tile Current
            {
                get 
                {
                    return _current;
                }
            }

            /// <summary>
            /// Disposes of all resources associated with this object.
            /// </summary>
            public void Dispose()
            {
                _range = null;
            }

            /// <summary>
            /// Returns the current tile.
            /// </summary>
            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            /// <summary>
            /// Holds the current desired direction.
            /// </summary>
            private DirectionEnum _direction = DirectionEnum.Up;

            private enum DirectionEnum
            {
                Up = 0,
                Right = 1,
                Down = 2,
                Left = 3
            }

            /// <summary>
            /// Move to the next tile.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (_current == null)
                { // start with the center tile.
                    int centerX = (int)System.Math.Floor((_range.XMax + _range.XMin) / 2.0);
                    int centerY = (int)System.Math.Ceiling((_range.YMax + _range.YMin) / 2.0);
                    _current = new Tile(centerX, centerY, _range.Zoom);
                    _enumeratedTiles.Add(_current);
                    return true;
                }

                // check if there are more tiles to be enumerated.
                if (_range.Count <= _enumeratedTiles.Count)
                { // no more tiles left.
                    return false;
                }

                // try to move in the desired direction.
                Tile next = null;
                while (next == null)
                { // try until a valid tile is found.
                    switch (_direction)
                    {
                        case DirectionEnum.Up: // up
                            next = new Tile(_current.X, _current.Y - 1, _range.Zoom);
                            if (_enumeratedTiles.Contains(next))
                            { // moving up does not work, try to move left.
                                _direction = DirectionEnum.Left;
                                next = null;
                            }
                            else
                            { // moved up, try right.
                                _direction = DirectionEnum.Right;
                            }
                            break;
                        case DirectionEnum.Left: // left
                            next = new Tile(_current.X - 1, _current.Y, _range.Zoom);
                            if (_enumeratedTiles.Contains(next))
                            { // moving left does not work, try to move down.
                                _direction = DirectionEnum.Down;
                                next = null;
                            }
                            else
                            { // moved left, try up.
                                _direction = DirectionEnum.Up;
                            }
                            break;
                        case DirectionEnum.Down: // down
                            next = new Tile(_current.X, _current.Y + 1, _range.Zoom);
                            if (_enumeratedTiles.Contains(next))
                            { // moving down does not work, try to move right.
                                _direction = DirectionEnum.Right;
                                next = null;
                            }
                            else
                            { // moved down, try left.
                                _direction = DirectionEnum.Left;
                            }
                            break;
                        case DirectionEnum.Right: // right
                            next = new Tile(_current.X + 1, _current.Y, _range.Zoom);
                            if (_enumeratedTiles.Contains(next))
                            { // moving right does not work, try to move up.
                                _direction = DirectionEnum.Up;
                                next = null;
                            }
                            else
                            { // moved right, try down.
                                _direction = DirectionEnum.Down;
                            }
                            break;
                    }

                    // test if the next is in range.
                    if (next != null && !_range.Contains(next))
                    { // not in range, do not enumerate but move to next.
                        _current = next; // pretend the next has been enumerated.
                        next = null;
                    }
                }

                // ok, next was found.
                _current = next;
                _enumeratedTiles.Add(_current);
                return true;
            }

            /// <summary>
            /// Resets this enumerator.
            /// </summary>
            public void Reset()
            {
                _current = null;
                _enumeratedTiles.Clear();
            }
        }

        /// <summary>
        /// Returns an enumerable that enumerates tiles with the center first.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tile> EnumerateInCenterFirst()
        {
            return new TileRangeCenterFirst(this);
        }

        /// <summary>
        /// Tile range center first enumerable.
        /// </summary>
        private class TileRangeCenterFirst : IEnumerable<Tile>
        {
            /// <summary>
            /// Holds the tile range to be enumerated.
            /// </summary>
            private TileRange _tileRange;

            /// <summary>
            /// Creates a new range center first enumerable.
            /// </summary>
            /// <param name="tileRange"></param>
            public TileRangeCenterFirst(TileRange tileRange)
            {
                _tileRange = tileRange;
            }

            /// <summary>
            /// Returns the enumerator.
            /// </summary>
            /// <returns></returns>
            public IEnumerator<Tile> GetEnumerator()
            {
                return new TileRangeCenteredEnumerator(_tileRange);
            }

            /// <summary>
            /// Returns the enumerator.
            /// </summary>
            /// <returns></returns>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new TileRangeCenteredEnumerator(_tileRange);
            }
        }
    }
}