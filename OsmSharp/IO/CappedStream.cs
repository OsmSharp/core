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
using System.IO;

namespace OsmSharp.IO
{
    /// <summary>
    /// Represents a capped stream that can only be used along a given region.
    /// </summary>
    public class CappedStream : Stream
    {
        private readonly Stream _stream;
        private readonly long _offset;
        private readonly long _length;

        /// <summary>
        /// Creates a new capped stream.
        /// </summary>
        public CappedStream(Stream stream, long offset, long length)
        {
            _stream = stream;
            _stream.Seek(offset, SeekOrigin.Begin);
            _offset = offset;
            _length = length;
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get { return false; }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            _stream.Flush();
        }

        /// <summary>
        /// Returns the current length of this stream.
        /// </summary>
        public override long Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Gets/sets the current position.
        /// </summary>
        public override long Position
        {
            get { return _stream.Position - _offset; }
            set { _stream.Position = value + _offset; }
        }

        /// <summary>
        /// Reads a sequence of bytes from the current
        ///     stream and advances the position within the stream by the number of bytes
        ///     read.
        /// </summary>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this.Position + count >= _length)
            {
                count = (int)(_length - this.Position);
                return _stream.Read(buffer, offset, count);
            }
            return _stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Sets the position within the current
        ///     stream.
        /// </summary>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (offset > _length)
            {
                throw new Exception("Cannot read past end of capped stream.");
            }
            return _stream.Seek(offset + _offset, origin);
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        public override void SetLength(long value)
        {
            _stream.SetLength(value + _offset);
        }

        /// <summary>
        ///  Tests if it's possible to write a sequence of bytes to the current
        ///     stream and advance the current position within this stream by the number
        ///     of bytes.
        /// </summary>
        /// <returns></returns>
        public bool WriteBytePossible()
        {
            return !(this.Position + 1 > this.Length);
        }

        /// <summary>
        ///  Tests if it's possible to write a sequence of bytes to the current
        ///     stream and advance the current position within this stream by the number
        ///     of bytes.
        /// </summary>
        /// <returns></returns>
        public bool WritePossible(int offset, int count)
        {
            return !(this.Position + count > this.Length);
        }

        /// <summary>
        ///  Writes a sequence of bytes to the current
        ///     stream and advances the current position within this stream by the number
        ///     of bytes written.
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this.Position + count > this.Length)
            {
                throw new Exception("Cannot write past end of capped stream.");
            }
            _stream.Write(buffer, offset, count);
        }
    }
}