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
using OsmSharp.Math.Primitives;

namespace OsmSharp.Math.Structures.KDTree
{
    /// <summary>
    /// Represents a node in a k-d tree.
    /// </summary>
    internal class Tree2DNode<PointType>
        where PointType : PointF2D
    {
        /// <summary>
        /// The point this node splits on.
        /// </summary>
        private PointType _value;

        /// <summary>
        /// The dimension this node splits on.
        /// </summary>
        private int _dimension;

        /// <summary>
        /// The lesser node.
        /// </summary>
        private Tree2DNode<PointType> _lesser;

        /// <summary>
        /// The bigger node.
        /// </summary>
        private Tree2DNode<PointType> _bigger;

        /// <summary>
        /// Keeps the distance calculation delegate.
        /// </summary>
        private Tree2D<PointType>.Distance _distance_delegate;

        /// <summary>
        /// Creates a 2D tree.
        /// </summary>
        /// <param name="distance_delegate"></param>
        /// <param name="value"></param>
        /// <param name="dimension"></param>
        public Tree2DNode(Tree2D<PointType>.Distance distance_delegate, PointType value, int dimension)
        {
            // set the distance delegate.
            _distance_delegate = distance_delegate;

            _value = value;
            _dimension = dimension;
        }

        /// <summary>
        /// Creates a 2D tree in a balanced way.
        /// </summary>
        /// <param name="distance_delegate"></param>
        /// <param name="sorted_points"></param>
        /// <param name="dimension"></param>
        public Tree2DNode(Tree2D<PointType>.Distance distance_delegate, List<PointType>[] sorted_points, int dimension)
        {
            // set the distance delegate.
            _distance_delegate = distance_delegate;

            // set the dimension.
            _dimension = dimension;

            // get the sorted list in question.
            List<PointType> points = sorted_points[_dimension];

            // get the middle point.
            int middle = points.Count / 2;

            // create this point.
            _value = points[middle];

            // split the list.
            List<PointType>[] lesser_points = new List<PointType>[2];
            List<PointType>[] bigger_points = new List<PointType>[2];
            lesser_points[_dimension] = new List<PointType>(points.GetRange(
                0, middle - 1));
            bigger_points[_dimension] = new List<PointType>(points.GetRange(
                middle + 1, points.Count - (middle + 1)));

            // calculate the other dimension.
            int other_dimension =
                (_dimension + 1) % 2;

            // remove the points from the other dimension lists.
            lesser_points[other_dimension] = new List<PointType>(
                sorted_points[other_dimension].Except<PointType>(bigger_points[_dimension]));
            lesser_points[other_dimension].Remove(_value);
            bigger_points[other_dimension] = new List<PointType>(
                sorted_points[other_dimension].Except<PointType>(lesser_points[_dimension]));
            bigger_points[other_dimension].Remove(_value);

            // create the other nodes.
            if (lesser_points[other_dimension].Count == 1)
            {
                _lesser = new Tree2DNode<PointType>(_distance_delegate,
                    lesser_points[other_dimension][0], other_dimension);
            }
            else if (lesser_points[other_dimension].Count > 1)
            {
                _lesser = new Tree2DNode<PointType>(_distance_delegate,
                    lesser_points, other_dimension);
            }
            if (bigger_points[other_dimension].Count == 1)
            {
                _bigger = new Tree2DNode<PointType>(_distance_delegate,
                    bigger_points[other_dimension][0], other_dimension);
            }
            else if (bigger_points[other_dimension].Count > 1)
            {
                _bigger = new Tree2DNode<PointType>(_distance_delegate,
                    bigger_points, other_dimension);
            }
        }

        /// <summary>
        /// Adds a point to the tree.
        /// </summary>
        /// <param name="value"></param>
        public void Add(PointType value)
        {
            double value_dimension = value[_dimension];
            if (value_dimension < _value[_dimension])
            { // add to the lesser side.
                if (_lesser == null)
                {
                    _lesser = new Tree2DNode<PointType>(_distance_delegate, value, _dimension + 1 % 2);
                }
                else
                {
                    _lesser.Add(value);
                }
            }
            else
            { // add to the bigger side.
                if (_bigger == null)
                {
                    _bigger = new Tree2DNode<PointType>(_distance_delegate, value, _dimension + 1 % 2);
                }
                else
                {
                    _bigger.Add(value);
                }
            }
        }

        /// <summary>
        /// Returns the nearest neighbours for the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public PointType SearchNearestNeighbour(PointType point, ICollection<PointType> exceptions)
        {
            // keeps the result.
            PointType result = default(PointType);

            // decide where to get the result from.
            double value_dimension = point[_dimension];
            bool lesser = true;
            if (value_dimension < _value[_dimension])
            { // get from the lesser side.
                if (_lesser == null)
                {
                    if (exceptions == null
                        || !exceptions.Contains(_value))
                    {
                        result = _value;
                    }
                }
                else
                {
                    result = _lesser.SearchNearestNeighbour(point, exceptions);
                }
            }
            else
            { // get from the bigger side.
                lesser = false;
                if (_bigger == null)
                {
                    if (exceptions == null
                        || !exceptions.Contains(_value))
                    {
                        result = _value;
                    }
                }
                else
                {
                    result = _bigger.SearchNearestNeighbour(point, exceptions);
                }
            }

            // do we need to search the other side.
            double distance = double.MaxValue;
            if (result != null)
            {
                _distance_delegate.Invoke(result, point);
            }
            
            // check if the current point is closer.
            double distance_this = _distance_delegate.Invoke(_value, point);
            if(distance > distance_this)
            {
                result = _value;
                distance = distance_this;
            }

            // test if the other side needs to be tested.
            double distance_to_other_side = 0;
            if (result != null)
            {
                System.Math.Abs(result[_dimension] - _value[_dimension]);
            }
            if(distance > distance_to_other_side)
            {
                PointType other_result = default(PointType);
                if(lesser)
                {
                    if (_bigger != null)
                    {
                        other_result = _bigger.SearchNearestNeighbour(point, exceptions);
                    }
                }
                else
                {
                    if (_lesser != null)
                    {
                        other_result = _lesser.SearchNearestNeighbour(point, exceptions);
                    }
                }
                if(other_result != null)
                {
                    double distance_other = _distance_delegate.Invoke(other_result, point);
                    if(distance_other < distance)
                    {
                        result = other_result;
                    }
                }
            }

            return result;
        }
    }
}
