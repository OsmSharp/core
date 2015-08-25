// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
//                    Scheinpflug Tommy
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
using System.Text.RegularExpressions;
using System.Globalization;
using OsmSharp.Collections.Tags;
using OsmSharp.Units.Speed;
using OsmSharp.Units.Weight;
using OsmSharp.Units.Distance;

namespace OsmSharp.Osm
{
    /// <summary>
    /// Contains extensions that aid in interpreting some of the OSM-tags.
    /// </summary>
    public static class TagExtensions
    {
        private static string[] BooleanTrueValues = { "yes", "true", "1" };
        private static string[] BooleanFalseValues = { "no", "false", "0" };

        private const string REGEX_DECIMAL = @"\s*(\d+(?:\.\d*)?)\s*";

        private const string REGEX_UNIT_TONNES = @"\s*(t|to|tonnes|tonnen)?\s*";
        private const string REGEX_UNIT_METERS = @"\s*(m|meters|metres|meter)?\s*";
        private const string REGEX_UNIT_KILOMETERS = @"\s*(km)?\s*";
        //private const string REGEX_UNIT_KILOMETERS_PER_HOUR = @"\s*(kmh|km/h|kph)?\s*";
        //private const string REGEX_UNIT_MILES_PER_HOUR = @"\s*(mph)?\s*";

        /// <summary>
        /// Returns true if the given tags key has an associated value that can be interpreted as true.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="tagKey"></param>
        /// <returns></returns>
        public static bool IsTrue(this TagsCollectionBase tags, string tagKey)
        {
            if (tags == null || string.IsNullOrWhiteSpace(tagKey))
                return false;

            // TryGetValue tests if the 'tagKey' is present, returns true if the associated value can be interpreted as true.
            //                                               returns false if the associated value can be interpreted as false.
            string tagValue;
            return tags.TryGetValue(tagKey, out tagValue) && 
                BooleanTrueValues.Contains(tagValue.ToLowerInvariant());
        }

        /// <summary>
        /// Returns true if the given tags key has an associated value that can be interpreted as false.
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="tagKey"></param>
        /// <returns></returns>
        public static bool IsFalse(this TagsCollectionBase tags, string tagKey)
        {
            if (tags == null || string.IsNullOrWhiteSpace(tagKey))
                return false;
            string tagValue;
            return tags.TryGetValue(tagKey, out tagValue) &&
                BooleanFalseValues.Contains(tagValue.ToLowerInvariant());
        }

        /// <summary>
        /// Searches for the tags collection for the <c>Access</c>-Tags and returns the associated values.
        /// 
        /// http://wiki.openstreetmap.org/wiki/Key:access
        /// </summary>
        /// <param name="tags">The tags to search.</param>
        /// <param name="accessTagHierachy">The hierarchy of <c>Access</c>-Tags for different vehicle types.</param>
        /// <returns>The best fitting value is returned.</returns>
        public static string GetAccessTag(this TagsCollectionBase tags, IEnumerable<string> accessTagHierachy)
        {
            if (tags == null)
                return null;
            foreach (string s in accessTagHierachy)
            {
                string access;
                if (tags.TryGetValue(s, out access))
                    return access;
            }
            return null;
        }

        #region Reading Tags

        /// <summary>
        /// Searches for a maxspeed tag and returns the associated value.
        /// 
        ///  http://wiki.openstreetmap.org/wiki/Key:maxspeed
        /// </summary>
        /// <param name="tags">The tags to search.</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetMaxSpeed(this TagsCollectionBase tags, out KilometerPerHour result)
        {
            result = double.MaxValue;
            string tagValue;
            if (tags == null || !tags.TryGetValue("maxspeed", out tagValue) || string.IsNullOrWhiteSpace(tagValue) ||
                tagValue == "none" || tagValue == "signals" || tagValue == "signs" || tagValue == "no")
                return false;
            return TagExtensions.TryParseSpeed(tagValue, out result);
        }

        /// <summary>
        /// Searches for a maxweight tag and returns the associated value.
        /// 
        ///  http://wiki.openstreetmap.org/wiki/Key:maxweight
        /// </summary>
        /// <param name="tags">The tags to search.</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetMaxWeight(this TagsCollectionBase tags, out Kilogram result)
        {
            result = double.MaxValue;
            string tagValue;
            if (tags == null || !tags.TryGetValue("maxweight", out tagValue) || string.IsNullOrWhiteSpace(tagValue))
                return false;
            return TagExtensions.TryParseWeight(tagValue, out result);
        }

        /// <summary>
        /// Searches for a max axle load tag and returns the associated value.
        /// 
        /// http://wiki.openstreetmap.org/wiki/Key:maxaxleload
        /// </summary>
        /// <param name="tags">The tags to search.</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetMaxAxleLoad(this TagsCollectionBase tags, out Kilogram result)
        {
            result = double.MaxValue;
            string tagValue;
            if (tags == null || !tags.TryGetValue("maxaxleload", out tagValue) || string.IsNullOrWhiteSpace(tagValue))
                return false;
            return TagExtensions.TryParseWeight(tagValue, out result);
        }

