// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using OsmSharp.Geo;
using OsmSharp.Math.Geo.Meta;
using OsmSharp.Math.Primitives;
using OsmSharp.Math.Random;
using OsmSharp.Units.Angle;
using OsmSharp.Units.Distance;

namespace OsmSharp.Math.Geo
{
    /// <summary>
    /// Represents a standard geo coordinate.
    /// 
    /// 0: longitude.
    /// 1: latitude.
    /// </summary>
    public class GeoCoordinate : PointF2D, ICoordinate
    {
        /// <summary>
        /// Creates a geo coordinate.
        /// </summary>
        public GeoCoordinate(double[] values)
            : base(values)
        {

        }        
        
        /// <summary>
        /// Creates a geo coordinate from another coordinate.
        /// </summary>
        public GeoCoordinate(ICoordinate coordinate)
            : this(coordinate.Latitude, coordinate.Longitude)
        {

        }

        /// <summary>
        /// Creates a geo coordinate.
        /// </summary>
        public GeoCoordinate(PointF2D point)
            : this(point[1], point[0])
        {

        }

        /// <summary>
        /// Creates a geo coordinate.
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        public GeoCoordinate(double latitude, double longitude)
            : base(new double[]{ longitude, latitude })
        {

        }

        #region Properties

        /// <summary>
        /// Gets/Sets the longitude.
        /// </summary>
        public double Longitude
        {
            get
            {
                return this[0];
            }
            //set
            //{
            //    this[0] = value;
            //}
        }

        /// <summary>
        /// Gets/Sets the latitude.
        /// </summary>
        public double Latitude
        {
            get
            {
                return this[1];
            }
            //set
            //{
            //    this[1] = value;
            //}
        }

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates the distance between this point and the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double Distance(GeoCoordinate point)
        {
            return PointF2D.Distance(this, point);
        }

        /// <summary>
        /// Estimates the distance between location1 and location2 in meters.
        /// Accuracy decreases with distance.
        /// </summary>
        /// <param name="location1"></param>
        /// <param name="location2"></param>
        /// <returns></returns>
        public static double DistanceEstimateInMeter(ICoordinate location1,
            ICoordinate location2)
        {
            return GeoCoordinate.DistanceEstimateInMeter(location1.Latitude, location1.Longitude,
                location2.Latitude, location2.Longitude);
        }

        /// <summary>
        /// Estimates the distance between lat1/lon1 and lat2/lon2 in meters.
        /// Accuracy decreases with distance.
        /// </summary>
        /// <param name="latitude1"></param>
        /// <param name="longitude1"></param>
        /// <param name="latitude2"></param>
        /// <param name="longitude2"></param>
        /// <returns></returns>
        public static double DistanceEstimateInMeter(double latitude1, double longitude1, 
            double latitude2, double longitude2)
        {
            var radiusEarth = Constants.RadiusOfEarth;

            var lat1Rad = (latitude1 / 180d) * System.Math.PI;
            var lon1Rad = (longitude1 / 180d) * System.Math.PI;
            var lat2Rad = (latitude2 / 180d) * System.Math.PI;
            var lon2Rad = (longitude2 / 180d) * System.Math.PI;

            var x = (lon2Rad - lon1Rad) * System.Math.Cos((lat1Rad + lat2Rad) / 2.0);
            var y = lat2Rad - lat1Rad;

            var m = System.Math.Sqrt(x * x + y * y) * radiusEarth;

            return m;
        }

        /// <summary>
        /// Estimates the distance between this point and the given point in meters.
        /// Accuracy decreases with distance.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Meter DistanceEstimate(GeoCoordinate point)
        {
            return GeoCoordinate.DistanceEstimateInMeter(this.Latitude, this.Longitude,
                point.Latitude, point.Longitude);
        }

        /// <summary>
        /// Calculates the real distance in meters between this point and the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        /// <remarks>http://en.wikipedia.org/wiki/Haversine_formula</remarks>
        public Meter DistanceReal(GeoCoordinate point)
        {
            Meter radius_earth = Constants.RadiusOfEarth;

            Radian lat1_rad = new Degree(this.Latitude);
            Radian lon1_rad = new Degree(this.Longitude);
            Radian lat2_rad = new Degree(point.Latitude);
            Radian lon2_rad = new Degree(point.Longitude);

            double dLat = (lat2_rad - lat1_rad).Value;
            double dLon = (lon2_rad - lon1_rad).Value;

            double a = System.Math.Pow(System.Math.Sin(dLat / 2), 2) +
                       System.Math.Cos(lat1_rad.Value) * System.Math.Cos(lat2_rad.Value) *
                       System.Math.Pow(System.Math.Sin(dLon / 2), 2);

            double c = 2 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1 - a));

            double distance = radius_earth.Value * c;

