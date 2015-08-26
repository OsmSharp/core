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

namespace OsmSharp.Math.Geo.Projections
{
	/// <summary>
	/// An abstract of a projection.
	/// </summary>
	public interface IProjection
	{
		/// <summary>
		/// Converts the given lat lon to pixels.
		/// </summary>
		/// <returns>The pixel.</returns>
		/// <param name="lat">Lat.</param>
		/// <param name="lon">Lon.</param>
		double[] ToPixel(double lat, double lon);

		/// <summary>
		/// Converts the given coordinate to pixels.
		/// </summary>
		/// <returns>The pixel.</returns>
		/// <param name="coordinate">Coordinate.</param>
		double[] ToPixel(GeoCoordinate coordinate);

		/// <summary>
		/// Converts the given x-y pixel coordinates into geocoordinates.
		/// </summary>
		/// <returns>The geo coordinates.</returns>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		GeoCoordinate ToGeoCoordinates(double x, double y);

		/// <summary>
		/// Converts the given x-pixel coordinate into logitude.
		/// </summary>
		/// <returns>The x.</returns>
		/// <param name="longitude">Longitude.</param>
		double LongitudeToX(double longitude);
		
		/// <summary>
		/// Converts the given y-pixel coordinate into logitude.
		/// </summary>
		/// <returns>The y.</returns>
		/// <param name="latitude">Latitude.</param>
		double LatitudeToY(double latitude);

		/// <summary>
		/// Converts the given y-coordinate to latitude.
		/// </summary>
		/// <returns>The latitude.</returns>
		/// <param name="y">The y coordinate.</param>
		double YToLatitude(double y);

		/// <summary>
		/// Converts the given x-coordinate to longitude.
		/// </summary>
		/// <returns>The longitude.</returns>
		/// <param name="x">The x coordinate.</param>
		double XToLongitude(double x);

        /// <summary>
        /// Converts the given zoom level to a given zoom factor for this projection.
        /// </summary>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        double ToZoomFactor(double zoomLevel);

        /// <summary>
        /// Converts the given zoom factor to a given zoom level for this projection.
        /// </summary>
        /// <param name="zoomFactor"></param>
        /// <returns></returns>
        double ToZoomLevel(double zoomFactor);

        /// <summary>
        /// Returns true if this projection uses lowest left, highest right. False otherwise.
        /// </summary>
        bool DirectionX { get; }

        /// <summary>
        /// Returns true if this projection uses lowest bottom, highest top. False otherwise.
        /// </summary>
        bool DirectionY { get; }
	}
}