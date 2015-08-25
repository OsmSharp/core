using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Osm.Data.Core.API
{
    /// <summary>
    /// Exception used for errors when speaking to the OSM API.
    /// </summary>
    public class APIException : Exception
    {
        /// <summary>
        /// Creates a simple API exception with just a message.
        /// </summary>
        /// <param name="message"></param>
        public APIException(string message)
            : base(message)
        {

        }
    }
}
