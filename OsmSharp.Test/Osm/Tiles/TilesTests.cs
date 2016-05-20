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
using NUnit.Framework;
using OsmSharp.Math.Geo;
using OsmSharp.Osm.Tiles;

namespace OsmSharp.Test.Osm.Tiles
{
    /// <summary>
    /// Does some tests on the tile calculations.
    /// </summary>
    [TestFixture]
    public class TilesTests
    {
        /// <summary>
        /// Tests creating a tile.
        /// </summary>
        [Test]
        public void TestTileCreation()
        {
            // 51.27056&lon=4.78849
            // http://tile.deltamedia.local/tile/16/33639/21862.png
            Tile tile = new Tile(33639, 21862, 16);
            Tile tile2 = Tile.CreateAroundLocation(tile.Box.Center, 16);

            Assert.AreEqual(tile.X, tile2.X);
            Assert.AreEqual(tile.Y, tile2.Y);
            Assert.AreEqual(tile.Zoom, tile2.Zoom);
        }

        /// <summary>
        /// Tests a tile box.
        /// </summary>
        [Test]
        public void TestTileBox()
        {
            Tile tile = new Tile(33639, 21862, 16);

            for (double longitude = tile.Box.MinLon; longitude < tile.Box.MaxLon; 
                longitude = longitude + tile.Box.DeltaLon / 100)
            {
                for (double latitude = tile.Box.MinLat; latitude < tile.Box.MaxLat;
                    latitude = latitude + tile.Box.DeltaLon / 100)
                {
                    Tile tile2 = Tile.CreateAroundLocation(new GeoCoordinate(
                        latitude, longitude), tile.Zoom);

                    Assert.AreEqual(tile.X, tile2.X);
                    Assert.AreEqual(tile.Y, tile2.Y);
                    Assert.AreEqual(tile.Zoom, tile2.Zoom);
                }
            }
        }

        /// <summary>
        /// Tests a tile range enumeration.
        /// </summary>
        [Test]
        public void TestTileRangeEnumerator()
        {
            TileRange range = new TileRange(0, 0, 1, 1, 16);

            HashSet<Tile> tiles = new HashSet<Tile>(range);

            Assert.IsTrue(tiles.Contains(new Tile(0, 0, 16)));
            Assert.IsTrue(tiles.Contains(new Tile(0, 1, 16)));
            Assert.IsTrue(tiles.Contains(new Tile(1, 1, 16)));
            Assert.IsTrue(tiles.Contains(new Tile(0, 1, 16)));
        }

        /// <summary>
        /// Tests the tile id generation.
        /// </summary>
        [Test]
        public void TestTileId()
        {
            var tile0 = new Tile(0, 0, 0);
            Assert.AreEqual(0, tile0.Id);

            var tile1_0_0 = new Tile(0, 0, 1);
            Assert.AreEqual(1, tile1_0_0.Id);

            var tile2_0_0 = new Tile(0, 0, 2);
            Assert.AreEqual(5, tile2_0_0.Id);

            var tile3_0_0 = new Tile(0, 0, 3);
            Assert.AreEqual(5 + 16, tile3_0_0.Id);

            var tile4_0_0 = new Tile(0, 0, 4);
            Assert.AreEqual(5 + 16 + 64, tile4_0_0.Id);

            var tile2_1_1 = new Tile(1, 1, 2);
            Assert.AreEqual(5 + 1 + 4, tile2_1_1.Id);

            var tile2_1_1_fromId = new Tile(5 + 1 + 4);
            Assert.AreEqual(tile2_1_1.Zoom, tile2_1_1_fromId.Zoom);
            Assert.AreEqual(tile2_1_1.X, tile2_1_1_fromId.X);
            Assert.AreEqual(tile2_1_1.Y, tile2_1_1_fromId.Y);

            for (ulong id = 0; id < 1000; id++)
            {
                Tile tile = new Tile(id);
                Assert.AreEqual(id, tile.Id);
            }
        }

        /// <summary>
        /// Tests the tile range count property.
        /// </summary>
        [Test]
        public void TestTileRangeCount()
        {
            var range = new TileRange(0, 0, 0, 0, 16);
            Assert.AreEqual(1, range.Count);
            range = new TileRange(0, 0, 1, 0, 16);
            Assert.AreEqual(2, range.Count);
            range = new TileRange(0, 0, 1, 1, 16);
            Assert.AreEqual(4, range.Count);
        }

        /// <summary>
        /// Tests a tile range spiral enumerator.
        /// </summary>
        [Test]
        public void TestTileSpiralEnumerator()
        {
            // simplest case, range with one tile.
            var range = new TileRange(0, 0, 0, 0, 16);
            var enumerator = new TileRange.TileRangeCenteredEnumerator(range);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current.X);
            Assert.AreEqual(0, enumerator.Current.Y);
            Assert.IsFalse(enumerator.MoveNext());

            // a range with 2 tiles.
            range = new TileRange(0, 0, 1, 0, 16);
            enumerator = new TileRange.TileRangeCenteredEnumerator(range);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current.X);
            Assert.AreEqual(0, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current.X);
            Assert.AreEqual(0, enumerator.Current.Y);
            Assert.IsFalse(enumerator.MoveNext());

