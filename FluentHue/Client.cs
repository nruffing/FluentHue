namespace FluentHue
{
    using FluentHue.Serializers;
    using RestSharp;

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
    }
}