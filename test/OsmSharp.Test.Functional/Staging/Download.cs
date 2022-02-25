// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.IO;
using System.Net;

namespace OsmSharp.Test.Functional.Staging
{
    /// <summary>
    /// Downloads all data needed for testing.
    /// </summary>
    public static class Download
    {
        public static string PBF = "http://planet.anyways.eu/planet/europe/luxembourg/luxembourg-latest.osm.pbf";
        public static string Local = "luxembourg-latest.osm.pbf";

        /// <summary>
        /// Downloads the belgium data.
        /// </summary>
        public static void DownloadAll()
        {
            if (!File.Exists(Download.Local))
            {
                var client = new WebClient();
                client.DownloadProgressChanged += (sender, e) =>
                { // Displays the operation identifier, and the transfer progress.
                    OsmSharp.Logging.Logger.Log("Download", Logging.TraceEventType.Information, 
                        "{0}    downloaded {1} of {2} bytes. {3} % complete...",
                        (string)e.UserState, e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage);
                };
                client.DownloadFile(Download.PBF,
                    Download.Local);
            }
        }
    }
}
