using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Math.Geo;

namespace OsmSharp.Math
{
    /// <summary>
    /// An object that represents a location.
    /// </summary>
    public interface ILocationObject
    {
        /// <summary>
        /// Returns the location of this object.
        /// </summary>
        GeoCoordinate Location
        {
            get;
        }
    }
}
