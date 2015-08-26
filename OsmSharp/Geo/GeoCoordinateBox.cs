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

using OsmSharp.Math.Primitives;
using OsmSharp.Math.Random;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OsmSharp.Math.Geo
{
    /// <summary>
    /// Represents a geo coordinate bounding box.
    /// 
    /// 0: longitude.
    /// 1: latitude.
    /// </summary>
	public class GeoCoordinateBox : BoxF2D
    {
        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="points"></param>
        public GeoCoordinateBox(IList<GeoCoordinate> points)
            : base(points.Cast<PointF2D>().ToArray<PointF2D>())
        {

        }

        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="points"></param>
        public GeoCoordinateBox(GeoCoordinate[] points)
            : base(points)
        {

        }

        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public GeoCoordinateBox(GeoCoordinate a, GeoCoordinate b)
            :base(a,b)
        {

        }

        /// <summary>
        /// Expands this geo coordinate box with the given coordinate.
        /// </summary>
        /// <param name="coordinate"></param>
        public void ExpandWith(GeoCoordinate coordinate)
        {
            if (!this.Contains(coordinate))
            {
                PointF2D[] newCorners = new PointF2D[3];
                newCorners[0] = this.TopLeft;
                newCorners[1] = this.BottomRight;
                newCorners[2] = coordinate;

                this.Mutate(newCorners);
            }
        }
        
        #region Calculations

        /// <summary>
        /// Generates a random point within this box.
        /// </summary>
        /// <returns></returns>
        public GeoCoordinate GenerateRandomIn()
        {
            return this.GenerateRandomIn(OsmSharp.Math.Random.StaticRandomGenerator.Get());
        }

        /// <summary>
        /// Generates a random point within this box.
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        public GeoCoordinate GenerateRandomIn(IRandomGenerator rand)
        {
            double lat = (double)rand.Generate(1.0) * this.DeltaLat;
            double lon = (double)rand.Generate(1.0) * this.DeltaLon;

            return new GeoCoordinate(this.MinLat + lat,
                this.MinLon + lon);
        }        
        
        /// <summary>
        /// Generates a random point within this box.
        /// </summary>
        /// <param name="rand"></param>
        /// <returns></returns>
        public GeoCoordinate GenerateRandomIn(System.Random rand)
        {
            double lat = (double)rand.NextDouble() * this.DeltaLat;
            double lon = (double)rand.NextDouble() * this.DeltaLon;

            return new GeoCoordinate(this.MinLat + lat,
                this.MinLon + lon);
        }

        /// <summary>
        /// Returns a scaled version of this bounding box.
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public GeoCoordinateBox Scale(double factor)
        {
            if (factor <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            GeoCoordinate center = this.Center;

            double diff_lat = (this.DeltaLat * factor) / 2.0;
            double diff_lon = (this.DeltaLon * factor) / 2.0;

            return new GeoCoordinateBox(
                new GeoCoordinate(
                center.Latitude - diff_lat,
                center.Longitude - diff_lon),
                new GeoCoordinate(
                center.Latitude + diff_lat,
                center.Longitude + diff_lon));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns the maximum longitude.
        /// </summary>
        public double MaxLon
        {
            get
            {
                return this.Max[0];
            }
        }

        /// <summary>
        /// Returns the maximum latitude.
        /// </summary>
        public double MaxLat
        {
            get
            {
                return this.Max[1];
            }
        }

        /// <summary>
        /// Returns the minimum longitude.
        /// </summary>
        public double MinLon
        {
            get
            {
                return this.Min[0];
            }
        }

        /// <summary>
        /// Returns the minimum latitude.
        /// </summary>
        public double MinLat
        {
            get
            {
                return this.Min[1];
            }
        }

        /// <summary>
        /// Returns the topleft coordinate.
        /// </summary>
        public GeoCoordinate TopLeft
        {
            get { return new GeoCoordinate(this.MaxLat, this.MinLon); }
        }

        /// <summary>
        /// Returns the topleft coordinate.
        /// </summary>
        public GeoCoordinate TopRight
        {
            get { return new GeoCoordinate(this.MaxLat, this.MaxLon); }
        }

        /// <summary>
        /// Returns the topleft coordinate.
        /// </summary>
        public GeoCoordinate BottomLeft
        {
            get { return new GeoCoordinate(this.MinLat, this.MinLon); }
        }

        /// <summary>
        /// Returns the topleft coordinate.
        /// </summary>
        public GeoCoordinate BottomRight
        {
            get { return new GeoCoordinate(this.MinLat, this.MaxLon); }
        }

        /// <summary>
        /// Returns the width on this box.
        /// </summary>
        public double DeltaLon
        {
            get
            {
                return this.Delta[0];
            }
        }  
        
        /// <summary>
        /// Returns the height on this box.
        /// </summary>
        public double DeltaLat
        {
            get
            {
                return this.Delta[1];
            }
        }

        /// <summary>
        /// Returns the center of this box.
        /// </summary>
        public GeoCoordinate Center 
        {
            get
            {
                return new GeoCoordinate(
                    (this.MaxLat + this.MinLat) / 2f,
                    (this.MaxLon + this.MinLon) / 2f);
            }
        }

        /// <summary>
        /// Returns all the corners of this box.
        /// </summary>
        public override PointF2D[] Corners
        {
            get
            {
                PointF2D[] corners = new PointF2D[4];
                corners[0] = this.TopLeft;
                corners[1] = this.TopRight;
                corners[2] = this.BottomLeft;
                corners[3] = this.BottomRight;
                return corners;
            }
        }

        #endregion

        #region Operators

        /// <summary>
        /// Adds two bounding boxes together yielding as result the smallest box that surrounds both.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static GeoCoordinateBox operator +(GeoCoordinateBox a, GeoCoordinate b)
        {
            List<GeoCoordinate> points = new List<GeoCoordinate>();
            points.AddRange(a.Corners.Cast<GeoCoordinate>());
            points.Add(b);

            return new GeoCoordinateBox(points);
        }

        /// <summary>
        /// Adds two bounding boxes together yielding as result the smallest box that surrounds both.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static GeoCoordinateBox operator +(GeoCoordinateBox a, GeoCoordinateBox b)
        {
            List<GeoCoordinate> points = new List<GeoCoordinate>();
            points.AddRange(a.Corners.Cast<GeoCoordinate>());
            points.AddRange(b.Corners.Cast<GeoCoordinate>());
            
            return new GeoCoordinateBox(points);
        }

        /// <summary>
        /// Subtracts two bounding boxes; returns the box that represents the overlap between the two.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static GeoCoordinateBox operator -(GeoCoordinateBox a, GeoCoordinateBox b)
        {
            if (a.Overlaps(b))
            {
                // sort all longitudes.
                List<double> longitudes = new List<double>();
                longitudes.Add(a.MinLon);
                longitudes.Add(a.MaxLon);
                longitudes.Add(b.MinLon);
                longitudes.Add(b.MaxLon);

                longitudes.Sort();

                // sort all latitudes.
                List<double> latitudes = new List<double>();
                latitudes.Add(a.MinLat);
                latitudes.Add(a.MaxLat);
                latitudes.Add(b.MinLat);
                latitudes.Add(b.MaxLat);

                latitudes.Sort();

                return new GeoCoordinateBox(
                    new GeoCoordinate(latitudes[1], longitudes[1]),
                    new GeoCoordinate(latitudes[2], longitudes[2]));
            }
            return null;
        }

        #endregion

        /// <summary>
        /// Calculates the intersection between this box and the given box.
        /// </summary>
        /// <param name="box">Box.</param>
        public GeoCoordinateBox Intersection(GeoCoordinateBox box)
        {// get the highest minimums and the lowest maximums.
            double minX = System.Math.Max(this.Min[0], box.Min[0]);
            double minY = System.Math.Max(this.Min[1], box.Min[1]);
            double maxX = System.Math.Min(this.Max[0], box.Max[0]);
            double maxY = System.Math.Min(this.Max[1], box.Max[1]);

            if (minX <= maxX && minY <= maxY)
            {
                return new GeoCoordinateBox(new GeoCoordinate(minY, minX), new GeoCoordinate(maxY, maxX));
            }
            return null;
        }

        /// <summary>
        /// Calculates the union of this box and the given box or the box that encompasses both original boxes.
        /// </summary>
        /// <param name="box">Box.</param>
        public GeoCoordinateBox Union(GeoCoordinateBox box)
        {// get the lowest minimums and the highest maximums.
            double minX = System.Math.Min(this.Min[0], box.Min[0]);
            double minY = System.Math.Min(this.Min[1], box.Min[1]);
            double maxX = System.Math.Max(this.Max[0], box.Max[0]);
            double maxY = System.Math.Max(this.Max[1], box.Max[1]);

            return new GeoCoordinateBox(new GeoCoordinate(minY, minX), new GeoCoordinate(maxY, maxX));
        }


        /// <summary>
        /// Resizes this bounding box with the given delta.
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public GeoCoordinateBox Resize(double delta)
        {
            return new GeoCoordinateBox(
                new GeoCoordinate(this.MaxLat + delta, this.MaxLon + delta),
                new GeoCoordinate(this.MinLat - delta, this.MinLon - delta));
        }
    }
}
