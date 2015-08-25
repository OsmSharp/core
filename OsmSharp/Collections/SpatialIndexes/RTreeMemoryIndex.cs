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
using System.Collections;
using System.Collections.Generic;
using OsmSharp.Math.Primitives;

namespace OsmSharp.Collections.SpatialIndexes
{
    /// <summary>
    /// R-tree implementation of a spatial index.
    /// http://en.wikipedia.org/wiki/R-tree
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RTreeMemoryIndex<T> : ISpatialIndex<T>
    {
        /// <summary>
        /// Holds the number of objects in this index.
        /// </summary>
        private int _count = 0;

        /// <summary>
        /// Holds the root node.
        /// </summary>
        private Node _root;

        /// <summary>
        /// Holds the maximum leaf size M.
        /// </summary>
        private readonly int _maxLeafSize = 200;

        /// <summary>
        /// Holds the minimum leaf size m.
        /// </summary>
        private readonly int _minLeafSize = 100;

        /// <summary>
        /// Creates a new index.
        /// </summary>
        public RTreeMemoryIndex()
        {

        }

        /// <summary>
        /// Creates a new index.
        /// </summary>
        /// <param name="minLeafSize"></param>
        /// <param name="maxLeafSize"></param>
        public RTreeMemoryIndex(int minLeafSize, int maxLeafSize)
        {
            _minLeafSize = minLeafSize;
            _maxLeafSize = maxLeafSize;
        }

        /// <summary>
        /// Returns the number of objects in this index.
        /// </summary>
        public int Count { get { return _count; } }

        /// <summary>
        /// Adds a new item with the corresponding box.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="item"></param>
		public void Add(BoxF2D box, T item)
        {
            _count++;

            if (_root == null)
            { // create the root.
                _root = new Node();
				_root.Boxes = new List<BoxF2D>();
                _root.Children = new List<T>();
            }

            // add new data.
            Node leaf = RTreeMemoryIndex<T>.ChooseLeaf(_root, box);
            Node newRoot = RTreeMemoryIndex<T>.Add(leaf, box, item, _minLeafSize, _maxLeafSize);
            if (newRoot != null)
            { // there should be a new root.
                _root = newRoot;
            }
        }

        /// <summary>
        /// Removes the given item.
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Removes the given item when it is contained in the given box.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="item"></param>
        public void Remove(BoxF2D box, T item)
        {
            if(RTreeMemoryIndex<T>.RemoveSimple(_root, box, item))
            {
                _count--;
            }
        }

        /// <summary>
        /// Queries this index and returns all objects with overlapping bounding boxes.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public IEnumerable<T> Get(BoxF2D box)
        {
            var result = new HashSet<T>();
            RTreeMemoryIndex<T>.Get(_root, box, result);
            return result;
        }

        /// <summary>
        /// Cancels the current request.
        /// </summary>
        public void GetCancel()
        {

        }

        /// <summary>
        /// Gets the root node.
        /// </summary>
        internal Node Root { get { return _root; } }

        #region Tree Structure

        /// <summary>
        /// Represents a simple node.
        /// </summary>
        internal class Node
        {
            /// <summary>
            /// Gets or sets boxes.
            /// </summary>
			public List<BoxF2D> Boxes { get; set; }

            /// <summary>
            /// Gets or sets the children.
            /// </summary>
            public IList Children { get; set; }

            /// <summary>
            /// Gets or sets the parent.
            /// </summary>
            public Node Parent { get; set; }

            /// <summary>
            /// Returns the bounding box for this node.
            /// </summary>
            /// <returns></returns>
			public BoxF2D GetBox()
            {
				BoxF2D box = this.Boxes[0];
                for (int idx = 1; idx < this.Boxes.Count; idx++)
                {
                    box = box.Union(this.Boxes[idx]);
                }
                return box;
            }
        }

        #region Tree Operations

        /// <summary>
        /// Fills the collection with data.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="box"></param>
        /// <param name="result"></param>
        private static void Get(Node node, BoxF2D box, HashSet<T> result)
        {
            if (node.Children is List<Node>)
            {
                var children = (node.Children as List<Node>);
                for (int idx = 0; idx < children.Count; idx++)
                {
                    if (box.Overlaps(node.Boxes[idx]))
                    {
                        if (box.Contains(node.Boxes[idx]))
                        { // add all the data from the child.
                            RTreeMemoryIndex<T>.GetAll(children[idx],
                                result);
                        }
                        else
                        { // add the data from the child.
                            RTreeMemoryIndex<T>.Get(children[idx],
                                box, result);
                        }
                    }
                }
            }
            else
            {
                var children = (node.Children as List<T>);
                if (children != null)
                { // the children are of the data type.
                    for (int idx = 0; idx < node.Children.Count; idx++)
                    {
                        if (node.Boxes[idx].Overlaps(box))
                        {
                            result.Add(children[idx]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fills the collection with data.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="result"></param>
        private static void GetAll(Node node, HashSet<T> result)
        {
            if (node.Children is List<Node>)
            {
                var children = (node.Children as List<Node>);
                for (int idx = 0; idx < children.Count; idx++)
                {
                    // add all the data from the child.
                    RTreeMemoryIndex<T>.GetAll(children[idx],
                                                     result);
                }
            }
            else
            {
                var children = (node.Children as List<T>);
                if (children != null)
                { // the children are of the data type.
                    for (int idx = 0; idx < node.Children.Count; idx++)
                    {
                        result.Add(children[idx]);
                    }
                }
            }
        }

        /// <summary>
        /// Adds the given item to the given box.
        /// </summary>
        /// <param name="leaf"></param>
        /// <param name="box"></param>
        /// <param name="item"></param>
        /// <param name="minimumSize"></param>
        /// <param name="maximumSize"></param>
		private static Node Add(Node leaf, BoxF2D box, T item, int minimumSize, int maximumSize)
        {
            if (box == null) throw new ArgumentNullException("box");
            if (leaf == null) throw new ArgumentNullException("leaf");

            Node ll = null;
            if (leaf.Boxes.Count == maximumSize)
            { // split the node.
                // add the child.
                leaf.Boxes.Add(box);
                leaf.Children.Add(item);

                Node[] split = RTreeMemoryIndex<T>.SplitNode(leaf, minimumSize);
                leaf.Boxes = split[0].Boxes;
                leaf.Children = split[0].Children;
                RTreeMemoryIndex<T>.SetParents(leaf);
                ll = split[1];
            }
            else
            {
                // add the child.
                leaf.Boxes.Add(box);
                leaf.Children.Add(item);
            }

            // adjust the tree.
            Node n = leaf;
            Node nn = ll;
            while (n.Parent != null)
            { // keep going until the root is reached.
                Node p = n.Parent;
                RTreeMemoryIndex<T>.TightenFor(p, n); // tighten the parent box around n.

                if (nn != null)
                { // propagate split if needed.
                    if (p.Boxes.Count == maximumSize)
                    { // parent needs to be split.
                        p.Boxes.Add(nn.GetBox());
                        p.Children.Add(nn);
                        Node[] split = RTreeMemoryIndex<T>.SplitNode(
                            p, minimumSize);
                        p.Boxes = split[0].Boxes;
                        p.Children = split[0].Children;
                        RTreeMemoryIndex<T>.SetParents(p);
                        nn = split[1];
                    }
                    else
                    { // add the other 'split' node.
                        p.Boxes.Add(nn.GetBox());
                        p.Children.Add(nn);
                        nn.Parent = p;
                        nn = null;
                    }
                }
                n = p;
            }
            if (nn != null)
            { // create a new root node and 
                var root = new Node();
				root.Boxes = new List<BoxF2D>();
                root.Boxes.Add(n.GetBox());
                root.Boxes.Add(nn.GetBox());
                root.Children = new List<Node>();
                root.Children.Add(n);
                n.Parent = root;
                root.Children.Add(nn);
                nn.Parent = root;
                return root;
            }
            return null; // no new root node needed.
        }

        /// <summary>
        /// Removes the given item but does not re-balance the tree.
        /// </summary>
        /// <param name="node">The node to begin the search for the item.</param>
        /// <param name="box">The box of the item.</param>
        /// <param name="item">The item to remove.</param>
        private static bool RemoveSimple(Node node, BoxF2D box, T item)
        {
            if (node.Children is List<Node>)
            {
                var children = (node.Children as List<Node>);
                for (int idx = 0; idx < children.Count; idx++)
                {
                    if (box.Overlaps(node.Boxes[idx]))
                    {
                        if(RTreeMemoryIndex<T>.RemoveSimple(node.Children[idx] as Node, box, item))
                        { // if sucessfull stop the search.
                            return true;
                        }
                    }
                }
            }
            else
            {
                var children = (node.Children as List<T>);
                if (children != null)
                { // the children are of the data type.
                    return children.Remove(item);
                }
            }
            return false;
        }

        /// <summary>
        /// Tightens the box for the given node in the given parent.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        private static void TightenFor(Node parent, Node child)
        {
            for (int idx = 0; idx < parent.Children.Count; idx++)
            {
                if (parent.Children[idx] == child)
                {
                    parent.Boxes[idx] = child.GetBox();
                }
            }
        }

        /// <summary>
        /// Choose the child to best place the given box.
        /// </summary>
        /// <param name="box"></param>
        /// <param name="node"></param>
        /// <returns></returns>
		private static Node ChooseLeaf(Node node, BoxF2D box)
        {
            if (box == null) throw new ArgumentNullException("box");
            if (node == null) throw new ArgumentNullException("node");

            // keep looping until a leaf is found.
            while (node.Children is List<Node>)
            { // choose the best leaf.
                Node bestChild = null;
				BoxF2D bestBox = null;
                double bestIncrease = double.MaxValue;
                var children = node.Children as List<Node>; // cast just once.
                for (int idx = 0; idx < node.Boxes.Count; idx++)
                {
					BoxF2D union = node.Boxes[idx].Union(box);
                    double increase = union.Surface - node.Boxes[idx].Surface; // calculates the increase.
                    if (bestIncrease > increase)
                    {
                        // the increase for this child is smaller.
                        bestIncrease = increase;
                        bestChild = children[idx];
                        bestBox = node.Boxes[idx];
                    }
                    else if (bestBox != null &&
                             bestIncrease == increase)
                    {
                        // the increase is indentical, choose the smalles child.
                        if (node.Boxes[idx].Surface < bestBox.Surface)
                        {
                            bestChild = children[idx];
                            bestBox = node.Boxes[idx];
                        }
                    }
                }
                if (bestChild == null)
                {
                    throw new Exception("Finding best child failed!");
                }
                node = bestChild;
            }
            return node;
        }

        /// <summary>
        /// Sets all the parent properties of the children of the given node.
        /// </summary>
        /// <param name="node"></param>
        private static void SetParents(Node node)
        {
            if (node.Children is List<Node>)
            {
                var children = (node.Children as List<Node>);
                for (int idx = 0; idx < node.Boxes.Count; idx++)
                {
                    children[idx].Parent = node;
                }
            }
        }

        /// <summary>
        /// Splits the given node in two other nodes.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="minimumSize"></param>
        /// <returns></returns>
        private static Node[] SplitNode(Node node, int minimumSize)
        {
            bool leaf = (node.Children is List<T>);

            // create the target nodes.
            var nodes = new Node[2];
            nodes[0] = new Node();
			nodes[0].Boxes = new List<BoxF2D>();
            if (leaf)
            {
                nodes[0].Children = new List<T>();
            }
            else
            {
                nodes[0].Children = new List<Node>();
            }
            nodes[1] = new Node();
			nodes[1].Boxes = new List<BoxF2D>();
            if (leaf)
            {
                nodes[1].Children = new List<T>();
            }
            else
            {
                nodes[1].Children = new List<Node>();
            }

            // select the seed boxes.
            int[] seeds = RTreeMemoryIndex<T>.SelectSeeds(node.Boxes);

            // add the boxes.
            nodes[0].Boxes.Add(node.Boxes[seeds[0]]);
            nodes[1].Boxes.Add(node.Boxes[seeds[1]]);
            nodes[0].Children.Add(node.Children[seeds[0]]);
            nodes[1].Children.Add(node.Children[seeds[1]]);

            // create the boxes.
			var boxes = new BoxF2D[2]
                            {
                                node.Boxes[seeds[0]], node.Boxes[seeds[1]]
                            };
            node.Boxes.RemoveAt(seeds[0]); // seeds[1] is always < seeds[0].
            node.Boxes.RemoveAt(seeds[1]);
            node.Children.RemoveAt(seeds[0]);
            node.Children.RemoveAt(seeds[1]);

            while (node.Boxes.Count > 0)
            {
                // check if one of them needs em all!
                if (nodes[0].Boxes.Count + node.Boxes.Count == minimumSize)
                { // all remaining boxes need te be assigned here.
                    for (int idx = 0; node.Boxes.Count > 0; idx++)
                    {
                        boxes[0] = boxes[0].Union(node.Boxes[0]);
                        nodes[0].Boxes.Add(node.Boxes[0]);
                        nodes[0].Children.Add(node.Children[0]);

                        node.Boxes.RemoveAt(0);
                        node.Children.RemoveAt(0);
                    }
                }
                else if (nodes[1].Boxes.Count + node.Boxes.Count == minimumSize)
                { // all remaining boxes need te be assigned here.
                    for (int idx = 0; node.Boxes.Count > 0; idx++)
                    {
                        boxes[1] = boxes[1].Union(node.Boxes[0]);
                        nodes[1].Boxes.Add(node.Boxes[0]);
                        nodes[1].Children.Add(node.Children[0]);

                        node.Boxes.RemoveAt(0);
                        node.Children.RemoveAt(0);
                    }
                }
                else
                { // choose one of the leaves.
                    int leafIdx;
                    int nextId = RTreeMemoryIndex<T>.PickNext(boxes, node.Boxes, out leafIdx);

                    boxes[leafIdx] = boxes[leafIdx].Union(node.Boxes[nextId]);

                    nodes[leafIdx].Boxes.Add(node.Boxes[nextId]);
                    nodes[leafIdx].Children.Add(node.Children[nextId]);

                    node.Boxes.RemoveAt(nextId);
                    node.Children.RemoveAt(nextId);
                }
            }

            RTreeMemoryIndex<T>.SetParents(nodes[0]);
            RTreeMemoryIndex<T>.SetParents(nodes[1]);

            return nodes;
        }

        /// <summary>
        /// Picks the next best box to add to one of the given nodes.
        /// </summary>
        /// <param name="nodeBoxes"></param>
        /// <param name="boxes"></param>
        /// <param name="nodeBoxIndex"></param>
        /// <returns></returns>
		protected static int PickNext(BoxF2D[] nodeBoxes, IList<BoxF2D> boxes, out int nodeBoxIndex)
        {
            double difference = double.MinValue;
            nodeBoxIndex = 0;
            int pickedIdx = -1;
            for (int idx = 0; idx < boxes.Count; idx++)
            {
				BoxF2D item = boxes[idx];
                double d1 = item.Union(nodeBoxes[0]).Surface -
                            item.Surface;
                double d2 = item.Union(nodeBoxes[1]).Surface -
                            item.Surface;

                double localDifference = System.Math.Abs(d1 - d2);
                if (difference < localDifference)
                {
                    difference = localDifference;
                    if (d1 == d2)
                    {
                        nodeBoxIndex = (nodeBoxes[0].Surface < nodeBoxes[1].Surface) ? 0 : 1;
                    }
                    else
                    {
                        nodeBoxIndex = (d1 < d2) ? 0 : 1;
                    }
                    pickedIdx = idx;
                }
            }
            return pickedIdx;
        }

        /// <summary>
        /// Selects the best seed boxes to start splitting a node.
        /// </summary>
        /// <param name="boxes"></param>
        /// <returns></returns>
		private static int[] SelectSeeds(List<BoxF2D> boxes)
        {
            if (boxes == null) throw new ArgumentNullException("boxes");
            if (boxes.Count < 2) throw new ArgumentException("Cannot select seeds from a list with less than two items.");

            // the Quadratic Split version: selecting the two items that waste the most space
            // if put together.

            var seeds = new int[2];
            double loss = double.MinValue;
            for (int idx1 = 0; idx1 < boxes.Count; idx1++)
            {
                for (int idx2 = 0; idx2 < idx1; idx2++)
                {
                    double localLoss = System.Math.Max(boxes[idx1].Union(boxes[idx2]).Surface -
                        boxes[idx1].Surface - boxes[idx2].Surface, 0);
                    if (localLoss > loss)
                    {
                        loss = localLoss;
                        seeds[0] = idx1;
                        seeds[1] = idx2;
                    }
                }
            }

            return seeds;
        }

        #endregion

        #endregion

        /// <summary>
        /// Returns an enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new RTreeMemoryIndexEnumerator(_root);
        }

        /// <summary>
        /// Returns a enumerator.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new RTreeMemoryIndexEnumerator(_root);
        }

        /// <summary>
        /// Enumerates everything in this index.
        /// </summary>
        internal class RTreeMemoryIndexEnumerator : IEnumerator<T>
        {
            /// <summary>
            /// Holds the root node.
            /// </summary>
            private Node _root;

            /// <summary>
            /// Holds the current position.
            /// </summary>
            private NodePosition _current;

            /// <summary>
            /// Creates a new enumerator.
            /// </summary>
            /// <param name="root"></param>
            public RTreeMemoryIndexEnumerator(Node root)
            {
                _root = root;
            }

            /// <summary>
            /// Returns the current node.
            /// </summary>
            public T Current
            {
                get { return (T)_current.Node.Children[_current.NodeIdx]; }
            }

            /// <summary>
            /// Diposes all resource associtated with this enumerator.
            /// </summary>
            public void Dispose()
            {
                _root = null;
                _current = null;
            }

            /// <summary>
            /// Returns the current object.
            /// </summary>
            object IEnumerator.Current
            {
                get { return this.Current; }
            }

            /// <summary>
            /// Move next.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                NodePosition position = null;
                if (_current == null)
                { // start with the root.
                    _current = new NodePosition() { Node = _root, Parent = null, NodeIdx = -1 }; 
                }
                position = MoveNextFrom(_current);

                _current = position;
                return _current != null;
            }

            /// <summary>
            /// Move to the next position from the given position.
            /// </summary>
            /// <param name="position"></param>
            /// <returns></returns>
            private static NodePosition MoveNextFrom(NodePosition position)
            {
                position.NodeIdx++; // move to the next position.
                while (position.Node.Children == null ||
                    position.Node.Children.Count <= position.NodeIdx ||
                    position.Node.Children[position.NodeIdx] is Node)
                { // there is a need to move to the next object because the current one does not exist.
                    if (position.Node.Children != null && 
                        position.Node.Children.Count > position.NodeIdx &&
                        position.Node.Children[position.NodeIdx] is Node)
                    { // the current child is not of type T move to the child of the child.
                        NodePosition nextPosition = new NodePosition();
                        nextPosition.Parent = position;
                        nextPosition.NodeIdx = -1;
                        nextPosition.Node = position.Node.Children[position.NodeIdx] as Node;

                        position = nextPosition;
                    }
                    else
                    { // there are no children or the children are finished, move to the parent.
                        position = position.Parent;
                    }

                    if (position == null)
                    { // the position is null, no more next position.
                        break;
                    }
                    position.NodeIdx++; // move to the next position.
                }
                return position;
            }

            /// <summary>
            /// Reset this enumerator.
            /// </summary>
            public void Reset()
            {
                _current = null;
            }

            private class NodePosition
            {
                /// <summary>
                /// Gets/sets the parent.
                /// </summary>
                public NodePosition Parent { get; set; }

                /// <summary>
                /// Gets/sets the node.
                /// </summary>
                public Node Node { get; set; }

                /// <summary>
                /// Gets/sets the current node index.
                /// </summary>
                public int NodeIdx { get; set; }
            }
        }
    }
}