#nullable enable
using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OsmSharp.Changesets;
using OsmSharp.Replication.Http;

namespace OsmSharp.Replication
{
    public static class ReplicationConfigExtensions
    {
        internal static readonly ThreadLocal<XmlSerializer> ThreadLocalXmlSerializer =
            new ThreadLocal<XmlSerializer>(() => new XmlSerializer(typeof(OsmChange)));
        
        /// <summary>
        /// Parses a replication state from a state file stream.
        /// </summary>
        /// <param name="config">The replication config.</param>
        /// <param name="streamReader">The stream reader.</param>
        /// <returns>The replication state.</returns>
        /// <exception cref="Exception"></exception>
        internal static ReplicationState ParseReplicationState(this ReplicationConfig config, StreamReader streamReader)
        {
            var sequenceNumber = long.MaxValue;
            var timestamp = default(DateTime);
            while (!streamReader.EndOfStream)
            {
                var line = streamReader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("#")) continue;
                if (line.StartsWith(ReplicationState.SequenceNumberKey))
                { // this line has the sequence number.
                    var keyValue = line.Split('=');
                    if (keyValue.Length != 2) throw new Exception($"Could not parse {ReplicationState.SequenceNumberKey}");
                    if (!long.TryParse(keyValue[1], out sequenceNumber)) throw new Exception($"Could not parse {ReplicationState.SequenceNumberKey}");
                }
                else if (line.StartsWith(ReplicationState.TimestampKey))
                {
                    var keyValue = line.Split('=');
                    if (keyValue.Length != 2) throw new Exception($"Could not parse {ReplicationState.TimestampKey}");
                    keyValue[1] = keyValue[1].Replace("\\", string.Empty);
                    if (!DateTime.TryParse(keyValue[1],null, DateTimeStyles.AdjustToUniversal, out timestamp)) throw new Exception($"Could not parse {ReplicationState.TimestampKey}");
                }
            }

            return new ReplicationState(config, sequenceNumber, timestamp);
        }

        /// <summary>
        /// Gets an enumerator to loop over incoming diffs.
        /// </summary>
        /// <param name="config">The replication config.</param>
        /// <param name="sequenceNumber">The sequence number, latest if empty.</param>
        /// <returns>The enumerator moved to the given sequence number or null if the sequence number doesn't exist.</returns>
        public static async Task<ReplicationDiffEnumerator?> GetDiffEnumerator(this ReplicationConfig config,
            long? sequenceNumber = null)
        {
            if (sequenceNumber == null)
            { // get the latest number.
                var latest = await config.LatestReplicationState();
                sequenceNumber = latest.SequenceNumber;
            }
            
            var enumerator = new ReplicationDiffEnumerator(config);
            if (!await enumerator.MoveTo(sequenceNumber.Value))
            {
                return null;
            }
            return enumerator;
        }

        /// <summary>
        /// Gets an enumerator to loop over incoming diffs moved to the first diff overlapping the given timestamp.
        /// </summary>
        /// <param name="config">The replication config.</param>
        /// <param name="timestamp">The sequence number, latest if empty.</param>
        /// <returns>The enumerator moved to the given sequence number or null if the sequence number doesn't exist.</returns>
        public static async Task<ReplicationDiffEnumerator?> GetDiffEnumerator(this ReplicationConfig config,
            DateTime timestamp)
        {
            var enumerator = await config.GetDiffEnumerator();
            if (enumerator == null) return null;
            if (!await enumerator.MoveTo(timestamp))
            {
                return null;
            }

            return enumerator;
        }
        
