#nullable enable
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("OsmSharp.Replication.Test")]
namespace OsmSharp.Replication.Http;

/// <summary>
/// Abstract representation of an http handler.
/// </summary>
internal interface IHttpHandler
{
    /// <summary>
    /// Gets a stream representing the data at the given url.
    /// </summary>
    /// <param name="requestUri">The uri.</param>
    /// <returns>The stream.</returns>
    Task<Stream?> TryGetStreamAsync(string requestUri);
}
