// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2016 Abelshausen Ben
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

namespace OsmSharp.IO.PBF
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
