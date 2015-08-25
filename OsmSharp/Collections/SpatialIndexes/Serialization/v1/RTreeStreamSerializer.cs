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
//using System.IO;
//using OsmSharp.IO.StreamCache;
//using OsmSharp.Math.Primitives;
//using ProtoBuf;
//using ProtoBuf.Meta;

//namespace OsmSharp.Collections.SpatialIndexes.Serialization.v1
//{
//    /// <summary>
//    /// Serializer for an R-tree spatial index.
//    /// </summary>
//    /// <typeparam name="T"></typeparam>
//    public abstract class RTreeStreamSerializer<T> : SpatialIndexSerializer<T>
//    {
//        /// <summary>
//        /// Holds the stream cache.
//        /// </summary>
//        private readonly IStreamCache _streamCache;

//        /// <summary>
//        /// Creates a new serializer.
//        /// </summary>
//        protected RTreeStreamSerializer()
//        {
//            _streamCache = new MemoryCachedStream();
//        }

//        /// <summary>
//        /// Holds the type model.
//        /// </summary>
//        private RuntimeTypeModel _typeModel;

//        /// <summary>
//        /// Returns the runtime type model.
//        /// </summary>
//        /// <returns></returns>
//        private RuntimeTypeModel GetRuntimeTypeModel()
//        {
//            if (_typeModel == null)
//            {
//                // build the run time type model.
//                _typeModel = TypeModel.Create();
//                //_typeModel.SetDefaultFactory(typeof(Factory).GetMethod("Create"));
//                _typeModel.Add(typeof(ChildrenIndex), true); // the tile metadata.
//                this.BuildRuntimeTypeModel(_typeModel);
//            }
//            return _typeModel;
//        }

//        /// <summary>
//        /// Builds the type model.
//        /// </summary>
//        /// <param name="typeModel"></param>
//        protected abstract void BuildRuntimeTypeModel(RuntimeTypeModel typeModel);

//        /// <summary>
//        /// Serializes the given index to the given stream.
//        /// </summary>
//        /// <param name="stream"></param>
//        /// <param name="index"></param>
//        protected override void DoSerialize(SpatialIndexSerializerStream stream,
//            RTreeMemoryIndex<T> index)
//        {
//            // build the run time type model.
//            RuntimeTypeModel typeModel = this.GetRuntimeTypeModel();

//            // serialize root node.
//            Stream serializedRoot = 
//                this.Serialize(typeModel, index.Root);
//            serializedRoot.Seek(0, SeekOrigin.Begin);
//            serializedRoot.CopyTo(stream);
//            stream.Flush();
//        }

//        /// <summary>
//        /// Serializes all children of the given node.
//        /// </summary>
//        /// <param name="typeModel"></param>
//        /// <param name="nodeBase"></param>
//        /// <returns></returns>
//        private Stream Serialize(RuntimeTypeModel typeModel, RTreeMemoryIndex<T>.Node nodeBase)
//        {
//            var stream = _streamCache.CreateNew();
//            if (nodeBase.Children is List<RTreeMemoryIndex<T>.Node>)
//            { // the node is not a leaf.
//                long position = 0;
//                var node = (nodeBase as RTreeMemoryIndex<T>.Node);
//                var childrenIndex = new ChildrenIndex();
//                childrenIndex.IsLeaf = new bool[nodeBase.Children.Count];
//                childrenIndex.MinX = new float[nodeBase.Children.Count];
//                childrenIndex.MinY = new float[nodeBase.Children.Count];
//                childrenIndex.MaxX = new float[nodeBase.Children.Count];
//                childrenIndex.MaxY = new float[nodeBase.Children.Count];
//                childrenIndex.Starts = new int[nodeBase.Children.Count];
//                var serializedChildren = new List<Stream>();
//                for (int idx = 0; idx < nodeBase.Children.Count; idx++)
//                {
//                    var child = (node.Children[idx] as RTreeMemoryIndex<T>.Node);
//                    RectangleF2D box = node.Boxes[idx];
//                    childrenIndex.MinX[idx] = (float) box.Min[0];
//                    childrenIndex.MinY[idx] = (float) box.Min[1];
//                    childrenIndex.MaxX[idx] = (float) box.Max[0];
//                    childrenIndex.MaxY[idx] = (float) box.Max[1];
//                    childrenIndex.IsLeaf[idx] = (
//                        child.Children is List<T>);

//                    Stream childSerialized = this.Serialize(typeModel, child);
//                    serializedChildren.Add(childSerialized);

