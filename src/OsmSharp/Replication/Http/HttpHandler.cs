#nullable enable
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OsmSharp.Logging;

namespace OsmSharp.Replication.Http
{
    internal class HttpHandler : IHttpHandler
    {
        // The timeout covers headers only, because the body is streamed — see the completion
        // option below. Raising it instead was the wrong fix: no deadline is right for a download
        // whose size is unbounded, and a generous one just means a stalled connection wastes that
        // long before failing.
        internal static readonly ThreadLocal<HttpClient> ThreadLocalClient =
            new ThreadLocal<HttpClient>(() => new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(2)
            });
        internal static readonly Lazy<IHttpHandler> LazyHttpHandler = 
            new Lazy<IHttpHandler>(() => new HttpHandler());
        
        public async Task<Stream?> TryGetStreamAsync(string requestUri)
        {
            // Created by the factory above on first touch per thread, so this cannot be null —
            // ThreadLocal<T>.Value is only null for a T that has no factory.
            var client = ThreadLocalClient.Value!;
            try
            {
                // Headers only, then stream the body. The default buffers the whole response
                // before returning, which puts the *entire* download under HttpClient.Timeout —
                // so a planet daily diff of several hundred megabytes gets aborted partway on any
                // ordinary connection, however well it was progressing.
                var response = await client.GetAsync(requestUri,
                    HttpCompletionOption.ResponseHeadersRead);
                if (response.IsSuccessStatusCode) return await response.Content.ReadAsStreamAsync();

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                
                throw new HttpRequestException("Unexpected response.");
            }
            catch (Exception e)
            {
                Logger.Log($"{nameof(HttpHandler)}.{nameof(TryGetStreamAsync)}", TraceEventType.Error,
                    $"Unhandled exception when getting {requestUri}: {e}");
                throw;
            }
        }

        private static IHttpHandler? _defaultHandler;

        /// <summary>
        /// Gets or sets the default http handler.
        /// </summary>
        public static IHttpHandler Default
        {
            get => _defaultHandler ??= LazyHttpHandler.Value;
            set => _defaultHandler = value;
        }
    }
}