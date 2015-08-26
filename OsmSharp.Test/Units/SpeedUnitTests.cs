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

using NUnit.Framework;
using OsmSharp.Units.Distance;
using OsmSharp.Units.Speed;
using OsmSharp.Units.Time;

namespace OsmSharp.Test.Units
{
    /// <summary>
    /// Tests for unit-conversion speed classes.
    /// </summary>
    [TestFixture]
    public class SpeedUnitTests
    {
        /// <summary>
        /// Tests the kilometer per hours class.
        /// </summary>
        [Test]
        public void TestKilometerPerHour()
        {
            // initialize value.
            KilometerPerHour ten = 10;

            // convert from.
            Knots tenInKnots = 5.39956803;
            MeterPerSecond tenInMeterPerSecond = 2.77777777778;
            MilesPerHour tenInMilesPerHour = 6.21371192;

            // test converts to.
            Assert.AreEqual(ten.Value, ((KilometerPerHour)tenInKnots).Value, 0.000001);
            Assert.AreEqual(ten.Value, ((KilometerPerHour)tenInMeterPerSecond).Value, 0.000001);
            Assert.AreEqual(ten.Value, ((KilometerPerHour)tenInMilesPerHour).Value, 0.000001);

            // tests division of distance with time resulting in speed.
            Kilometer twenty = ten * (Hour)2;
            Assert.AreEqual(20, twenty.Value);
            
            // tests some parsing functions.
            KilometerPerHour tenPointFive = 10.5;
            KilometerPerHour tenPointFiveParsed;
            Assert.IsTrue(KilometerPerHour.TryParse(tenPointFive.ToString(), out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);
            Assert.IsTrue(KilometerPerHour.TryParse("10.5", out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);
            Assert.IsTrue(KilometerPerHour.TryParse("10.5 km/h", out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);
            Assert.IsTrue(KilometerPerHour.TryParse("10.5 kmh", out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);
            Assert.IsTrue(KilometerPerHour.TryParse("10.5 kph", out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);
            Assert.IsTrue(KilometerPerHour.TryParse("10.5 kmph", out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);
            Assert.IsTrue(KilometerPerHour.TryParse("10.5km/h", out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);
            Assert.IsTrue(KilometerPerHour.TryParse("10.5kmh", out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);
            Assert.IsTrue(KilometerPerHour.TryParse("10.5kph", out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);
            Assert.IsTrue(KilometerPerHour.TryParse("10.5kmph", out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);

            Assert.IsFalse(KilometerPerHour.TryParse("10.5 m/s", out tenPointFiveParsed));
            Assert.IsFalse(KilometerPerHour.TryParse("10.5 mph", out tenPointFiveParsed));
            Assert.IsFalse(KilometerPerHour.TryParse("10.5 knots", out tenPointFiveParsed));
        }

        /// <summary>
        /// Tests the knots class.
        /// </summary>
        [Test]
        public void TestKnots()
        {
            // initialize value.
            Knots ten = 10;

            // convert from.
            KilometerPerHour tenInKilometerPerHour = 18.52;
            MeterPerSecond tenInMeterPerSecond = 5.14444444444;
            MilesPerHour tenInMilesPerHour = 11.50779;

            // test converts to.
            Assert.AreEqual(ten.Value, ((Knots)tenInKilometerPerHour).Value, 0.000001);
            Assert.AreEqual(ten.Value, ((Knots)tenInMeterPerSecond).Value, 0.000001);
            Assert.AreEqual(ten.Value, ((Knots)tenInMilesPerHour).Value, 0.000001);

            // tests division of distance with time resulting in speed.
            Kilometer distance = ten * (Hour)2;
            Assert.AreEqual(18.52 * 2.0, distance.Value);

            // tests some parsing functions.
            Knots tenPointFive = 10.5;
            Knots tenPointFiveParsed;
            Assert.IsTrue(Knots.TryParse(tenPointFive.ToString(), out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);
            Assert.IsTrue(Knots.TryParse("10.5", out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);
            Assert.IsTrue(Knots.TryParse("10.5 knots", out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);

            Assert.IsFalse(Knots.TryParse("10.5 m/s", out tenPointFiveParsed));
            Assert.IsFalse(Knots.TryParse("10.5 mph", out tenPointFiveParsed));
            Assert.IsFalse(Knots.TryParse("10.5 k/h", out tenPointFiveParsed));
        }

        /// <summary>
        /// Tests the meter per second class.
        /// </summary>
        [Test]
        public void TestMeterPerSecond()
        {
            // initialize value.
            MeterPerSecond ten = 10;

            // convert from.
            Knots tenInKnots = 19.43844492440605;
            KilometerPerHour tenInKilometerPerHour = 36;
            MilesPerHour tenInMilesPerHour = 22.36936292054402;

            // test converts to.
            Assert.AreEqual(ten.Value, ((MeterPerSecond)tenInKnots).Value, 0.000001);
            Assert.AreEqual(ten.Value, ((MeterPerSecond)tenInKilometerPerHour).Value, 0.000001);
            Assert.AreEqual(ten.Value, ((MeterPerSecond)tenInMilesPerHour).Value, 0.000001);

            // tests division of distance with time resulting in speed.
            Kilometer twenty = ten * (Hour)2;
            Assert.AreEqual(tenInKilometerPerHour.Value * 2.0, twenty.Value);

            // tests some parsing functions.
            MeterPerSecond tenPointFive = 10.5;
            MeterPerSecond tenPointFiveParsed;
            Assert.IsTrue(MeterPerSecond.TryParse(tenPointFive.ToString(), out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);
            Assert.IsTrue(MeterPerSecond.TryParse("10.5", out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);
            Assert.IsTrue(MeterPerSecond.TryParse("10.5 m/s", out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);
            Assert.IsTrue(MeterPerSecond.TryParse("10.5m/s", out tenPointFiveParsed));
            Assert.AreEqual(tenPointFive.Value, tenPointFiveParsed.Value);

            Assert.IsFalse(MeterPerSecond.TryParse("10.5 km/h", out tenPointFiveParsed));
            Assert.IsFalse(MeterPerSecond.TryParse("10.5 mph", out tenPointFiveParsed));
            Assert.IsFalse(MeterPerSecond.TryParse("10.5 knots", out tenPointFiveParsed));
        }
    }
}
