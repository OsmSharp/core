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
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Net;
//using System.IO;
//using System.IO.Compression;
//using OsmSharp.Osm;

//namespace OsmSharp.Osm.Xml.Processor.Replication
//{
//    /// <summary>
//    /// A replication data changeset source.
//    /// </summary>
//    public class Replicator : DataProcessorChangeSetSource
//    {
//        private string _url;

//        private int _sequenceNumber;

//        private bool _started;

//        private int _milli;

//        private int _max;

//        /// <summary>
//        /// Creates a new changeset replication source.
//        /// </summary>
//        /// <param name="start_sequence_number"></param>
//        /// <param name="url"></param>
//        /// <param name="milli"></param>
//        /// <param name="count"></param>
//        public Replicator(int start_sequence_number, string url, int milli,int count)
//        {
//            _url = url;
//            _milli = milli;
//            _sequenceNumber = start_sequence_number - 1;
//            _max = _sequenceNumber + count;
//        }

//        /// <summary>
//        /// The last sequence number.
//        /// </summary>
//        public int LastSequenceNumber 
//        {
//            get
//            {
//                return _sequenceNumber;
//            }
//        }

//        #region Changes Downloads

//        /// <summary>
//        /// Starts the replication.
//        /// </summary>
//        private void Start()
//        {
//            _started = true;

//            while (_started)
//            {
//                if (!this.SyncOne())
//                {
//                    Thread.Sleep(10000);
//                }

//                Thread.Sleep(_milli);
//            }
//        }

//        /// <summary>
//        /// Syncs one replication file.
//        /// </summary>
//        /// <returns></returns>
//        private bool SyncOne()
//        {
//            if (_current_source == null)
//            {
//                int next = _sequenceNumber + 1;

//                // try to download the new changes.
//                int next_temp = next;
//                string folder1 = (next / 1000000).ToString("000");
//                next_temp = next_temp - ((next / 1000000) * 1000000);
//                string folder = (next_temp / 1000).ToString("000");
//                string file = (next_temp - ((next_temp / 1000) * 1000)).ToString("000");

//                string url_change = _url + string.Format("/{0}/{1}/{2}.osc.gz", folder1, folder, file);
//                string url_state = _url + string.Format("/{0}/{1}/{2}.state.txt", folder1, folder, file);

//                try
//                {
//                    // get changeset file.
//                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(
//                        url_state);
//                    request.Timeout = 10000;

//                     WebResponse myResp = request.GetResponse();

//                    Stream stream = myResp.GetResponseStream();
//                    string state_file = (new StreamReader(stream)).ReadToEnd();

//                    stream.Close();
//                    stream.Dispose();

//                    // state file was found!
//                    string[] state_array = state_file.Split(new string[]{"\n"},StringSplitOptions.None);

//                    // download changeset                
//                    request = (HttpWebRequest)HttpWebRequest.Create(
//                        url_change);
//                    request.Timeout = 10000;
//                    myResp = request.GetResponse();
//                    Stream change_stream = myResp.GetResponseStream();
//                    GZipStream unzipped_stream = new GZipStream(change_stream, CompressionMode.Decompress);

//                    // stream all the data into a byte array.
//                    //byte[] changeset_data = unzipped_stream.
//                    MemoryStream memory_stream = new MemoryStream();
//                    unzipped_stream.CopyTo(memory_stream);
//                    unzipped_stream.Close();
//                    change_stream.Close();
//                    memory_stream.Seek(0, SeekOrigin.Begin);

//                    // used the memory stream.
//                    XmlDataProcessorChangeSetSource current_source = new XmlDataProcessorChangeSetSource(memory_stream);
//                    current_source.Initialize();
//                    _sequenceNumber = next;

//                    OsmSharp.IO.Output.OutputStreamHost.WriteLine("");
//                    OsmSharp.IO.Output.OutputStreamHost.WriteLine("Started applying changeset {0}:{1}!", _sequenceNumber,state_array[0]);

//                    _current_source = current_source;

//                    return true;
//                }
//                catch (WebException)
//                {

//                }
//            }
//            return false;
//        }

//        /// <summary>
//        /// Stops the replication process.
//        /// </summary>
//        public void Stop()
//        {
//            _started = false;
//        }

//        #endregion

//        #region Change Set Source-implementation

//        private XmlDataProcessorChangeSetSource _current_source;

//        /// <summary>
//        /// Initializes this replicator.
//        /// </summary>
//        public override void Initialize()
//        {

//        }

//        /// <summary>
//        /// Moves to the next changeset.
//        /// </summary>
//        /// <returns></returns>
//        public override bool MoveNext()
//        {
//            // start downloading changes.
//            if (!_started)
//            {
//                _started = true;
//                Thread thr = new Thread(new ThreadStart(Start));
//                thr.Start();
//            }

//            // wait for new data.
//            while (_current_source == null)
//            {
//                Thread.Sleep(50);
//            }

//            // get the next data.
//            if (!_current_source.MoveNext())
//            {
//                // reset the current source; it's finished!
//                _current_source.Close();
//                _current_source = null;

//                // if the max is passed return false
//                if (_sequenceNumber + 1 > _max)
//                {
//                    this.Stop();

//                    return false;
//                }

//                // wait for any next data to exit.
//                this.MoveNext();
//            }
//            return true;
//        }

//        /// <summary>
//        /// Returns the current changeset.
//        /// </summary>
//        /// <returns></returns>
//        public override SimpleChangeSet Current()
//        {
//            return _current_source.Current();
//        }

//        /// <summary>
//        /// Resets this replicator.
//        /// </summary>
//        public override void Reset()
//        {
//            // do nothing, unable to reset!
//            throw new NotSupportedException();
//        }

//        /// <summary>
//        /// Closes this replicator.
//        /// </summary>
//        public override void Close()
//        {
//            this.Stop();
//        }

//        #endregion

//    }
//}
