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

using OsmSharp.Math.Geo.Lambert.Ellipsoids;

namespace OsmSharp.Math.Geo.Lambert
{
    /// <summary>
    /// Represents an ellipsoid used with the lamber projection.
    /// </summary>
    public abstract class LambertEllipsoid
    {
        private readonly double _semiMajorAxis;
        private readonly double _flattening;
        private readonly double _eccentricity;

        /// <summary>
        /// Creates a new ellipsoid.
        /// </summary>
        /// <param name="semi_major_axis"></param>
        /// <param name="flattening"></param>
        protected LambertEllipsoid(double semi_major_axis,
            double flattening)
        {
            _semiMajorAxis = semi_major_axis;
            _flattening = flattening;

            // calculate eccentricity.
            _eccentricity = System.Math.Sqrt(_flattening * (2 - _flattening));
        }

        /// <summary>
        /// Returns the calculated eccentricity of this ellipsoid.
        /// </summary>
        public double Eccentricity
        {
            get
            {
                return _eccentricity;
            }
        }

        /// <summary>
        /// Returns the semi major axis size.
        /// </summary>
        public double SemiMajorAxis
        {
            get
            {
                return _semiMajorAxis;
            }
        }

        /// <summary>
        /// Returns the flattening of this ellipsoid.
        /// </summary>
        public double Flattening
        {
            get
            {
                return _flattening;
            }
        }

        #region Static Default Ellipsoids

        private static Hayford1924Ellipsoid _hayford1924Ellipsoid;

        /// <summary>
        /// Returns the hayford 1924 ellisoid.
        /// </summary>
        public static Hayford1924Ellipsoid Hayford1924Ellipsoid
        {
            get
            {
                if (_hayford1924Ellipsoid == null)
                {
                    _hayford1924Ellipsoid = new Hayford1924Ellipsoid();
                }
                return _hayford1924Ellipsoid;
            }
        }

        private static Wgs1984Ellipsoid _wgs1984Ellipsoid;

        /// <summary>
        /// Returns the wgs 1984 ellisoid.
        /// </summary>
        public static Wgs1984Ellipsoid Wgs1984Ellipsoid
        {
            get
            {
                if (_wgs1984Ellipsoid == null)
                {
                    _wgs1984Ellipsoid = new Wgs1984Ellipsoid();
                }
                return _wgs1984Ellipsoid;
            }
        }

        #endregion
    }
}