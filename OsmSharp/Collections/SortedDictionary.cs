//
// System.Collections.Generic.SortedDictionary
//
// Author:
//    Raja R Harinath <rharinath@novell.com>
//
// Authors of previous (superseded) version:
//    Kazuki Oikawa (kazuki@panicode.com)
//    Atsushi Enomoto (atsushi@ximian.com)
//
// Modified by Ben Abelshausen removing Serializable attributes.

//
// Copyright (C) 2007, Novell, Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace OsmSharp.Collections
{
    /// <summary>
    /// Sorted dictionary.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [DebuggerDisplay("Count={Count}")]
    public class SortedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
    {
        class Node : RBTree.Node
        {
            public TKey key;
            public TValue value;

            public Node(TKey key)
            {
                this.key = key;
            }

            public Node(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }

            public override void SwapValue(RBTree.Node other)
            {
                Node o = (Node)other;
                TKey k = key; key = o.key; o.key = k;
                TValue v = value; value = o.value; o.value = v;
            }

            public KeyValuePair<TKey, TValue> AsKV()
            {
                return new KeyValuePair<TKey, TValue>(key, value);
            }

            public DictionaryEntry AsDE()
            {
                return new DictionaryEntry(key, value);
            }
        }

        class NodeHelper : RBTree.INodeHelper<TKey>
        {
            public IComparer<TKey> cmp;

            public int Compare(TKey key, RBTree.Node node)
            {
                return cmp.Compare(key, ((Node)node).key);
            }

            public RBTree.Node CreateNode(TKey key)
            {
                return new Node(key);
            }

            private NodeHelper(IComparer<TKey> cmp)
            {
                this.cmp = cmp;
            }
            static NodeHelper Default = new NodeHelper(Comparer<TKey>.Default);
            public static NodeHelper GetHelper(IComparer<TKey> cmp)
            {
                if (cmp == null || cmp == Comparer<TKey>.Default)
                    return Default;
                return new NodeHelper(cmp);
            }
        }

        RBTree tree;
        NodeHelper hlp;

        #region Constructor

        /// <summary>
        /// Creates a new sorted dictionary.
        /// </summary>
        public SortedDictionary()
            : this((IComparer<TKey>)null)
        {
        }

        /// <summary>
        /// Creates a new sorted dictionary.
        /// </summary>
        /// <param name="comparer"></param>
        public SortedDictionary(IComparer<TKey> comparer)
        {
            hlp = NodeHelper.GetHelper(comparer);
            tree = new RBTree(hlp);
        }

        /// <summary>
        /// Creates a new sorted dictionary.
        /// </summary>
        /// <param name="dic"></param>
        public SortedDictionary(IDictionary<TKey, TValue> dic)
            : this(dic, null)
        {
        }

        /// <summary>
        /// Creates a new sorted dictionary.
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="comparer"></param>
        public SortedDictionary(IDictionary<TKey, TValue> dic, IComparer<TKey> comparer)
            : this(comparer)
        {
            if (dic == null)
                throw new ArgumentNullException();
            foreach (KeyValuePair<TKey, TValue> entry in dic)
                Add(entry.Key, entry.Value);
        }
        #endregion

        #region PublicProperty

        /// <summary>
        /// Gets the current comparer.
        /// </summary>
        public IComparer<TKey> Comparer
        {
            get { return hlp.cmp; }
        }

        /// <summary>
        /// Returns the count.
        /// </summary>
        public int Count
        {
            get { return (int)tree.Count; }
        }

        /// <summary>
        /// Returns the value associated with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]
        {
            get
            {
                Node n = (Node)tree.Lookup(key);
                if (n == null)
                    throw new KeyNotFoundException();
                return n.value;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");
                Node n = (Node)tree.Intern(key, null);
                n.value = value;
            }
        }

        /// <summary>
        /// Returns the key collection.
        /// </summary>
        public KeyCollection Keys
        {
            get { return new KeyCollection(this); }
        }

        /// <summary>
        /// Returns the value collection.
        /// </summary>
        public ValueCollection Values
        {
            get { return new ValueCollection(this); }
        }
        #endregion

        #region PublicMethod

        /// <summary>
        /// Adds a new key value pair.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            RBTree.Node n = new Node(key, value);
            if (tree.Intern(key, n) != n)
                throw new ArgumentException("key already present in dictionary", "key");
        }

        /// <summary>
        /// Clears all items.
        /// </summary>
        public void Clear()
        {
            tree.Clear();
        }

        /// <summary>
        /// Returns true if the given key is in this dictionary.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return tree.Lookup(key) != null;
        }

        /// <summary>
        /// Return true if the given value is in this dictionary.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ContainsValue(TValue value)
        {
            IEqualityComparer<TValue> vcmp = EqualityComparer<TValue>.Default;
            foreach (Node n in tree)
                if (vcmp.Equals(value, n.value))
                    return true;
            return false;
        }

        /// <summary>
        /// Copies all key value pairs to this dictionary.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (Count == 0)
                return;
            if (array == null)
                throw new ArgumentNullException();
            if (arrayIndex < 0 || array.Length <= arrayIndex)
                throw new ArgumentOutOfRangeException();
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException();

            foreach (Node n in tree)
                array[arrayIndex++] = n.AsKV();
        }

        /// <summary>
        /// Returns the enumerator.
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Removes the item associated with the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            return tree.Remove(key) != null;
        }

        /// <summary>
        /// Gets the value associated with the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            Node n = (Node)tree.Lookup(key);
            value = n == null ? default(TValue) : n.value;
            return n != null;
        }

        #endregion

        #region PrivateMethod
        TKey ToKey(object key)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (!(key is TKey))
                throw new ArgumentException(String.Format("Key \"{0}\" cannot be converted to the key type {1}.", key, typeof(TKey)));
            return (TKey)key;
        }

        TValue ToValue(object value)
        {
            if (!(value is TValue) && (value != null || typeof(TValue).IsValueType))
                throw new ArgumentException(String.Format("Value \"{0}\" cannot be converted to the value type {1}.", value, typeof(TValue)));
            return (TValue)value;
        }
        #endregion

        #region IDictionary<TKey,TValue> Member

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get { return new KeyCollection(this); }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get { return new ValueCollection(this); }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Member

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            return TryGetValue(item.Key, out value) &&
                EqualityComparer<TValue>.Default.Equals(item.Value, value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            return TryGetValue(item.Key, out value) &&
                EqualityComparer<TValue>.Default.Equals(item.Value, value) &&
                Remove(item.Key);
        }

        #endregion

        #region IDictionary Member

        void IDictionary.Add(object key, object value)
        {
            Add(ToKey(key), ToValue(value));
        }

        bool IDictionary.Contains(object key)
        {
            return ContainsKey(ToKey(key));
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(this);
        }

        bool IDictionary.IsFixedSize
        {
            get { return false; }
        }

        bool IDictionary.IsReadOnly
        {
            get { return false; }
        }

        ICollection IDictionary.Keys
        {
            get { return new KeyCollection(this); }
        }

        void IDictionary.Remove(object key)
        {
            Remove(ToKey(key));
        }

        ICollection IDictionary.Values
        {
            get { return new ValueCollection(this); }
        }

        object IDictionary.this[object key]
        {
            get { return this[ToKey(key)]; }
            set { this[ToKey(key)] = ToValue(value); }
        }

        #endregion

        #region ICollection Member

        void ICollection.CopyTo(Array array, int index)
        {
            if (Count == 0)
                return;
            if (array == null)
                throw new ArgumentNullException();
            if (index < 0 || array.Length <= index)
                throw new ArgumentOutOfRangeException();
            if (array.Length - index < Count)
                throw new ArgumentException();

            foreach (Node n in tree)
                array.SetValue(n.AsDE(), index++);
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        // TODO:Is this correct? If this is wrong,please fix.
        object ICollection.SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region IEnumerable Member

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        #region IEnumerable<TKey> Member

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        #endregion

        /// <summary>
        /// Represents a value collection.
        /// </summary>
        [DebuggerDisplay("Count={Count}")]
        public sealed class ValueCollection : ICollection<TValue>,
            IEnumerable<TValue>, ICollection, IEnumerable
        {
            SortedDictionary<TKey, TValue> _dic;

            /// <summary>
            /// Creates a new value collection.
            /// </summary>
            /// <param name="dic"></param>
            public ValueCollection(SortedDictionary<TKey, TValue> dic)
            {
                _dic = dic;
            }

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<TValue>.Contains(TValue item)
            {
                return _dic.ContainsValue(item);
            }

            /// <summary>
            /// Copies all values into this value collection.
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                if (Count == 0)
                    return;
                if (array == null)
                    throw new ArgumentNullException();
                if (arrayIndex < 0 || array.Length <= arrayIndex)
                    throw new ArgumentOutOfRangeException();
                if (array.Length - arrayIndex < Count)
                    throw new ArgumentException();
                foreach (Node n in _dic.tree)
                    array[arrayIndex++] = n.value;
            }

            /// <summary>
            /// Returns the count.
            /// </summary>
            public int Count
            {
                get { return _dic.Count; }
            }

            bool ICollection<TValue>.IsReadOnly
            {
                get { return true; }
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException();
            }

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <summary>
            /// Returns the enumerator.
            /// </summary>
            /// <returns></returns>
            public Enumerator GetEnumerator()
            {
                return new Enumerator(_dic);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (Count == 0)
                    return;
                if (array == null)
                    throw new ArgumentNullException();
                if (index < 0 || array.Length <= index)
                    throw new ArgumentOutOfRangeException();
                if (array.Length - index < Count)
                    throw new ArgumentException();
                foreach (Node n in _dic.tree)
                    array.SetValue(n.value, index++);
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            object ICollection.SyncRoot
            {
                get { return _dic; }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(_dic);
            }

            /// <summary>
            /// Represents an enumerator.
            /// </summary>
            public struct Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable
            {
                RBTree.NodeEnumerator host;

                TValue current;

                internal Enumerator(SortedDictionary<TKey, TValue> dic)
                    : this()
                {
                    host = dic.tree.GetEnumerator();
                }

                /// <summary>
                /// Returns the current value.
                /// </summary>
                public TValue Current
                {
                    get { return current; }
                }

                /// <summary>
                /// Move to the next value.
                /// </summary>
                /// <returns></returns>
                public bool MoveNext()
                {
                    if (!host.MoveNext())
                        return false;
                    current = ((Node)host.Current).value;
                    return true;
                }

                /// <summary>
                /// Diposes this enumerator.
                /// </summary>
                public void Dispose()
                {
                    host.Dispose();
                }

                object IEnumerator.Current
                {
                    get
                    {
                        host.check_current();
                        return current;
                    }
                }

                void IEnumerator.Reset()
                {
                    host.Reset();
                }
            }
        }

        /// <summary>
        /// Represents a key collection.
        /// </summary>
        [DebuggerDisplay("Count={Count}")]
        public sealed class KeyCollection : ICollection<TKey>,
            IEnumerable<TKey>, ICollection, IEnumerable
        {
            SortedDictionary<TKey, TValue> _dic;

            /// <summary>
            /// Creates a new key collection.
            /// </summary>
            /// <param name="dic"></param>
            public KeyCollection(SortedDictionary<TKey, TValue> dic)
            {
                _dic = dic;
            }

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<TKey>.Contains(TKey item)
            {
                return _dic.ContainsKey(item);
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <summary>
            /// Copies all values to this key collection.
            /// </summary>
            /// <param name="array"></param>
            /// <param name="arrayIndex"></param>
            public void CopyTo(TKey[] array, int arrayIndex)
            {
                if (Count == 0)
                    return;
                if (array == null)
                    throw new ArgumentNullException();
                if (arrayIndex < 0 || array.Length <= arrayIndex)
                    throw new ArgumentOutOfRangeException();
                if (array.Length - arrayIndex < Count)
                    throw new ArgumentException();
                foreach (Node n in _dic.tree)
                    array[arrayIndex++] = n.key;
            }

            /// <summary>
            /// Returns the count.
            /// </summary>
            public int Count
            {
                get { return _dic.Count; }
            }

            bool ICollection<TKey>.IsReadOnly
            {
                get { return true; }
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException();
            }

            /// <summary>
            /// Gets the enumerator.
            /// </summary>
            /// <returns></returns>
            public Enumerator GetEnumerator()
            {
                return new Enumerator(_dic);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (Count == 0)
                    return;
                if (array == null)
                    throw new ArgumentNullException();
                if (index < 0 || array.Length <= index)
                    throw new ArgumentOutOfRangeException();
                if (array.Length - index < Count)
                    throw new ArgumentException();
                foreach (Node n in _dic.tree)
                    array.SetValue(n.key, index++);
            }

            bool ICollection.IsSynchronized
            {
                get { return false; }
            }

            object ICollection.SyncRoot
            {
                get { return _dic; }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(_dic);
            }

            /// <summary>
            /// Represents the enumerator.
            /// </summary>
            public struct Enumerator : IEnumerator<TKey>, IEnumerator, IDisposable
            {
                RBTree.NodeEnumerator host;

                TKey current;

                internal Enumerator(SortedDictionary<TKey, TValue> dic)
                    : this()
                {
                    host = dic.tree.GetEnumerator();
                }

                /// <summary>
                /// Returns the current itme.
                /// </summary>
                public TKey Current
                {
                    get { return current; }
                }

                /// <summary>
                /// Move to the next item.
                /// </summary>
                /// <returns></returns>
                public bool MoveNext()
                {
                    if (!host.MoveNext())
                        return false;
                    current = ((Node)host.Current).key;
                    return true;
                }

                /// <summary>
                /// Diposes the current enumerator.
                /// </summary>
                public void Dispose()
                {
                    host.Dispose();
                }

                object IEnumerator.Current
                {
                    get
                    {
                        host.check_current();
                        return current;
                    }
                }

                void IEnumerator.Reset()
                {
                    host.Reset();
                }
            }
        }

        /// <summary>
        /// Represents a key-value enumerator.
        /// </summary>
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDisposable, IDictionaryEnumerator, IEnumerator
        {
            RBTree.NodeEnumerator host;

            KeyValuePair<TKey, TValue> current;

            internal Enumerator(SortedDictionary<TKey, TValue> dic)
                : this()
            {
                host = dic.tree.GetEnumerator();
            }

            /// <summary>
            /// Returns the current item.
            /// </summary>
            public KeyValuePair<TKey, TValue> Current
            {
                get { return current; }
            }

            /// <summary>
            /// Move to the next item.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (!host.MoveNext())
                    return false;
                current = ((Node)host.Current).AsKV();
                return true;
            }

            /// <summary>
            /// Diposes this enumerator.
            /// </summary>
            public void Dispose()
            {
                host.Dispose();
            }

            Node CurrentNode
            {
                get
                {
                    host.check_current();
                    return (Node)host.Current;
                }
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get { return CurrentNode.AsDE(); }
            }

            object IDictionaryEnumerator.Key
            {
                get { return CurrentNode.key; }
            }

            object IDictionaryEnumerator.Value
            {
                get { return CurrentNode.value; }
            }

            object IEnumerator.Current
            {
                get { return CurrentNode.AsDE(); }
            }

            void IEnumerator.Reset()
            {
                host.Reset();
            }
        }
    }

	internal class RBTree : IEnumerable, IEnumerable<RBTree.Node> {
		public interface INodeHelper<T> {
			int Compare (T key, Node node);
			Node CreateNode (T key);
		}

		public abstract class Node {
			public Node left, right;
			uint size_black;

			const uint black_mask = 1;
			const int black_shift = 1;
			public bool IsBlack {
				get { return (size_black & black_mask) == black_mask; }
				set { size_black = value ? (size_black | black_mask) : (size_black & ~black_mask); }
			}

			public uint Size {
				get { return size_black >> black_shift; }
				set { size_black = (value << black_shift) | (size_black & black_mask); }
			}

			public uint FixSize ()
			{
				Size = 1;
				if (left != null)
					Size += left.Size;
				if (right != null)
					Size += right.Size;
				return Size;
			}

			public Node ()
			{
				size_black = 2; // Size == 1, IsBlack = false
			}

			public abstract void SwapValue (Node other);

#if TEST
			public int VerifyInvariants ()
			{
				int black_depth_l = 0;
				int black_depth_r = 0;
				uint size = 1;
				bool child_is_red = false;
				if (left != null) {
					black_depth_l = left.VerifyInvariants ();
					size += left.Size;
					child_is_red |= !left.IsBlack;
				}

				if (right != null) {
					black_depth_r = right.VerifyInvariants ();
					size += right.Size;
					child_is_red |= !right.IsBlack;
				}

				if (black_depth_l != black_depth_r)
					throw new SystemException ("Internal error: black depth mismatch");

				if (!IsBlack && child_is_red)
					throw new SystemException ("Internal error: red-red conflict");
				if (Size != size)
					throw new SystemException ("Internal error: metadata error");

				return black_depth_l + (IsBlack ? 1 : 0);
			}

			public abstract void Dump (string indent);
#endif
		}

		Node root;
		object hlp;
		uint version;

#if ONE_MEMBER_CACHE
#if TARGET_JVM
		static readonly LocalDataStoreSlot _cachedPathStore = System.Threading.Thread.AllocateDataSlot ();

		static List<Node> cached_path {
			get { return (List<Node>) System.Threading.Thread.GetData (_cachedPathStore); }
			set { System.Threading.Thread.SetData (_cachedPathStore, value); }
		}
#else
		[ThreadStatic]
		static List<Node> cached_path;
#endif

		static List<Node> alloc_path ()
		{
			if (cached_path == null)
				return new List<Node> ();

			List<Node> path = cached_path;
			cached_path = null;
			return path;
		}

		static void release_path (List<Node> path)
		{
			if (cached_path == null || cached_path.Capacity < path.Capacity) {
				path.Clear ();
				cached_path = path;
			}
		}
#else
		static List<Node> alloc_path ()
		{
			return new List<Node> ();
		}

		static void release_path (List<Node> path)
		{
		}
#endif

		public RBTree (object hlp)
		{
			// hlp is INodeHelper<T> for some T
			this.hlp = hlp;
		}

		public void Clear ()
		{
			root = null;
			++version;
		}

		// if key is already in the tree, return the node associated with it
		// if not, insert new_node into the tree, and return it
		public Node Intern<T> (T key, Node new_node)
		{
			if (root == null) {
				if (new_node == null)
					new_node = ((INodeHelper<T>) hlp).CreateNode (key);
				root = new_node;
				root.IsBlack = true;
				++version;
				return root;
			}

			List<Node> path = alloc_path ();
			int in_tree_cmp = find_key (key, path);
			Node retval = path [path.Count - 1];
			if (retval == null) {
				if (new_node == null)
					new_node = ((INodeHelper<T>) hlp).CreateNode (key);
				retval = do_insert (in_tree_cmp, new_node, path);
			}
			// no need for a try .. finally, this is only used to mitigate allocations
			release_path (path);
			return retval;
		}

		// returns the just-removed node (or null if the value wasn't in the tree)
		public Node Remove<T> (T key)
		{
			if (root == null)
				return null;

			List<Node> path = alloc_path ();
			int in_tree_cmp = find_key (key, path);
			Node retval = null;
			if (in_tree_cmp == 0)
				retval = do_remove (path);
			// no need for a try .. finally, this is only used to mitigate allocations
			release_path (path);
			return retval;
		}

		public Node Lookup<T> (T key)
		{
			INodeHelper<T> hlp = (INodeHelper<T>) this.hlp;
			Node current = root;
			while (current != null) {
				int c = hlp.Compare (key, current);
				if (c == 0)
					break;
				current = c < 0 ? current.left : current.right;
			}
			return current;
		}

		public void Bound<T> (T key, ref Node lower, ref Node upper)
		{
			INodeHelper<T> hlp = (INodeHelper<T>) this.hlp;
			Node current = root;
			while (current != null) {
				int c = hlp.Compare (key, current);
				if (c <= 0)
					upper = current;
				if (c >= 0)
					lower = current;
				if (c == 0)
					break;
				current = c < 0 ? current.left : current.right;
			}
		}

		public int Count {
			get { return root == null ? 0 : (int) root.Size; }
		}

		public Node this [int index] {
			get {
				if (index < 0 || index >= Count)
					throw new IndexOutOfRangeException ("index");

				Node current = root;
				while (current != null) {
					int left_size = current.left == null ? 0 : (int) current.left.Size;
					if (index == left_size)
						return current;
					if (index < left_size) {
						current = current.left;
					} else {
						index -= left_size + 1;
						current = current.right;
					}
				}
				throw new Exception ("Internal Error: index calculation");
			}
		}

		public NodeEnumerator GetEnumerator ()
		{
			return new NodeEnumerator (this);
		}

		// Get an enumerator that starts at 'key' or the next higher element in the tree
		public NodeEnumerator GetSuffixEnumerator<T> (T key)
		{
			var pennants = new Stack<Node> ();
			INodeHelper<T> hlp = (INodeHelper<T>) this.hlp;
			Node current = root;
			while (current != null) {
				int c = hlp.Compare (key, current);
				if (c <= 0)
					pennants.Push (current);
				if (c == 0)
					break;
				current = c < 0 ? current.left : current.right;
			}
			return new NodeEnumerator (this, pennants);
		}

		IEnumerator<Node> IEnumerable<Node>.GetEnumerator ()
		{
			return GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator ();
		}

#if TEST
		public void VerifyInvariants ()
		{
			if (root != null) {
				if (!root.IsBlack)
					throw new SystemException ("Internal Error: root is not black");
				root.VerifyInvariants ();
			}
		}

		public void Dump ()
		{
			if (root != null)
				root.Dump ("");
		}
#endif

		// Pre-condition: root != null
		int find_key<T> (T key, List<Node> path)
		{
			INodeHelper<T> hlp = (INodeHelper<T>) this.hlp;
			int c = 0;
			Node sibling = null;
			Node current = root;

			if (path != null)
				path.Add (root);

			while (current != null) {
				c = hlp.Compare (key, current);
				if (c == 0)
					return c;

				if (c < 0) {
					sibling = current.right;
					current = current.left;
				} else {
					sibling = current.left;
					current = current.right;
				}

				if (path != null) {
					path.Add (sibling);
					path.Add (current);
				}
			}

			return c;
		}

		Node do_insert (int in_tree_cmp, Node current, List<Node> path)
		{
			path [path.Count - 1] = current;
			Node parent = path [path.Count - 3];

			if (in_tree_cmp < 0)
				parent.left = current;
			else
				parent.right = current;
			for (int i = 0; i < path.Count - 2; i += 2)
				++ path [i].Size;

			if (!parent.IsBlack)
				rebalance_insert (path);

			if (!root.IsBlack)
				throw new Exception ("Internal error: root is not black");

			++version;
			return current;
		}

		Node do_remove (List<Node> path)
		{
			int curpos = path.Count - 1;

			Node current = path [curpos];
			if (current.left != null) {
				Node pred = right_most (current.left, current.right, path);
				current.SwapValue (pred);
				if (pred.left != null) {
					Node ppred = pred.left;
					path.Add (null); path.Add (ppred);
					pred.SwapValue (ppred);
				}
			} else if (current.right != null) {
				Node succ = current.right;
				path.Add (null); path.Add (succ);
				current.SwapValue (succ);
			}

			curpos = path.Count - 1;
			current = path [curpos];

			if (current.Size != 1)
				throw new Exception ("Internal Error: red-black violation somewhere");

			// remove it from our data structures
			path [curpos] = null;
			node_reparent (curpos == 0 ? null : path [curpos-2], current, 0, null);

			for (int i = 0; i < path.Count - 2; i += 2)
				-- path [i].Size;

			if (current.IsBlack) {
				current.IsBlack = false;
				if (curpos != 0)
					rebalance_delete (path);
			}

			if (root != null && !root.IsBlack)
				throw new Exception ("Internal Error: root is not black");

			++version;
			return current;
		}

		// Pre-condition: current is red
		void rebalance_insert (List<Node> path)
		{
			int curpos = path.Count - 1;
			do {
				// parent == curpos-2, uncle == curpos-3, grandpa == curpos-4
				if (path [curpos-3] == null || path [curpos-3].IsBlack) {
					rebalance_insert__rotate_final (curpos, path);
					return;
				}

				path [curpos-2].IsBlack = path [curpos-3].IsBlack = true;

				curpos -= 4; // move to the grandpa

				if (curpos == 0) // => current == root
					return;
				path [curpos].IsBlack = false;
			} while (!path [curpos-2].IsBlack);
		}

		// Pre-condition: current is black
		void rebalance_delete (List<Node> path)
		{
			int curpos = path.Count - 1;
			do {
				Node sibling = path [curpos-1];
				// current is black => sibling != null
				if (!sibling.IsBlack) {
					// current is black && sibling is red 
					// => both sibling.left and sibling.right are black, and are not null
					curpos = ensure_sibling_black (curpos, path);
					// one of the nephews became the new sibling -- in either case, sibling != null
					sibling = path [curpos-1];
				}

				if ((sibling.left != null && !sibling.left.IsBlack) ||
				    (sibling.right != null && !sibling.right.IsBlack)) {
					rebalance_delete__rotate_final (curpos, path);
					return;
				}

				sibling.IsBlack = false;

				curpos -= 2; // move to the parent

				if (curpos == 0)
					return;
			} while (path [curpos].IsBlack);
			path [curpos].IsBlack = true;
		}

		void rebalance_insert__rotate_final (int curpos, List<Node> path)
		{
			Node current = path [curpos];
			Node parent = path [curpos-2];
			Node grandpa = path [curpos-4];

			uint grandpa_size = grandpa.Size;

			Node new_root;

			bool l1 = parent == grandpa.left;
			bool l2 = current == parent.left;
			if (l1 && l2) {
				grandpa.left = parent.right; parent.right = grandpa;
				new_root = parent;
			} else if (l1 && !l2) {
				grandpa.left = current.right; current.right = grandpa;
				parent.right = current.left; current.left = parent;
				new_root = current;
			} else if (!l1 && l2) {
				grandpa.right = current.left; current.left = grandpa;
				parent.left = current.right; current.right = parent;
				new_root = current;
			} else { // (!l1 && !l2)
				grandpa.right = parent.left; parent.left = grandpa;
				new_root = parent;
			}

			grandpa.FixSize (); grandpa.IsBlack = false;
			if (new_root != parent)
				parent.FixSize (); /* parent is red already, so no need to set it */

			new_root.IsBlack = true;
			node_reparent (curpos == 4 ? null : path [curpos-6], grandpa, grandpa_size, new_root);
		}

		// Pre-condition: sibling is black, and one of sibling.left and sibling.right is red
		void rebalance_delete__rotate_final (int curpos, List<Node> path)
		{
			//Node current = path [curpos];
			Node sibling = path [curpos-1];
			Node parent = path [curpos-2];

			uint parent_size = parent.Size;
			bool parent_was_black = parent.IsBlack;

			Node new_root;
			if (parent.right == sibling) {
				// if far nephew is black
				if (sibling.right == null || sibling.right.IsBlack) {
					// => near nephew is red, move it up
					Node nephew = sibling.left;
					parent.right = nephew.left; nephew.left = parent;
					sibling.left = nephew.right; nephew.right = sibling;
					new_root = nephew;
				} else {
					parent.right = sibling.left; sibling.left = parent;
					sibling.right.IsBlack = true;
					new_root = sibling;
				}
			} else {
				// if far nephew is black
				if (sibling.left == null || sibling.left.IsBlack) {
					// => near nephew is red, move it up
					Node nephew = sibling.right;
					parent.left = nephew.right; nephew.right = parent;
					sibling.right = nephew.left; nephew.left = sibling;
					new_root = nephew;
				} else {
					parent.left = sibling.right; sibling.right = parent;
					sibling.left.IsBlack = true;
					new_root = sibling;
				}
			}

			parent.FixSize (); parent.IsBlack = true;
			if (new_root != sibling)
				sibling.FixSize (); /* sibling is already black, so no need to set it */

			new_root.IsBlack = parent_was_black;
			node_reparent (curpos == 2 ? null : path [curpos-4], parent, parent_size, new_root);
		}

		// Pre-condition: sibling is red (=> parent, sibling.left and sibling.right are black)
		int ensure_sibling_black (int curpos, List<Node> path)
		{
			Node current = path [curpos];
			Node sibling = path [curpos-1];
			Node parent = path [curpos-2];

			bool current_on_left;
			uint parent_size = parent.Size;

			if (parent.right == sibling) {
				parent.right = sibling.left; sibling.left = parent;
				current_on_left = true;
			} else {
				parent.left = sibling.right; sibling.right = parent;
				current_on_left = false;
			}

			parent.FixSize (); parent.IsBlack = false;

			sibling.IsBlack = true;
			node_reparent (curpos == 2 ? null : path [curpos-4], parent, parent_size, sibling);

			// accomodate the rotation
			if (curpos+1 == path.Count) {
				path.Add (null);
				path.Add (null);
			}

			path [curpos-2] = sibling;
			path [curpos-1] = current_on_left ? sibling.right : sibling.left;
			path [curpos] = parent;
			path [curpos+1] = current_on_left ? parent.right : parent.left;
			path [curpos+2] = current;

			return curpos + 2;
		}

		void node_reparent (Node orig_parent, Node orig, uint orig_size, Node updated)
		{
			if (updated != null && updated.FixSize () != orig_size)
				throw new Exception ("Internal error: rotation");

			if (orig == root)
				root = updated;
			else if (orig == orig_parent.left)
				orig_parent.left = updated;
			else if (orig == orig_parent.right)
				orig_parent.right = updated;
			else
				throw new Exception ("Internal error: path error");
		}

		// Pre-condition: current != null
		static Node right_most (Node current, Node sibling, List<Node> path)
		{
			for (;;) {
				path.Add (sibling);
				path.Add (current);
				if (current.right == null)
					return current;
				sibling = current.left;
				current = current.right;
			}
		}

		public struct NodeEnumerator : IEnumerator, IEnumerator<Node> {
			RBTree tree;
			uint version;

			Stack<Node> pennants, init_pennants;

			internal NodeEnumerator (RBTree tree)
				: this ()
			{
				this.tree = tree;
				version = tree.version;
			}

			internal NodeEnumerator (RBTree tree, Stack<Node> init_pennants)
				: this (tree)
			{
				this.init_pennants = init_pennants;
			}

			public void Reset ()
			{
				check_version ();
				pennants = null;
			}

			public Node Current {
				get { return pennants.Peek (); }
			}

			object IEnumerator.Current {
				get {
					check_current ();
					return Current;
				}
			}

			public bool MoveNext ()
			{
				check_version ();

				Node next;
				if (pennants == null) {
					if (tree.root == null)
						return false;
					if (init_pennants != null) {
						pennants = init_pennants;
						init_pennants = null;
						return pennants.Count != 0;
					}
					pennants = new Stack<Node> ();
					next = tree.root;
				} else {
					if (pennants.Count == 0)
						return false;
					Node current = pennants.Pop ();
					next = current.right;
				}
				for (; next != null; next = next.left)
					pennants.Push (next);

				return pennants.Count != 0;
			}

			public void Dispose ()
			{
				tree = null;
				pennants = null;
			}

			void check_version ()
			{
				if (tree == null)
					throw new ObjectDisposedException ("enumerator");
				if (version != tree.version)
					throw new InvalidOperationException ("tree modified");
			}

			internal void check_current ()
			{
				check_version ();
				if (pennants == null)
					throw new InvalidOperationException ("state invalid before the first MoveNext()");
			}
		}
	}
}