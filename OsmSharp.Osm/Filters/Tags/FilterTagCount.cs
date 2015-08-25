//// OsmSharp - OpenStreetMap (OSM) SDK
//// Copyright (C) 2013 Abelshausen Ben
//// 
//// This file is part of OsmSharp.
//// 
//// OsmSharp is free software: you can redistribute it and/or modify
//// it under the terms of the GNU General Public License as published by
//// the Free Software Foundation, either version 2 of the License, or
//// (at your option) any later version.
//// 
//// OsmSharp is distributed in the hope that it will be useful,
//// but WITHOUT ANY WARRANTY; without even the implied warranty of
//// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//// GNU General Public License for more details.
//// 
//// You should have received a copy of the GNU General Public License
//// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace OsmSharp.Osm.Filters.Tags
//{
//    /// <summary>
//    /// Filters on the number of
//    /// </summary>
//    internal class FilterTagCount : FilterTag
//    {
//        private int _cnt;

//        private bool _exact;

//        public FilterTagCount()
//        {
//            _cnt = 0;
//            _exact = false;
//        }

//        public FilterTagCount(int cnt, bool exact)
//        {
//            _cnt = cnt;
//            _exact = exact;
//        }

//        public FilterTagCount(int cnt)
//        {
//            _cnt = cnt;
//        }
        
//        public override bool Evaluate(OsmBase obj)
//        {
//            if (_exact)
//            {
//                return obj.Tags.Count == _cnt;
//            }
//            return obj.Tags.Count >= _cnt;
//        }
//    }
//}
