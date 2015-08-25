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
using System.Text;
using OsmSharp.Math.Geo.Lambert.Ellipsoids;

namespace OsmSharp.Math.Geo.Lambert
{
    /// <summary>
    /// Represents an ellipsoid used with the lamber projection.
    /// </summary>
    public abstract class LambertEllipsoid
    {
        /// <summary>
        /// The distance from the center of the ellipsoid to one of the focus points.
        /// </summary>
        private double _semi_major_axis;

        /// <summary>
        /// The flattening of this ellipsoid.
        /// </summary>
        private double _flattening;

        /// <summary>
        /// The eccentricity of this ellipsoid, calculated upon creation.
        /// </summary>
        private double _eccentricity;

        /// <summary>
        /// Creates a new ellipsoid.
        /// </summary>
        /// <param name="semi_major_axis"></param>
        /// <param name="flattening"></param>
        protected LambertEllipsoid(double semi_major_axis,
            double flattening)
        {
            _semi_major_axis = semi_major_axis;
            _flattening = flattening;

            // calculate eccentricity.
            _eccentricity = System.Math.Sqrt(_flattening * (2 - _flattening));
        }

        #region Properties

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
                return _semi_major_axis;
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

        #endregion

        #region Static Default Ellipsoids

        #region Hayford 1924

        private static Hayford1924Ellipsoid _hayford_1924_ellipsoid;

        /// <summary>
        /// Returns the hayford 1924 ellisoid.
        /// </summary>
        public static Hayford1924Ellipsoid Hayford1924Ellipsoid
        {
            get
            {
                if (_hayford_1924_ellipsoid == null)
                {
                    _hayford_1924_ellipsoid = new Hayford1924Ellipsoid();
                }
                return _hayford_1924_ellipsoid;
            }
        }

        #endregion

        #region Wgs 1984

        private static Wgs1984Ellipsoid _wgs_1984_ellipsoid;

        /// <summary>
        /// Returns the wgs 1984 ellisoid.
        /// </summary>
        public static Wgs1984Ellipsoid Wgs1984Ellipsoid
        {
            get
            {
                if (_wgs_1984_ellipsoid == null)
                {
                    _wgs_1984_ellipsoid = new Wgs1984Ellipsoid();
                }
                return _wgs_1984_ellipsoid;
            }
        }

        #endregion

        #endregion
    }
}
