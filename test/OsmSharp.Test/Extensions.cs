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
using System.IO;

namespace OsmSharp.Test
{
    /// <summary>
    /// Contains extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Reads a string.
        /// </summary>
        public static string ReadBeginToEnd(this MemoryStream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();
        }
    }


    public static class DateTimeHelpers
    {
        public static DateTime UnixEpoch()
        {
#if NET462
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
#else
            return DateTime.UnixEpoch;
#endif
        }

        public static DateTime FromMillisecondsSinceUnixEpoch(long milliseconds)
        {
            return UnixEpoch().AddMilliseconds(milliseconds).ToLocalTime();
        }

        public static long ToMillisecondsSinceUnixEpoch(DateTime dateTime)
        {
            return (long)(dateTime - UnixEpoch()).TotalMilliseconds;
        }
    }
}