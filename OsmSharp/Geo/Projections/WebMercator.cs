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

using OsmSharp.Units.Angle;

namespace OsmSharp.Math.Geo.Projections
{
    /// <summary>
    /// Projection that corresponds to the projection used for webtiles. This will project webtiles nicely into 256x256 squares.
    /// </summary>
    /// <remarks>http://wiki.openstreetmap.org/wiki/Slippy_map_tilenames
    /// Zoom level 16 is used as the default level.</remarks>
    public class WebMercator : IProjection
    {
        /// <summary>
        /// Holds the constant default zoom level.
        /// </summary>
        private const int DefaultZoom = 15;

        /// <summary>
        /// Converts the lat/lon to projected coordinates.
        /// </summary>
        /// <returns></returns>
        public double[] ToPixel(double lat, double lon)
        {
            var n = System.Math.Floor(System.Math.Pow(2, DefaultZoom));

            var rad = (Radian)(new Degree(lat));

            var x = (((lon + 180.0f) / 360.0f) * (double)n);
            var y = (
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);

            return new double[] {x, y};
        }

        /// <summary>
        /// Converts the lat/lon to projected coordinates.
        /// </summary>
        /// <returns></returns>
        public double[] ToPixel(GeoCoordinate coordinate)
        {
            return this.ToPixel(coordinate.Latitude, coordinate.Longitude);
        }

        /// <summary>
        /// Converts the given projected coordinates to lat/lon.
        /// </summary>
        /// <returns></returns>
        public GeoCoordinate ToGeoCoordinates(double x, double y)
        {
            var n = System.Math.PI - ((2.0 * System.Math.PI * (y)) / System.Math.Pow(2.0, DefaultZoom));

            var longitude = (((x) / System.Math.Pow(2.0, DefaultZoom) * 360.0) - 180.0);
            var latitude = (180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(n)));

            return new GeoCoordinate(latitude, longitude);
        }

        /// <summary>
        /// Converts longitude to a projected x-coordinate.
        /// </summary>
        /// <returns></returns>
        public double LongitudeToX(double lon)
        {
            var n = System.Math.Floor(System.Math.Pow(2, DefaultZoom));

            return (((lon + 180.0f) / 360.0f) * n);
        }

        /// <summary>
        /// Converts the latitude to a projected y-coordinate.
        /// </summary>
        /// <returns></returns>
        public double LatitudeToY(double lat)
        {
            var n = System.Math.Floor(System.Math.Pow(2, DefaultZoom));

            var rad = (Radian)(new Degree(lat));

            return (
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);
        }

        /// <summary>
        /// Converts the projected y-coordinate to latitude.
        /// </summary>
        /// <returns></returns>
        public double YToLatitude(double y)
        {
            var n = System.Math.PI - ((2.0 * System.Math.PI * (y)) / System.Math.Pow(2.0, DefaultZoom));

            return  (180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(n)));
        }

        /// <summary>
        /// Converts the projected x-coordinate to longitude.
        /// </summary>
        /// <returns></returns>
        public double XToLongitude(double x)
        {
            return (((x) / System.Math.Pow(2.0, DefaultZoom) * 360.0) - 180.0);
        }

        /// <summary>
        /// Returns the scale for the given zoomlevel.
        /// </summary>
        /// <returns></returns>
        public double ToZoomFactor(double zoomLevel)
        {
            return System.Math.Pow(2, zoomLevel - DefaultZoom) * 256.0;
        }

        /// <summary>
        /// Returns the scale for the given zoomFactor.
        /// </summary>
        /// <returns></returns>
        public double ToZoomLevel(double zoomFactor)
        {
            return System.Math.Log(zoomFactor/256.0, 2) + DefaultZoom;
        }

        /// <summary>
        /// Returns true if this projection uses lowest left, highest right. False otherwise.
        /// </summary>
        public bool DirectionX { get { return true; } }

        /// <summary>
        /// Returns true if this projection uses lowest bottom, highest top. False otherwise.
        /// </summary>
        public bool DirectionY { get { return false; } }
    }
}