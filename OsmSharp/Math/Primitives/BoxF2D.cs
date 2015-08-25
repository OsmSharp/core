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
    /// Represents a retangular box with edges always parallel to x- and y-axis.
    /// </summary>
    public class BoxF2D : PrimitiveF2D, IPointList, ILineList, IEnumerable<PointF2D>, IEnumerable<LineF2D>
    {        
        /// <summary>
        /// Holds the maximum values of the box.
        /// </summary>
        private double[] _max;

        /// <summary>
        /// Holds the minimum values of the box.
        /// </summary>
        private double[] _min;

        #region Constructors

        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
		public BoxF2D(double x1, double y1, double x2, double y2)
            : this(new PointF2D(x1, y1), new PointF2D(x2, y2))
        {

        }

        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="a"></param>
        public BoxF2D(PointF2D a)
            : this(new PointF2D[] { a })
        {

        }

        /// <summary>
        /// Creates a new box.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
		public BoxF2D(PointF2D a, PointF2D b)
            :this(new PointF2D[] { a, b })
        {
            
        }

        /// <summary>
        /// Creates a new box around the given points.
        /// </summary>
        /// <param name="points"></param>
		public BoxF2D(PointF2D[] points)
        {
            this.Mutate(points);
        }

        /// <summary>
        /// Creates a new box around the given points.
        /// </summary>
        /// <param name="points"></param>
		public BoxF2D(IList<PointF2D> points)
            : this(points.ToArray<PointF2D>())
        {

        }

        /// <summary>
        /// Creates a new box around the given points.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
		public BoxF2D(double[] x, double[] y)
		{
			PointF2D[] points = new PointF2D[x.Length];
			for (int idx = 0; idx < x.Length; idx++) {
				points [idx] = new PointF2D (x [idx], y [idx]);
			}
			this.Mutate (points);
		}

        #endregion

        #region Mutators

        /// <summary>
        /// Mutates this generic box to another generic box.
        /// </summary>
        /// <param name="points"></param>
        protected void Mutate(
            PointF2D[] points)
        {
            // initialize the maximum array.
            _max = new double[2];

            // initialize the minimum array.
            _min = new double[2];

            // intialize with the first point.
            //PointF2D a = points[0];

            // loop over all points and store the max and minimum.
            for (int idx = 0; idx < 2; idx++)
            {
                _max[idx] = double.MinValue;
                _min[idx] = double.MaxValue;

                for (int p_idx = 0; p_idx < points.Length; p_idx++)
                {
                    PointF2D b = points[p_idx];
                    if (_max[idx] < b[idx])
                    {
                        _max[idx] = b[idx];
                    }
                    if (_min[idx] > b[idx])
                    {
                        _min[idx] = b[idx];
                    }
                }
            }

            return;
        }

        #endregion

        #region Properties
        
        private double[] _delta;

        /// <summary>
        /// Returns the delta (difference) between min and max for every dimension.
        /// </summary>
        public double[] Delta
        {
            get
            {
                if (_delta == null)
                {
                    _delta = new double[2];

                    for (int idx = 0; idx < 2; idx++)
                    {
                        _delta[idx] = System.Math.Abs(_max[idx] - _min[idx]);
                    }
                }
                return _delta;
            }
        }
        
        /// <summary>
        /// Returns the max of this box for each dimension.
        /// </summary>
        public double[] Max
        {
            get
            {
                return _max;
            }
        }

        /// <summary>
        /// Returns the min of this box for each dimension.
        /// </summary>
        public double[] Min
        {
            get
            {
                return _min;
            }
        }


        /// <summary>
        /// Returns all the corners of this box.
        /// </summary>
        public virtual PointF2D[] Corners
        {
            get
            {
                PointF2D[] corners = new PointF2D[(int)System.Math.Pow(2, 2)];
                for (int cnt = 0; cnt < ((int)System.Math.Pow(2, 2)); cnt++)
                {
                    double[] p = new double[2];
                    for (int idx = 0; idx < _max.Length; idx++)
                    {
                        bool max = (((cnt / ((int)System.Math.Pow(2, idx))) 
                            % (int)System.Math.Pow(2, idx + 1)) == 0);
                        if (max)
                        {
                            p[idx] = _max[idx];
                        }
                        else
                        {
                            p[idx] = _min[idx];
                        }
                    }

                    corners[cnt] = new PointF2D(p);
                }

                return corners;
            }
        }

        /// <summary>
        /// Returns the middle of this box.
        /// </summary>
        public PointF2D Middle
        {
            get
            {
                double[] middle = new double[2];

                for (int idx = 0; idx < 2; idx++)
                {
                    middle[idx] = (_max[idx] + _min[idx]) / 2.0f;
                }

                return new PointF2D(middle);
            }
        }

        /// <summary>
        /// Returns the size of the surface of this rectangle.
        /// </summary>
        public double Surface
        {
            get
            {
                double surface = 1;
                foreach (double dimension in this.Delta)
                {
                    surface =
                        dimension * surface;
                }

                return surface;
            }
        }

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates the distance from the given point to this rectangle.
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

        #region Is Inside

		/// <summary>
		/// Returns true if the given coordinates are inside this box.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public bool Contains(double x, double y){
			return this.Contains (new PointF2D (x, y));
		}

        /// <summary>
        /// Returns true if the point lies inside this box.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool Contains(PointF2D a)
        {
            bool isInside = true;

            for (int idx = 0; idx < 2; idx++)
            {
                isInside = isInside && (_max[idx] > a[idx] && a[idx] >= _min[idx]);
            }

            return isInside;
        }

        /// <summary>
        /// Returns true if the given box is completely inside this box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
		public bool Contains(BoxF2D box)
        {
            foreach (PointF2D p in box.Corners)
            {
                if (!this.Contains(p))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the list of points from the given list that are inside the given box.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public List<PointF2D> Contains(List<PointF2D> points)
        {
            List<PointF2D> points_inside = new List<PointF2D>();

            foreach (PointF2D point in points)
            {
                if (this.Contains(point))
                {
                    points_inside.Add(point);
                }
            }

            return points_inside;
        }

        /// <summary>
        /// Returns the list of points from the given list that are inside the given box.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public List<PointF2D> Contains(PointF2D[] points)
        {
            List<PointF2D> points_inside = new List<PointF2D>();

            foreach (PointF2D point in points)
            {
                if (this.Contains(point))
                {
                    points_inside.Add(point);
                }
            }

            return points_inside;
        }

        /// <summary>
        /// Returns true if any of the given points lie inside this bounding box.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public bool ContainsAny(PointF2D[] points)
        {
            if (points != null)
            {
                for (int idx = 0; idx < points.Length; idx++)
                {
                    if (this.Contains(points[idx]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region Overlaps

        /// <summary>
        /// Returns true if the boxes overlap.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
		public bool Overlaps(BoxF2D box)
        {
			double minX = System.Math.Max(this.Min[0], box.Min[0]);
			double minY = System.Math.Max(this.Min[1], box.Min[1]);
			double maxX = System.Math.Min(this.Max[0], box.Max[0]);
			double maxY = System.Math.Min(this.Max[1], box.Max[1]);

            if (minX <= maxX && minY <= maxY)
            {
                return true;
            }
            return false;
        }

        #endregion

        #endregion

        #region Intersects

        /// <summary>
        /// Returns true if the two points could potentially intersect this box.
        /// 
        /// This is a rudemantairy quick test to rule out intersection. 
        ///     - If false is returned there can be no intersection.
        ///     - If true is returned there might be intersection but it is not certain.
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public bool IntersectsPotentially(double x1, double y1, double x2, double y2)
        {
            if (x1 > _max[0] && x2 > _max[0])
            {
                return false;
            }
            if (x1 < _min[0] && x2 < _min[0])
            {
                return false;
            }
            if (y1 > _max[1] && y2 > _max[1])
            {
                return false;
            }
            if (y1 < _min[1] && y2 < _min[1])
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if the two points could potentially intersect this box.
        /// 
        /// This is a rudemantairy quick test to rule out intersection. 
        ///     - If false is returned there can be no intersection.
        ///     - If true is returned there might be intersection but it is not certain.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public bool IntersectsPotentially(PointF2D point1, PointF2D point2)
        {
            for (int idx = 0; idx < 2; idx++)
            {
                if(point1[idx] > _max[idx] && point2[idx] > _max[idx])
                {
                    return false;
                }
                if (point1[idx] < _min[idx] && point2[idx] < _min[idx])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if the two points could potentially intersect this bounding box.
        /// 
        /// This is a rudemantairy quick test to rule out intersection. 
        ///     - If false is returned there can be no intersection.
        ///     - If true is returned there might be intersection but it is not certain.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool IntersectsPotentially(LineF2D line)
        {
            return this.IntersectsPotentially(line.Point1, line.Point2);
        }

        /// <summary>
        /// Returns true if the line intersects with this bounding box.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool Intersects(LineF2D line)
        {
            // if all points have the same relative position with respect to the line
            // there is no intersection. In the other case there is.

            PointF2D[] corners = this.Corners;
            LinePointPosition first_position = line.PositionOfPoint(corners[0]);

            for (int idx = 1; idx <= corners.Length; idx++)
            {
                if (line.PositionOfPoint(corners[idx]) != first_position)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the line intersects with this bounding box.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public bool Intersects(PointF2D point1, PointF2D point2)
        {
            // if all points have the same relative position with respect to the line
            // there is no intersection. In the other case there is.

            PointF2D[] corners = this.Corners;
            LineF2D line = new LineF2D(point1,point2,true);

            LinePointPosition first_position = line.PositionOfPoint(corners[0]);

            for (int idx = 1; idx < corners.Length; idx++)
            {
                if (line.PositionOfPoint(corners[idx]) != first_position)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #endregion

        #region ILineList<PointType> Members

        /// <summary>
        /// Returns the number of lines.
        /// </summary>
        int ILineList.Count
        {
            get 
            {
                return 4; 
            }
        }

        /// <summary>
        /// Returns the line at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        LineF2D ILineList.this[int idx]
        {
            get 
            {
                IPointList this_as_point_list = this;
                switch (idx)
                {
                    case 0:
                        return new LineF2D(
                            this_as_point_list[0],
                            this_as_point_list[1]);
                    case 1:
                        return new LineF2D(
                            this_as_point_list[1],
                            this_as_point_list[2]);
                    case 2:
                        return new LineF2D(
                            this_as_point_list[2],
                            this_as_point_list[3]);
                    case 3:
                        return new LineF2D(
                            this_as_point_list[3],
                            this_as_point_list[0]);
                    default:
                        throw new ArgumentOutOfRangeException();
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

        #region IPointList<PointType> Members
        
        /// <summary>
        /// Returns the number of points.
        /// </summary>
        int IPointList.Count
        {
            get 
            { 
                return 4; 
            }
        }

        /// <summary>
        /// Returns the point at the given index.
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        PointF2D IPointList.this[int idx]
        {
            get 
            {
                switch (idx)
                {
                    case 0:
                        return new PointF2D(new double[] { _min[0], _min[1] });
                    case 1:
                        return new PointF2D(new double[] { _max[0], _min[1] });
                    case 2:
                        return new PointF2D(new double[] { _min[0], _max[1] });
                    case 3:
                        return new PointF2D(new double[] { _max[0], _max[1] });
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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

		/// <summary>
		/// Calculates the intersection between this box and the given box.
		/// </summary>
		/// <param name="box">Box.</param>
		public BoxF2D Intersection (BoxF2D box)
		{// get the highest minimums and the lowest maximums.
			double minX = System.Math.Max(this.Min[0], box.Min[0]);
			double minY = System.Math.Max(this.Min[1], box.Min[1]);
			double maxX = System.Math.Min(this.Max[0], box.Max[0]);
			double maxY = System.Math.Min(this.Max[1], box.Max[1]);

            if (minX <= maxX && minY <= maxY)
            {
				return new BoxF2D(new PointF2D(minX, minY), new PointF2D(maxX, maxY));
            }
		    return null;
		}

		/// <summary>
		/// Calculates the union of this box and the given box or the box that encompasses both original boxes.
		/// </summary>
		/// <param name="box">Box.</param>
		public BoxF2D Union(BoxF2D box)
		{// get the lowest minimums and the highest maximums.
			double minX = System.Math.Min(this.Min[0], box.Min[0]);
			double minY = System.Math.Min(this.Min[1], box.Min[1]);
			double maxX = System.Math.Max(this.Max[0], box.Max[0]);
			double maxY = System.Math.Max(this.Max[1], box.Max[1]);
			
			return new BoxF2D(new PointF2D(minX, minY), new PointF2D(maxX, maxY));
		}

        /// <summary>
        /// Returns a scaled version of this bounding box.
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public BoxF2D ScaleWith(double factor)
        {
            if (factor <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            PointF2D center = this.Middle;

            double diffX = (this.Delta[0] * factor) / 2.0;
            double diffY = (this.Delta[1] * factor) / 2.0;

            return new BoxF2D(
                new PointF2D(
                center[0] - diffX,
                center[1] - diffY),
                new PointF2D(
                center[0] + diffX,
                center[1] + diffY));
        }

        /// <summary>
        /// Resizes this bounding box with the given delta.
        /// </summary>
        /// <param name="delta"></param>
        /// <returns></returns>
        public BoxF2D ResizeWith(double delta)
        {
            return new BoxF2D(
                new PointF2D(this.Max[0] + delta, this.Max[1] + delta),
                new PointF2D(this.Min[0] - delta, this.Min[1] - delta));
        }

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("RectF:[({0},{1}),({2},{3})]",
                                 this.Min[0].ToInvariantString(), this.Min[1].ToInvariantString(),
                                 this.Max[0].ToInvariantString(), this.Max[1].ToInvariantString());
        }

        /// <summary>
        /// Returns an enumerator for PointF2D.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<PointF2D> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator for PointF2D.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator for LineF2D.
        /// </summary>
        /// <returns></returns>
        IEnumerator<LineF2D> IEnumerable<LineF2D>.GetEnumerator()
        {
			List<LineF2D> lines = new List<LineF2D> ();
			lines.Add (new LineF2D (this.Corners [0], this.Corners [1], true));
			lines.Add (new LineF2D (this.Corners [1], this.Corners [2], true));
			lines.Add (new LineF2D (this.Corners [2], this.Corners [3], true));
			lines.Add (new LineF2D (this.Corners [3], this.Corners [0], true));
			return lines.GetEnumerator ();
        }
    }
}