//                    childrenIndex.Starts[idx] = (int)position;
//                    position = position + childSerialized.Length;
//                }
//                childrenIndex.End = (int)position;

//                // serialize this index object.
//                var indexStream = new MemoryStream();
//                typeModel.Serialize(indexStream, childrenIndex);
//                byte[] indexBytes = indexStream.ToArray();

//                // START WRITING THE DATA TO THE TARGET STREAM HERE!

//                // 1: write the type of data.
//                byte[] leafFlag = new[] { (byte)0 };
//                stream.Write(leafFlag, 0, 1);
                
//                // 2: Write the length of the meta data.
//                byte[] indexLength = BitConverter.GetBytes(indexBytes.Length);
//                stream.Write(indexLength, 0, indexLength.Length);

//                // 3: write the meta data or the node-index.
//                stream.Write(indexBytes, 0, indexBytes.Length);

//                // 4: write the actual children.
//                for (int idx = 0; idx < serializedChildren.Count; idx++)
//                {
//                    serializedChildren[idx].Seek(0, SeekOrigin.Begin);
//                    serializedChildren[idx].CopyTo(stream);
//                    _streamCache.Dispose(serializedChildren[idx]);
//                    serializedChildren[idx] = null;
//                }
//            }
//            else if (nodeBase.Children is List<T>)
//            { // the node is a leaf node.
//                // START WRITING THE DATA TO THE TARGET STREAM HERE!

//                // 1: write the type of data.
//                byte[] leafFlag = new[] { (byte)1 };
//                stream.Write(leafFlag, 0, 1);

//                // 2: write the leaf data.
//                byte[] data = this.Serialize(typeModel,
//                    nodeBase.Children as List<T>, 
//                    nodeBase.Boxes);
//                stream.Write(data, 0, data.Length);
//            }
//            else
//            {
//                throw new Exception("Unknown node type!");
//            }
//            return stream;
//        }

//        /// <summary>
//        /// Serializes all data on one leaf.
//        /// </summary>
//        /// <param name="typeModel"></param>
//        /// <param name="data"></param>
//        /// <param name="boxes"></param>
//        /// <returns></returns>
//        protected abstract byte[] Serialize(RuntimeTypeModel typeModel, List<T> data, 
//            List<RectangleF2D> boxes);

//        /// <summary>
//        /// Deserializes all data on one leaf.
//        /// </summary>
//        /// <param name="typeModel"></param>
//        /// <param name="data"></param>
//        /// <param name="boxes"></param>
//        /// <returns></returns>
//        protected abstract List<T> DeSerialize(RuntimeTypeModel typeModel, byte[] data,
//            out List<RectangleF2D> boxes);

//        /// <summary>
//        /// Deserializes the given stream into an index.
//        /// </summary>
//        /// <param name="stream"></param>
//        /// <param name="lazy"></param>
//        /// <returns></returns>
//        protected override ISpatialIndexReadonly<T> DoDeserialize(SpatialIndexSerializerStream stream, bool lazy)
//        {
//            if (stream == null) throw new ArgumentNullException("stream");
//            if (!lazy) throw new NotSupportedException();

//            return new RTreeStreamIndex<T>(this, stream);
//        }

//        /// <summary>
//        /// Deserializes the data that is relevant for the given box.
//        /// </summary>
//        /// <param name="stream"></param>
//        /// <param name="box"></param>
//        /// <param name="result"></param>
//        /// <returns></returns>
//        public void Search(SpatialIndexSerializerStream stream, 
//            RectangleF2D box, HashSet<T> result)
//        {
//            var leafFlag = new byte[1];
//            stream.Read(leafFlag, 0, 1);

//            if (leafFlag[0] == 1)
//            { // cannot deserialize a leaf without a given size.
//                // try to read the entire stream as a leaf.
//                this.SearchInLeaf(stream, 1, stream.Length - 1, box, result);
//                return;
//            }

//            // build the run time type model.
//            RuntimeTypeModel typeModel = this.GetRuntimeTypeModel();

