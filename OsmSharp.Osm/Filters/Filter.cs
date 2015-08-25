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
using OsmSharp.Osm.Filters.Tags;
using OsmSharp.Osm;

namespace OsmSharp.Osm.Filters
{
    /// <summary>
    /// A basic class defining a gilter on OsmBase object.
    /// </summary>
    public abstract class Filter
    {
        /// <summary>
        /// Evaluates the filter against the osm object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Evaluate(CompleteOsmGeo obj)
        {
            return this.Evaluate(obj.ToSimple());
        }

        /// <summary>
        /// Evaluates the filter against the osm object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract bool Evaluate(OsmGeo obj);

        /// <summary>
        /// Returns description of this filter.
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();

        #region Operators

        /// <summary>
        /// Combines two filters into a new filter by using and.
        /// </summary>
        /// <param name="filter1"></param>
        /// <param name="filter2"></param>
        /// <returns></returns>
        public static Filter operator &(Filter filter1, Filter filter2)
        {
            return new FilterCombined(filter1, FilterCombineOperatorEnum.And, filter2);
        }

        /// <summary>
        /// Combines two filters into a new filter by using or.
        /// </summary>
        /// <param name="filter1"></param>
        /// <param name="filter2"></param>
        /// <returns></returns>
        public static Filter operator |(Filter filter1, Filter filter2)
        {
            return new FilterCombined(filter1, FilterCombineOperatorEnum.Or, filter2);
        }

        /// <summary>
        /// Negates a filter into a new filter by using not.
        /// </summary>
        /// <param name="filter1"></param>
        /// <returns></returns>
        public static Filter operator !(Filter filter1)
        {
            return new FilterCombined(filter1, FilterCombineOperatorEnum.Not, null);
        }

        #endregion

        #region Tag Filter Creation

        /// <summary>
        /// Returns a filter that filters on type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Filter Type(OsmGeoType type)
        {
            return new FilterType(type);
        }

        /// <summary>
        /// Returns a filter that matches any kind of tags or objects.
        /// </summary>
        /// <returns></returns>
        public static Filter Any()
        {
            return new FilterAny();
        }

        ///// <summary>
        ///// Returns a filter that matches objects with at least the number of tags given.
        ///// </summary>
        ///// <returns></returns>
        //public static Filter MoreThan(int count)
        //{
        //    return new FilterTagCount(count);
        //}

        ///// <summary>
        ///// Returns a filter that matches objects with the exact number of tags given.
        ///// </summary>
        ///// <returns></returns>
        //public static Filter Exact(int count)
        //{
        //    return new FilterTagCount(count, true);
        //}

        /// <summary>
        /// Returns a filter that matches one tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Filter Match(string tag, string value)
        {
            return new FilterTagMatch(tag, value);
        }

        /// <summary>
        /// Returns a filter that matches a tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Filter MatchAny(string tag, string[] value)
        {
            if (value != null && value.Length > 0)
            {
                Filter filter = new FilterTagMatch(tag, value[0]);
                for (int idx = 1; idx < value.Length; idx++)
                {
                    filter = filter | new FilterTagMatch(tag, value[idx]);
                }
                return filter;
            }
            return null;
        }

        /// <summary>
        /// Returns a filter for the existence of a tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static Filter Exists(string tag)
        {
            return new FilterTagExists(tag);
        }

        /// <summary>
        /// Returns a filter for the existence of any of the given tags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static Filter Exists(string[] tags)
        {
            if (tags != null && tags.Length > 0)
            {
                Filter filter = new FilterTagExists(tags[0]);
                for (int idx = 1; idx < tags.Length; idx++)
                {
                    filter = filter | new FilterTagExists(tags[idx]);
                }
                return filter;
            }
            return null;
        }


        #endregion
    }
}
