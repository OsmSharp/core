using System;

namespace OsmSharp
{
    /// <summary>
    /// A unique identifier including types.
    /// </summary>
    public readonly struct OsmGeoKey : IEquatable<OsmGeoKey>, IComparable<OsmGeoKey>
    {
        /// <summary>
        /// Creates a version key.
        /// </summary>
        public OsmGeoKey(OsmGeoType type, long id)
        {
            this.Type = type;
            this.Id = id;
        }

        /// <summary>
        /// Creates a version key for the given object.
        /// </summary>
        public OsmGeoKey(OsmGeo osmGeo)
        {
            if (!osmGeo.Id.HasValue) throw new ArgumentException("Object has no id.", nameof(osmGeo));
            
            this.Type = osmGeo.Type;
            this.Id = osmGeo.Id.Value;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public OsmGeoType Type { get; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public long Id { get; }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^
                this.Type.GetHashCode();
        }

        /// <inheritdoc/>
        public bool Equals(OsmGeoKey other)
        {
            return this.Type == other.Type 
                   && this.Id == other.Id;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is OsmGeo geo && Equals(geo);
        }

        public static bool operator <(OsmGeoKey key1, OsmGeoKey key2)
        {
            return key1.CompareTo(key2) < 0;
        }

        public static bool operator >(OsmGeoKey key1, OsmGeoKey key2)
        {
            return key1.CompareTo(key2) > 0;
        }

        public static bool operator >=(OsmGeoKey key1, OsmGeoKey key2)
        {
            return key1.CompareTo(key2) >= 0;
        }

        public static bool operator <=(OsmGeoKey key1, OsmGeoKey key2)
        {
            return key1.CompareTo(key2) <= 0;
        }

        /// <inheritdoc/>
        public int CompareTo(OsmGeoKey other)
        {
            if (this.Type == other.Type)
            {
                return Id.CompareTo(other.Id);
            }

            switch (this.Type)
            {
                case OsmGeoType.Node:
                case OsmGeoType.Way when other.Type == OsmGeoType.Relation:
                    return -1;
                case OsmGeoType.Relation:
                    break;
                default:
                    break;
            }
            return 1;
        }

        /// <summary>
        /// Returns a description of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{this.Type}[{this.Id}]";
        }
    }
}