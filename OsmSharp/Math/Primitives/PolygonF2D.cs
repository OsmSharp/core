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

using System;
using System.Collections.Generic;
using System.Linq;
using OsmSharp.Math.Primitives.Enumerators.Lines;
using OsmSharp.Math.Primitives.Enumerators.Points;

namespace OsmSharp.Math.Primitives
{
    /// <summary>
    /// Represents a polyfon.
    /// </summary>
    public class PolygonF2D : PrimitiveF2D, IPointList, ILineList
    {
        /// <summary>
        /// Holds the array of points representing this polygon.
        /// </summary>
        private PointF2D[] _points;

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        /// <param name="points"></param>
        public PolygonF2D(IList<PointF2D> points)
        {
            // make a copy.
            _points = points.ToArray<PointF2D>();

            if (_points.Length <= 2)
            {
                throw new ArgumentOutOfRangeException("Minimum three points make a polygon!");
            }
        }

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        /// <param name="points"></param>
        public PolygonF2D(PointF2D[] points)
        {
            // make a copy.
            _points = new List<PointF2D>(points).ToArray();
            if (_points.Length <= 2)
            {
                throw new ArgumentOutOfRangeException("Minimum three points make a polygon!");
            }
        }

        /// <summary>
        /// Creates a new polygon.
        /// </summary>
        /// <param name="points"></param>
        public PolygonF2D(IEnumerable<PointF2D> points)
        {
            // make a copy.
            _points = new List<PointF2D>(points).ToArray();
            if (_points.Length <= 2)
            {
                throw new ArgumentOutOfRangeException("Minimum three points make a polygon!");
            }
        }

        #region Properties

        /// <summary>
        /// Returns the point at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public PointF2D this[int idx]
        {
            get
            {
                return _points[idx];
            }
        }

        /// <summary>
        /// Returns the number of points in this list.
        /// </summary>
        public int Count
        {
            get
            {
				return _points.Length;
            }
        }

        /// <summary>
        /// Holds the bounding box for this polygon.
        /// </summary>
		private BoxF2D _bounding_box;

        /// <summary>
        /// Returns the bouding box around this polygon.
        /// </summary>
		public BoxF2D BoundingBox
        {
            get
            {
                if (_bounding_box == null)
                {
					_bounding_box = new BoxF2D(_points);
                }
                return _bounding_box;
            }
        }

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates the distance from the given point to this polygon.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override double Distance(PointF2D p)
        {
            // initialize to the max possible value.
            double distance = double.MaxValue;

            // loop over all lines and store minimum.
            foreach (LineF2D line in this.LineEnumerator)
            {
                // calculate new distance.
                double new_distance = line.Distance(p);

                // keep it if is smaller.
                if (new_distance < distance)
                { // new distance is smaller.
                    distance = new_distance;
                }
            }

            // TODO: what to do when the point is inside the polygon?

            return distance;
        }

        #region IsInside

        /// <summary>
        /// Returns true if the point is inside of the polygon.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsInside(PointF2D point)
        {
            // http://en.wikipedia.org/wiki/Even-odd_rule
            // create a line parallel to the x-axis.
//            PointF2D second_point = new PointF2D(
//                new double[]{point[0] + 10,point[1]});

            // intersect line with polygon.


            return false;
        }

        #endregion

        #region Intersects

        /// <summary>
        /// Returns all the intersections the line has with this polygon.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public PointF2D[] Intersections(LineF2D line)
        {
            List<PointF2D> points = new List<PointF2D>();
            foreach (LineF2D polygon_line in this.LineEnumerator)
            {
                // calculate the intersection.
                PrimitiveF2D primitive = line.Intersection(polygon_line);
                
                // the primitive not null.
                if (primitive != null)
                {
                    if (primitive is LineF2D)
                    { // primitive is a line; convert.                        
                        LineF2D intersect_line =
                            (primitive as LineF2D);

                        // we are sure the line is a segment.
                        // if the line is not a segment this means that the polygon contains an line with infinite length; impossible.

                        // TODO: how to determine the order?
                        points.Add(intersect_line.Point1);
                        points.Add(intersect_line.Point2);                        
                    }
                    else if (primitive is PointF2D)
                    { // primitive is a point; convert.
                        PointF2D point = (primitive as PointF2D);
                        points.Add(point);
                    }
                }
            }

            return points.ToArray();
        }

        #endregion

        #endregion

        #region ILineList Members

        /// <summary>
        /// Returns the number of lines in this polygon.
        /// </summary>
        int ILineList.Count
        {
            get 
            {
                return this.Count;
            }
        }

        /// <summary>
        /// Returns the line at the given idx.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        LineF2D ILineList.this[int idx]
        {
            get
            {
                if (idx < this.Count - 1)
                {
                    return new LineF2D(this[idx], this[idx + 1], true);
                }
                else
                {
                    return new LineF2D(this[idx], this[0], true);
                }
            }
        }

        /// <summary>
        /// Returns the line enumerator.
        /// </summary>
        public LineEnumerable LineEnumerator
        {
            get
            {
                return new LineEnumerable
                    (new LineEnumerator(this));
            }
        }


        #endregion

        #region IPointList<PointF2D> Members

        int IPointList.Count
        {
            get { return this.Count; }
        }

        PointF2D IPointList.this[int idx]
        {
            get { return this[idx]; }
        }

        /// <summary>
        /// Returns the point enumerator.
        /// </summary>
        public PointEnumerable PointEnumerator
        {
            get
            {
                return new PointEnumerable
                    (new PointEnumerator(this));
            }
        }

        #endregion
    }
}
