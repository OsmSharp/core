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
using System.Linq;
using System.Text;

namespace OsmSharp.Collections
{
    /// <summary>
    /// A dictionary that uses a string table behind.
    /// </summary>
    public class StringTableDictionary<Type> : IDictionary<Type, Type>
    {
        /// <summary>
        /// Holds the string table.
        /// </summary>
        private ObjectTable<Type> _string_table;

        /// <summary>
        /// The dictionary behind.
        /// </summary>
        private Dictionary<uint, uint> _dictionary;

        /// <summary>
        /// Creates a new dictionary.
        /// </summary>
        /// <param name="string_table"></param>
        public StringTableDictionary(ObjectTable<Type> string_table)
        {
            _string_table = string_table;
            _dictionary = new Dictionary<uint, uint>();
        }

        /// <summary>
        /// Adds key-value pair of strings.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(Type key, Type value)
        {
            uint key_int = _string_table.Add(key);
            uint value_int = _string_table.Add(value);

            _dictionary.Add(key_int, value_int);
        }

        /// <summary>
        /// Returns true if the given key is present.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(Type key)
        {
            uint key_int = _string_table.Add(key);  // TODO: this could be problematic, testing contains adds objects to string table.

            return _dictionary.ContainsKey(key_int);
        }

        /// <summary>
        /// Returns a collection of keys.
        /// </summary>
        public ICollection<Type> Keys
        {
            get 
            { // i know pretty naive implementation.
                // TODO: add an ICollection<string> accepting a StringTable object and an ICollection<uint>
                List<Type> keys = new List<Type>();
                foreach (uint key_int in _dictionary.Keys)
                {
                    keys.Add(_string_table.Get(key_int));
                }
                return keys;
            }
        }

        /// <summary>
        /// Removes a value with the given key from this dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(Type key)
        {
            uint key_int = _string_table.Add(key);

            return _dictionary.Remove(key_int);
        }

        /// <summary>
        /// Tries to get a value from this dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(Type key, out Type value)
        {
            uint key_int = _string_table.Add(key);
            uint value_int;
            value = default(Type);
            if (_dictionary.TryGetValue(key_int, out value_int))
            {
                value = _string_table.Get(value_int);
            }
            return false;
        }

        /// <summary>
        /// Returns all values in this dictionary.
        /// </summary>
        public ICollection<Type> Values
        {
            get
            { // i know pretty naive implementation.
                // TODO: add an ICollection<string> accepting a StringTable object and an ICollection<uint>
                List<Type> keys = new List<Type>();
                foreach (uint key_int in _dictionary.Values)
                {
                    keys.Add(_string_table.Get(key_int));
                }
                return keys;
            }
        }

        /// <summary>
        /// Gets/sets a value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Type this[Type key]
        {
            get
            {
                uint key_int = _string_table.Add(key);
                uint value_int = _dictionary[key_int];
                return _string_table.Get(value_int);
            }
            set
            {
                uint key_int = _string_table.Add(key);
                uint value_int = _string_table.Add(value);
                _dictionary[key_int] = value_int;
            }
        }

        /// <summary>
        /// Adds a key value pair.
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<Type, Type> item)
        {
            KeyValuePair<uint, uint> item_int = new KeyValuePair<uint, uint>(
                _string_table.Add(item.Key), _string_table.Add(item.Key));
            _dictionary.Add(item_int.Key, item_int.Value);
        }

        /// <summary>
        /// Clears all content from this dictionary.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <summary>
        /// Returns true if the given key value pair is contained in this dictionary.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<Type, Type> item)
        {
            uint value_int;
            if (_dictionary.TryGetValue(_string_table.Add(item.Key), out value_int))
            {
                return value_int == _string_table.Add(item.Value); // TODO: this could be problematic, testing contains adds objects to string table.
            }
            return false;
        }

        /// <summary>
        /// Copies all objects 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<Type, Type>[] array, int arrayIndex)
        {
            foreach (KeyValuePair<Type, Type> pair in this)
            {
                array[arrayIndex] = pair;
                arrayIndex++;
            }
        }

        /// <summary>
        /// Returns the number of objects in this collection.
        /// </summary>
        public int Count
        {
            get { return _dictionary.Count; }
        }

        /// <summary>
        /// This dictionary is not readonly.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Removes an item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<Type, Type> item)
        {
            return this.Remove(item.Key);
        }

        #region Enumeration

        /// <summary>
        /// Returns a enumerator for this dictionary.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<Type, Type>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a enumerator for this dictionary.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        private class StringTableDictionaryEnumerator : IEnumerator<KeyValuePair<Type, Type>>
        {
            private KeyValuePair<Type, Type> _current;

            private IEnumerator<KeyValuePair<uint, uint>> _enumerator;

            private ObjectTable<Type> _string_table;

            public StringTableDictionaryEnumerator(ObjectTable<Type> string_table, IEnumerator<KeyValuePair<uint, uint>> enumerator)
            {
                _enumerator = enumerator;
                _string_table = string_table;
            }

            public KeyValuePair<Type, Type> Current
            {
                get { return _current; }
            }

            public void Dispose()
            {

            }

            object System.Collections.IEnumerator.Current
            {
                get { return _current; }
            }

            public bool MoveNext()
            {
                if (_enumerator.MoveNext())
                {
                    _current = new KeyValuePair<Type, Type>(
                        _string_table.Get(_enumerator.Current.Key), _string_table.Get(_enumerator.Current.Value));
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                _enumerator.Reset();
            }
        }


        #endregion
    }
}
