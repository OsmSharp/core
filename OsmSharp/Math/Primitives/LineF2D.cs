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

namespace OsmSharp.Math.Primitives
{
    /// <summary>
    /// Represents a line.
    /// </summary>
    public class LineF2D : PrimitiveF2D
    {
        /// <summary>
        /// The first point for this line.
        /// </summary>
        private PointF2D _a;

        /// <summary>
        /// The second point for this line.
        /// </summary>
        private PointF2D _b;

        /// <summary>
        /// The direction of this line from point1 -> point2.
        /// </summary>
        private VectorF2D _dir;

        /// <summary>
        /// True if this represents only a segment.
        /// </summary>
        private bool _is_segment1;

        /// <summary>
        /// True if this represents only a segment.
        /// </summary>
        private bool _is_segment2;

        #region Constructors

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public LineF2D(PointF2D a, PointF2D b)
            : this(a,b,false)
        {

        }

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="is_segment"></param>
        public LineF2D(PointF2D a, PointF2D b, bool is_segment)
        {
            _a = a;
            _b = b;

            _dir = _b - _a;
            _is_segment1 = is_segment;
            _is_segment2 = is_segment;
        }

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="is_segment1"></param>
        /// <param name="is_segment2"></param>
        public LineF2D(PointF2D a, PointF2D b, bool is_segment1, bool is_segment2)
        {
            _a = a;
            _b = b;

            _dir = _b - _a;
            _is_segment1 = is_segment1;
            _is_segment2 = is_segment2;
        }

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        /// <param name="xb"></param>
        /// <param name="yb"></param>
        public LineF2D(double xa, double ya, double xb, double yb)
            : this(new PointF2D(xa, ya), new PointF2D(xb, yb)) { }
        
        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        /// <param name="xb"></param>
        /// <param name="yb"></param>
        /// <param name="is_segment"></param>
        public LineF2D(double xa, double ya, double xb, double yb, bool is_segment)
            : this(new PointF2D(xa, ya), new PointF2D(xb, yb), is_segment) { }

        /// <summary>
        /// Creates a new line.
        /// </summary>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        /// <param name="xb"></param>
        /// <param name="yb"></param>
        /// <param name="is_segment1"></param>
        /// <param name="is_segment2"></param>
        public LineF2D(double xa, double ya, double xb, double yb, bool is_segment1, bool is_segment2)
            : this(new PointF2D(xa, ya), new PointF2D(xb, yb), is_segment1, is_segment2) { }

        #endregion

        #region Properties
        
        /// <summary>
        /// Returns the first point of this line.
        /// </summary>
        public PointF2D Point1
        {
            get
            {
                return _a;
            }
        }

        /// <summary>
        /// Returns the second point of this line.
        /// </summary>
        public PointF2D Point2
        {
            get
            {
                return _b;
            }
        }

        /// <summary>
        /// Returns the direction of this line (point1 -> point2).
        /// </summary>
        public VectorF2D Direction
        {
            get
            {
                return _dir;
            }
        }

        /// <summary>
        /// Returns the length of this line (as if it were a segment).
        /// </summary>
        public double Length
        {
            get
            {
                return this.Direction.Size;
            }
        }

        /// <summary>
        /// Returns true if this line is just a segment.
        /// </summary>
        public bool IsSegment
        {
            get
            {
                return _is_segment1 && _is_segment2;
            }
        }

        /// <summary>
        /// Returns true if the first point is the end of the line.
        /// </summary>
        /// <returns></returns>
        public bool IsSegment1
        {
            get
            {
                return _is_segment1;
            }
        }

        /// <summary>
        /// Returns true if the second point is the end of the line.
        /// </summary>
        /// <returns></returns>
        public bool IsSegment2
        {
            get
            {
                return _is_segment2;
            }
        }

        #region Line-Equation Parameters

        /// <summary>
        /// Returns parameter A of an equation describing this line as Ax + By = C
        /// </summary>
        internal double A
        {
            get
            {
                return this.Point2[1] - this.Point1[1];
            }
        }

        /// <summary>
        /// Returns parameter B of an equation describing this line as Ax + By = C
        /// </summary>
        internal double B
        {
            get
            {
                return this.Point1[0] - this.Point2[0];
            }
        }

