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
using System.Net;
using System.Collections.Generic;

namespace OsmSharp.Logging
{
	/// <summary>
    /// Compatibility class with .NET to use the tracing facilities. 
    /// </summary>
    public class TraceSource
    {
		/// <summary>
		/// Initializes a new instance of the TraceSource class.
		/// </summary>
		/// <param name="name">Name.</param>
        public TraceSource(string name)
		{
			//_tag = name;
            this.Listeners = new List<TraceListener>();
        }

		/// <summary>
		/// Initializes a new instance of the TraceSource class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="level">Level.</param>
        public TraceSource(string name, SourceLevels level)
		{
            //_tag = name;
            this.Listeners = new List<TraceListener>();
        }

        /// <summary>
        /// Traces an event.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="message"></param>
        internal void TraceEvent(TraceEventType type, int id, string message)
        {
			switch (type) {
			case TraceEventType.Critical:
			case TraceEventType.Error:
                this.WriteLine(message);
				break;
			case TraceEventType.Warning:
                this.WriteLine(message);
				break;
			default:
                this.WriteLine(message);
				break;
			}
        }

        /// <summary>
        /// Writes the message to all listeners.
        /// </summary>
        /// <param name="message"></param>
        private void WriteLine(string message)
        {
            foreach (TraceListener listener in this.Listeners)
            {
                listener.WriteLine(message);
            }
        }

        /// <summary>
        /// Traces an event.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
		/// <param name="messageWithParams"></param>
        /// <param name="args"></param>
        internal void TraceEvent(TraceEventType type, int id, string messageWithParams, object[] args)
        {
			string message = string.Format (messageWithParams, args);
			this.TraceEvent (type, id, message);
        }

        /// <summary>
        /// The list of listeners.
        /// </summary>
        public List<TraceListener> Listeners { get; private set; }
    }
}