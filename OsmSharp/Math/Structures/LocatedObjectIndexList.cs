using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OsmSharp.Collections;
using OsmSharp.Math.Primitives;

namespace OsmSharp.Math.Structures
{
    /// <summary>
    /// A naive reference implementation of the ILocatedObjectIndex interface.
    /// </summary>
    /// <typeparam name="PointType"></typeparam>
    /// <typeparam name="DataType"></typeparam>
    public class LocatedObjectIndexList<PointType, DataType> : ILocatedObjectIndex<PointType, DataType>
        where PointType : PointF2D
    {
        /// <summary>
        /// Holds a list of data.
        /// </summary>
        private List<KeyValuePair<PointType, DataType>> _data;

        /// <summary>
        /// Creates a new located object(s) index list.
        /// </summary>
        public LocatedObjectIndexList()
        {
            _data = new List<KeyValuePair<PointType, DataType>>();
        }

        /// <summary>
        /// Returns all data inside the given box.
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
		public IEnumerable<DataType> GetInside(BoxF2D box)
        {
            HashSet<DataType> dataset = new HashSet<DataType>();
            foreach (KeyValuePair<PointType, DataType> data in _data)
            {
                if (box.Contains(data.Key))
                {
                    dataset.Add(data.Value);
                }
            }
            return dataset;
        }

        /// <summary>
        /// Adds new located data.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="data"></param>
        public void Add(PointType location, DataType data)
        {
            _data.Add(new KeyValuePair<PointType, DataType>(location, data));
        }

        /// <summary>
        /// Clears all data from this index.
        /// </summary>
        public void Clear()
        {
            _data.Clear();
        }
    }
}
