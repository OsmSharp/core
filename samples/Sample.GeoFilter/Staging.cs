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

using GeoAPI.Geometries;
using NetTopologySuite.Features;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace Sample.GeoFilter
{
    /// <summary>
    /// Contains code to handle test data.
    /// </summary>
    public static class Staging
    {
        /// <summary>
        /// Downloads a file if it doesn't exist yet.
        /// </summary>
        public static void ToFile(string url, string filename)
        {
            if (File.Exists(filename)) return;
            var client = new WebClient();
            client.DownloadFile(url, filename);
        }

        /// <summary>
        /// Loads the test polygon.
        /// </summary>
        /// <returns></returns>
        internal static IPolygon LoadPolygon()
        {
            using (var stream = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Sample.GeoFilter.polygon.geojson")))
            {
                var jsonSerializer = NetTopologySuite.IO.GeoJsonSerializer.Create();
                var features = jsonSerializer.Deserialize<FeatureCollection>(new JsonTextReader(stream));
                return features.Features[0].Geometry as IPolygon;
            }
        }
    }
}
