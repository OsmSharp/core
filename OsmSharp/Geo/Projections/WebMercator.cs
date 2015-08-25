using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        public double[] ToPixel(double lat, double lon)
        {
            double n = System.Math.Floor(System.Math.Pow(2, DefaultZoom));

            Radian rad = new Degree(lat);

            double x = (((lon + 180.0f) / 360.0f) * (double)n);
            double y = (
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);

            return new double[]{x,y};
        }

        /// <summary>
        /// Converts the lat/lon to projected coordinates.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public double[] ToPixel(GeoCoordinate coordinate)
        {
            return this.ToPixel(coordinate.Latitude, coordinate.Longitude);
        }

        /// <summary>
        /// Converts the given projected coordinates to lat/lon.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public GeoCoordinate ToGeoCoordinates(double x, double y)
        {
            double n = System.Math.PI - ((2.0 * System.Math.PI * (y)) / System.Math.Pow(2.0, DefaultZoom));

            double longitude = (((x) / System.Math.Pow(2.0, DefaultZoom) * 360.0) - 180.0);
            double latitude = (180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(n)));

            return new GeoCoordinate(latitude, longitude);
        }

        /// <summary>
        /// Converts longitude to a projected x-coordinate.
        /// </summary>
        /// <param name="lon"></param>
        /// <returns></returns>
        public double LongitudeToX(double lon)
        {
            double n = System.Math.Floor(System.Math.Pow(2, DefaultZoom));

            return (((lon + 180.0f) / 360.0f) * n);
        }

        /// <summary>
        /// Converts the latitude to a projected y-coordinate.
        /// </summary>
        /// <param name="lat"></param>
        /// <returns></returns>
        public double LatitudeToY(double lat)
        {
            double n = System.Math.Floor(System.Math.Pow(2, DefaultZoom));

            Radian rad = new Degree(lat);

            return (
                (1.0f - (System.Math.Log(System.Math.Tan(rad.Value) + (1.0f / System.Math.Cos(rad.Value))))
                / System.Math.PI) / 2f * (double)n);
        }

        /// <summary>
        /// Converts the projected y-coordinate to latitude.
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public double YToLatitude(double y)
        {
            double n = System.Math.PI - ((2.0 * System.Math.PI * (y)) / System.Math.Pow(2.0, DefaultZoom));

            return  (180.0 / System.Math.PI * System.Math.Atan(System.Math.Sinh(n)));
        }

        /// <summary>
        /// Converts the projected x-coordinate to longitude.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double XToLongitude(double x)
        {
            return (((x) / System.Math.Pow(2.0, DefaultZoom) * 360.0) - 180.0);
        }

        /// <summary>
        /// Returns the scale for the given zoomlevel.
        /// </summary>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        public double ToZoomFactor(double zoomLevel)
        {
            return System.Math.Pow(2, zoomLevel - DefaultZoom) * 256.0;
        }

        /// <summary>
        /// Returns the scale for the given zoomFactor.
        /// </summary>
        /// <param name="zoomFactor"></param>
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