            return distance;
        }

        /// <summary>
        /// Offset this coordinate with the given distance in meter in both lat-lon directions.
        /// The difference in distance will be sqrt(2) * meter.
        /// </summary>
        /// <param name="meter"></param>
        /// <returns></returns>
        public GeoCoordinate OffsetWithDistances(Meter meter)
        {
            GeoCoordinate offsetLat = new GeoCoordinate(this.Latitude + 0.1,
                                          this.Longitude);
            GeoCoordinate offsetLon = new GeoCoordinate(this.Latitude,
                                          this.Longitude + 0.1);
            Meter latDistance = offsetLat.DistanceReal(this);
            Meter lonDistance = offsetLon.DistanceReal(this);

            return new GeoCoordinate(this.Latitude + (meter.Value / latDistance.Value) * 0.1,
                this.Longitude + (meter.Value / lonDistance.Value) * 0.1);
        }

        /// <summary>
        /// Offset this coordinate with the given distance in meter along the provided direction.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public GeoCoordinate OffsetWithDirection(Meter distance, DirectionEnum direction)
        {
            double radius_earth = Constants.RadiusOfEarth;
            Radian ratio = distance.Value / radius_earth;

            Radian oldLat = (Degree)(this.Latitude);
            Radian oldLon = (Degree)(this.Longitude);
            Radian bearing = (Degree)(int)direction;

            Radian newLatitude = System.Math.Asin(
                                     System.Math.Sin(oldLat.Value) *
                                     System.Math.Cos(ratio.Value) +
                                     System.Math.Cos(oldLat.Value) *
                                     System.Math.Sin(ratio.Value) *
                                     System.Math.Cos(bearing.Value));

            Radian newLongitude = oldLon.Value + System.Math.Atan2(
                                      System.Math.Sin(bearing.Value) *
                                      System.Math.Sin(ratio.Value) *
                                      System.Math.Cos(oldLat.Value), 
                                      System.Math.Cos(ratio.Value) -
                                      System.Math.Sin(oldLat.Value) *
                                      System.Math.Sin(newLatitude.Value));
                
            // TODO: make this work in other hemispheres
            var newLat = ((Degree)newLatitude).Value;
            if(newLat > 180)
            {
                newLat = newLat - 360;
            }
            var newLon = ((Degree)newLongitude).Value;
            if (newLon > 180)
            {
                newLon = newLon - 360;
            }
            return new GeoCoordinate(newLat, newLon);
        }

        /// <summary>
        /// Offsets this coordinate in a random direction.
        /// </summary>
        /// <param name="meter"></param>
        /// <returns></returns>
        public GeoCoordinate OffsetRandom(Meter meter)
        {
            return this.OffsetRandom(OsmSharp.Math.Random.StaticRandomGenerator.Get(), meter);
        }

        /// <summary>
        /// Offsets this coordinate in a random direction.
        /// </summary>
        /// <param name="randomGenerator"></param>
        /// <param name="meter"></param>
        /// <returns></returns>
        public GeoCoordinate OffsetRandom(IRandomGenerator randomGenerator, Meter meter)
        {
            GeoCoordinate offsetCoordinate = this.OffsetWithDistances(meter.Value /
                                             System.Math.Sqrt(2));
            double offsetLat = offsetCoordinate.Latitude - this.Latitude;
            double offsetLon = offsetCoordinate.Longitude - this.Longitude;

            offsetLat = (1.0 - randomGenerator.Generate(2.0)) * offsetLat;
            offsetLon = (1.0 - randomGenerator.Generate(2.0)) * offsetLon;

            return new GeoCoordinate(this.Latitude + offsetLat, this.Longitude + offsetLon);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Adds two geo coordinates.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static GeoCoordinate operator +(GeoCoordinate a, GeoCoordinate b)
        {
            double[] c = new double[2];

            for (int idx = 0; idx < 2; idx++)
            {
                c[idx] = a[idx] + b[idx];
            }

            return new GeoCoordinate(c);
        }

        /// <summary>
        /// Divides the given geo coordinate with the given value.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static GeoCoordinate operator /(GeoCoordinate a, double value)
        {
            double[] c = new double[2];

            for (int idx = 0; idx < 2; idx++)
            {
                c[idx] = a[idx] / value;
            }

            return new GeoCoordinate(c);
        }

        #endregion

        /// <summary>
        /// Returns a description of this coordinate.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[{0},{1}]",
                this.Latitude.ToInvariantString(),
                this.Longitude.ToInvariantString());
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.Latitude.GetHashCode() ^
            this.Longitude.GetHashCode();
        }

        /// <summary>
        /// Returns true if both objects are equal in value.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is GeoCoordinate)
            {
                return (obj as GeoCoordinate).Latitude.Equals(this.Latitude) &&
                (obj as GeoCoordinate).Longitude.Equals(this.Longitude);
            }
            return false;
        }

        float ICoordinate.Latitude
        {
            get { return (float)this.Latitude; }
        }

        float ICoordinate.Longitude
        {
            get { return (float)this.Longitude; }
        }
    }
}