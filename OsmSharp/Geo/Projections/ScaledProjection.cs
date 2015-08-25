//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace OsmSharp.Math.Geo.Projections
//{
//    /// <summary>
//    /// A scaled project to have more numerically stable calculations in some cases (for example when rendering).
//    /// </summary>
//    public class ScaledProjection : IProjection
//    {
//        /// <summary>
//        /// Holds the real projection.
//        /// </summary>
//        private readonly IProjection _projection;

//        /// <summary>
//        /// Holds the scale factor.
//        /// </summary>
//        private readonly double _scaleFactor;

//        /// <summary>
//        /// Creates a new scaled projection.
//        /// </summary>
//        /// <param name="projection">The real projection.</param>
//        /// <param name="scaleFactor">The scalefactor to scale the projection.</param>
//        public ScaledProjection(IProjection projection, double scaleFactor)
//        {
//            _projection = projection;
//            _scaleFactor = scaleFactor;
//        }

//        /// <summary>
//        /// Converts the given lat lon to pixels.
//        /// </summary>
//        /// <returns>The pixel.</returns>
//        /// <param name="lat">Lat.</param>
//        /// <param name="lon">Lon.</param>
//        public double[] ToPixel(double lat, double lon)
//        {
//            double[] unscaled = _projection.ToPixel(lat, lon);
//            return new double[] { unscaled[0] * _scaleFactor, unscaled[1] * _scaleFactor };
//        }

//        /// <summary>
//        /// Converts the given coordinate to pixels.
//        /// </summary>
//        /// <returns>The pixel.</returns>
//        /// <param name="coordinate">Coordinate.</param>
//        public double[] ToPixel(GeoCoordinate coordinate)
//        {
//            double[] unscaled = _projection.ToPixel(coordinate);
//            return new double[] { unscaled[0] * _scaleFactor, unscaled[1] * _scaleFactor };
//        }

//        /// <summary>
//        /// Converts the given x-y pixel coordinates into geocoordinates.
//        /// </summary>
//        /// <returns>The geo coordinates.</returns>
//        /// <param name="x">The x coordinate.</param>
//        /// <param name="y">The y coordinate.</param>
//        public GeoCoordinate ToGeoCoordinates(double x, double y)
//        {
//            return _projection.ToGeoCoordinates(x / _scaleFactor, y / _scaleFactor);
//        }

//        /// <summary>
//        /// Converts the given x-pixel coordinate into logitude.
//        /// </summary>
//        /// <returns>The x.</returns>
//        /// <param name="longitude">Longitude.</param>
//        public double LongitudeToX(double longitude)
//        {
//            return _projection.LongitudeToX(longitude) * _scaleFactor;
//        }

//        /// <summary>
//        /// Converts the given y-pixel coordinate into logitude.
//        /// </summary>
//        /// <returns>The y.</returns>
//        /// <param name="latitude">Latitude.</param>
//        public double LatitudeToY(double latitude)
//        {
//            return _projection.LatitudeToY(latitude) * _scaleFactor;
//        }

//        /// <summary>
//        /// Converts the given y-coordinate to latitude.
//        /// </summary>
//        /// <returns>The latitude.</returns>
//        /// <param name="y">The y coordinate.</param>
//        public double YToLatitude(double y)
//        {
//            return _projection.YToLatitude(y / _scaleFactor);
//        }

//        /// <summary>
//        /// Converts the given x-coordinate to longitude.
//        /// </summary>
//        /// <returns>The longitude.</returns>
//        /// <param name="x">The x coordinate.</param>
//        public double XToLongitude(double x)
//        {
//            return _projection.XToLongitude(x / _scaleFactor);
//        }

//        /// <summary>
//        /// Converts the given zoom level to a zoomfactor for this projection.
//        /// </summary>
//        /// <param name="zoomLevel"></param>
//        /// <returns></returns>
//        public double ToZoomFactor(double zoomLevel)
//        {
//            return _projection.ToZoomFactor(zoomLevel)/_scaleFactor;
//        }
//    }
//}