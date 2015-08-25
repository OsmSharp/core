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
using System.Net;
using System.Text;
using System.Threading;
using OsmSharp.Xml;
using OsmSharp.Xml.Nominatim.Reverse.v1;
using OsmSharp.Xml.Sources;
using OsmSharp.Xml.Nominatim.Search;
using OsmSharp.IO.Xml.Nominatim.Search.v1;

namespace OsmSharp.GeoCoding.Nominatim
{
    /// <summary>
    /// Creates this geocoder.
    /// </summary>
    public class GeoCoder : IGeoCoder, IDisposable
    {
        /// <summary>
        /// The url of the nomatim service.
        /// </summary>
        private string _geocodingUrl; // = ConfigurationManager.AppSettings["NomatimAddress"] + ;

        ///// <summary>
        ///// The default timeout.
        ///// </summary>
//        private int _timeOut = 10000;

        /// <summary>
        /// Holds the web client used to access the nomatim service.
        /// </summary>
        private WebClient _webClient;

        /// <summary>
        /// Creates a new nominatim geocoder.
        /// </summary>
        /// <param name="geocodingUrl">The nominatim API URL. (ex: "http://nominatim.openstreetmap.org/search?q={0}")</param>
        public GeoCoder(string geocodingUrl)
        {
            _geocodingUrl = geocodingUrl;
        }

        /// <summary>
        /// Creates a new nominatim geocoder.
        /// </summary>
        /// <param name="geocodingUrl">The nominatim API URL. (ex: "http://nominatim.openstreetmap.org/search?q={0}")</param>
        /// <param name="timeOut">The maximum time-out allowed for the request to complete.</param>
        public GeoCoder(string geocodingUrl, int timeOut)
        {
            _geocodingUrl = geocodingUrl;
            //_timeOut = timeOut;
        }

        #region IGeoCoder Members

        /// <summary>
        /// Geocodes and returns the result.
        /// </summary>
        /// <param name="street"></param>
        /// <param name="houseNumber"></param>
        /// <param name="country"></param>
        /// <param name="postalCode"></param>
        /// <param name="commune"></param>
        /// <returns></returns>
        public IGeoCoderResult Code(
            string country,
            string postalCode,
            string commune,
            string street,
            string houseNumber)
        {
            // build the request url.        
            var builder = new StringBuilder();
            builder.Append(street);
            builder.Append(" ");
            builder.Append(houseNumber);
            builder.Append(" ");
            builder.Append(postalCode);
            builder.Append(" ");
            builder.Append(commune);
            builder.Append(" ");
            builder.Append(country);
            builder.Append(" ");
            string url = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                _geocodingUrl + "&format=xml&polygon=1&addressdetails=1", builder);

            // create the source and get the xml.
            IXmlSource source = this.DownloadXml(url);

            // create the kml.
            var search_doc = new SearchDocument(source);

            // check if there are responses.
            var res = new GeoCoderResult();
            res.Accuracy = AccuracyEnum.UnkownLocationLevel;

