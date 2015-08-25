// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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
using System.Collections.Generic;

namespace OsmSharp.Data.SQLite
{
    /// <summary>
    /// A wrapper-class for all platform-specific SQLite connections.
    /// </summary>
    public abstract class SQLiteConnectionBase : IDisposable
    {
        /// <summary>
        /// Creates an SQLLite command.
        /// </summary>
        public abstract List<T> Query<T>(string query, params object[] args) where T : new();

        /// <summary>
        /// Disposes of all resources associated with this connection.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Closes this SQLite command.
        /// </summary>
        public abstract void Close();
    }
}