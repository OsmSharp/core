// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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
using System.Linq;

namespace OsmSharp
{
    /// <summary>
    /// Range.
    /// </summary>
    public static class Range
    {
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<sbyte> SByte(sbyte from, sbyte to, int step)
        {
            return Range.Int32(from, to, step).Select(i => (sbyte)i);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<byte> Byte(byte from, byte to, int step)
        {
            return Range.Int32(from, to, step).Select(i => (byte)i);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<char> Char(char from, char to, int step)
        {
            return Range.Int32(from, to, step).Select(i => (char)i);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<short> Int16(short from, short to, int step)
        {
            return Range.Int32(from, to, step).Select(i => (short)i);
        }

        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<ushort> UInt16(ushort from, ushort to, int step)        
        {
            return Range.Int32(from, to, step).Select(i => (ushort)i);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<int> Int32(int from, int to, int step)
        {
            if (step <= 0) step = (step == 0) ? 1 : -step;
            if (from <= to)
            {
                for (int i = from; i <= to; i += step) yield return i;
            }
            else
            {
                for (int i = from; i >= to; i -= step) yield return i;
            }
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<uint> UInt32(uint from, uint to, uint step)
        {
            if (step == 0U) step = 1U; 
            if (from <= to)
            {
                for (uint ui = from; ui <= to; ui += step) yield return ui;
            }
            else
            {
                for (uint ui = from; ui >= to; ui -= step) yield return ui;
            }
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<long> Int64(long from, long to, long step)
        {
            if (step <= 0L) step = (step == 0L) ? 1L : -step;
 
            if (from <= to)
            {
                for (long l = from; l <= to; l += step) yield return l;
            }
            else
            {
                for (long l = from; l >= to; l -= step) yield return l;
            }
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> UInt64(ulong from, ulong to, ulong step)
        {
            if (step == 0UL) step = 1UL;
 
            if (from <= to)
            {
                for (ulong ul = from; ul <= to; ul += step) yield return ul;
            }
            else
            {
                for (ulong ul = from; ul >= to; ul -= step) yield return ul;
            }
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<float> Single(float from, float to, float step)
        {
            if (step <= 0.0f) step = (step == 0.0f) ? 1.0f : -step;
 
            if (from <= to)
            {
                for (float f = from; f <= to; f += step) yield return f;
            }
            else
            {
                for (float f = from; f >= to; f -= step) yield return f;
            }
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<double> Double(double from, double to, double step)
        {
            if (step <= 0.0) step = (step == 0.0) ? 1.0 : -step;
 
            if (from <= to)
            {
                for (double d = from; d <= to; d += step) yield return d;
            }
            else
            {
                for (double d = from; d >= to; d -= step) yield return d;
            }
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<decimal> Decimal(decimal from, decimal to, decimal step)
        {
            if (step <= 0.0m) step = (step == 0.0m) ? 1.0m : -step;
 
            if (from <= to)
            {
                for (decimal m = from; m <= to; m += step) yield return m;
            }
            else
            {
                for (decimal m = from; m >= to; m -= step) yield return m;
            }
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> DateTime(DateTime from, DateTime to, double step)
        {
            if (step <= 0.0) step = (step == 0.0) ? 1.0 : -step;
 
            if (from <= to)
            {
                for (DateTime dt = from; dt <= to; dt = dt.AddDays(step)) yield return dt;
            }
            else
            {
                for (DateTime dt = from; dt >= to; dt = dt.AddDays(-step)) yield return dt;
            }
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<sbyte> SByte(sbyte from, sbyte to)
        {
            return Range.SByte(from, to, 1);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<byte> Byte(byte from, byte to)
        {
            return Range.Byte(from, to, 1);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<char> Char(char from, char to)
        {
            return Range.Char(from, to, 1);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<short> Int16(short from, short to)
        {
            return Range.Int16(from, to, 1);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<ushort> UInt16(ushort from, ushort to)
        {
            return Range.UInt16(from, to, 1);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<int> Int32(int from, int to)
        {
            return Range.Int32(from, to, 1);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<uint> UInt32(uint from, uint to)
        {
            return Range.UInt32(from, to, 1U);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<long> Int64(long from, long to)
        {
            return Range.Int64(from, to, 1L);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<ulong> UInt64(ulong from, ulong to)
        {
            return Range.UInt64(from, to, 1UL);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<float> Single(float from, float to)
        {
            return Range.Single(from, to, 1.0f);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<double> Double(double from, double to)
        {
            return Range.Double(from, to, 1.0);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<decimal> Decimal(decimal from, decimal to)
        {
            return Range.Decimal(from, to, 1.0m);
        }
 
        /// <summary>
        /// Range enumerator.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static IEnumerable<DateTime> DateTime(DateTime from, DateTime to)
        {
            return Range.DateTime(from, to, 1.0);
        }
    }
}