            if (search_doc.Search is searchresults)
            {
                searchresults result_v1 = search_doc.Search as searchresults;
                if (result_v1.place != null && result_v1.place.Length > 0)
                {
                    double latitude;
                    double longitude;

                    if (double.TryParse(result_v1.place[0].lat, System.Globalization.NumberStyles.Float,
                                        System.Globalization.CultureInfo.InvariantCulture, out latitude)
                        &&
                        double.TryParse(result_v1.place[0].lon, System.Globalization.NumberStyles.Float,
                                        System.Globalization.CultureInfo.InvariantCulture, out longitude))
                    {
                        res.Latitude = latitude;
                        res.Longitude = longitude;
                        res.Text = result_v1.place[0].display_name;

                        switch (result_v1.place[0].@class)
                        {
                            case "place":
                                switch (result_v1.place[0].type)
                                {
                                    case "town":
                                        res.Accuracy = AccuracyEnum.TownLevel;
                                        break;
                                    case "house":
                                        res.Accuracy = AccuracyEnum.AddressLevel;
                                        break;
                                }
                                break;
                            case "highway":
                                res.Accuracy = AccuracyEnum.StreetLevel;
                                break;
                            case "boundary":
                                res.Accuracy = AccuracyEnum.PostalCodeLevel;
                                break;
                        }
                    }
                }
            }
            else if (search_doc.Search is OsmSharp.Xml.Nominatim.Reverse.v1.reversegeocode)
            {
                reversegeocode result_v1 = search_doc.Search as OsmSharp.Xml.Nominatim.Reverse.v1.reversegeocode;
                if (result_v1.result != null && result_v1.result.Length > 0)
                {
                    double latitude;
                    double longitude;

                    if (double.TryParse(result_v1.result[0].lat, System.Globalization.NumberStyles.Float,
                                        System.Globalization.CultureInfo.InvariantCulture, out latitude)
                        &&
                        double.TryParse(result_v1.result[0].lon, System.Globalization.NumberStyles.Float,
                                        System.Globalization.CultureInfo.InvariantCulture, out longitude))
                    {
                        res.Latitude = latitude;
                        res.Longitude = longitude;
                        res.Text = result_v1.result[0].Value;
                        res.Accuracy = AccuracyEnum.UnkownLocationLevel;
                    }
                }
            }

            return res;
        }

        #endregion

        #region Base-Api-Functions

        /// <summary>
        /// Downloads an xml from an url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private IXmlSource DownloadXml(string url)
        {
            // download the xml string.
            string xml = this.DownloadString(url);

            // parse the xml if it exists.
            IXmlSource source = null;
            if (!string.IsNullOrEmpty(xml))
            {
                source = new XmlStringSource(xml);
            }
            return source;
        }

        /// <summary>
        /// Async geocoding result downloader.
        /// </summary>
        private class AsyncStringDownloader : IDisposable
        {
            /// <summary>
            /// Holds the result when available.
            /// </summary>
            private string _result = null;

            /// <summary>
            /// Thread-waiting functionality!
            /// </summary>
            private AutoResetEvent _autoResetEvent;

            /// <summary>
            /// Do the sync download using the async functionality on webclient.
            /// </summary>
            /// <param name="client">The webclient object to use.</param>
            /// <param name="url">The url to download from.</param>
            /// <param name="timeOut">The timeout in milliseconds.</param>
            /// <returns></returns>
            public string DownloadString(WebClient client, string url, int timeOut)
            {
                // initialize the reset-event.
                _autoResetEvent = new AutoResetEvent(false);

                // make the call!
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(ClientDownloadStringCompleted);
                client.DownloadStringAsync(new Uri(url));

                // keep waiting until result or time-out.
                if (!_autoResetEvent.WaitOne(timeOut))
                { // oeps! the flag was not set!
                    throw new TimeoutException(string.Format("Request could not be completed in allowed timeout: {0}!",
                        timeOut));
                }

                return _result;
            }

            /// <summary>
            /// Event handler when result is available.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            void ClientDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
            {
                _result = e.Result;

                // signal the other thread the result is available.
                _autoResetEvent.Set();
            }

            /// <summary>
            /// Releases all resources associated with this object.
            /// </summary>
            public void Dispose()
            {
                if (_autoResetEvent != null)
                {
                    _autoResetEvent.Dispose();
                    _autoResetEvent = null;
                }
            }
        }

        /// <summary>
        /// Downloads a string from an url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string DownloadString(string url)
        {
            // create the webclient if needed.
            if (_webClient == null)
            {
                _webClient = new WebClient();
            }

            try
            { // try to download the string.
                return (new AsyncStringDownloader()).DownloadString(_webClient, url, 10000);
            }
            catch (WebException)
            {
                return string.Empty;
            }
        }

        #endregion
        
        /// <summary>
        /// Releases all resources associated with this object.
        /// </summary>
        public void Dispose()
        {
            if (_webClient != null)
            {
                _webClient.Dispose();
                _webClient = null;
            }
        }
    }
}