//            // get the lenght of the meta info.
//            var metaLengthBytes = new byte[4];
//            stream.Read(metaLengthBytes, 0, metaLengthBytes.Length);
//            int metaLength = BitConverter.ToInt32(metaLengthBytes, 0);
//            var metaBytes = new byte[metaLength];
//            stream.Read(metaBytes, 0, metaBytes.Length);
//            var index = typeModel.Deserialize(new MemoryStream(metaBytes), null, 
//                typeof(ChildrenIndex)) as ChildrenIndex;
//            if (index != null)
//            { // the index was deserialized.
//                long position = stream.Position;
//                int leafs = 0;
//                for (int idx = 0; idx < index.IsLeaf.Length; idx++)
//                {
//                    var localBox = new RectangleF2D(index.MinX[idx], index.MinY[idx],
//                        index.MaxX[idx], index.MaxY[idx]);
//                    if (localBox.Overlaps(box))
//                    { // there will be data in one of the children.
//                        if (index.IsLeaf[idx])
//                        { // deserialize and search the leaf-data.
//                            // calculate size.
//                            int size;
//                            if (idx == index.IsLeaf.Length - 1)
//                            { // the last index.
//                                size = index.End - index.Starts[idx] - 1;
//                            }
//                            else
//                            { // not the last index.
//                                size = index.Starts[idx + 1] - 
//                                    index.Starts[idx] - 1;
//                            }
//                            this.SearchInLeaf(stream, position + index.Starts[idx] + 1, 
//                                size, box, result);
//                            leafs++;
//                        }
//                        else
//                        { // deserialize the node and the children.
//                            stream.Seek(position + index.Starts[idx], SeekOrigin.Begin);

//                            this.Search(stream, box, result);
//                        }
//                    }
//                }
//                return;
//            }
//            throw new Exception("Cannot deserialize node!");
//        }

//        /// <summary>
//        /// Deserializes the leaf data at the given offset and with the given length and 
//        /// returns the data overlapping the given box.
//        /// </summary>
//        /// <param name="stream"></param>
//        /// <param name="offset"></param>
//        /// <param name="size"></param>
//        /// <param name="box"></param>
//        /// <param name="result"></param>
//        /// <returns></returns>
//        protected void SearchInLeaf(SpatialIndexSerializerStream stream, long offset, long size,
//            RectangleF2D box, HashSet<T> result)
//        {
//            // position the stream.
//            stream.Seek(offset, SeekOrigin.Begin);

//            // build the run time type model.
//            RuntimeTypeModel typeModel = this.GetRuntimeTypeModel();

//            if (size > 0)
//            { // the data is a leaf and can be read.
//                int before = result.Count;
//                var dataBytes = new byte[size];
//                stream.Read(dataBytes, 0, dataBytes.Length);
//                List<RectangleF2D> boxes;
//                List<T> data = this.DeSerialize(typeModel, dataBytes, out boxes);

//                for (int idx = 0; idx < data.Count; idx++)
//                { // check each overlapping box.
//                    if (boxes[idx].Overlaps(box))
//                    { // adds the object to the result set.
//                        result.Add(data[idx]);
//                    }
//                }
//                //OsmSharp.Logging.Log.TraceEvent("RTreeStreamSerializer", OsmSharp.Logging.TraceEventType.Verbose,
//                //    string.Format("Deserialized leaf@{1} and added {0} objects.", result.Count - before,
//                //        offset));
//                return;
//            }
//            throw new Exception("Cannot deserialize node!");
//        }
//    }

//    /// <summary>
//    /// Represents a reserializable index of children of an R-tree node.
//    /// </summary>
//    [ProtoContract]
//    public class ChildrenIndex
//    {
//        /// <summary>
//        /// Creates a new children index.
//        /// </summary>
//        public ChildrenIndex()
//        {

//        }

//        /// <summary>
//        /// The min X of each child.
//        /// </summary>
//        [ProtoMember(1)]
//        public float[] MinX { get; set; }

//        /// <summary>
//        /// The min Y of each child.
//        /// </summary>
//        [ProtoMember(2)]
//        public float[] MinY { get; set; }

//        /// <summary>
//        /// The max X of each child.
//        /// </summary>
//        [ProtoMember(3)]
//        public float[] MaxX { get; set; }

//        /// <summary>
//        /// The max Y of each child.
//        /// </summary>
//        [ProtoMember(4)]
//        public float[] MaxY { get; set; }

//        /// <summary>
//        /// The start position of each node in the stream.
//        /// </summary>
//        [ProtoMember(5)]
//        public int[] Starts { get; set; }

//        /// <summary>
//        /// The end of this node in the stream.
//        /// </summary>
//        [ProtoMember(6)]
//        public int End { get; set; }

//        /// <summary>
//        /// Gets or sets the type flags.
//        /// </summary>
//        [ProtoMember(7)]
//        public bool[] IsLeaf { get; set; }
//    }
//}