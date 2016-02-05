using System;

namespace OsmSharp.IO.Json
{
    /// <summary>
    /// Allows users to control class loading and mandate what class to load.
    /// </summary>
    public abstract class SerializationBinder
    {
        /// <summary>
        /// When overridden in a derived class, controls the binding of a serialized object to a type.
        /// </summary>
        public abstract Type BindToType(string assemblyName, string typeName);

        /// <summary>
        /// When overridden in a derived class, controls the binding of a serialized object to a type.
        /// </summary>
        public virtual void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = null;
        }
    }
}