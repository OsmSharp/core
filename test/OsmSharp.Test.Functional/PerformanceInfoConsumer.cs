// The MIT License (MIT)

// Copyright (c) 2017 Ben Abelshausen

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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OsmSharp.Test.Functional
{
    /// <summary>
    /// A class that consumes performance information.
    /// </summary>
    public class PerformanceInfoConsumer
    {
        private readonly string _name; // Holds the name of this consumer.
        private System.Threading.Timer _memoryUsageTimer; // Holds the memory usage timer.
        private List<double> _memoryUsageLog = new List<double>(); // Holds the memory usage log.
        private long _memoryUsageLoggingDuration = 0; // Holds the time spent on logging memory usage.

        /// <summary>
        /// Creates a new performance info consumer.
        /// </summary>
        public PerformanceInfoConsumer(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Creates a new performance info consumer.
        /// </summary>
        public PerformanceInfoConsumer(string name, int memUseLoggingInterval)
        {
            _name = name;
            _memoryUsageTimer = new System.Threading.Timer(LogMemoryUsage, null, memUseLoggingInterval, memUseLoggingInterval);
        }

        /// <summary>
        /// Called when it's time to log memory usage.
        /// </summary>
        private void LogMemoryUsage(object state)
        {
            var ticksBefore = DateTime.Now.Ticks;
            lock (_memoryUsageLog)
            {
                GC.Collect();
                var p = Process.GetCurrentProcess();
                _memoryUsageLog.Add(System.Math.Round((p.PrivateMemorySize64 - _memory.Value) / 1024.0 / 1024.0, 4));

                _memoryUsageLoggingDuration = _memoryUsageLoggingDuration + (DateTime.Now.Ticks - ticksBefore);
            }
        }

        /// <summary>
        /// Creates a new performance consumer.
        /// </summary>
        /// <param name="key"></param>
        public static PerformanceInfoConsumer Create(string key)
        {
            return new PerformanceInfoConsumer(key);
        }

        /// <summary>
        /// Holds the ticks when started.
        /// </summary>
        private long? _ticks;

        /// <summary>
        /// Holds the amount of memory before start.
        /// </summary>
        private long? _memory;

        /// <summary>
        /// Reports the start of the process/time period to measure.
        /// </summary>
        public void Start()
        {
            GC.Collect();

            Process p = Process.GetCurrentProcess();
            _memory = p.PrivateMemorySize64;
            _ticks = DateTime.Now.Ticks;
            OsmSharp.Logging.Logger.Log(_name, OsmSharp.Logging.TraceEventType.Information, "Started!");
        }

        /// <summary>
        /// Reports a message in the middle of progress.
        /// </summary>
        public void Report(string message)
        {
            OsmSharp.Logging.Logger.Log(_name, OsmSharp.Logging.TraceEventType.Information, message);
        }

        /// <summary>
        /// Reports a message in the middle of progress.
        /// </summary>
        public void Report(string message, params object[] args)
        {
            OsmSharp.Logging.Logger.Log(_name, OsmSharp.Logging.TraceEventType.Information, message, args);
        }

        private int previousPercentage = 0;

        /// <summary>
        /// Reports a message about progress.
        /// </summary>
        public void Report(string message, long i, long max)
        {
            var currentPercentage = (int)System.Math.Round((i / (double)max) * 10, 0);
            if (previousPercentage != currentPercentage)
            {
                OsmSharp.Logging.Logger.Log(_name, OsmSharp.Logging.TraceEventType.Information, message, currentPercentage * 10);
                previousPercentage = currentPercentage;
            }
        }

        /// <summary>
        /// Reports the end of the process/time period to measure.
        /// </summary>
        public void Stop()
        {
            if (_memoryUsageTimer != null)
            { // only dispose and stop when there IS a timer.
                _memoryUsageTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                _memoryUsageTimer.Dispose();
            }
            if (_ticks.HasValue)
            {
                lock (_memoryUsageLog)
                {
                    var seconds = new TimeSpan(DateTime.Now.Ticks - _ticks.Value - _memoryUsageLoggingDuration).TotalMilliseconds / 1000.0;

                    GC.Collect();
                    var p = Process.GetCurrentProcess();
                    var memoryDiff = System.Math.Round((p.PrivateMemorySize64 - _memory.Value) / 1024.0 / 1024.0, 4);

                    if (_memoryUsageLog.Count > 0)
                    { // there was memory usage logging.
                        double max = _memoryUsageLog.Max();
                        OsmSharp.Logging.Logger.Log(_name, OsmSharp.Logging.TraceEventType.Information, "Ended at at {0}, spent {1}s and {2}MB of memory diff with {3}MB max used.",
                                new DateTime(_ticks.Value).ToShortTimeString(),
                                seconds, memoryDiff, max);
                    }
                    else
                    { // no memory usage logged.
                        OsmSharp.Logging.Logger.Log(_name, OsmSharp.Logging.TraceEventType.Information, "Ended at at {0}, spent {1}s and {2}MB of memory diff.",
                                new DateTime(_ticks.Value).ToShortTimeString(),
                                seconds, memoryDiff);
                    }
                }
            }
        }
    }
}