using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using OsmSharp.Collections.SpatialIndexes.Serialization.v2;
using OsmSharp.Collections.Tags;
using OsmSharp.IO;
using OsmSharp.Math.Geo;

namespace OsmSharp.Collections.SpatialIndexes.Serialization
{
    /// <summary>
    /// Abstract representation of a spatial index serializer.
    /// </summary>
    /// <remarks>Versioning is implemented in the file format to guarantee backward compatibility.</remarks>
    public abstract class SpatialIndexSerializer<T>
    {
        #region Versioning

        /// <summary>
        /// Returns the version number.
        /// </summary>
        public abstract string VersionString { get; }

        /// <summary>
        /// Builds a uniform version header.
        /// </summary>
        /// <returns></returns>
        private byte[] BuildVersionHeader()
        {
            return System.Text.UTF8Encoding.UTF8.GetBytes(this.VersionString);
        }

        /// <summary>
        /// Writes the version header.
        /// </summary>
        private void WriteVersionHeader(Stream stream)
        {
            // seek to the beginning of the stream.
            stream.Seek(0, SeekOrigin.Begin);

            // write the header bytes.
            byte[] header = this.BuildVersionHeader();
            stream.Write(header, 0, header.Length);
        }

        /// <summary>
        /// Reads and validates the header.
        /// </summary>
        /// <param name="stream"></param>
        private bool ReadAndValidateHeader(Stream stream)
        {
            // get the original version header.
            byte[] header = this.BuildVersionHeader();

            try
            {
                // get the version string.
                byte[] presentHeader = new byte[header.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(presentHeader, 0, header.Length);

                for (int idx = 0; idx < header.Length; idx++)
                {
                    if (header[idx] != presentHeader[idx])
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {

            }
            return false;
        }

        #endregion

        /// <summary>
        /// Returns true if this serializer can deserialize the data in the given stream.
        /// </summary>
        /// <param name="stream"></param>
        public virtual bool CanDeSerialize(Stream stream)
        {
            bool canRead = this.ReadAndValidateHeader(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return canRead;
        }

        /// <summary>
        /// Serializes the given graph and tags index to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="index"></param>
        public void Serialize(Stream stream, RTreeMemoryIndex<T> index)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (index == null)
                throw new ArgumentNullException("index");

            // write the header.
            this.WriteVersionHeader(stream);

            // wrap the stream.
            var spatialIndexSerializerStream = new SpatialIndexSerializerStream(stream);

            // do the version-specific serialization.
            this.DoSerialize(spatialIndexSerializerStream, index);
        }

        /// <summary>
        /// Serializes the given index to the given stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="index"></param>
        protected abstract void DoSerialize(SpatialIndexSerializerStream stream, RTreeMemoryIndex<T> index);

        /// <summary>
        /// Deserializes the given stream into an index.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lazy"></param>
        /// <returns></returns>
        public ISpatialIndexReadonly<T> Deserialize(Stream stream, bool lazy = true)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (this.CanDeSerialize(stream))
            {
                // read/verify the current version header.
                this.ReadAndValidateHeader(stream);

                // wrap the stream.
                var spatialIndexSerializerStream = new SpatialIndexSerializerStream(stream);

                // do the actual version-specific deserialization.
                return this.DoDeserialize(spatialIndexSerializerStream, lazy);
            }
            throw new ArgumentOutOfRangeException("stream", "Cannot deserialize the given stream, version unsupported or content unrecognized!");
        }

        /// <summary>
        /// Deserializes the given stream into an index.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="lazy"></param>
        /// <returns></returns>
        protected abstract ISpatialIndexReadonly<T> DoDeserialize(SpatialIndexSerializerStream stream, bool lazy);
    }
}