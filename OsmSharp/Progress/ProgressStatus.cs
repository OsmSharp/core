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

namespace OsmSharp.Progress
{
    /// <summary>
    /// Status object reporting progress information.
    /// </summary>
    public class ProgressStatus
    {
        private int _totalItemsNumber;

        /// <summary>
        /// Returns total number.
        /// </summary>
        public int TotalNumber
        {
            get { return _totalItemsNumber; }
            set { _totalItemsNumber = value; }
        }
        
        /// <summary>
        /// Returns the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Progress status enum.
        /// </summary>
        public enum ProgressStatusEnum
        {
            /// <summary>
            /// Failed.
            /// </summary>
            Failed,
            /// <summary>
            /// Querying.
            /// </summary>
            Querying,
            /// <summary>
            /// Succeeded.
            /// </summary>
            Succeeded,
            /// <summary>
            /// Down.
            /// </summary>
            Down,
            /// <summary>
            /// Busy.
            /// </summary>
            Busy
        }

        private ProgressStatusEnum _status;
        /// <summary>
        /// Status.
        /// </summary>
        public ProgressStatusEnum Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private int _currentNumber;
        /// <summary>
        /// Current number.
        /// </summary>
        public int CurrentNumber
        {
            get { return _currentNumber; }
            set { _currentNumber = value; }
        }


        private object _ent;
        /// <summary>
        /// Current entity.
        /// </summary>
        public object CurrentEntity
        {
            get { return _ent; }
            set { _ent = value; }
        }

        private DateTime _timeRemaining;
        /// <summary>
        /// Time remaining.
        /// </summary>
        public DateTime TimeRemaining
        {
            get { return _timeRemaining; }
            set { _timeRemaining = value; }
        }

        private DateTime _timePassed;
        /// <summary>
        /// Time passed.
        /// </summary>
        public DateTime TimePassed
        {
            get { return _timePassed; }
            set { _timePassed = value; }
        }

        /// <summary>
        /// Time passed string.
        /// </summary>
        public string TimePassedString
        {
            get
            {
                string str = "";
                if (this.TimePassed.Day - 1 > 0)
                {
                    str = str + (this.TimePassed.Day - 1).ToString() + " days ";
                }
                if (this.TimePassed.TimeOfDay.Hours > 0)
                {
                    str = str + this.TimePassed.TimeOfDay.Hours + " hours ";
                }
                return str + this.TimePassed.TimeOfDay.Minutes + "min " + this.TimePassed.TimeOfDay.Seconds + "s";
            }
        }

        /// <summary>
        /// Time remaining string.
        /// </summary>
        public string TimeRemainingString
        {
            get
            {
                string str = "";
                if (this.TimeRemaining.Day - 1 > 0)
                {
                    str = str + (this.TimeRemaining.Day - 1).ToString() + " days ";
                }
                if (this.TimeRemaining.TimeOfDay.Hours > 0)
                {
                    str = str + this.TimeRemaining.TimeOfDay.Hours + " hours ";
                }
                return str + this.TimeRemaining.TimeOfDay.Minutes + "min " + this.TimeRemaining.TimeOfDay.Seconds + "s";
            }
        }

        /// <summary>
        /// Progress percentage.
        /// </summary>
        public double ProgressPercentage
        {
            get
            {
                if (this.TotalNumber > 0)
                {
                    double percentage =
                        (((double)this.CurrentNumber) / ((double)this.TotalNumber)) * 100.0;
                    if (percentage > 100)
                    {
                        percentage = 100;
                    }
                    else if (percentage < 0)
                    {
                        percentage = 0;
                    }
                    return percentage;
                }
                return 0;
            }
        }
    } 
}
