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
//using OsmSharp.Math.Geo;

//namespace OsmSharp.Osm.Filters
//{
//    internal class FilterBox : Filter
//    {
//        private GeoCoordinateBox _box;

//        public FilterBox(GeoCoordinateBox box)
//        {
//            _box = box;
//        }

//        public override bool Evaluate(OsmBase obj)
//        {
//            if(obj.BoundingBox.Overlaps(_box))
//            {
//                if (obj.Type == OsmType.ChangeSet)
//                {
//                    foreach(OsmGeo geo in (obj as ChangeSet).Objects)
//                    {
//                        if(geo.Shape.Inside(_box))
//                        {
//                            return true;
//                        }
//                    }
//                    return false;
//                }
//                else 
//                {
//                    if ((obj as OsmGeo).Shape.Inside(_box))
//                    {
//                        return true;
//                    }
//                }
//            }
//            return false;
//        }
//    }
//}
