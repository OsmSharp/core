#nullable enable
using System;
using System.IO;
using System.Threading.Tasks;
using OsmSharp.Replication.Http;

namespace OsmSharp.Replication
{
    /// <summary>
    /// Replication configuration.
    /// </summary>
    public class ReplicationConfig
    {
        /// <summary>
        /// The maximum possible sequence number.
        /// </summary>
        public const long MaxSequenceNumber = 999999999;

        /// <summary>
        /// Gets the default configuration for minutely updates.
        /// </summary>
        public static ReplicationConfig Minutely =>
            new ReplicationConfig("https://planet.openstreetmap.org/replication/minute/", 60);

        /// <summary>
        /// Gets the default configuration for hourly updates.
        /// </summary>
        public static ReplicationConfig Hourly => 
            new ReplicationConfig("https://planet.openstreetmap.org/replication/hour/", 3600);

        /// <summary>
        /// Gets the default configuration for daily updates.
        /// </summary>
        public static ReplicationConfig Daily => 
            new ReplicationConfig("https://planet.openstreetmap.org/replication/day/", 3600 * 24);
        
        /// <summary>
        /// Creates a new replication config.
        /// </summary>
        /// <param name="url">The url.</param>
        /// <param name="period">The period.</param>
        public ReplicationConfig(string url, int period)
        {
            // A trailing slash, always: the sequence paths are resolved with
            // new Uri(base, "000/004/940.state.txt"), and without one that replaces the last
            // segment of the base instead of appending to it — ".../europe/andorra-updates"
            // silently becomes ".../europe/000/004/940.state.txt" and every lookup 404s, which
            // surfaces as a missing sequence rather than a wrong URL.
            if (url == null) throw new ArgumentNullException(nameof(url));

            this.Url = url.EndsWith("/") ? url : url + "/";
            this.Period = period;
        }
        
        /// <summary>
        /// Gets the url.
        /// </summary>
        public string Url { get; }
        
        /// <summary>
        /// Gets the replication period in seconds.
        /// </summary>
        public int Period { get; }

        private ReplicationState? _state = null;

        /// <summary>
        /// Gets the latest replication state.
        /// </summary>
        /// <returns>The latest replication state.</returns>
        public async Task<ReplicationState> LatestReplicationState()
        {
            if (_state != null &&
                _state.EndTimestamp > DateTime.Now.AddSeconds(this.Period))
            { // there cannot be a new latest.
                return _state;
            }

            var url = new Uri(new Uri(this.Url), "state.txt").ToString();
            using var stream = await HttpHandler.Default.TryGetStreamAsync(url);
            if (stream == null)
            {
                // A 404 here means the endpoint has no state at all, which is a wrong url rather
                // than a sequence that has not been published yet.
                throw new InvalidOperationException($"No replication state at {url}.");
            }

            using var streamReader = new StreamReader(stream);
            _state =  this.ParseReplicationState(streamReader);

            return _state;
        }

        /// <summary>
        /// Returns true if this config is daily.
        /// </summary>
        public bool IsDaily => this.Period == ReplicationConfig.Daily.Period;

        /// <summary>
        /// Returns true if this config is hourly.
        /// </summary>
        public bool IsHourly => this.Period == ReplicationConfig.Hourly.Period;

        /// <summary>
        /// Returns true if this config is minutely.
        /// </summary>
        public bool IsMinutely => this.Period == ReplicationConfig.Minutely.Period;

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Url} ({this.Period}s)";
        }
    }
}