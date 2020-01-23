// The MIT License (MIT)

// Copyright (c) 2017 Ben Abelshausen

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

using System;

namespace OsmSharp.API
{
    /// <summary>
    /// Represents a GPX file.
    /// </summary>
    public partial class GpxFile
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double? Lat { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double? Lon { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets or sets the visibility.
        /// </summary>
        public Visibility? Visibility { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the pending approval.
        /// </summary>
        public bool Pending { get; set; }

        /// <summary>
        /// Gets or sets the tags.
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTime TimeStamp { get; set; }
    }

    public enum Visibility
    {
        /// <summary>
        /// The trace will not show up in any public listings, but trackpoints from it will
        /// still be available through the public GPS API without timestamps but will not be
        /// chronologically ordered.
        /// </summary>
        Private,

        /// <summary>
        /// The trace will be shown publicly in Your GPS traces and in public GPS trace listings.
        /// Data served via the API does not reference your trace page. Timestamps of the trace
        /// points are not available through the public GPS API, and the points are not
        /// chronologically ordered. However, other users are still able to download the raw
        /// trace from the public trace list and any timestamps contained within.
        /// </summary>
        Public,

        /// <summary>
        /// The trace will not show up in any public listings but trackpoints from it will still be
        /// available through the public GPS API with timestamps. Other users will only be able to
        /// download processed trackpoints from your trace which can't be associated with you directly.
        /// </summary>
        Trackable,

        /// <summary>
        /// The trace will be shown publicly in Your GPS traces and in public GPS trace listings,
        /// i.e. other users will be able to download the raw trace and associate it with your username.
        /// Data served via the trackpoints API will reference your original trace page. Timestamps of
        /// the trace points are available through the public GPS API.
        /// </summary>
        Identifiable
    }
}
