// The MIT License (MIT)

// Copyright (c) 2016 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace OsmSharp
{
    /// <summary>
    /// Contains general utilities.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Returns true if the given coordinate is inside the box.
        /// </summary>
        public static bool IsInside(float boxLat1, float boxLon1, float boxLat2, float boxLon2, 
            float lat, float lon)
        {
            if (boxLat1 > boxLat2)
            {
                var t = boxLat1;
                boxLat1 = boxLat2;
                boxLat2 = t;
            }
            if (boxLon1 > boxLon2)
            {
                var t = boxLon1;
                boxLon1 = boxLon2;
                boxLon2 = t;
            }

            return boxLat1 <= lat && lat <= boxLat2 && boxLon1 <= lon && lon <= boxLon2; 
        }
    }
}