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
    /// Represents a relation.
    /// </summary>
    public partial class Relation : OsmGeo
    {
        /// <summary>
        /// Creates new relation.
        /// </summary>
        public Relation()
        {
            this.Type = OsmGeoType.Relation;
        }

        /// <summary>
        /// The relation members.
        /// </summary>
        public RelationMember[] Members { get; set; }

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        public override string ToString()
        {
            var tags = "{no tags}";
            if (this.Tags != null && this.Tags.Count > 0)
            {
                tags = this.Tags.ToString();
            }
            if (!this.Id.HasValue)
            {
                return string.Format("Relation[null]{0}", tags);
            }
            return string.Format("Relation[{0}]{1}", this.Id.Value, tags);
        }
    }
    
    /// <summary>
    /// Represents a relation member.
    /// </summary>
    public partial class RelationMember
    {
        /// <summary>
        /// Creates a new relation member.
        /// </summary>
        public RelationMember()
        {

        }

        /// <summary>
        /// Creates a new relation member.
        /// </summary>
        public RelationMember(long id, string role, OsmGeoType memberType)
        {
            this.Type = memberType;
            this.Id = id;
            this.Role = role;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public OsmGeoType Type { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public string Role { get; set; }
    }
}