        /// <summary>
        /// Returns parameter C of an equation describing this line as Ax + By = C
        /// </summary>
        internal double C
        {
            get
            {
                return this.A * this.Point1[0] + this.B * this.Point1[1];
            }
        }

        #endregion

        #endregion

        #region Calculations

        /// <summary>
        /// Calculates the position of this point relative to this line.
        /// 
        /// Left/Right is viewed from point1 in the direction of point2.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public LinePointPosition PositionOfPoint(PointF2D point)
        {
            VectorF2D a_point = point - _a;

            double dot_value = VectorF2D.Cross(this.Direction, a_point);
            if (dot_value > 0)
            {
                return LinePointPosition.Left;
            }
            else if(dot_value < 0)
            {
                return LinePointPosition.Right;
            }
            else
            {
                return LinePointPosition.On;
            }
        }

        /// <summary>
        /// Returns the distance from the point to this line.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override double Distance(PointF2D point)
        {
            double distance = 0.0f;

            // get the second vector for the cross product.
            VectorF2D a_point = point - _a;

            // get the cross product.
            double cross = VectorF2D.Cross(this.Direction, a_point);
            
            // distances between a, b and point.
            double distance_a_b = this.Length;
            double distance_a = point.Distance(_a);
            double distance_b = point.Distance(_b);

            // calculate distance to line as if it were no segment.
            distance = System.Math.Abs(cross / distance_a_b);

            // if this line is a segment.
            if(this.IsSegment)
            {
                double dot1 = VectorF2D.Dot(a_point, this.Direction);
                if (dot1 < 0 && cross != 0)
                {
                    distance = _a.Distance(point);
                }
                else if (cross == 0 && 
                    (distance_a >= distance_a_b
                    || distance_b >= distance_a_b))
                { // check if the point is between the points.
                    if (distance_a > distance_b)
                    { // if the distance to a is greater then the point is at the b-side.
                        distance = _b.Distance(point);
                    }
                    else
                    {// if the distance to b is greater then the point is at the a-side.
                        distance = _a.Distance(point);
                    }
                }
                VectorF2D b_point = point - _b;
                double dot2 = VectorF2D.Dot(b_point, this.Direction.Inverse);
                if (dot2 < 0 && cross != 0)
                {
                    distance = _b.Distance(point);
                } 
            }
            return distance;
        }

        #region Intersections

		/// <summary>
		/// Return true if the given line intersects with this line.
		/// </summary>
		/// <param name="line">Line.</param>
		public bool Intersects(LineF2D line){
			return this.Intersection (line) != null;
		}

		/// <summary>
		/// Return true if the given line intersects with this line.
		/// </summary>
		/// <param name="line">Line.</param>
		/// <param name="doSegment"></param>
		public bool Intersects(LineF2D line, bool doSegment){
			return this.Intersection (line, doSegment) != null;
		}

        /// <summary>
        /// Calculates and returns the line intersection.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public PrimitiveF2D Intersection(LineF2D line)
        {
            return this.Intersection(line, true);
        }