        /// <summary>
        /// Guesses the sequence number for the diff overlapping the given date time.
        /// </summary>
        /// <param name="config">The replication config.</param>
        /// <param name="dateTime">The date time.</param>
        /// <returns>The sequence number.</returns>
        /// <remarks>This is just a guess because the time doesn't always exactly align with the increase of sequence numbers.</remarks>
        public static async Task<long> GuessSequenceNumberAt(this ReplicationConfig config, DateTime dateTime)
        {
            var latest = await config.LatestReplicationState();
            var start = latest.EndTimestamp.AddSeconds(-config.Period);
            var diff = (int)(start - dateTime).TotalSeconds;
            var leftOver = (diff % config.Period);
            var sequenceOffset = (diff - leftOver) / config.Period;

            return latest.SequenceNumber - sequenceOffset - 1;
        }

        /// <summary>
        /// Gets the replication state.
        /// </summary>
        /// <param name="config">The replication config.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <returns>The latest replication state.</returns>
        public static async Task<ReplicationState?> GetReplicationState(this ReplicationConfig config, long sequenceNumber)
        {
            if (sequenceNumber <= 0) return null;
            if (sequenceNumber > ReplicationConfig.MaxSequenceNumber) return null;

            var stream = await HttpHandler.Default.TryGetStreamAsync(config.ReplicationStateUrl(sequenceNumber));
            if (stream == null) return null;

            using (stream)
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return config.ParseReplicationState(streamReader);
                }
            }
        }

        /// <summary>
        /// Gets the url for the diff associated with the given replication state.
        /// </summary>
        /// <param name="config">The replication config.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <returns>The url to download the diff at.</returns>
        internal static string ReplicationStateUrl(this ReplicationConfig config, long sequenceNumber)
        {
            var sequenceNumberString =  "000000000" + sequenceNumber;
            sequenceNumberString = sequenceNumberString.Substring(sequenceNumberString.Length - 9);
            var folder1 = sequenceNumberString.Substring(0, 3);
            var folder2 = sequenceNumberString.Substring(3, 3);
            var name = sequenceNumberString.Substring(6, 3);
            return new Uri(new Uri(config.Url), $"{folder1}/{folder2}/{name}.state.txt").ToString();
        }

        /// <summary>
        /// Gets the url for the diff associated with the given replication state.
        /// </summary>
        /// <param name="config">The replication config.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <returns>The url to download the diff at.</returns>
        internal static string DiffUrl(this ReplicationConfig config, long sequenceNumber)
        {
            var sequenceNumberString =  "000000000" + sequenceNumber;
            sequenceNumberString = sequenceNumberString.Substring(sequenceNumberString.Length - 9);
            var folder1 = sequenceNumberString.Substring(0, 3);
            var folder2 = sequenceNumberString.Substring(3, 3);
            var name = sequenceNumberString.Substring(6, 3);
            return new Uri(new Uri(config.Url), $"{folder1}/{folder2}/{name}.osc.gz").ToString();
        }

        /// <summary>
        /// Downloads the diff associated with the given replication state.
        /// </summary>
        /// <param name="config">The replication config.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <returns>The raw diff stream.</returns>
        internal static async Task<Stream?> DownloadDiffStream(this ReplicationConfig config, long sequenceNumber)
        {
            return await HttpHandler.Default.TryGetStreamAsync(config.DiffUrl(sequenceNumber));
        }

        /// <summary>
        /// Downloads the diff associated with the given replication state.
        /// </summary>
        /// <param name="config">The replication config.</param>
        /// <param name="sequenceNumber">The sequence number.</param>
        /// <returns>The diff.</returns>
        public static async Task<OsmChange?> DownloadDiff(this ReplicationConfig config, long sequenceNumber)
        {
            using var stream = await config.DownloadDiffStream(sequenceNumber);
            if (stream == null) return null;   // not published yet, which a caller retries

            using var decompressed = new GZipStream(stream, CompressionMode.Decompress);
            using var streamReader = new StreamReader(decompressed);
            
            // Same as above: the factory makes one per thread, so there is always a serializer.
            var serializer = ThreadLocalXmlSerializer.Value!;

            return serializer.Deserialize(streamReader) as OsmChange;
        }
    }
}