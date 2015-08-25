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
using OsmSharp.Units.Angle;
using OsmSharp.Math.Geo.Lambert.International.Belgium;
using OsmSharp.Math.Geo.Lambert.Ellipsoids;

namespace OsmSharp.Math.Geo.Lambert
{
    /// <summary>
    /// A standard lamber projection.
    /// </summary>
    public abstract class LambertProjectionBase
    {
        /// <summary>
        /// The name of this projection.
        /// </summary>
        private string _name;

        /// <summary>
        /// The ellipsoid used in this projection.
        /// </summary>
        private LambertEllipsoid _ellipsoid;

        /// <summary>
        /// The first standard parallel in decimal degrees.
        /// </summary>
        private double _standard_parallel_1;
        /// <summary>
        /// The first standard parallel in radians.
        /// </summary>
        public double _standard_parallel_1_radians;

        /// <summary>
        /// The second standard parallel in decimal degrees.
        /// </summary>
        private double _standard_parallel_2;
        /// <summary>
        /// The second standard parallell in radians.
        /// </summary>
        public double _standard_parallel_2_radians;

        /// <summary>
        /// The latitude of the origin.
        /// </summary>
        private double _latitude_origin;
        /// <summary>
        /// THe latitude of the orgine in radians.
        /// </summary>
        public double _latitude_origin_radians;

        /// <summary>
        /// The longitude of the origin.
        /// </summary>
        private double _longitude_origin;
        /// <summary>
        /// The longitude of the orgin.
        /// </summary>
        public double _longitude_origin_radians;

        /// <summary>
        /// The x-coordinate of the origin.
        /// </summary>
        private double _x_origin;

        /// <summary>
        /// The y-coordinate of the origin.
        /// </summary>
        private double _y_origin;

        #region Calculation Intermediates

        private double _m_1;
        private double _m_2;

        private double _t_0;
        private double _t_1;
        private double _t_2;

        private double _n;

        private double _g;

        private double _r_0;

        #endregion

        /// <summary>
        /// Creates a new lambert projection.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ellipsoid"></param>
        /// <param name="standard_parallel_1"></param>
        /// <param name="standard_parallel_2"></param>
        /// <param name="latitude_origin_1"></param>
        /// <param name="longitude_origin_2"></param>
        /// <param name="x_origin"></param>
        /// <param name="y_origin"></param>
        protected LambertProjectionBase(string name,
            LambertEllipsoid ellipsoid,
            double standard_parallel_1,
            double standard_parallel_2,
            double latitude_origin_1,
            double longitude_origin_2,
            double x_origin,
            double y_origin)
        {
            _name = name;
            _ellipsoid = ellipsoid;

            _standard_parallel_1 = standard_parallel_1;
            Radian temp = new Degree(_standard_parallel_1);
            _standard_parallel_1_radians = temp.Value;

            _standard_parallel_2 = standard_parallel_2;
            temp = new Degree(_standard_parallel_2);
            _standard_parallel_2_radians = temp.Value;

            _latitude_origin = latitude_origin_1;
            temp = new Degree(_latitude_origin);
            _latitude_origin_radians = temp.Value;

            _longitude_origin = longitude_origin_2;
            temp = new Degree(_longitude_origin);
            _longitude_origin_radians = temp.Value;

            _x_origin = x_origin;
            _y_origin = y_origin;

            // calculate common calculation intermidiates.
            _m_1 = (System.Math.Cos(_standard_parallel_1_radians) /
                System.Math.Sqrt((1.0 - _ellipsoid.Eccentricity * _ellipsoid.Eccentricity *
                System.Math.Pow(System.Math.Sin(_standard_parallel_1_radians), 2.0))));

            _m_2 = (System.Math.Cos(_standard_parallel_2_radians) /
                System.Math.Sqrt((1 - _ellipsoid.Eccentricity * _ellipsoid.Eccentricity *
                System.Math.Pow(System.Math.Sin(_standard_parallel_2_radians), 2.0))));

            _t_0 = (System.Math.Tan(System.Math.PI / 4.0 - _latitude_origin_radians / 2.0) /
                System.Math.Pow(((1 - _ellipsoid.Eccentricity * System.Math.Sin(_latitude_origin_radians)) /
                (1 + _ellipsoid.Eccentricity * System.Math.Sin(_latitude_origin_radians))), _ellipsoid.Eccentricity / 2.0));

            _t_1 = (System.Math.Tan(System.Math.PI / 4.0 - _standard_parallel_1_radians / 2.0) /
                System.Math.Pow(((1 - _ellipsoid.Eccentricity * System.Math.Sin(_standard_parallel_1_radians)) /
                (1 + _ellipsoid.Eccentricity * System.Math.Sin(_standard_parallel_1_radians))), _ellipsoid.Eccentricity / 2.0));

            _t_2 = (System.Math.Tan(System.Math.PI / 4.0 - _standard_parallel_2_radians / 2.0) /
                System.Math.Pow(((1 - _ellipsoid.Eccentricity * System.Math.Sin(_standard_parallel_2_radians)) /
                (1 + _ellipsoid.Eccentricity * System.Math.Sin(_standard_parallel_2_radians))), _ellipsoid.Eccentricity / 2.0));

            _n = ((System.Math.Log(_m_1) - System.Math.Log(_m_2))
                / (System.Math.Log(_t_1) - System.Math.Log(_t_2)));

            _g = _m_1 / (_n * System.Math.Pow(_t_1, _n));

            _r_0 = _ellipsoid.SemiMajorAxis * _g * System.Math.Pow(System.Math.Abs(_t_0), _n);
        }

        /// <summary>
        /// The name of this projection.
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        #region Conversion Functions

