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

using System;

namespace OsmSharp.IO.Web
{
    /// <summary>
    /// A cross-plaform wrapper around plaform-specific HttpWebRequest classes.
    /// 
    /// Creates the PCL-version of an HttpWebRequest by default.
    /// </summary>
    public abstract class HttpWebRequest
    {
        /// <summary>
        /// Delegate that can be used to create a native webrequest.
        /// </summary>
        /// <returns></returns>
        public delegate HttpWebRequest CreateNativeWebRequestDelegate(string url);

        /// <summary>
        /// Holds and instances of the delegate to create a native webrequest.
        /// </summary>
        public static CreateNativeWebRequestDelegate CreateNativeWebRequest;

        /// <summary>
        /// Creates a new web request.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpWebRequest Create(string url)
        {
            if (HttpWebRequest.CreateNativeWebRequest != null)
            {
                return HttpWebRequest.CreateNativeWebRequest(url);
            }
            return new HttpWebRequestDefault(url);
        }

        /// <summary>
        /// Gets or sets the accept header.
        /// </summary>
        public abstract string Accept { get; set; }

        /// <summary>
        /// Returns true if the user-agent can be set.
        /// </summary>
        public abstract bool IsUserAgentSupported
        {
            get;
        }

        /// <summary>
        /// Gets or sets the user-agent if possible (check IsUserAgentSupported).
        /// </summary>
        public abstract string UserAgent { get; set; }

        /// <summary>
        /// Begins an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public abstract IAsyncResult BeginGetResponse(AsyncCallback callback, object state);

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="iar"></param>
        /// <returns></returns>
        public abstract HttpWebResponse EndGetResponse(IAsyncResult iar);

		/// <summary>
		/// Abort this instance.
		/// </summary>
		public abstract void Abort();
    }

    /// <summary>
    /// A default implementation the HttpWebRequest.
    /// </summary>
	internal class HttpWebRequestDefault : HttpWebRequest
    {
        /// <summary>
        /// Holds the http webrequest.
        /// </summary>
        private System.Net.HttpWebRequest _httpWebRequest;

        /// <summary>
        /// Creates a new default http webrequest.
        /// </summary>
        /// <param name="url"></param>
        public HttpWebRequestDefault(string url)
        {
            _httpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);
        }

        /// <summary>
        /// Gets or sets the value of the Accept HTTP header.
        /// </summary>
        public override string Accept
        {
            get
            {
                return _httpWebRequest.Accept;
            }
            set
            {
                _httpWebRequest.Accept = value;
            }
        }

        /// <summary>
        /// Returns true if the user-agent can be set.
        /// </summary>
        public override bool IsUserAgentSupported
        {
            get { return false; }
        }

        /// <summary>
        /// Gets or sets the user-agent if possible.
        /// </summary>
        public override string UserAgent
        {
            get
            {
                throw new NotSupportedException("Getting or setting the user-agent is not possible. Check IsUserAgentSupported.");
            }
            set
            {
                throw new NotSupportedException("Getting or setting the user-agent is not possible. Check IsUserAgentSupported.");
            }
        }

        /// <summary>
        /// Begins an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            return _httpWebRequest.BeginGetResponse(callback, state);
        }

        /// <summary>
        /// Ends an asynchronous request to an Internet resource.
        /// </summary>
        /// <param name="iar"></param>
        /// <returns></returns>
        public override HttpWebResponse EndGetResponse(IAsyncResult iar)
        {
            return new HttpWebResponseDefault((System.Net.HttpWebResponse)_httpWebRequest.EndGetResponse(iar));
        }

		/// <summary>
		/// Abort this instance.
		/// </summary>
		public override void Abort ()
		{
			_httpWebRequest.Abort ();
		}
    }
}