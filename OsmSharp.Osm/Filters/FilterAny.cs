using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharp.Osm.Filters
{
    /// <summary>
    /// Filter that filters no objects.
    /// </summary>
    internal class FilterAny : Filter
    {
        /// <summary>
        /// Returns true.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Evaluate(OsmGeo obj)
        {
            return true;
        }

        /// <summary>
        /// Returns a description of this filter.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("*");
        }
    }
}
