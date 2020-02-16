using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FluentHue.Tests")]
namespace FluentHue
{
    using FluentHue.Contracts;
    using FluentHue.Exceptions;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Validation;

    /// <summary>
    /// A collection of static methods for accessing an <see cref="IHueBridge"/>.
    /// </summary>
    public static class HueBridgeLocator
    {
        private const string BridgeDiscoveryUrl = "https://discovery.meethue.com";
        
        internal static IRestClient DiscoveryClient = Client.CreateRestClient(BridgeDiscoveryUrl);

        /// <summary>
        /// Asynchronously retrieves the first Hue bridge found on the local network. This is intended to be 
        /// an efficiency when you know there is only one bridge on the local network.
        /// </summary>
        /// <returns>A task to retrieve the first Hue bridge found on the local network.</returns>
        public static async Task<IHueBridge> SelectFirstAsync()
            => new HueBridge((await FindBridgesAsync().ConfigureAwait(false)).First());

        /// <summary>
        /// Retrieves the first Hue bridge found on the local network. This is intended to be 
        /// an efficiency when you know there is only one bridge on the local network.
        /// </summary>
        /// <returns>The first Hue bridge found on the local network.</returns>
        public static IHueBridge SelectFirst()
            => SelectFirstAsync().Result;

        /// <summary>
        /// Creates a new <see cref="IHueBridge"/> with the specified local IP address.
        /// </summary>
        /// <param name="localIpAddress">The private IP address of the bridge on its local network.</param>
        /// <returns>A new <see cref="IHueBridge"/> with the specified local IP address.</returns>
        public static IHueBridge SelectWithLocalIp(string localIpAddress)
            => new HueBridge(localIpAddress);

        private static async Task<IEnumerable<HueBridgeMetadata>> FindBridgesAsync()
        {
            Requires.NotNull(DiscoveryClient, nameof(DiscoveryClient));

            var response = await DiscoveryClient.ExecuteAsync<IEnumerable<HueBridgeMetadata>>(new RestRequest(Method.GET)).ConfigureAwait(false);
            if (response.IsSuccessful == false)
            {
                throw new Exception("There was an error finding a Hue bridge on the local network");
            }

            if (response.Data?.Any() ?? false == false)
            {
                throw new HueBridgeNotFoundException();
            }

            return response.Data;
        }
    }
}