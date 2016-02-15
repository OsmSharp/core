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

using System;

namespace OsmSharp.API
{
    /// <summary>
    /// Represents a user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the account created timestamp.
        /// </summary>
        public DateTime AccountCreated { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the contributor terms agreed flag.
        /// </summary>
        public bool ContributorTermsAgreed { get; set; }

        /// <summary>
        /// Gets or sets the contributor terms public domain.
        /// </summary>
        public bool ContributorTermsPublicDomain { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Gets or sets the roles array.
        /// </summary>
        public Role[] Roles { get; set; }

        /// <summary>
        /// Gets or sets the changeset count.
        /// </summary>
        public int ChangeSetCount { get; set; }

        /// <summary>
        /// Gets or sets the tracecount.
        /// </summary>
        public int TraceCount { get; set; }

        /// <summary>
        /// Gets or sets the blocks recieved.
        /// </summary>
        public Block[] BlocksReceived { get; set; }

        /// <summary>
        /// Gets or sets the home location.
        /// </summary>
        public Home Home { get; set; }
    }
    
    /// <summary>
    /// Represents a home location.
    /// </summary>
    public class Home
    {
        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public float Longitude { get; set; }

        /// <summary>
        /// Gets or sets the zoom.
        /// </summary>
        public float Zoom { get; set; }
    }

    /// <summary>
    /// Represent a block.
    /// </summary>
    public class Block
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the active count.
        /// </summary>
        public int Active { get; set; }
    }

    /// <summary>
    /// Represents a role.
    /// </summary>
    public class Role
    {

    }
}