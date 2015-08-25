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
using OsmSharp.Osm;

namespace OsmSharp.Osm.Filters
{
    /// <summary>
    /// Filter that combines two other filters.
    /// </summary>
    internal class FilterCombined : Filter
    {
        /// <summary>
        /// Holds the left filter;
        /// </summary>
        private Filter _filter1;

        /// <summary>
        /// Holds the combine operator enum.
        /// </summary>
        private FilterCombineOperatorEnum _op;

        /// <summary>
        /// Holds the right filter.
        /// </summary>
        private Filter _filter2;

        /// <summary>
        /// Creates a new combined filter.
        /// </summary>
        /// <param name="filter1"></param>
        /// <param name="op"></param>
        /// <param name="filter2"></param>
        public FilterCombined(Filter filter1, FilterCombineOperatorEnum op, Filter filter2)
        {
            _op = op;
            _filter1 = filter1;
            _filter2 = filter2;
        }

        /// <summary>
        /// Returns true if this object pass through this filter.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Evaluate(OsmGeo obj)
        {
            switch (_op)
            {
                case FilterCombineOperatorEnum.And:
                    return _filter1.Evaluate(obj) && _filter2.Evaluate(obj);
                case FilterCombineOperatorEnum.Or:
                    return _filter1.Evaluate(obj) || _filter2.Evaluate(obj);
                case FilterCombineOperatorEnum.Not:
                    return !_filter1.Evaluate(obj);
            }
            return false;
        }

        /// <summary>
        /// Returns a description of this filter.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_op == FilterCombineOperatorEnum.Not)
            {
                return string.Format("(not {0})",
                                 _filter1.ToString());
            }
            return string.Format("({0} {1} {2})",
                                 _filter1.ToString(),
                                 _op.ToString(),
                                 _filter2.ToString());

        }
    }

    /// <summary>
    /// Possible filter combinations.
    /// </summary>
    internal enum FilterCombineOperatorEnum
    {
        And,
        Or,
        Not
    }
}
