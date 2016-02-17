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

using OsmSharp.Changesets;
using System;

namespace OsmSharp.Db
{
    /// <summary>
    /// An expanded diff result result.
    /// </summary>
    public class DiffResultResult
    {
        /// <summary>
        /// Creates a new diffresult result as en error.
        /// </summary>
        public DiffResultResult(string message)
            : this(message, DiffResultStatus.UknownError)
        {

        }

        /// <summary>
        /// Creates a new diffresult result as en error.
        /// </summary>
        public DiffResultResult(string message, DiffResultStatus status)
        {
            if (status == DiffResultStatus.BestEffortOK || status == DiffResultStatus.OK)
            {
                throw new ArgumentOutOfRangeException("Cannot create an error-result with an ok status.");
            }

            this.Message = message;
            this.Result = null;
            this.Status = status;
        }

        /// <summary>
        /// Creates a new diffresult result.
        /// </summary>
        public DiffResultResult(DiffResult result,
            DiffResultStatus status)
        {
            if (result == null) { throw new ArgumentNullException("result"); }
            if (status != DiffResultStatus.BestEffortOK && status != DiffResultStatus.OK)
            {
                throw new ArgumentOutOfRangeException("Cannot create an ok-result with a non-ok status.");
            }

            this.Status = status;
            this.Result = result;
        }

        /// <summary>
        /// Gets or sets the diff result.
        /// </summary>
        public DiffResult Result { get; set; }

        /// <summary>
        /// Gets or sets the diff result status.
        /// </summary>
        public DiffResultStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Status after applying a changeset.
    /// </summary>
    public enum DiffResultStatus
    {
        /// <summary>
        /// Changeset was applied correctly.
        /// </summary>
        OK,
        /// <summary>
        /// Changeset was applied using best effort correctly.
        /// </summary>
        BestEffortOK,
        /// <summary>
        /// Conflict in one of the changes.
        /// </summary>
        Conflict,
        /// <summary>
        /// Changeset too big.
        /// </summary>
        TooBig,
        /// <summary>
        /// Changeset was not open.
        /// </summary>
        NotOpen,
        /// <summary>
        /// Unknown error.
        /// </summary>
        UknownError
    };
}