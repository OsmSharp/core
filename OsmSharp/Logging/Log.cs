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

using System.Collections.Generic;

namespace OsmSharp.Logging
{
    /// <summary>
    /// Logging class.
    /// </summary>
    public static class Log
    {
        /// <summary>
        /// Holds the tracesource.
        /// </summary>
        private static TraceSource _source = new TraceSource("OsmSharp", SourceLevels.All);

        /// <summary>
        /// Holds the logging enabled flag.
        /// </summary>
        private static bool _loggingEnabled = false;

        /// <summary>
        /// Holds the sources to ignore.
        /// </summary>
        private static HashSet<string> _ignore = new HashSet<string>();

        /// <summary>
        /// Disables all logging.
        /// </summary>
        public static void Disable()
        {
            _loggingEnabled = false;
        }

        /// <summary>
        /// Enables all logging.
        /// </summary>
        public static void Enable()
        {
            _loggingEnabled = true;
        }

        /// <summary>
        /// Clears the ignore list.
        /// </summary>
        public static void ClearIgnore()
        {
            _ignore.Clear();
        }

        /// <summary>
        /// Adds the given name to the ignore list.
        /// </summary>
        /// <param name="name"></param>
        public static void Ignore(string name)
        {
            _ignore.Add(name);
        }

        /// <summary>
        /// Removes the given name from the ignore list.
        /// </summary>
        /// <param name="name"></param>
        public static void DontIgnore(string name)
        {
            _ignore.Remove(name);
		}

		/// <summary>
		/// Holds the sources to whitelist.
		/// </summary>
		private static HashSet<string> _whitelist = new HashSet<string>();

		/// <summary>
		/// Clears the whitelist.
		/// </summary>
		public static void ClearWhitelist()
		{
			_whitelist.Clear();
		}

		/// <summary>
		/// Adds the given name to the logging whitelist.
		/// </summary>
		/// <param name="name">Name.</param>
		public static void AddToWhiteList(string name)
		{
			_whitelist.Add(name);
		}

		/// <summary>
		/// Removes the given name from the loggin whitelist.
		/// </summary>
		/// <param name="name">Name.</param>
		public static void RemoveFromWhiteList(string name)
		{
			_whitelist.Remove(name);
		}

		/// <summary>
		/// Returns true if the events with the given name need to be reported.
		/// </summary>
		/// <param name="name">Name.</param>
		private static bool ReportAbout(string name)
		{
			if (_whitelist.Count > 0)
			{
				return _whitelist.Contains(name);
			}
			return !_ignore.Contains(name);
		}

        /// <summary>
        /// Writes a trace event message.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        public static void TraceEvent(string name, TraceEventType type, string message)
        {
			if (_loggingEnabled && Log.ReportAbout(name))
            {
                _source.TraceEvent(type, 0, "[" + name + "]: " + message);
            }
		}

        /// <summary>
        /// Writes a trace event message.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void TraceEvent(string name, TraceEventType type, string message, params object[] args)
        {
			if (_loggingEnabled && Log.ReportAbout(name))
            {
                _source.TraceEvent(type, 0, "[" + name + "]: " + message, args);
            }
        }

        /// <summary>
        /// Registers a trace listener.
        /// </summary>
        /// <param name="listener"></param>
        public static void RegisterListener(TraceListener listener)
        {
            _source.Listeners.Add(listener);
        }
    }
}