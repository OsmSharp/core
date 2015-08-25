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
using System.IO;
using System.Linq;
using System.Text;

namespace OsmSharp.Collections
{
    /// <summary>
    /// An object table containing and index of object to reduce memory usage by preventing duplicates.
    /// </summary>
    public class ObjectTable<Type>
    {
        /// <summary>
        /// The array containing all strings.
        /// </summary>
        private Type[] _objects;

        /// <summary>
        /// A dictionary containing the index of each string.
        /// </summary>
        private Dictionary<Type, uint> _reverseIndex;

        /// <summary>
        /// Holds the initial capacity and is also used as an allocation step.
        /// </summary>
        private int _initCapacity;

        /// <summary>
        /// Holds the next idx.
        /// </summary>
        private uint _nextIdx = 0;

        /// <summary>
        /// Holds the allow duplicates flag.
        /// </summary>
        private bool _allowDuplicates;

        /// <summary>
        /// Holds the default initial capactiy.
        /// </summary>
        public const int INITIAL_CAPACITY = 1000;

        /// <summary>
        /// Creates a new string table.
        /// </summary>
        /// <param name="reverseIndex">The reverse index is enabled if true.</param>
        public ObjectTable(bool reverseIndex)
            : this(reverseIndex, INITIAL_CAPACITY, false)
        {

        }

        /// <summary>
        /// Creates a new string table.
        /// </summary>
        /// <param name="initCapacity">The inital capacity.</param>
        /// <param name="allowDuplicates">Flag preventing this object table for checking for duplicates. Use this when sure almost all objects will be unique.</param>
        public ObjectTable(int initCapacity, bool allowDuplicates)
            : this(true, INITIAL_CAPACITY, allowDuplicates)
        {

        }

        /// <summary>
        /// Creates a new string table.
        /// </summary>
        /// <param name="reverseIndex">The reverse index is enable if true.</param>
        /// <param name="initCapacity">The inital capacity.</param>
        /// <param name="allowDuplicates">Flag preventing this object table for checking for duplicates. Use this when sure almost all objects will be unique.</param>
        public ObjectTable(bool reverseIndex, int initCapacity, bool allowDuplicates)
        {
            _objects = new Type[initCapacity];
            _initCapacity = initCapacity;

            if (reverseIndex && !allowDuplicates)
            {
                this.BuildReverseIndex();
            }
            _allowDuplicates = allowDuplicates;
        }

        /// <summary>
        /// Clears all data from this object table.
        /// </summary>
        public void Clear()
        {
            _objects = new Type[_initCapacity];
            _nextIdx = 0;
            if (_reverseIndex != null)
            {
                _reverseIndex.Clear();
            }
        }

        #region Reverse Index

        /// <summary>
        /// Builds the reverse index.
        /// </summary>
        public void BuildReverseIndex()
        {
            _reverseIndex = new Dictionary<Type, uint>();
            for(uint idx = 0; idx < _objects.Length; idx++)
            {
                Type value = _objects[idx];
                if (value != null)
                {
                    _reverseIndex[value] = idx;
                }
            }
        }

        /// <summary>
        /// Drops the reverse index.
        /// </summary>
        public void DropReverseIndex()
        {
            _reverseIndex = null;
        }

        #endregion

        #region Table

        /// <summary>
        /// Adds a new object without checking if it exists already.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public uint AddObject(Type value)
        {
            uint value_int = _nextIdx;

            if (_objects.Length <= _nextIdx)
            { // the string table is not big enough anymore.
                Array.Resize<Type>(ref _objects, _objects.Length + _initCapacity);
            }
            _objects[_nextIdx] = value;

            if (_reverseIndex != null)
            {
                _reverseIndex[value] = _nextIdx;
            }

            _nextIdx++;
            return value_int;
        }

        #endregion

        /// <summary>
        /// Returns the highest id in this object table.
        /// </summary>
        public uint Count
        {
            get
            {
                return _nextIdx;
            }
        }

        /// <summary>
        /// Returns an index for the given string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public uint Add(Type value)
        {
            uint valueInt;
            if(_allowDuplicates)
            { // just add the object, don't check anything.
                return this.AddObject(value);
            }
            if (_reverseIndex != null)
            { // add string based on the reverse index, is faster.
                if (!_reverseIndex.TryGetValue(value, out valueInt))
                { // string was not found.
                    valueInt = this.AddObject(value);
                }
            }
            else
            {
                int idx = Array.IndexOf<Type>(_objects, value); // this is O(n), a lot worse compared to the best-case O(1).
                if (idx < 0)
                { // string was not found.
                    valueInt = this.AddObject(value);
                }
                else
                { // string was found.
                    valueInt = (uint)idx;
                }
            }
            return valueInt;
        }

        /// <summary>
        /// Returns a string given it's encoded index.
        /// </summary>
        /// <param name="valueIdx"></param>
        /// <returns></returns>
        public Type Get(uint valueIdx)
        {
            return _objects[valueIdx];
        }

        /// <summary>
        /// Returns true if the object with the given id are in this collection.
        /// </summary>
        /// <param name="valueIdx"></param>
        /// <returns></returns>
        public bool Contains(uint valueIdx)
        {
            return _objects.Length > valueIdx;
        }

        /// <summary>
        /// Returns a copy of all data in this object table.
        /// </summary>
        /// <returns></returns>
        public Type[] ToArray()
        {
            Type[] copy = new Type[_nextIdx];
            for (int idx = 0; idx < _nextIdx; idx++)
            {
                copy[idx] = _objects[idx];
            }
            return copy;
        }

        /// <summary>
        /// Serializes/deserializes an object table.
        /// </summary>
        public abstract class ObjectTableSerializer
        {
            /// <summary>
            /// Serializes the given object table to the stream.
            /// </summary>
            /// <param name="stream"></param>
            /// <param name="objectTable"></param>
            public void Serialize(Stream stream, ObjectTable<Type> objectTable)
            {
                // [OBJECT_COUNT][OBJECT_DATA]
                lock (objectTable)
                {
                    // write object-count.
                    var countBytes = BitConverter.GetBytes(objectTable._nextIdx);
                    stream.Write(countBytes, 0, 4);

                    for (int idx = 0; idx < objectTable._nextIdx; idx++)
                    { // serialize objects one-by-one.
                        this.SerializeObject(stream, objectTable._objects[idx]);
                    }
                }
            }

            /// <summary>
            /// Serializes one object.
            /// </summary>
            /// <param name="stream"></param>
            /// <param name="value"></param>
            public abstract void SerializeObject(Stream stream, Type value);

            /// <summary>
            /// Deserializes an object table.
            /// </summary>
            /// <param name="stream"></param>
            /// <returns></returns>
            public ObjectTable<Type> Deserialize(Stream stream)
            {
                // deserialize count.
                var countBytes = new byte[4];
                stream.Read(countBytes, 0, 4);
                int count = BitConverter.ToInt32(countBytes, 0);

                // deserialize objects.
                var objectTable = new ObjectTable<Type>(false, count, true);
                int idx = 0;
                while(idx < count)
                {
                    objectTable._objects[idx] = this.DeserializeObject(stream);
                    idx++;
                }
                return objectTable;
            }

            /// <summary>
            /// Deserializes one object.
            /// </summary>
            /// <param name="stream"></param>
            /// <returns></returns>
            public abstract Type DeserializeObject(Stream stream);
        }
    }
}