        /// <summary>
        /// Calculates and returns the line intersection.
        /// </summary>
        /// <param name="line"></param>
		/// <param name="doSegment"></param>
        /// <returns></returns>
		public PrimitiveF2D Intersection(LineF2D line, bool doSegment)
        {
            if (line == this)
            { // if the lines equal, the full lines intersect.
                return line;
            }
            else if (line.A == this.A
                    && line.B == this.B
                    && line.C == this.C)
            { // if the lines equal in direction and position; return the smallest segment.
                KeyValuePair<double, PointF2D> point1 = new KeyValuePair<double, PointF2D>(
                    0, this.Point1);

                KeyValuePair<double, PointF2D> point2 = new KeyValuePair<double, PointF2D>(
                     this.Point1.Distance(this.Point2), this.Point2);

                // sort.
                KeyValuePair<double, PointF2D> temp;
                if (point2.Key < point1.Key)
                { // point2 smaller than point1.
                    temp = point2;
                    point2 = point1;
                    point1 = temp;
                }

                KeyValuePair<double, PointF2D> point = new KeyValuePair<double, PointF2D>(
                     this.Point1.Distance(line.Point1), line.Point1);

                if (point.Key < point2.Key) // sort.
                { // point smaller than point2.
                    temp = point;
                    point = point2;
                    point2 = temp;
                }
                if (point2.Key < point1.Key)
                { // point2 smaller than point1.
                    temp = point2;
                    point2 = point1;
                    point1 = temp;
                }

                point = new KeyValuePair<double, PointF2D>(
                     this.Point1.Distance(line.Point2), line.Point2);

                if (point.Key < point2.Key) // sort.
                { // point smaller than point2.
                    temp = point;
                    point = point2;
                    point2 = temp;
                }
                if (point2.Key < point1.Key)
                { // point2 smaller than point1.
                    temp = point2;
                    point2 = point1;
                    point1 = temp;
                }

                // this line is a segment.
                if (doSegment && (this.IsSegment1 || this.IsSegment2))
                { // test where the intersection lies.
                    double this_distance =
                        this.Point1.Distance(this.Point2);
                    if (this.IsSegment)
                    { // if in any directions one of the points are further away from the point.
                        double this_distance_1 = System.Math.Min(this.Point1.Distance(point1.Value),
                            this.Point1.Distance(point2.Value));
                        if (this_distance_1 > this_distance)
                        { // the point is further away.
                            return null;
                        }
                    }
                    else if (this.IsSegment1)
                    { // if in any directions one of the points are further away from the point.
                        double this_distance_1 = this.Point1.Distance(point1.Value);
                        if (this_distance_1 > this_distance)
                        { // the point is further away.
                            return null;
                        }
                    }
                    else if (this.IsSegment2)
                    { // if in any directions one of the points are further away from the point.
                        double this_distance_2 = this.Point2.Distance(point1.Value);
                        if (this_distance_2 > this_distance)
                        { // the point is further away.
                            return null;
                        }
                    }
                }
                // other line is a segment.
                if (doSegment && (line.IsSegment1 || line.IsSegment2))
                { // test where the intersection lies.
                    double this_distance =
                        line.Point1.Distance(line.Point2);
                    if (line.IsSegment)
                    {// if in any directions one of the points are further away from the point.
                        double this_distance_1 = System.Math.Min(line.Point1.Distance(point1.Value),
                            line.Point1.Distance(point2.Value));
                        if (this_distance_1 > this_distance)
                        { // the point is further away.
                            return null;
                        }
                    }
                    else if (line.IsSegment1)
                    { // if in any directions one of the points are further away from the point.
                        double this_distance_1 = line.Point1.Distance(point1.Value);
                        if (this_distance_1 > this_distance)
                        { // the point is further away.
                            return null;
                        }
                    }
                    else if (line.IsSegment2)
                    { // if in any directions one of the points are further away from the point.
                        double this_distance_2 = line.Point2.Distance(point2.Value);
                        if (this_distance_2 > this_distance)
                        { // the point is further away.
                            return null;
                        }
                    }
                }
                return new LineF2D(
                    point1.Value,
                    point2.Value,
                    true);
            }
            else
            { // line.A = A1, line.B = B1, line.C = C1, this.A = A2, this.B = B2, this.C = C2
                double det = (line.A * this.B - this.A * line.B);
                if (det == 0) // TODO: implement an accuracy threshold epsilon.
                { // lines are parallel; no intersections.
                    return null;
                }
                else
                { // lines are not the same and not parallel so they will intersect.
                    double x = (this.B * line.C - line.B * this.C) / det;
                    double y = (line.A * this.C - this.A * line.C) / det;

                    // create the point.
                    PointF2D point = new PointF2D(new double[]{x, y});

                    // this line is a segment.
                    if (doSegment && (this.IsSegment1 || this.IsSegment2))
                    { // test where the intersection lies.
                        double this_distance =
                            this.Point1.Distance(this.Point2);
                        if (this.IsSegment)
                        {// if in any directions one of the points are further away from the point.
                            double this_distance_1 = this.Point1.Distance(point);
                            if (this_distance_1 > this_distance)
                            { // the point is further away.
                                return null;
                            }
                            double this_distance_2 = this.Point2.Distance(point);
                            if (this_distance_2 > this_distance)
                            { // the point is further away.
                                return null;
                            }
                        }
                        else if (this.IsSegment1 && this.Point2.Distance(point) > this_distance)
                        { // only check this direction and only if the points lies far enough away from point2.
                            VectorF2D pointDirection = point - this.Point2;
                            VectorF2D point2ToPoint1 = this.Direction.Inverse;
                            if (pointDirection.CompareNormalized(point2ToPoint1, 0.001))
                            { // point lies in the direction of the segmented point1 and far away from point2
                                return null;
                            }
                        }
                        else if (this.IsSegment1 && this.Point1.Distance(point) > this_distance)
                        { // only check this direction and only if the points lies far enough away from point1.
                            VectorF2D pointDirection = point - this.Point1;
                            VectorF2D point1ToPoint2 = this.Direction;
                            if (pointDirection.CompareNormalized(point1ToPoint2, 0.001))
                            { // point lies in the direction of the segmented point1 and far away from point2
                                return null;
                            }
                        }
                    }
                    // line this is a segment.
                    if (doSegment && (line.IsSegment1 || line.IsSegment2))
                    { // test where the intersection lies.
                        double line_distance =
                            line.Point1.Distance(line.Point2);
                        if (line.IsSegment)
                        {// if in any directions one of the points are further away from the point.
                            double line_distance_1 = line.Point1.Distance(point);
                            if (line_distance_1 > line_distance)
                            { // the point is further away.
                                return null;
                            }
                            double line_distance_2 = line.Point2.Distance(point);
                            if (line_distance_2 > line_distance)
                            { // the point is further away.
                                return null;
                            }
                        }
                        else if (line.IsSegment1 && line.Point2.Distance(point) > line_distance)
                        { // only check line direction and only if the points lies far enough away from point2.
                            VectorF2D pointDirection = point - line.Point2;
                            VectorF2D point2ToPoint1 = line.Direction.Inverse;
                            if (pointDirection.CompareNormalized(point2ToPoint1, 0.001))
                            { // point lies in the direction of the segmented point1 and far away from point2
                                return null;
                            }
                        }
                        else if (line.IsSegment1 && line.Point1.Distance(point) > line_distance)
                        { // only check line direction and only if the points lies far enough away from point1.
                            VectorF2D pointDirection = point - line.Point1;
                            VectorF2D point1ToPoint2 = line.Direction;
                            if (pointDirection.CompareNormalized(point1ToPoint2, 0.001))
                            { // point lies in the direction of the segmented point1 and far away from point2
                                return null;
                            }
                        }
                    }

                    // the intersection is valid.
                    return point;
                }
            }
        }