        /// <summary>
        /// Converts the given lambert coordinates to mercator coordinates.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public GeoCoordinate ConvertToWGS84(double x, double y)
        {
            double latitude;
            double longitude;

            double r = (System.Math.Sqrt(
                System.Math.Pow(x - _x_origin, 2.0) +
                System.Math.Pow((_r_0 - (y - _y_origin)), 2.0))
                );

            double t = System.Math.Pow((r / (_ellipsoid.SemiMajorAxis * _g)), 1.0 / _n);

            double phi = System.Math.Atan((x - _x_origin) /
                (_r_0 - (y - _y_origin)));

            longitude = ((phi / _n) + _longitude_origin_radians);

            latitude = (System.Math.PI / 2.0 - 2.0 * System.Math.Atan(t));

            double e = _ellipsoid.Eccentricity;

            double new_latitude = 0;
            while (new_latitude != latitude)
            { // iterate 100 times.
                new_latitude = latitude;
                latitude = (System.Math.PI / 2.0 - 2.0 *
                    System.Math.Atan(t *
                    System.Math.Pow(
                    ((1.0 - e * System.Math.Sin(latitude)) / (1.0 + e * System.Math.Sin(latitude))),
                    e / 2.0)));
            }



            Hayford1924Ellipsoid hayford = new Hayford1924Ellipsoid();

            double phi_72 = latitude; // phi = latitude!
            double lambda_72 = longitude; // lambda = longitude!
            double h_72 = 100;

            double a_72 = hayford.SemiMajorAxis;
            double e_72 = hayford.Eccentricity;
            double es_72 = e_72 * e_72;
            double sin_phi_72 = System.Math.Sin(phi_72);
            double cos_phi_72 = System.Math.Cos(phi_72);
            double sin_lambda_72 = System.Math.Sin(lambda_72);
            double cos_lambda_72 = System.Math.Cos(lambda_72);
            double v_72 = a_72 / System.Math.Sqrt(1 - (es_72 * sin_phi_72 * sin_phi_72));

            double X_72 = (v_72 + h_72) * cos_phi_72 * cos_lambda_72;
            double Y_72 = (v_72 + h_72) * cos_phi_72 * sin_lambda_72;
            double Z_72 = ((1 - es_72) * v_72 + h_72) * sin_phi_72;

            // ALL OK

            //X_72 = X_72 * (1 / 1.0000012747);
            //Y_72 = Y_72 * (1 / 1.0000012747);
            //Z_72 = Z_72 * (1 / 1.0000012747);

            // translations.
            double x_trans = 106.868628;
            double y_trans = 52.297783;
            double z_trans = 103.723893;

            double X_89 = X_72 - x_trans;
            double Y_89 = Y_72 + y_trans;
            double Z_89 = Z_72 - z_trans;

            // rotations.
            double x_angle = ((Radian)new Degree(0.336570 / 3600)).Value;
            double sin_x_angle = System.Math.Sin(x_angle);
            double cos_x_angle = System.Math.Cos(x_angle);
            double y_angle = ((Radian)new Degree(-0.456955 / 3600)).Value;
            double sin_y_angle = System.Math.Sin(y_angle);
            double cos_y_angle = System.Math.Cos(y_angle);
            double z_angle = ((Radian)new Degree(1.842183 / 3600)).Value;
            double sin_z_angle = System.Math.Sin(z_angle);
            double cos_z_angle = System.Math.Cos(z_angle);
            // rotate around x.
            //X_89 = X_89;
            Y_89 = Y_89 * cos_x_angle - Z_89 * sin_x_angle;
            Z_89 = Y_89 * sin_x_angle + Z_89 * cos_x_angle;
            // rotate around y.
            X_89 = X_89 * cos_y_angle + Z_89 * sin_y_angle;
            //Y_89 = Y_89;
            Z_89 = X_89 * (-sin_y_angle) + Z_89 * cos_y_angle;
            // rotate around Z.
            X_89 = X_89 * cos_z_angle - Y_89 * sin_z_angle;
            Y_89 = X_89 * sin_z_angle + Y_89 * cos_z_angle;
            //Z_89 = Z_89;

            Wgs1984Ellipsoid wgs1984 = new Wgs1984Ellipsoid();

            e = wgs1984.Eccentricity;
            double es = e * e;
            double ps = X_89 * X_89 + Y_89 * Y_89;
            double p = System.Math.Sqrt(ps);

            r = System.Math.Sqrt(ps + Z_89 * Z_89);

            double f = wgs1984.Flattening;
            double a = wgs1984.SemiMajorAxis;

            double u = System.Math.Atan((Z_89 / p) * ((1 - f) + (es * a / r)));
            double lambda = System.Math.Atan(Y_89 / X_89);
            phi = System.Math.Atan((Z_89 * (1 - f) + (es * a * System.Math.Pow(System.Math.Sin(u), 3)))
                /
               ((1 - f) * (p - (es * a * System.Math.Pow(System.Math.Cos(u), 3)))));

            Degree longitude_84 = new Radian(lambda);
            Degree latitude_84 = new Radian(phi);

            return new GeoCoordinate(latitude_84.Value, longitude_84.Value);
        }

        #endregion


        #region Projections

        private static Belgium1972LambertProjection _belgium_1972_lambert_projection;

        /// <summary>
        /// Returns an instance of the belgium 1972 lambert projection.
        /// </summary>
        public static Belgium1972LambertProjection Belgium1972LambertProjection
        {
            get
            {
                if (_belgium_1972_lambert_projection == null)
                {
                    _belgium_1972_lambert_projection = new Belgium1972LambertProjection();
                }
                return _belgium_1972_lambert_projection;
            }
        }

        #endregion
    }
}
