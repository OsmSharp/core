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

namespace OsmSharp.Math.Geo.Meta
{
    /// <summary>
    /// Relative direction calculator.
    /// </summary>
    public static class RelativeDirectionCalculator
    {
        /// <summary>
        /// Calculates the relative direction.
        /// </summary>
        /// <param name="from"></param>
        /// <param name="along"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static RelativeDirection Calculate(GeoCoordinate from, GeoCoordinate along, GeoCoordinate to)
        {
            var direction = new RelativeDirection();

            var margin = 65.0;
            var straight_on = 10.0;
            var turn_back = 5.0;

            var lineFrom = new GeoCoordinateLine(from, along);
            var lineTo = new GeoCoordinateLine(along, to);

            var angle = lineFrom.Direction.Angle(lineTo.Direction);

            if (angle >= new Degree(360 - straight_on)
                || angle < new Degree(straight_on))
            {
                direction.Direction = RelativeDirectionEnum.StraightOn;
            }
            else if (angle >= new Degree(straight_on)
                && angle < new Degree(90 - margin))
            {
                direction.Direction = RelativeDirectionEnum.SlightlyLeft;
            }
            else if (angle >= new Degree(90 - margin)
                && angle < new Degree(90 + margin))
            {
                direction.Direction = RelativeDirectionEnum.Left;
            }
            else if (angle >= new Degree(90 + margin)
                && angle < new Degree(180 - turn_back))
            {
                direction.Direction = RelativeDirectionEnum.SharpLeft;
            }
            else if (angle >= new Degree(180 - turn_back)
                && angle < new Degree(180 + turn_back))
            {
                direction.Direction = RelativeDirectionEnum.TurnBack;
            }
            else if (angle >= new Degree(180 + turn_back)
                && angle < new Degree(270-margin))
            {
                direction.Direction = RelativeDirectionEnum.SharpRight;
            }
            else if (angle >= new Degree(270 - margin)
                && angle < new Degree(270 + margin))
            {
                direction.Direction = RelativeDirectionEnum.Right;
            }
            else if (angle >= new Degree(270 + margin)
                && angle < new Degree(360- straight_on))
            {
                direction.Direction = RelativeDirectionEnum.SlightlyRight;
            }
            direction.Angle = angle;

            return direction;
        }
    }
}