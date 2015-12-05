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

using OsmSharp.IO;
using OsmSharp.Math.Random;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp
{
    /// <summary>
    /// Class containing some utilities and extension methods.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Returns the largest power of 10 that is smaller than value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Power10Floor(this double value)
        {
            return (int)(System.Math.Pow(10.0, (System.Math.Floor(System.Math.Log10(value)))));
        }
        /// <summary>
        /// Returns the largest power of 10 that is smaller than value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int Power10Floor(this float value)
        {
            return Utilities.Power10Floor((double)value);
        }

        /// <summary>
        /// Copies all elements from the list into the given array starting at the given index but in reverse order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list to copy from.</param>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index to start copying to in the array.</param>
        public static void CopyToReverse<T>(this List<T> list, T[] array, int arrayIndex)
        {
            list.CopyToReverse(0, array, arrayIndex, list.Count);
        }

        /// <summary>
        /// Copies elements from the list and the range into the given array starting at the given index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list to copy from.</param>
        /// <param name="index">The start of the elements </param>
        /// <param name="count"></param>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index to start copying to in the array.</param>
        public static void CopyToReverse<T>(this List<T> list, int index, T[] array, int arrayIndex, int count)
        {
            for (int idx = index + count - 1; idx >= index; idx--)
            {
                array[arrayIndex] = list[idx];
                arrayIndex++;
            }
        }

        /// <summary>
        /// Copies elements from the list and the range into the given array starting at the given index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The array to copy from.</param>
        /// <param name="index">The start of the elements </param>
        /// <param name="count"></param>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index to start copying to in the array.</param>
        public static void CopyTo<T>(this T[] source, int index, T[] array, int arrayIndex, int count)
        {
            for (int idx = index; idx < index + count; idx++)
            {
                array[arrayIndex] = source[idx];
                arrayIndex++;
            }
        }

        /// <summary>
        /// Copies all elements from the list into the given array starting at the given index but in reverse order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The array to copy from.</param>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index to start copying to in the array.</param>
        public static void CopyToReverse<T>(this T[] source, T[] array, int arrayIndex)
        {
            source.CopyToReverse(0, array, arrayIndex, source.Length);
        }

        /// <summary>
        /// Copies elements from the list and the range into the given array starting at the given index.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The array to copy from.</param>
        /// <param name="index">The start of the elements </param>
        /// <param name="count"></param>
        /// <param name="array">The array to copy to.</param>
        /// <param name="arrayIndex">The index to start copying to in the array.</param>
        public static void CopyToReverse<T>(this T[] source, int index, T[] array, int arrayIndex, int count)
        {
            for (int idx = index + count - 1; idx >= index; idx--)
            {
                array[arrayIndex] = source[idx];
                arrayIndex++;
            }
        }

        /// <summary>
        /// Inserts the given elements at given positions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <param name="count"></param>
        public static void InsertTo<T>(this T[] source, int index, T[] array, int arrayIndex, int count)
        {
            // move elements after index to make room.
            for (int idx = array.Length - 1; idx >= arrayIndex + count; idx--)
            {
                array[idx] = array[idx - count];
            }

            // copy elements from source.
            source.CopyTo(index, array, arrayIndex, count);
        }

        /// <summary>
        /// Inserts the given elements at given positions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="index"></param>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        /// <param name="count"></param>
        public static void InsertToReverse<T>(this T[] source, int index, T[] array, int arrayIndex, int count)
        {
            // move elements after index to make room.
            for (int idx = array.Length - 1; idx >= arrayIndex + count; idx--)
            {
                array[idx] = array[idx - count];
            }

            // copy elements from source.
            source.CopyToReverse(index, array, arrayIndex, count);
        }

        /// <summary>
        /// Shuffles the list using Fisher-Yates shuffle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this IList<T> list)
        {
            list.Shuffle(OsmSharp.Math.Random.StaticRandomGenerator.Get());
        }

        /// <summary>
        /// Shuffles the list using Fisher-Yates shuffle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Shuffle<T>(this IList<T> list, IRandomGenerator random)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = random.Generate(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Generates a random string.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            byte[] randBuffer = new byte[length];
            OsmSharp.Math.Random.StaticRandomGenerator.Get().Generate(randBuffer);
            return System.Convert.ToBase64String(randBuffer).Remove(length);
        }

        /// <summary>
        /// Ticks since 1/1/1970
        /// </summary>
        public static long EpochTicks = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

        /// <summary>
        /// Converts a number of milliseconds from 1/1/1970 into a standard DateTime.
        /// </summary>
        /// <returns></returns>
        public static DateTime FromUnixTime(this long milliseconds)
        {
            return new DateTime(EpochTicks + milliseconds * 10000); // to a multiple of 100 nanosec or ticks.
        }

        /// <summary>
        /// Converts a standard DateTime into the number of milliseconds since 1/1/1970.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long ToUnixTime(this DateTime date)
        {
            return (date.Ticks - EpochTicks) / 10000; // from a multiple of 100 nanosec or ticks to milliseconds.
        }

        /// <summary>
        /// Returns a trucated string if the string is larger than the given max length.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string Truncate(this string value, int maxLength)
        {
            if (value != null && value.Length > maxLength)
            {
                return value.Substring(0, maxLength);
            }
            return value;
        }

        /// <summary>
        /// Retuns a string of a fixed length.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string PadRightAndCut(this string s, int length)
        {
            return s.ToStringEmptyWhenNull().PadRight(length).Substring(0, length);
        }

        /// <summary>
        /// Matches two string that contain a given percentage of the same characters.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static bool LevenshteinMatch(this string s, string t, float percentage)
        {
            if (s == null || t == null)
            {
                return false;
            }
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];
            int match = -1;
            int size = System.Math.Max(n, m);

            if (size == 0)
            { // empty strings cannot be matched.
                return false;
            }

            // Step 1
            if (n == 0)
            {
                match = m;
            }

            if (m == 0)
            {
                match = n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = System.Math.Min(
                        System.Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            match = d[n, m];

            // calculate the percentage.
            return ((float)(size - match) / (float)size) > (percentage / 100.0);
        }

        /// <summary>
        /// Returns a string with init caps.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string InitCap(this string value)
        {
            // use other code, ToTileCase is not supported in windows phone.
            if (value == null)
                return null;
            if (value.Length == 0)
                return value;

            StringBuilder result = new StringBuilder(value);
            result[0] = char.ToUpper(result[0]);
            for (int i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                    result[i] = char.ToUpper(result[i]);
                else
                    result[i] = char.ToLower(result[i]);
            }
            return result.ToString();
        }

        /// <summary>
        /// Removes all objects that match the given predicate from the given list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static int RemoveAll<T>(this List<T> list, Predicate<T> match)
        {
            int removed = 0;
            for (int idx = list.Count - 1; idx >= 0; idx--)
            {
                if (match.Invoke(list[idx]))
                {
                    removed++;
                    list.RemoveAt(idx);
                }
            }
            return removed;
        }

        /// <summary>
        /// Returns the numeric part of the string for the beginning part of the string only.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NumericPartFloat(this string value)
        {
            string ret_string = string.Empty;
            if (value != null && value.Length > 0)
            {
                //StringBuilder numbers = new StringBuilder();
                for (int c = 1;c <= value.Length;c++)
                {
                    float result_never_used;
                    string value_tested = value.Substring(0, c);
                    if (float.TryParse(value_tested, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result_never_used))
                    {
                        if (value[c - 1] != '.')
                        {
                            ret_string = value_tested;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return ret_string;
        }

        /// <summary>
        /// Returns the numeric part of the string for the beginning part of the string only.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NumericPartInt(this string value)
        {
            string ret_string = string.Empty;
            if (value != null && value.Length > 0)
            {
                //StringBuilder numbers = new StringBuilder();
                for (int c = 1; c <= value.Length; c++)
                {
                    int result_never_used;
                    string value_tested = value.Substring(0, c);
                    if (int.TryParse(value_tested, out result_never_used))
                    {
                        ret_string = value_tested;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            return ret_string;
        }

        /// <summary>
        /// Splists this string into parts with sizes given in the array.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sizes"></param>
        /// <returns></returns>
        public static string[] SplitMultiple(this string value,int[] sizes)
        {
            string[] result = new string[sizes.Length];

            int position = 0;
            for (int i = 0; i < sizes.Length; i++)
            {
                result[i] = value.Substring(position, sizes[i]);

                position = position + sizes[i];
            }

            return result;
        }

        /// <summary>
        /// Returns the result of the ToString() method or an empty string
        /// when the given object is null.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToStringEmptyWhenNull(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            return obj.ToString();
        }

        /// <summary>
        /// Remove whitespaces.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        /// <summary>
        /// Converts an array of double to long using a factor value.
        /// </summary>
        /// <param name="doubleArray">The double array.</param>
        /// <param name="factor">A factor to use to convert to doubles.</param>
        /// <returns></returns>
        public static long[] ConvertToLongArray(this double[] doubleArray, int factor)
        {
            long[] longArray = new long[doubleArray.Length];
            for (int idx = 0; idx < doubleArray.Length; idx++)
            {
                longArray[idx] = (long)(doubleArray[idx] * factor);
            }
            return longArray;
        }

        /// <summary>
        /// Converts an array of longs back to doubles using a factor value.
        /// </summary>
        /// <param name="longArray">The long array.</param>
        /// <param name="factor">A factor to use to convert the long values.</param>
        /// <returns></returns>
        public static double[] ConvertFromLongArray(this long[] longArray, int factor)
        {
            double[] doubleArray = new double[longArray.Length];
            for (int idx = 0; idx < doubleArray.Length; idx++)
            {
                doubleArray[idx] = (longArray[idx] / (double)factor);
            }
            return doubleArray;
        }

        /// <summary>
        /// Converts an array of longs back to doubles using a factor value.
        /// </summary>
        /// <param name="longArray">The long array.</param>
        /// <param name="factor">A factor to use to convert the long values.</param>
        /// <returns></returns>
        public static double[] ConvertFromLongArray(this List<long> longArray, int factor)
        {
            double[] doubleArray = new double[longArray.Count];
            for (int idx = 0; idx < doubleArray.Length; idx++)
            {
                doubleArray[idx] = (longArray[idx] / (double)factor);
            }
            return doubleArray;
        }

        /// <summary>
        /// Delta encodes an array.
        /// </summary>
        /// <param name="longArray"></param>
        public static long[] EncodeDelta(this long[] longArray)
        {
            if (longArray.Length > 0)
            { // there is data!
                long previous = longArray[0];
                for (int idx = 1; idx < longArray.Length; idx++)
                {
                    long delta = longArray[idx] - previous;
                    previous = longArray[idx];
                    longArray[idx] = delta;
                }
            }
            return longArray;
        }

        /// <summary>
        /// Delta encodes an array.
        /// </summary>
        /// <param name="longArray"></param>
        public static List<long> EncodeDelta(this List<long> longArray)
        {
            if (longArray.Count > 0)
            { // there is data!
                long previous = longArray[0];
                for (int idx = 1; idx < longArray.Count; idx++)
                {
                    long delta = longArray[idx] - previous;
                    previous = longArray[idx];
                    longArray[idx] = delta;
                }
            }
            return longArray;
        }

        /// <summary>
        /// Delta decodes an array.
        /// </summary>
        /// <param name="longArray"></param>
        public static long[] DecodeDelta(this long[] longArray)
        {
            if (longArray.Length > 0)
            { // there is data!
                for (int idx = 1; idx < longArray.Length; idx++)
                {
                    longArray[idx] = longArray[idx - 1] + longArray[idx];
                }
            }
            return longArray;
        }

        /// <summary>
        /// Delta decodes an array.
        /// </summary>
        /// <param name="longArray"></param>
        public static List<long> DecodeDelta(this List<long> longArray)
        {
            if (longArray.Count > 0)
            { // there is data!
                for (int idx = 1; idx < longArray.Count; idx++)
                {
                    longArray[idx] = longArray[idx - 1] + longArray[idx];
                }
            }
            return longArray;
        }

        /// <summary>
        /// Serializes to the given stream but writes the size of the serialized data in the first 4 bytes.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dest"></param>
        /// <param name="value"></param>
        public static void SerializeWithSize(this RuntimeTypeModel model, Stream dest, object value)
        {
            // save position.
            long position = dest.Position;

            // seek until after 8 bytes to store size.
            dest.Seek(8, System.IO.SeekOrigin.Current);

            // serialize.
            model.Serialize(dest, value);

            // calculate size.
            long size = dest.Position - position - 8L;
            dest.Seek(position, System.IO.SeekOrigin.Begin);
            byte[] sizeBytes = BitConverter.GetBytes(size);
            dest.Write(sizeBytes, 0, 8);
            dest.Seek(size, System.IO.SeekOrigin.Current);
        }

        /// <summary>
        /// Deserializes an object from the given stream but uses the first 4 bytes as an indication of size and limits the data accordingly.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DeserializeWithSize(this RuntimeTypeModel model, Stream source, object value, Type type)
        {
            // create a cappedstream.
            var sizeBytes = new byte[8];
            source.Read(sizeBytes, 0, 8);
            long size = BitConverter.ToInt64(sizeBytes, 0);
            var cappedStream = new CappedStream(source, source.Position, size);

            // deserialize.
            return model.Deserialize(cappedStream, value, type);

            //// read only the relevant data from the stream.
            //var sizeBytes = new byte[8];
            //source.Read(sizeBytes, 0, 8);
            //long size = BitConverter.ToInt32(sizeBytes, 0);
            //var data = new byte[size];
            //source.Read(data, 0, (int)size);

            //// deserialize.
            //var dataStream = new MemoryStream(data);
            //var deserializedValue = model.Deserialize(dataStream, value, type);
            //dataStream.Dispose();
            //return deserializedValue;
        }

        /// <summary>
        /// Typed try get value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryGetValue<T>(this Dictionary<string, object> dictionary, string key, out T value)
        {
            object valueObject;
            if(dictionary.TryGetValue(key, out valueObject))
            {
                value = (T)valueObject;
                return true;
            }
            value = default(T);
            return false;
        }
    }

    /// <summary>
    /// Contains enumeration parsing methods for compatibility purposes between difference .NET/Mono/WindowsPhone flavours.
    /// </summary>
    public static class EnumHelper
    {
        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object. A parameter
        /// specifies whether the operation is case-sensitive. The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type to which to convert value.</typeparam>
        /// <param name="value">The string representation of the enumeration name or underlying value to convert.</param>
        /// <param name="result">When this method returns, contains an object of type TEnum whose value is represented by value. This parameter is passed uninitialized.</param>
        /// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct
        {
            return Enum.TryParse<TEnum>(value, out result);
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object. A parameter
        /// specifies whether the operation is case-sensitive. The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <typeparam name="TEnum">The enumeration type to which to convert value.</typeparam>
        /// <param name="value">The string representation of the enumeration name or underlying value to convert.</param>
        /// <param name="ignoreCase">true to ignore case; false to consider case.</param>
        /// <param name="result">When this method returns, contains an object of type TEnum whose value is represented by value. This parameter is passed uninitialized.</param>
        /// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
        public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct
        {
            return Enum.TryParse<TEnum>(value, ignoreCase, out result);
        }

        /// <summary>
        /// Returns a string representing the object in a culture invariant way.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToInvariantString(this object obj)
        {
            return obj is IConvertible ? ((IConvertible)obj).ToString(CultureInfo.InvariantCulture)
                : obj is IFormattable ? ((IFormattable)obj).ToString(null, CultureInfo.InvariantCulture)
                : obj.ToString();
        }

        /// <summary>
        /// Shrinks and copies the given list, removes elements with indices in the toRemove set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static List<T> ShrinkAndCopyList<T>(this List<T> list, HashSet<int> toRemove)
        {
            var shrinked = new List<T>(System.Math.Max(list.Count - toRemove.Count, 0));
            for (var i = 0; i < list.Count; i++)
            {
                if (!toRemove.Contains(i))
                {
                    shrinked.Add(list[i]);
                }
            }
            return shrinked;
        }

        /// <summary>
        /// Shrinks and copies the matrix and removes rows/columns with indices in the toRemove set.
        /// </summary>
        /// <returns></returns>
        public static T[][] SchrinkAndCopyMatrix<T>(this T[][] matrix, HashSet<int> toRemove)
        {
            var schrunk = new T[matrix.Length - toRemove.Count][];
            var schrunkX = 0;
            for (var x = 0; x < matrix.Length; x++)
            {
                if (!toRemove.Contains(x))
                { // keep this element.
                    var schrunkY = 0;
                    schrunk[schrunkX] = new T[matrix.Length - toRemove.Count];
                    for (var y = 0; y < matrix[x].Length; y++)
                    {
                        if (!toRemove.Contains(y))
                        { // keep this element.
                            schrunk[schrunkX][schrunkY] = matrix[x][y];
                            schrunkY++;
                        }
                    }
                    schrunkX++;
                }
            }
            return schrunk;
        }
    }

    /// <summary>
    /// An implementation of the EqualityComparer that allows the use of delegates.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelegateEqualityComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// A delegate to calculate the hashcode.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public delegate int GetHashCodeDelegate(T obj);

        /// <summary>
        /// A delegate to compare two objects.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public delegate bool EqualsDelegate(T x, T y);

        /// <summary>
        /// Creates a new equality comparer.
        /// </summary>
        /// <param name="hashCodeDelegate"></param>
        /// <param name="equalsDelegate"></param>
        public DelegateEqualityComparer(GetHashCodeDelegate hashCodeDelegate, EqualsDelegate equalsDelegate)
        {
            if (hashCodeDelegate == null) { throw new ArgumentNullException("hashCodeDelegate"); }
            if (equalsDelegate == null) { throw new ArgumentNullException("equalsDelegate"); }

            _equalsDelegate = equalsDelegate;
            _hashCodeDelegate = hashCodeDelegate;
        }

        /// <summary>
        /// Holds the equals delegate.
        /// </summary>
        private EqualsDelegate _equalsDelegate;

        /// <summary>
        /// Returns true if the two given objects are considered equal.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(T x, T y)
        {
            return _equalsDelegate.Invoke(x, y);
        }

        /// <summary>
        /// Holds the hashcode delegate.
        /// </summary>
        private GetHashCodeDelegate _hashCodeDelegate;

        /// <summary>
        /// Calculates the hashcode for the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(T obj)
        {
            return _hashCodeDelegate.Invoke(obj);
        }
    }
}