// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
//                    Scheinpflug Tommy
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

using NUnit.Framework;
using OsmSharp.Collections.Tags;
using OsmSharp.Osm;
using OsmSharp.Units.Speed;
using OsmSharp.Units.Weight;
using OsmSharp.Units.Distance;

namespace OsmSharp.Test.Osm
{
    /// <summary>
    /// Contains test methods for parsing functions.
    /// </summary>
    [TestFixture]
    public class TagParsingTests
    {
        /// <summary>
        /// Tests weight parsing.
        /// </summary>
        [Test]
        public void TestWeightParsing()
        {
            Kilogram result;

            // Official Valid Values
            Assert.AreEqual(true, TagExtensions.TryParseWeight("5", out result));
            Assert.AreEqual(5 * 1000, result.Value);

            Assert.AreEqual(true, TagExtensions.TryParseWeight("5 t", out result));
            Assert.AreEqual(5 * 1000, result.Value);

            Assert.AreEqual(true, TagExtensions.TryParseWeight("5.5 t", out result));
            Assert.AreEqual(5.5 * 1000, result.Value);

            // Additional Valid Values
            Assert.AreEqual(true, TagExtensions.TryParseWeight("5.5 to", out result));
            Assert.AreEqual(5.5 * 1000, result.Value);

            Assert.AreEqual(true, TagExtensions.TryParseWeight("5.5  tonnes  ", out result));
            Assert.AreEqual(5.5 * 1000, result.Value);

            Assert.AreEqual(true, TagExtensions.TryParseWeight("   5.5    Tonnen   ", out result));
            Assert.AreEqual(5.5 * 1000, result.Value);

            // Invalid Values
            Assert.AreEqual(false, TagExtensions.TryParseWeight("3 persons", out result));
            Assert.AreEqual(false, TagExtensions.TryParseWeight("0,6", out result));
        }

        /// <summary>
        /// Tests length parsing.
        /// </summary>
        [Test]
        public void TestLengthParsing()
        {
            Meter result;

            // Official Valid Values
            Assert.AreEqual(true, TagExtensions.TryParseLength("3", out result));
            Assert.AreEqual(3, result.Value);

            Assert.AreEqual(true, TagExtensions.TryParseLength("3 m", out result));
            Assert.AreEqual(3, result.Value);

            Assert.AreEqual(true, TagExtensions.TryParseLength("3.8 m", out result));
            Assert.AreEqual(3.8, result.Value);

            Assert.AreEqual(true, TagExtensions.TryParseLength("6'7\"", out result));
            Assert.AreEqual(6 * 0.3048 + 7 * 0.0254, result.Value);

            // Additional Valid Values
            Assert.AreEqual(true, TagExtensions.TryParseLength("3.8 meters", out result));
            Assert.AreEqual(3.8, result.Value);

            Assert.AreEqual(true, TagExtensions.TryParseLength("3.8  metres  ", out result));
            Assert.AreEqual(3.8, result.Value);

            Assert.AreEqual(true, TagExtensions.TryParseLength("   3.8    Meter   ", out result));
            Assert.AreEqual(3.8, result.Value);

            // Invalid Values
            Assert.AreEqual(false, TagExtensions.TryParseLength("2.3; 7'9\"", out result));
            Assert.AreEqual(false, TagExtensions.TryParseLength("2,3", out result));
            Assert.AreEqual(false, TagExtensions.TryParseLength("6'", out result));
            Assert.AreEqual(false, TagExtensions.TryParseLength("6 ft", out result));
        }

        /// <summary>
        /// Tests boolean parsing.
        /// </summary>
        [Test]
        public void TestBooleanParsing()
        {
            // test IsTrue.
            TagsCollectionBase tags = new TagsCollection();
            tags.Add("area", "yes");
            Assert.IsTrue(tags.IsTrue("area"));

            tags = new TagsCollection();
            tags.Add("area", "1");
            Assert.IsTrue(tags.IsTrue("area"));

            tags = new TagsCollection();
            tags.Add("area", "true");
            Assert.IsTrue(tags.IsTrue("area"));

            tags = new TagsCollection();
            tags.Add("area", "false");
            Assert.IsFalse(tags.IsTrue("area"));

            tags = new TagsCollection();
            tags.Add("area", "0");
            Assert.IsFalse(tags.IsTrue("area"));

            tags = new TagsCollection();
            tags.Add("area", "no");
            Assert.IsFalse(tags.IsTrue("area"));

            // test IsFalse.
            tags = new TagsCollection();
            tags.Add("area", "yes");
            Assert.IsFalse(tags.IsFalse("area"));

            tags = new TagsCollection();
            tags.Add("area", "1");
            Assert.IsFalse(tags.IsFalse("area"));

            tags = new TagsCollection();
            tags.Add("area", "true");
            Assert.IsFalse(tags.IsFalse("area"));

            tags = new TagsCollection();
            tags.Add("area", "false");
            Assert.IsTrue(tags.IsFalse("area"));

            tags = new TagsCollection();
            tags.Add("area", "0");
            Assert.IsTrue(tags.IsFalse("area"));

            tags = new TagsCollection();
            tags.Add("area", "no");
            Assert.IsTrue(tags.IsFalse("area"));
        }

