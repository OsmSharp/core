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

namespace OsmSharp.Osm.Data
{
    /// <summary>
    /// An expanded diff result result.
    /// </summary>
    public class DiffResultResult
    {
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
        /// Unknown error.
        /// </summary>
        UknownError
    };
}