        /// <summary>
        /// Searches for a max height tag and returns the associated value.
        /// 
        /// http://wiki.openstreetmap.org/wiki/Maxheight
        /// </summary>
        /// <param name="tags">The tags to search.</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetMaxHeight(this TagsCollectionBase tags, out Meter result)
        {
            result = double.MaxValue;

            string tagValue;
            if (tags == null || !tags.TryGetValue("maxheight", out tagValue) || string.IsNullOrWhiteSpace(tagValue))
                return false;

            return TagExtensions.TryParseLength(tagValue, out result);
        }

        /// <summary>
        /// Searches for a max width tag and returns the associated value.
        /// 
        /// http://wiki.openstreetmap.org/wiki/Key:maxwidth
        /// </summary>
        /// <param name="tags">The tags to search.</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetMaxWidth(this TagsCollectionBase tags, out Meter result)
        {
            result = double.MaxValue;
            string tagValue;
            if (tags == null || !tags.TryGetValue("maxwidth", out tagValue) || string.IsNullOrWhiteSpace(tagValue))
                return false;
            return TagExtensions.TryParseLength(tagValue, out result);
        }

        /// <summary>
        /// Searches for a max length tag and returns the associated value.
        /// 
        /// http://wiki.openstreetmap.org/wiki/Key:maxlength
        /// </summary>
        /// <param name="tags">The tags to search.</param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryGetMaxLength(this IDictionary<string, string> tags, out Meter result)
        {
            result = double.MaxValue;

            string tagValue;
            if (tags == null || !tags.TryGetValue("maxlength", out tagValue) || string.IsNullOrWhiteSpace(tagValue))
                return false;

            return TagExtensions.TryParseLength(tagValue, out result);
        }

        #endregion

        #region Parsing Units

        /// <summary>
        /// Tries to parse a speed value from a given tag-value.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseSpeed(string s, out KilometerPerHour result)
        {
            result = double.MaxValue;

            if (string.IsNullOrWhiteSpace(s))
                return false;

            if (s[0] != '0' && s[0] != '1' && s[0] != '2' && s[0] != '3' && s[0] != '4' &&
                s[0] != '5' && s[0] != '6' && s[0] != '7' && s[0] != '8' && s[0] != '9')
            { // performance inprovement, quick negative answer.
                return false;
            }

            if(s.Contains(","))
            { // refuse comma as a decimal seperator or anywhere else in the number.
                return false;
            }

            // try regular speed: convention in OSM is km/h in this case.
            double kmphDouble;
            if (double.TryParse(s, NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out kmphDouble))
            {
                result = kmphDouble;
            }

            // try km/h
            if (KilometerPerHour.TryParse(s, out result))
            {
                return true;
            }

            // try mph.
            MilesPerHour resultMph;
            if(MilesPerHour.TryParse(s, out resultMph))
            {
                result = resultMph;
                return true;
            }

            // try knots.
            Knots resultKnots;
            if (Knots.TryParse(s, out resultKnots))
            {
                result = resultKnots;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to parse a weight value from a given tag-value.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseWeight(string s, out Kilogram result)
        {
            result = double.MaxValue;

            if (string.IsNullOrWhiteSpace(s))
                return false;

            Regex tonnesRegex = new Regex("^" + REGEX_DECIMAL + REGEX_UNIT_TONNES + "$", RegexOptions.IgnoreCase);
            Match tonnesMatch = tonnesRegex.Match(s);
            if (tonnesMatch.Success)
            {
                result = (Kilogram)(double.Parse(tonnesMatch.Groups[1].Value, CultureInfo.InvariantCulture) * 1000);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to parse a distance measure from a given tag-value.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static bool TryParseLength(string s, out Meter result)
        {
            result = double.MaxValue;

            if (string.IsNullOrWhiteSpace(s))
                return false;

            Regex metresRegex = new Regex("^" + REGEX_DECIMAL + REGEX_UNIT_METERS + "$", RegexOptions.IgnoreCase);
            Match metresMatch = metresRegex.Match(s);
            if (metresMatch.Success)
            {
                result = double.Parse(metresMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                return true;
            }

            Regex feetInchesRegex = new Regex("^(\\d+)\\'(\\d+)\\\"$", RegexOptions.IgnoreCase);
            Match feetInchesMatch = feetInchesRegex.Match(s);
            if (feetInchesMatch.Success)
            {
                int feet = int.Parse(feetInchesMatch.Groups[1].Value, CultureInfo.InvariantCulture);
                int inches = int.Parse(feetInchesMatch.Groups[2].Value, CultureInfo.InvariantCulture);

                result = feet * 0.3048 + inches * 0.0254;
                return true;
            }

            return false;
        }

        #endregion

    }
}
