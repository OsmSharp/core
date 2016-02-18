// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;

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
        public Logger(string name)
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
            if (Logger.LogAction != null)
            {
                Logger.LogAction(_name, type.ToInvariantString().ToLower(), string.Format(message, args), null);
            }
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        public static void Log(string name, TraceEventType type, string message, params object[] args)
        {
            if (Logger.LogAction != null)
            {
                Logger.LogAction(name, type.ToInvariantString().ToLower(), string.Format(message, args), null);
            }
        }

        /// <summary>
        /// Defines the log action function.
        /// </summary>
        /// <param name="origin">The origin of the message, a class or module name.</param>
        /// <param name="level">The level of the message, 'critical', 'error', 'warning', 'verbose' or 'information'.</param>
        /// <param name="message">The message content.</param>
        /// <param name="parameters">Any parameters that may be useful.</param>
        public delegate void LogActionFunction(string origin, string level, string message,
            IDictionary<string, object> parameters);

        /// <summary>
        /// Gets or sets the action to actually log a message.
        /// </summary>
        public static LogActionFunction LogAction
        {
            get;
            set;
        }
    }
}