            // a range with 4 tiles.
            range = new TileRange(0, 0, 1, 1, 16);
            enumerator = new TileRange.TileRangeCenteredEnumerator(range);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current.X);
            Assert.AreEqual(1, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current.X);
            Assert.AreEqual(0, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current.X);
            Assert.AreEqual(0, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current.X);
            Assert.AreEqual(1, enumerator.Current.Y);
            Assert.IsFalse(enumerator.MoveNext());

            // a range with 6 tiles.
            range = new TileRange(0, 0, 2, 1, 16);
            enumerator = new TileRange.TileRangeCenteredEnumerator(range);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current.X);
            Assert.AreEqual(1, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current.X);
            Assert.AreEqual(0, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current.X);
            Assert.AreEqual(0, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current.X);
            Assert.AreEqual(1, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current.X);
            Assert.AreEqual(1, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current.X);
            Assert.AreEqual(0, enumerator.Current.Y);
            Assert.IsFalse(enumerator.MoveNext());

            // a range with 9 tiles.
            range = new TileRange(0, 0, 2, 2, 16);
            enumerator = new TileRange.TileRangeCenteredEnumerator(range);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current.X);
            Assert.AreEqual(1, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current.X);
            Assert.AreEqual(0, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current.X);
            Assert.AreEqual(0, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current.X);
            Assert.AreEqual(1, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current.X);
            Assert.AreEqual(2, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current.X);
            Assert.AreEqual(2, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current.X);
            Assert.AreEqual(2, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current.X);
            Assert.AreEqual(1, enumerator.Current.Y);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current.X);
            Assert.AreEqual(0, enumerator.Current.Y);
            Assert.IsFalse(enumerator.MoveNext());

            // rudementary tests.
            range = new TileRange(-10, -10, 10, 10, 16);
            var tiles = new HashSet<Tile>(range.EnumerateInCenterFirst());
            Assert.AreEqual(range.Count, tiles.Count);

            // rudementary tests.
            range = new TileRange(33638, 21862, 33640, 21864, 16);
            tiles = new HashSet<Tile>(range.EnumerateInCenterFirst());
            Assert.AreEqual(range.Count, tiles.Count);
        }

        /// <summary>
        /// Tests the subtile calculation.
        /// </summary>
        [Test]
        public void TestTileSubtiles()
        {
            var tile = new Tile(0, 0, 0);

            var subtiles = tile.SubTiles;
            Assert.AreEqual(0, subtiles.XMin);
            Assert.AreEqual(0, subtiles.YMin);
            Assert.AreEqual(1, subtiles.XMax);
            Assert.AreEqual(1, subtiles.YMax);
            Assert.AreEqual(1, subtiles.Zoom);

            subtiles = tile.GetSubTiles(1);
            Assert.AreEqual(0, subtiles.XMin);
            Assert.AreEqual(0, subtiles.YMin);
            Assert.AreEqual(1, subtiles.XMax);
            Assert.AreEqual(1, subtiles.YMax);
            Assert.AreEqual(1, subtiles.Zoom);

            subtiles = tile.GetSubTiles(2);
            Assert.AreEqual(0, subtiles.XMin);
            Assert.AreEqual(0, subtiles.YMin);
            Assert.AreEqual(3, subtiles.XMax);
            Assert.AreEqual(3, subtiles.YMax);
            Assert.AreEqual(2, subtiles.Zoom);

            tile = new Tile(1, 1, 1);

            subtiles = tile.SubTiles;
            Assert.AreEqual(2, subtiles.XMin);
            Assert.AreEqual(2, subtiles.YMin);
            Assert.AreEqual(3, subtiles.XMax);
            Assert.AreEqual(3, subtiles.YMax);
            Assert.AreEqual(2, subtiles.Zoom);

            subtiles = tile.GetSubTiles(2);
            Assert.AreEqual(2, subtiles.XMin);
            Assert.AreEqual(2, subtiles.YMin);
            Assert.AreEqual(3, subtiles.XMax);
            Assert.AreEqual(3, subtiles.YMax);
            Assert.AreEqual(2, subtiles.Zoom);

            subtiles = tile.GetSubTiles(3);
            Assert.AreEqual(4, subtiles.XMin);
            Assert.AreEqual(4, subtiles.YMin);
            Assert.AreEqual(7, subtiles.XMax);
            Assert.AreEqual(7, subtiles.YMax);
            Assert.AreEqual(3, subtiles.Zoom);
        }

        /// <summary>
        /// Tests the parent calculation.
        /// </summary>
        [Test]
        public void TestTileParents()
        {
            var tile = new Tile(112, 254, 16);

            var subtiles = tile.SubTiles;
            foreach(var subtile in subtiles)
            {
                Assert.AreEqual(tile.X, subtile.Parent.X);
                Assert.AreEqual(tile.Y, subtile.Parent.Y);
                Assert.AreEqual(tile.Zoom, subtile.Parent.Zoom);
            }
        }

        /// <summary>
        /// Tests the overlaps function.
        /// </summary>
        [Test]
        public void TestTileOverlaps()
        {
            var tile = new Tile(1, 1, 1);

            Assert.IsTrue(tile.Overlaps(new Tile(1, 1, 1)));
            Assert.IsTrue(tile.Overlaps(new Tile(3, 3, 2)));
            Assert.IsTrue(tile.Overlaps(new Tile(8, 8, 4)));

            Assert.IsFalse(tile.Overlaps(new Tile(0, 0, 3)));
            Assert.IsFalse(tile.Overlaps(new Tile(0, 0, 4)));
        }
    }
}