        /// <summary>
        /// Tests length parsing.
        /// </summary>
        [Test]
        public void TestSpeedParsing()
        {
            KilometerPerHour result;

            // Official Valid Values
            Assert.AreEqual(true, TagExtensions.TryParseSpeed("30", out result));
            Assert.AreEqual(30, result.Value);

            Assert.AreEqual(true, TagExtensions.TryParseSpeed("30 mph", out result));
            Assert.AreEqual(30, ((MilesPerHour)result).Value);

            Assert.AreEqual(true, TagExtensions.TryParseSpeed("30.8", out result));
            Assert.AreEqual(30.8, result.Value);

            Assert.AreEqual(true, TagExtensions.TryParseSpeed("30.8 mph", out result));
            Assert.AreEqual(30.8, ((MilesPerHour)result).Value);

            // Invalid Values
            Assert.AreEqual(false, TagExtensions.TryParseSpeed("2.3; 7'9\"", out result));
            Assert.AreEqual(false, TagExtensions.TryParseSpeed("2,3", out result));
            Assert.AreEqual(false, TagExtensions.TryParseSpeed("6'", out result));
            Assert.AreEqual(false, TagExtensions.TryParseSpeed("6 ft", out result));
        }

        /// <summary>
        /// Tests maxlength parsing.
        /// </summary>
        [Test]
        public void TestMaxSpeedParsing()
        {
            KilometerPerHour result;

            Assert.AreEqual(true, TagExtensions.TryGetMaxSpeed(new TagsCollection(Tag.Create("maxspeed","30")), out result));
            Assert.AreEqual(30, result.Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxSpeed(new TagsCollection(Tag.Create("maxspeed", "30.8")), out result));
            Assert.AreEqual(30.8, result.Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxSpeed(new TagsCollection(Tag.Create("maxspeed", "30 mph")), out result));
            Assert.AreEqual(30, ((MilesPerHour)result).Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxSpeed(new TagsCollection(Tag.Create("maxspeed", "30.8 mph")), out result));
            Assert.AreEqual(30.8, ((MilesPerHour)result).Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxSpeed(new TagsCollection(Tag.Create("maxspeed", "30 knots")), out result));
            Assert.AreEqual(30, ((Knots)result).Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxSpeed(new TagsCollection(Tag.Create("maxspeed", "30.8 knots")), out result));
            Assert.AreEqual(30.8, ((Knots)result).Value);

            Assert.AreEqual(false, TagExtensions.TryGetMaxSpeed(new TagsCollection(Tag.Create("max", "30.8 knots")), out result));
        }

        /// <summary>
        /// Tests maxweight parsing.
        /// </summary>
        [Test]
        public void TestMaxWeightParsing()
        {
            Kilogram result;

            Assert.AreEqual(true, TagExtensions.TryGetMaxWeight(new TagsCollection(Tag.Create("maxweight", "30")), out result));
            Assert.AreEqual(30 * 1000, result.Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxWeight(new TagsCollection(Tag.Create("maxweight", "30.8")), out result));
            Assert.AreEqual(30.8 * 1000, result.Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxWeight(new TagsCollection(Tag.Create("maxweight", "30 t")), out result));
            Assert.AreEqual(30 * 1000, result.Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxWeight(new TagsCollection(Tag.Create("maxweight", "30.8 t")), out result));
            Assert.AreEqual(30.8 * 1000, result.Value);

            Assert.AreEqual(false, TagExtensions.TryGetMaxWeight(new TagsCollection(Tag.Create("max", "30.8 t")), out result));
        }

        /// <summary>
        /// Tests maxweight axle load.
        /// </summary>
        [Test]
        public void TestMaxAxlLoadParsing()
        {
            Kilogram result;

            Assert.AreEqual(true, TagExtensions.TryGetMaxAxleLoad(new TagsCollection(Tag.Create("maxaxleload", "30")), out result));
            Assert.AreEqual(30 * 1000, result.Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxAxleLoad(new TagsCollection(Tag.Create("maxaxleload", "30.8")), out result));
            Assert.AreEqual(30.8 * 1000, result.Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxAxleLoad(new TagsCollection(Tag.Create("maxaxleload", "30 t")), out result));
            Assert.AreEqual(30 * 1000, result.Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxAxleLoad(new TagsCollection(Tag.Create("maxaxleload", "30.8 t")), out result));
            Assert.AreEqual(30.8 * 1000, result.Value);

            Assert.AreEqual(false, TagExtensions.TryGetMaxAxleLoad(new TagsCollection(Tag.Create("max", "30.8 t")), out result));
        }

        /// <summary>
        /// Tests maxheight parsing.
        /// </summary>
        [Test]
        public void TestMaxHeightParsing()
        {
            Meter result;

            Assert.AreEqual(true, TagExtensions.TryGetMaxHeight(new TagsCollection(Tag.Create("maxheight", "3")), out result));
            Assert.AreEqual(3, result.Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxHeight(new TagsCollection(Tag.Create("maxheight", "3.8")), out result));
            Assert.AreEqual(3.8, result.Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxHeight(new TagsCollection(Tag.Create("maxheight", "6'7\"")), out result));
            Assert.AreEqual(6 * 0.3048 + 7 * 0.0254, result.Value);

            Assert.AreEqual(false, TagExtensions.TryGetMaxHeight(new TagsCollection(Tag.Create("max", "6'7\"")), out result));
        }

        /// <summary>
        /// Tests maxwidth parsing.
        /// </summary>
        [Test]
        public void TestMaxWidthParsing()
        {
            Meter result;

            Assert.AreEqual(true, TagExtensions.TryGetMaxWidth(new TagsCollection(Tag.Create("maxwidth", "3")), out result));
            Assert.AreEqual(3, result.Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxWidth(new TagsCollection(Tag.Create("maxwidth", "3.8")), out result));
            Assert.AreEqual(3.8, result.Value);

            Assert.AreEqual(true, TagExtensions.TryGetMaxWidth(new TagsCollection(Tag.Create("maxwidth", "6'7\"")), out result));
            Assert.AreEqual(6 * 0.3048 + 7 * 0.0254, result.Value);

            Assert.AreEqual(false, TagExtensions.TryGetMaxWidth(new TagsCollection(Tag.Create("max", "6'7\"")), out result));
        }
    }
}