        /// <summary>
        /// Projects a point onto the given line.
        /// 
        /// (= intersection of the line with an angle 90° different from this line through the given point)
        /// </summary>
        /// <param name="point"></param>
        /// <returns>The projection point if it occurs inside the segmented line.</returns>
        public PointF2D ProjectOn(PointF2D point)
        {
            if(this.Length == 0 && this.IsSegment)
            { // cannot project on a line of length zero.
                return null;
            }

            // get the direction.
            VectorF2D direction = this.Direction;

            // rotate.
            VectorF2D rotated = direction.Rotate90(true);

            // create second point
            PointF2D point2 = new PointF2D((point + rotated).ToArray());

            if (point[0] != point2[0] || point[1] != point2[1])
            {
                // create line.
                LineF2D line = new LineF2D(
                    point,
                    point2,
                    false,
                    false);

                // intersect.
                PrimitiveF2D primitive =
                    this.Intersection(line, true);

                if (primitive == null)
                {
                    return null;
                }
                else if (primitive is PointF2D)
                {
                    return primitive as PointF2D;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                return point2;
            }
        }

        #endregion

        #endregion

        /// <summary>
        /// Returns a description of this line.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Line{0}{1},{2}{3}",
                this.IsSegment1 ? "[" : "]",
                this.Point1.ToString(),
                this.Point2.ToString(),
                this.IsSegment2 ? "]" : "[");
        }
    }

    /// <summary>
    /// The line-point positions.
    /// </summary>
    public enum LinePointPosition
    {
        /// <summary>
        /// Left.
        /// </summary>
        Left,
        /// <summary>
        /// Right.
        /// </summary>
        Right,
        /// <summary>
        /// On.
        /// </summary>
        On
    }
}
