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

namespace OsmSharp.Logging
{
    /// <summary>
    /// A logger.
    /// </summary>
    public class Logger
    {
        private readonly string _name;

        /// <summary>
        /// Creates a new logger.
        /// </summary>
        internal Logger(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Creates a new logger.
        /// </summary>
        public static Logger Create(string name)
        {
            return new Logger(name);
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        public void Log(TraceEventType type, string message, params object[] args)
        {
            if(Logger.LogAction != null)
            {
                Logger.LogAction(_name, type, string.Format(message, args));
            }
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        public static void Log(string name, TraceEventType type, string message, params object[] args)
        {
            if (Logger.LogAction != null)
            {
                Logger.LogAction(name, type, string.Format(message, args));
            }
        }

        /// <summary>
        /// Gets or sets the action to actually log a message.
        /// </summary>
        public static Action<string, TraceEventType, string> LogAction
        {
            get;
            set;
        }
    }
}
