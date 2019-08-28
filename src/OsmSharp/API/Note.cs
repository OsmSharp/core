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
    /// Represents a Note.
    /// </summary>
    public partial class Note
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// The latitude.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// The longitude.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// The url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The comment url.
        /// </summary>
        public string CommentUrl { get; set; }

        /// <summary>
        /// The close url.
        /// </summary>
        public string CloseUrl { get; set; }

        /// <summary>
        /// The creation date.
        /// </summary>
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// The status.
        /// </summary>
        public NoteStatus? Status { get; set; }

        /// <summary>
        /// The comment.
        /// </summary>
        public CommentsContainer Comments { get; set; }

        public enum NoteStatus
        {
            Open,
            Closed
        }

        /// <summary>
        /// Represents a set of CommentsContainer.
        /// </summary>
        public partial class CommentsContainer
        {
            /// <summary>
            /// The comments.
            /// </summary>
            public Comment[] Comments { get; set; }
        }

        /// <summary>
        /// Represents a Comment.
        /// </summary>
        public partial class Comment
        {
            /// <summary>
            /// The date.
            /// </summary>
            public DateTime? Date { get; set; }

            /// <summary>
            /// The id of the user that created the comment, or null if the comment is anonymous.
            /// </summary>
            public long? UserId { get; set; }

            /// <summary>
            /// The name of the user that created the comment, or null if the comment is anonymous.
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// The url of the user that created the comment, or null if the comment is anonymous.
            /// </summary>
            public string UserUrl { get; set; }

            /// <summary>
            /// The action performed with this comment.
            /// </summary>
            public CommentAction? Action { get; set; }

            /// <summary>
            /// The text.
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// The text, styled to be embedded in html.
            /// </summary>
            public string HTML { get; set; }

            public enum CommentAction
            {
                Opened,
                Commented,
                Closed,
                ReOpened
            }
        }
    }
}