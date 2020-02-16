namespace FluentHue
{
    using FluentHue.Serializers;
    using RestSharp;
    using Validation;

    /// <summary>
    /// A collection of static methods for creating clients.
    /// </summary>
    internal static class Client
    {
        /// <summary>
        /// Creates an <see cref="IRestClient"/> with default settings for this library.
        /// </summary>
        /// <param name="baseUrl">The base url.</param>
        /// <returns>An <see cref="IRestClient"/>.</returns>
        internal static IRestClient CreateRestClient(string baseUrl)
        {
            IRestClient client = new RestClient(baseUrl);
            client.UseSerializer(() => new JsonNetSerializer());
            return client;
        }

        /// <summary>
        /// Create an <see cref="IRestClient"/> with default settings for this library for the
        /// specified Philips Hue bridge.
        /// </summary>
        /// <param name="bridge">The bridge.</param>
        /// <returns>An <see cref="IRestClient"/>.</returns>
        internal static IRestClient CreateRestClientForBridge(HueBridge bridge)
        {
            Requires.NotNull(bridge, nameof(bridge));
            Requires.That(string.IsNullOrWhiteSpace(bridge.User), nameof(bridge.User), "A user must be specified using WithUser before any further communication with the Hue Bridge.");            
            return Client.CreateRestClient(string.Format("http://{0}/api/{1}", bridge.LocalIpAddress, bridge.User));
        }
            
    }
}