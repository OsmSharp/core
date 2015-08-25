using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.Collections.SpatialIndexes.Serialization
{
    /// <summary>
    /// Wraps a stream to prevent some fixed data from being overwritten.
    /// </summary>
    public class SpatialIndexSerializerStream : Stream
    {
        /// <summary>
        /// Holds the offset or the header length.
        /// </summary>
        private readonly long _offset;

        /// <summary>
        /// Holds the stream.
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        /// Creates a new spatial index serializer stream.
        /// </summary>
        /// <param name="stream"></param>
        public SpatialIndexSerializerStream(Stream stream)
        {
            _stream = stream;
            _offset = _stream.Position;
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
            get { return _stream.CanWrite; }
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
            get { return _stream.Length - _offset; }
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
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        /// <summary>
        /// Sets the position within the current
        ///     stream.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset + _offset, origin);
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            _stream.SetLength(value + _offset);
        }

        /// <summary>
        ///  Writes a sequence of bytes to the current
        ///     stream and advances the current position within this stream by the number
        ///     of bytes written.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }
    }
}