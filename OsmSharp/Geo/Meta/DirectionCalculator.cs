// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2015 Abelshausen Ben
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

using OsmSharp.Units.Angle;
using System;

namespace OsmSharp.Math.Geo.Meta
{
    /// <summary>
    /// Direction calculator.
    /// </summary>
    public static class DirectionCalculator
    {
        /// <summary>
        /// Calculates the direction.
        /// </summary>
        /// <returns></returns>
        public static DirectionEnum Calculate(GeoCoordinate from, GeoCoordinate to)
        {
            double offset = 0.01;

            // calculate the angle with the horizontal and vertical axes.
            var verticalFrom = new GeoCoordinate(from.Latitude + offset, from.Longitude);

            // create line.
            var line = new GeoCoordinateLine(from, to);
            var verticalLine = new GeoCoordinateLine(from, verticalFrom);

            // calculate angle.
            var verticalAngle = line.Direction.Angle(verticalLine.Direction);

            if (verticalAngle < new Degree(22.5) 
                || verticalAngle >= new Degree(360- 22.5))
            { // north
                return DirectionEnum.North;
            }
            else if (verticalAngle >= new Degree(22.5)
                 && verticalAngle < new Degree(90 - 22.5))
            { // north-east.
                return DirectionEnum.NorthEast;
            }
            else if (verticalAngle >= new Degree(90 - 22.5)
                 && verticalAngle < new Degree(90 + 22.5))
            { // east.
                return DirectionEnum.East;
            }
            else if (verticalAngle >= new Degree(90 + 22.5)
                 && verticalAngle < new Degree(180 - 22.5))
            { // south-east.
                return DirectionEnum.SouthEast;
            }
            else if (verticalAngle >= new Degree(180 - 22.5)
                 && verticalAngle < new Degree(180 + 22.5))
            { // south
                return DirectionEnum.South;
            }
            else if (verticalAngle >= new Degree(180 + 22.5)
                 && verticalAngle < new Degree(270 - 22.5))
            { // south-west.
                return DirectionEnum.SouthWest;
            }
            else if (verticalAngle >= new Degree(270 - 22.5)
                 && verticalAngle < new Degree(270 + 22.5))
            { // south-west.
                return DirectionEnum.West;
            }
            else if (verticalAngle >= new Degree(270 + 22.5)
                 && verticalAngle < new Degree(360-22.5))
            { // south-west.
                return DirectionEnum.NorthWest;
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}