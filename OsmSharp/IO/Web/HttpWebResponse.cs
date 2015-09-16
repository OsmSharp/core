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

using System.IO;

namespace OsmSharp.IO.Web
{

    /// <summary>
    /// A cross-plaform wrapper around plaform-specific HttpWebRepond classes.
    /// </summary>
    public abstract class HttpWebResponse
    {
        /// <summary>
        /// Gets the status of the response.
        /// </summary>
        public abstract HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Returns the data stream from the Internet resource.
        /// </summary>
        /// <returns></returns>
        public abstract Stream GetResponseStream();

		/// <summary>
		/// Closes this instance.
		/// </summary>
		public abstract void Close();
    }

    /// <summary>
    /// A default implementation the HttpWebResponse.
    /// </summary>
	internal class HttpWebResponseDefault : HttpWebResponse
    {
        /// <summary>
        /// Holds the http webresponse.
        /// </summary>
        private System.Net.HttpWebResponse _httpWebResponse;

        /// <summary>
        /// Creates a new http webresponse.
        /// </summary>
        /// <param name="httpWebResponse"></param>
        public HttpWebResponseDefault(System.Net.HttpWebResponse httpWebResponse)
        {
            _httpWebResponse = httpWebResponse;
        }

        /// <summary>
        /// Gets the status of the response.
        /// </summary>
        public override HttpStatusCode StatusCode
        {
            get
            {
                switch (_httpWebResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        return HttpStatusCode.NotFound;
                    case System.Net.HttpStatusCode.Forbidden:
                        return HttpStatusCode.Forbidden;
                    default:
                        return HttpStatusCode.Other;
                };
            }
        }

        /// <summary>
        /// Returns the data stream from the Internet resource.
        /// </summary>
        /// <returns></returns>
        public override Stream GetResponseStream()
        {
            return _httpWebResponse.GetResponseStream();
        }

		/// <summary>
		/// Closes this instance.
		/// </summary>
		public override void Close ()
		{
			
		}
    }
}
