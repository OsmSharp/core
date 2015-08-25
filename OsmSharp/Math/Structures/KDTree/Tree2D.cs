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
    /// An implementation of a 2-dimensional k-d tree.
    /// </summary>
    public class Tree2D<PointType> 
        where PointType : PointF2D
    {
        /// <summary>
        /// The root node of the tree.
        /// </summary>
        private Tree2DNode<PointType> _root;

        /// <summary>
        /// Delegate to calculate the distance between two points.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public delegate double Distance(PointType x, PointType y);

        /// <summary>
        /// Holds the distance delegate.
        /// </summary>
        private Distance _distance_delegate;

        /// <summary>
        /// Creates a new 2-dimensional k-d tree.
        /// </summary>
        /// <param name="points"></param>
        /// <param name="distance_delegate"></param>
        public Tree2D(IEnumerable<PointType> points,
            Distance distance_delegate)
        {
            // set the distance delegate.
            _distance_delegate = distance_delegate;

            // create the list.
            List<PointType>[] sorted_points = new List<PointType>[2];

            // sort points per dimension.
            for (int dim = 0; dim < 2; dim++)
            {
                // create the points list.
                List<PointType> points_list = new List<PointType>(points);

                // sort the list.
                points_list.Sort(new Comparison<PointType>(delegate(PointType p1, PointType p2)
                {
                    return p1[dim].CompareTo(p2[dim]);
                }));

                // add the list to the array.
                sorted_points[dim] = points_list;
            }
            
            // construct the root.
            _root = new Tree2DNode<PointType>(_distance_delegate, sorted_points, 0);
        }

        /// <summary>
        /// Adds a point to this tree.
        /// </summary>
        /// <param name="point"></param>
        public void Add(PointType point)
        {
            _root.Add(point);
        }

        /// <summary>
        /// Returns the nearest neighbours for the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public PointType SearchNearestNeighbour(PointType point)
        {
            return _root.SearchNearestNeighbour(point, null);
        }

        /// <summary>
        /// Returns the nearest neighbours for the given point but exclude any of the points in the exceptions list.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="exceptions"></param>
        /// <returns></returns>
        public PointType SearchNearestNeighbour(PointType point, ICollection<PointType> exceptions)
        {
            return _root.SearchNearestNeighbour(point, exceptions);
        }
    }
}
