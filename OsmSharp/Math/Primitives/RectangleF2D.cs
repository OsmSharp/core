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
using OsmSharp.Math.Algorithms;
using OsmSharp.Units.Angle;

namespace OsmSharp.Math.Primitives
{
	/// <summary>
	/// Represents a rectangle.
	/// </summary>
	/// <remarks>This is not a Rectangle in the traditional sense, this rectangle can be tilted.</remarks>
	public class RectangleF2D : PrimitiveF2D
	{
		/// <summary>
		/// Holds the vector of the direction of the rectangle relative to the x-axis and it's size is the 'width' along this direction.
		/// </summary>
		private VectorF2D _vectorX;

		/// <summary>
		/// Holds the vector of the direction of the rectangle relative to the y-axis and it's size is the 'height' along this direction.
		/// </summary>
		/// <remarks>Both directional vectors are stored for performance reasons.</remarks>
		private VectorF2D _vectorY;

		/// <summary>
		/// Holds the bottom left point of the rectangle.
		/// </summary>
		/// <remarks>The other corners can be calculate using the directional vectors.</remarks>
		private PointF2D _bottomLeft;

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Math.Primitives.RectangleF2D"/> class.
		/// </summary>
		/// <param name="x">The x coordinate of the bottom-left corner.</param>
		/// <param name="y">The y coordinate of the bottom-left corner.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <remarks>This creates a rectangle in the direction of the x- and y-axis, performance is almost always better when using <see cref="OsmSharp.Math.Primitives.BoxF2D"/> in this case.</remarks>
		public RectangleF2D(double x, double y, double width, double height){
			_bottomLeft = new PointF2D (x, y);
			_vectorX = new VectorF2D (width, 0);
			_vectorY = new VectorF2D (0, height);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Math.Primitives.RectangleF2D"/> class.
		/// </summary>
		/// <param name="x">The x coordinate of the bottom-left corner.</param>
		/// <param name="y">The y coordinate of the bottom-left corner.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="angleY">The angle relative to the y-axis.</param>
		public RectangleF2D(double x, double y, double width, double height, Degree angleY){
			_bottomLeft = new PointF2D (x, y);
			VectorF2D directionY = VectorF2D.FromAngleY (angleY);
			_vectorY = directionY * height;
			_vectorX = directionY.Rotate90 (true) * width;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Math.Primitives.RectangleF2D"/> class.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="directionY">Direction y.</param>
		public RectangleF2D(double x, double y, double width, double height, VectorF2D directionY) {
			_bottomLeft = new PointF2D (x, y);
			directionY = directionY.Normalize ();
			_vectorY = directionY * height;
			_vectorX = directionY.Rotate90 (true) * width;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OsmSharp.Math.Primitives.RectangleF2D"/> class.
		/// </summary>
		/// <param name="bottomLeft">Bottom left.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="directionY">Direction y.</param>
		public RectangleF2D(PointF2D bottomLeft, double width, double height, VectorF2D directionY){
			_bottomLeft = bottomLeft;
			VectorF2D directionYNormal = directionY.Normalize ();
			_vectorY = directionYNormal * height;
			_vectorX = directionYNormal.Rotate90 (true) * width;
		}

        /// <summary>
        /// Creates a new RectangleF2D from given bounds, center and angle.
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="angleY">The angle.</param>
        /// <returns></returns>
        public static RectangleF2D FromBoundsAndCenter(double width, double height, double centerX, double centerY, Degree angleY)
        {
            return RectangleF2D.FromBoundsAndCenter(width, height, centerX, centerY, VectorF2D.FromAngleY(angleY));
        }

        /// <summary>
        /// Creates a new RectangleF2D from given bounds, center and direction.
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="directionY">The direction of the y-axis.</param>
        /// <returns></returns>
        public static RectangleF2D FromBoundsAndCenter(double width, double height, double centerX, double centerY, VectorF2D directionY)
        {
            VectorF2D directionYNormal = directionY.Normalize();
            VectorF2D directionXNormal = directionYNormal.Rotate90(true);
            PointF2D center = new PointF2D(centerX, centerY);
            PointF2D bottomLeft = center - (directionYNormal * (height / 2)) - (directionXNormal * (width / 2));
            return new RectangleF2D(bottomLeft, width, height, directionY);
        }

		/// <summary>
		/// Gets the bottom left.
		/// </summary>
		/// <value>The bottom left.</value>
		public PointF2D BottomLeft{
			get{
				return _bottomLeft;
			}
		}

		/// <summary>
		/// Gets the top left.
		/// </summary>
		/// <value>The top left.</value>
		public PointF2D TopLeft {
			get{
				return _bottomLeft + _vectorY;
			}
		}

		/// <summary>
		/// Gets the bottom right.
		/// </summary>
		/// <value>The bottom right.</value>
		public PointF2D BottomRight{
			get{
				return _bottomLeft + _vectorX;
			}
		}

		/// <summary>
		/// Gets the top right.
		/// </summary>
		/// <value>The top right.</value>
		public PointF2D TopRight {
			get{
				return _bottomLeft + _vectorX + _vectorY;
			}
		}

		/// <summary>
		/// Gets the center.
		/// </summary>
		/// <value>The center.</value>
		public PointF2D Center {
			get{
				return _bottomLeft + _vectorX / 2.0 + _vectorY / 2.0;
			}
		}

		/// <summary>
		/// Gets the width.
		/// </summary>
		/// <value>The width.</value>
		public double Width{
			get{
				return _vectorX.Size;
			}
		}

		/// <summary>
		/// Gets the height.
		/// </summary>
		/// <value>The height.</value>
		public double Height{
			get{
				return _vectorY.Size;
			}
		}

		/// <summary>
		/// Gets the angle.
		/// </summary>
		/// <value>The angle.</value>
		public Degree Angle {
			get {
				return _vectorY.Angle (new VectorF2D (0, 1));
			}
		}

		/// <summary>
		/// Returns the bouding box around this rectangle.
		/// </summary>
		public BoxF2D BoundingBox
		{
			get
			{
				return new BoxF2D(new PointF2D[] { this.BottomLeft, this.TopRight, this.BottomRight, this.TopLeft });
			}
		}

		/// <summary>
		/// Gets the direction x.
		/// </summary>
		/// <value>The direction x.</value>
		public VectorF2D DirectionX {
			get{
				return _vectorX;
			}
		}

		/// <summary>
		/// Gets the direction y.
		/// </summary>
		/// <value>The direction y.</value>
		public VectorF2D DirectionY {
			get{
				return _vectorY;
			}
		}

        /// <summary>
        /// Fits this rectangle to the given points.
        /// </summary>
        /// <param name="points">The points to wrap the rectangle around.</param>
        /// <param name="percentage">The margin in percentage.</param>
        /// <returns></returns>
        public RectangleF2D Fit(PointF2D[] points, double percentage)
        {
            if (points == null) { throw new ArgumentNullException("points"); }
            if (points.Length < 2) { throw new ArgumentOutOfRangeException("Rectangle fit needs at least two points."); }

            // calculate the center.
            double[] center = new double[] { points[0][0], points[0][1] };
            for (int idx = 1; idx < points.Length; idx++)
            {
                center[0] = center[0] + points[idx][0];
                center[1] = center[1] + points[idx][1];
            }
            center[0] = center[0] / points.Length;
            center[1] = center[1] / points.Length;
            PointF2D centerPoint = new PointF2D(center);

            LineF2D line = null;
            // calculate the width.
            double width = 0;
            for (int idx = 0; idx < points.Length; idx++)
            {
                line = new LineF2D(points[idx], points[idx] + this._vectorY);
                double distance = line.Distance(centerPoint);
                if (distance > width)
                { // the distance is larger.
                    width = distance;
                }
            }
            width = width * 2;

            // calculate the height.
            double height = 0;
            for (int idx = 0; idx < points.Length; idx++)
            {
                line = new LineF2D(points[idx], points[idx] + this._vectorX);
                double distance = line.Distance(centerPoint);
                if (distance > height)
                { // this distance is larger.
                    height = distance;
                }
            }
            height = height * 2;

            // expand with the given percentage.
            width = width + (width / 100.0 * percentage);
            height = height + (height / 100.0 * percentage);

            return RectangleF2D.FromBoundsAndCenter(width, height, centerPoint[0], centerPoint[1],
                this.DirectionY);
        }

        /// <summary>
        /// Fits this rectangle to this given points and keeps aspect ratio.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public RectangleF2D FitAndKeepAspectRatio(PointF2D[] points, double percentage)
        {
            RectangleF2D fitted = this;
            if (points.Length > 1)
            { // multiple points can be fitted and zoomed to.
                fitted = this.Fit(points, percentage);
            }
            else if(points.Length == 1)
            { // a single point can only be moved to.
                fitted = new RectangleF2D(points[0][0], points[0][1], this.Width, this.Height, this.Angle);
            }

            // although this may seem a strange approach, think about
            // numerical stability before changing this!            

            double width = fitted.Width;
            double height = fitted.Height;
            double targetRatio = this.Width / this.Height; // this is the target ratio.
            if (fitted.Height > fitted.Width)
            { // the height is bigger.
                double targetWidth = fitted.Height * targetRatio;
                if (targetWidth < fitted.Width)
                { // increase height instead.
                    height = fitted.Width / targetRatio;
                }
                else
                { // ok, the width is increased and ratio's match now.
                    width = targetWidth;
                }
            }
            else
            { // the width is bigger.
                double targetHeight = fitted.Width / targetRatio;
                if (targetHeight < fitted.Height)
                { // increase width instead.
                    width = fitted.Height * targetRatio;
                }
                else
                { // ok, the height is increase and ratio's match now.
                    height = targetHeight;
                }
            }
            return RectangleF2D.FromBoundsAndCenter(width, height,
                fitted.Center[0], fitted.Center[1], fitted.DirectionY);
        }

        /// <summary>
        /// Returns true if this box contains the specified x, y.
        /// </summary>
        /// <param name="point">The point.</param>
        public bool Contains(PointF2D point)
        {
			double[] coordinates = this.TransformTo (100, 100, false, false, point);
			return (coordinates [0] >= 0 && coordinates [0] <= 100 &&
				coordinates [1] >= 0 && coordinates [1] <= 100);
		}                                      

		/// <summary>
		/// Returns true if this box contains the specified x, y.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public bool Contains(double x, double y){
			return this.Contains (new PointF2D (x, y));
		}

		/// <summary>
		/// Returns true if this rectangle overlaps with the given box.
		/// </summary>
		/// <param name="box">Box.</param>
		public bool Overlaps(BoxF2D box){
			// Yes, I know this code can be shorter but it would turn into a mess!
			if (box.Contains (this.BottomLeft) || box.Contains (this.BottomRight) ||
				box.Contains (this.TopLeft) || box.Contains (this.TopRight)) {
				return true;
			}
			if (this.Contains (box.Corners [0]) || this.Contains (box.Corners [2]) ||
			    this.Contains (box.Corners [3]) || this.Contains (box.Corners [0])) {
				return true;
			}

			List<LineF2D> lines = new List<LineF2D> ();
            lines.Add(new LineF2D(this.BottomLeft, this.BottomRight, true));
            lines.Add(new LineF2D(this.BottomRight, this.TopRight, true));
            lines.Add(new LineF2D(this.TopRight, this.TopLeft, true));
            lines.Add(new LineF2D(this.TopLeft, this.BottomLeft, true));
			foreach (LineF2D line in (box as IEnumerable<LineF2D>)) {
				foreach (LineF2D otherLine in lines) {
					if (line.Intersects (otherLine)) {
						return true;
					}
				}
			}
			return false;
		}

        /// <summary>
        /// Returns true if this rectangle overlaps with the given box.
        /// </summary>
        /// <param name="rectangle">Rectangle.</param>
        public bool Overlaps(RectangleF2D rectangle)
        {
            // Yes, I know this code can be shorter but it would turn into a mess!
            if (rectangle.Contains(this.BottomLeft) || rectangle.Contains(this.BottomRight) ||
                rectangle.Contains(this.TopLeft) || rectangle.Contains(this.TopRight))
            {
                return true;
            }
            if (this.Contains(rectangle.BottomLeft) || this.Contains(rectangle.BottomRight) ||
                this.Contains(rectangle.TopLeft) || this.Contains(rectangle.TopRight))
            {
                return true;
            }

            List<LineF2D> lines = new List<LineF2D>();
            lines.Add(new LineF2D(this.BottomLeft, this.BottomRight, true));
            lines.Add(new LineF2D(this.BottomRight, this.TopRight, true));
            lines.Add(new LineF2D(this.TopRight, this.TopLeft, true));
            lines.Add(new LineF2D(this.TopLeft, this.BottomLeft, true));
            List<LineF2D> otherLines = new List<LineF2D>();
            otherLines.Add(new LineF2D(rectangle.BottomLeft, rectangle.BottomRight, true));
            otherLines.Add(new LineF2D(rectangle.BottomRight, rectangle.TopRight, true));
            otherLines.Add(new LineF2D(rectangle.TopRight, rectangle.TopLeft, true));
            otherLines.Add(new LineF2D(rectangle.TopLeft, rectangle.BottomLeft, true));
            foreach (LineF2D line in lines)
            {
                foreach (LineF2D otherLine in otherLines)
                {
                    if (line.Intersects(otherLine))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

		/// <summary>
		/// Rotates this rectangle around it's center point with a given angle in clockwise direction.
		/// </summary>
		/// <returns>The around center.</returns>
		/// <param name="angle">Angle.</param>
		public RectangleF2D RotateAroundCenter(Degree angle){
			return this.RotateAround (angle, this.Center);
		}

		/// <summary>
		/// Rotates this rectangle around the given center point with a given angle in clockwise direction.
		/// </summary>
		/// <returns>The around.</returns>
		/// <param name="angle">Angle.</param>
		/// <param name="center">Center.</param>
		public RectangleF2D RotateAround(Degree angle, PointF2D center) {
			PointF2D[] corners = new PointF2D[] { this.TopLeft, this.TopRight, this.BottomLeft, this.BottomRight };
			PointF2D[] cornersRotated = Rotation.RotateAroundPoint (angle, center, corners);

			return new RectangleF2D (cornersRotated [2], this.Width, this.Height,	
			                        cornersRotated [0] - cornersRotated [2]);
		}

		#region Affine Transformations

		/// <summary>
		/// Transforms the given coordinates to the coordinate system this rectangle is defined in.
		/// </summary>
		/// <param name="width">The width of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="height">The height of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="reverseX">Assumes that the origin of the x-axis is on the top of this rectangle if false.</param>
		/// <param name="reverseY">Assumes that the origin of the y-axis is on the right of this rectangle if false.</param>
		/// <param name="coordinates">The coordinates to transform.</param>
		public double[] TransformFrom(double width, double height, bool reverseX, bool reverseY,
		                              double[] coordinates){
			return this.TransformFrom (width, height, reverseX, reverseY, coordinates [0], coordinates [1]);
		}

		/// <summary>
		/// Transforms the given coordinates to the coordinate system this rectangle is defined in.
		/// </summary>
		/// <param name="width">The width of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="height">The height of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="reverseX">Assumes that the origin of the x-axis is on the top of this rectangle if false.</param>
		/// <param name="reverseY">Assumes that the origin of the y-axis is on the right of this rectangle if false.</param>
		/// <param name="x">The x-coordinates to transform.</param>
		/// <param name="y">The y-coordinates to transform.</param>
		public double[] TransformFrom(double width, double height, bool reverseX, bool reverseY,
		                          	  double x, double y) {
			PointF2D reference = _bottomLeft;
			VectorF2D vectorX = _vectorX;
			VectorF2D vectorY = _vectorY;

			if (reverseX && !reverseY) {
				reference = this.BottomRight;
				vectorX = _vectorX * -1;
			} else if (!reverseX && reverseY) {
				reference = this.TopLeft;
				vectorY = _vectorY * -1;
			} else if (reverseX && reverseY) {
				reference = this.TopRight;
				vectorX = _vectorX * -1;
				vectorY = _vectorY * -1;
			}

			double widthFactor = x / width;
			double heightFactor = y / height;

			PointF2D result = reference +
				(vectorX * widthFactor) +
				(vectorY * heightFactor);
			return result.ToArray ();
		}

		/// <summary>
		/// Transforms the given coordinates to the coordinate system this rectangle is defined in.
		/// </summary>
		/// <param name="width">The width of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="height">The height of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="reverseX">Assumes that the origin of the x-axis is on the top of this rectangle if false.</param>
		/// <param name="reverseY">Assumes that the origin of the y-axis is on the right of this rectangle if false.</param>
		/// <param name="x">The x-coordinates to transform.</param>
		/// <param name="y">The y-coordinates to transform.</param>
		public double[][] TransformFrom(double width, double height, bool reverseX, bool reverseY,
		                              double[] x, double[] y){
			PointF2D reference = _bottomLeft;
			VectorF2D vectorX = _vectorX;
			VectorF2D vectorY = _vectorY;

			if (reverseX && !reverseY) {
				reference = this.BottomRight;
				vectorX = _vectorX * -1;
			} else if (!reverseX && reverseY) {
				reference = this.TopLeft;
				vectorY = _vectorY * -1;
			} else if (reverseX && reverseY) {
				reference = this.TopRight;
				vectorX = _vectorX * -1;
				vectorY = _vectorY * -1;
			}

			double[][] transformed = new double[x.Length][];
			for(int idx = 0; idx < x.Length; idx++) {
				double widthFactor = x[idx] / width;
				double heightFactor = y[idx] / height;

				PointF2D result = reference +
					(vectorX * widthFactor) +
						(vectorY * heightFactor);
				transformed[idx] = result.ToArray ();
			}
			return transformed;
		}
		

		/// <summary>
		/// Transforms the given the coordinates to a coordinate system defined inside this rectangle.
		/// </summary>
		/// <param name="width">The width of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="height">The height of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="reverseX">Assumes that the origin of the x-axis is on the top of this rectangle if false.</param>
		/// <param name="reverseY">Assumes that the origin of the y-axis is on the right of this rectangle if false.</param>
		/// <param name="coordinates">The coordinates to transform.</param>
		public double[] TransformTo(double width, double height, bool reverseX, bool reverseY,
		                            double[] coordinates){
			return this.TransformTo (width, height, reverseX, reverseY, new PointF2D (coordinates[0], coordinates[1]));
		}
		
		/// <summary>
		/// Transforms the given the coordinates to a coordinate system defined inside this rectangle.
		/// </summary>
		/// <param name="width">The width of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="height">The height of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="reverseX">Assumes that the origin of the x-axis is on the top of this rectangle if false.</param>
		/// <param name="reverseY">Assumes that the origin of the y-axis is on the right of this rectangle if false.</param>
		/// <param name="x">The x-coordinate to transform.</param>
		/// <param name="y">The y-coordinate to transform.</param>
		public double[] TransformTo(double width, double height, bool reverseX, bool reverseY,
		                            double x, double y){
			return this.TransformTo (width, height, reverseX, reverseY, new PointF2D (x, y));
		}

		/// <summary>
		/// Transforms the given the coordinates to a coordinate system defined inside this rectangle.
		/// </summary>
		public void TransformTo(double width, double height, bool reverseX, bool reverseY,
			double x, double y, double[] transformed){
			this.TransformTo (width, height, reverseX, reverseY, new PointF2D (x, y), transformed);
		}

		/// <summary>
		/// Transforms the given the coordinates to a coordinate system defined inside this rectangle.
		/// </summary>
		/// <param name="width">The width of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="height">The height of the rectangle in the coordinate system of the given coordinates.</param>
		/// <param name="reverseX">Assumes that the origin of the x-axis is on the top of this rectangle if false.</param>
		/// <param name="reverseY">Assumes that the origin of the y-axis is on the right of this rectangle if false.</param>
		/// <param name="x">The x-coordinate to transform.</param>
		/// <param name="y">The y-coordinate to transform.</param>
		public double[][] TransformTo(double width, double height, bool reverseX, bool reverseY,
		                            double[] x, double[] y){
			PointF2D reference = _bottomLeft;
			VectorF2D vectorX = _vectorX;
			VectorF2D vectorY = _vectorY;

			if (reverseX && !reverseY) {
				reference = this.BottomRight;
				vectorX = _vectorX * -1;
			} else if (!reverseX && reverseY) {
				reference = this.TopLeft;
				vectorY = _vectorY * -1;
			} else if (reverseX && reverseY) {
				reference = this.TopRight;
				vectorX = _vectorX * -1;
				vectorY = _vectorY * -1;
			}
			
			double[][] transformed = new double[x.Length][];
			for (int idx = 0; idx < x.Length; idx++) {
				PointF2D point = new PointF2D (x [idx], y [idx]);
				LineF2D xLine = new LineF2D (point, point + vectorX, false);
				PointF2D yIntersection = xLine.Intersection (new LineF2D (reference, reference + vectorY)) as PointF2D;
				VectorF2D yIntersectionVector = (yIntersection - reference);
				double yFactor = yIntersectionVector.Size / vectorY.Size;
				if (!yIntersectionVector.CompareNormalized (vectorY, 0.0001)) {
					yFactor = -yFactor;
				}

				LineF2D yLine = new LineF2D (point, point + vectorY);
				PointF2D xIntersection = yLine.Intersection (new LineF2D (reference, reference + vectorX)) as PointF2D;
				VectorF2D xIntersectionVector = (xIntersection - reference);
				double xFactor = xIntersectionVector.Size / vectorX.Size;
				if (!xIntersectionVector.CompareNormalized (vectorX, 0.0001)) {
					xFactor = -xFactor;
				}

				transformed[idx] = new double[] { xFactor * width, yFactor * height };
			}
			return transformed;
		}

		/// <summary>
		/// Transforms the given the coordinates to a coordinate system defined inside this rectangle.
		/// </summary>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		/// <param name="reverseX">If set to <c>true</c> reverse x.</param>
		/// <param name="reverseY">If set to <c>true</c> reverse y.</param>
		/// <param name="point">Point.</param>
		/// <param name="transformed">Transformed.</param>
		public void TransformTo(double width, double height, bool reverseX, bool reverseY,
			PointF2D point, double[] transformed){
			if (transformed == null) {
				throw new ArgumentNullException ();
			}
			if (transformed.Length != 2) {
				throw new ArgumentException ("Tranformed array needs to be of length 2.");
			}

			PointF2D reference = _bottomLeft;
			VectorF2D vectorX = _vectorX;
			VectorF2D vectorY = _vectorY;

			if (reverseX && !reverseY) {
				reference = this.BottomRight;
				vectorX = _vectorX * -1;
			} else if (!reverseX && reverseY) {
				reference = this.TopLeft;
				vectorY = _vectorY * -1;
			} else if (reverseX && reverseY) {
				reference = this.TopRight;
				vectorX = _vectorX * -1;
				vectorY = _vectorY * -1;
			}

			LineF2D xLine = new LineF2D (point, point + vectorX, false);
			PointF2D yIntersection = xLine.Intersection (new LineF2D (reference, reference + vectorY)) as PointF2D;
			VectorF2D yIntersectionVector = (yIntersection - reference);
			double yFactor = yIntersectionVector.Size / vectorY.Size;
			if (!yIntersectionVector.CompareNormalized (vectorY, 0.0001)) {
				yFactor = -yFactor;
			}

			LineF2D yLine = new LineF2D (point, point + vectorY);
			PointF2D xIntersection = yLine.Intersection (new LineF2D (reference, reference + vectorX)) as PointF2D;
			VectorF2D xIntersectionVector = (xIntersection - reference);
			double xFactor = xIntersectionVector.Size / vectorX.Size;
			if (!xIntersectionVector.CompareNormalized (vectorX, 0.0001)) {
				xFactor = -xFactor;
			}

			transformed [0] = xFactor * width;
			transformed [1] = yFactor * height;
		}

		/// <summary>
		/// Transforms the given the coordinates to a coordinate system defined inside this rectangle.
		/// </summary>
		public double[] TransformTo(double width, double height, bool reverseX, bool reverseY,
		                            PointF2D point){
			PointF2D reference = _bottomLeft;
			VectorF2D vectorX = _vectorX;
			VectorF2D vectorY = _vectorY;

			if (reverseX && !reverseY) {
				reference = this.BottomRight;
				vectorX = _vectorX * -1;
			} else if (!reverseX && reverseY) {
				reference = this.TopLeft;
				vectorY = _vectorY * -1;
			} else if (reverseX && reverseY) {
				reference = this.TopRight;
				vectorX = _vectorX * -1;
				vectorY = _vectorY * -1;
			}

			LineF2D xLine = new LineF2D (point, point + vectorX, false);
			PointF2D yIntersection = xLine.Intersection (new LineF2D (reference, reference + vectorY)) as PointF2D;
			VectorF2D yIntersectionVector = (yIntersection - reference);
			double yFactor = yIntersectionVector.Size / vectorY.Size;
			if (!yIntersectionVector.CompareNormalized (vectorY, 0.0001)) {
				yFactor = -yFactor;
			}

			LineF2D yLine = new LineF2D (point, point + vectorY);
			PointF2D xIntersection = yLine.Intersection (new LineF2D (reference, reference + vectorX)) as PointF2D;
			VectorF2D xIntersectionVector = (xIntersection - reference);
			double xFactor = xIntersectionVector.Size / vectorX.Size;
			if (!xIntersectionVector.CompareNormalized (vectorX, 0.0001)) {
				xFactor = -xFactor;
			}

			return new double[] { xFactor * width, yFactor * height };
		}

		#endregion

		#region implemented abstract members of PrimitiveF2D

		/// <summary>
		/// Calculates the distance of this primitive to the given point.
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		public override double Distance (PointF2D p)
		{
			double distance = (new LineF2D (this.BottomLeft, this.BottomRight, true)).Distance (p);
			double newDistance = (new LineF2D (this.BottomRight, this.TopRight, true)).Distance (p);
			if (newDistance < distance) {
				distance = newDistance;
			}
			newDistance = (new LineF2D (this.TopRight, this.TopLeft, true)).Distance (p);
			if (newDistance < distance) {
				distance = newDistance;
			}
			newDistance = (new LineF2D (this.TopLeft, this.BottomLeft, true)).Distance (p);
			if (newDistance < distance) {
				distance = newDistance;
			}
			return distance;
		}

		#endregion

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="OsmSharp.Math.Primitives.RectangleF2D"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="OsmSharp.Math.Primitives.RectangleF2D"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
        /// <see cref="OsmSharp.Math.Primitives.RectangleF2D"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            RectangleF2D rectangle = (obj as RectangleF2D);
            if (rectangle != null)
            {
                return rectangle.BottomLeft.Equals(this.BottomLeft) &&
                    rectangle.DirectionX.Equals(this.DirectionX) &&
                    rectangle.DirectionY.Equals(this.DirectionY);
            }
            return false;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="OsmSharp.Math.Primitives.RectangleF2D"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode()
        {
            return this.BottomLeft.GetHashCode() ^
                this.DirectionX.GetHashCode() ^
                this.DirectionY.GetHashCode();
        }
    }
}