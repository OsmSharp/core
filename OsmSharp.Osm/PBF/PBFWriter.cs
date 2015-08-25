using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.Osm.PBF
{
    /// <summary>
    /// Writes PBF files.
    /// </summary>
    internal class PBFWriter
    {
        /// <summary>
        /// Holds the stream.
        /// </summary>
        private Stream _stream;
        
        /// <summary>
        /// Creates a new PBF write.
        /// </summary>
        /// <param name="stream"></param>
        public PBFWriter(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Closes this writer.
        /// </summary>
        public void Dispose()
        {
            _stream.Dispose();
        }

        /// <summary>
        /// Writes one PBF primitive block.
        /// </summary>
        /// <param name="block">The block to write.</param>
        public void ReadAll(PrimitiveBlock block)
        {
            // check parameters.
            if (block == null)
            {
                throw new ArgumentNullException("block");
            }

            // TODO: all the important stuff!
        }
    }
}
