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

namespace OsmSharp
{
	/// <summary>
	/// An abstract tags source definition.
	/// </summary>
	public interface ITagsSource
	{
		/// <summary>
		/// Tries to get the value for the given key.
		/// </summary>
		/// <returns><c>true</c>, if get value was found, <c>false</c> otherwise.</returns>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		bool TryGetValue(string key, out string value);
	}
}