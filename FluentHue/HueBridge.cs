namespace FluentHue
{
    using FluentHue.Contracts;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Validation;

    /// <summary>
    /// Represents a Philips Hue bridge.
    /// </summary>
    public sealed class HueBridge : IHueBridge
    {
        internal HueBridge(HueBridgeContract contract)
        {
            Requires.NotNull(contract, nameof(contract));
            Requires.NotNullOrWhiteSpace(contract.Id, nameof(contract.Id));
            Requires.NotNullOrWhiteSpace(contract.InternalIpAddress, nameof(contract.InternalIpAddress));

            this.Id = contract.Id;
            this.LocalIpAddress = contract.InternalIpAddress;
        }

        internal HueBridge(string localIpAddress)
        {
            Requires.NotNullOrWhiteSpace(localIpAddress, nameof(localIpAddress));

            this.LocalIpAddress = localIpAddress;
        }

        /// <summary>
        /// The id of the bridge (e.g. 001788fffe4c2912). This is not when explicitly providing
        /// the internal IP address (e.g. HueBrigeLocator.SelectWithLocalIp).
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The private IP address of the bridge on its local network.
        /// </summary>
        public string LocalIpAddress { get; }

        internal string User { get; private set; }

        /// <summary>
        /// Sets the user to use when communicating with the bridge. This is required to be called
        /// before any further communication with the bridge.
        /// </summary>
        /// <param name="user">The Hue user that has permission to communicate with this bridge.</param>
        /// <returns>The current bridge.</returns>
        public IHueBridge WithUser(string user)
        {
            Requires.NotNullOrWhiteSpace(user, nameof(user));
            this.User = user;
            return this;
        }

        /// <summary>
        /// Initiate the Hue bridge to search for new lights. Optionally supply the serial numbers
        /// to look for specific light bulbs or when the general search does not find all the lights.
        /// </summary>
        /// <param name="serialNumbers">Optional. Specifically search for these serial numbers.</param>
        /// <returns>The current bridge.</returns>
        /// <remarks>https://developers.meethue.com/develop/hue-api/lights-api/#search-for-new-lights</remarks>
        public async Task<IHueBridge> DiscoverNewLightsAsync(params string[] serialNumbers)
        {
            var request = new RestRequest("lights");
            if (serialNumbers?.Any() ?? false)
            {
                request.AddJsonBody(new DiscoverNewHueLightsContract()
                {
                    SerialNumbers = serialNumbers,
                });
            }

            var response = await Client.CreateRestClientForBridge(this)
                .ExecuteAsync(request, Method.POST);

            if (response.IsSuccessful == false)
            {
                throw new Exception("There was an error initiating the bridge to discover new lights.");
            }

            return this;
        }

        /// <summary>
        /// Initiate the Hue bridge to search for new lights. Optionally supply the serial numbers
        /// to look for specific light bulbs or when the general search does not find all the lights.
        /// </summary>
        /// <param name="serialNumbers">Optional. Specifically search for these serial numbers.</param>
        /// <returns>The current bridge.</returns>
        /// <remarks>https://developers.meethue.com/develop/hue-api/lights-api/#search-for-new-lights</remarks>
        public IHueBridge DiscoverNewLights(params string[] serialNumbers)
            => this.DiscoverNewLightsAsync(serialNumbers).Result;

        /// <summary>
        /// Asynchronously gets all lights connected to the bridge.
        /// </summary>
        /// <returns>All lights connected to the bridge.</returns>
        public async Task<IEnumerable<IHueLight>> GetAllLightsAsync()
        {
            var response = await Client.CreateRestClientForBridge(this)
                .ExecuteAsync(new RestRequest("lights"), Method.GET);

            if (response.IsSuccessful == false)
            {
                throw new Exception("There was an error retrieving all lights connected to a the bridge.");
            }

            var jObject = JsonConvert.DeserializeObject<JObject>(response.Content);
            var lights = new List<HueLight>();
            foreach (JProperty light in (JToken)jObject)
            {
                lights.Add(new HueLight(this, light.Name, JsonConvert.DeserializeObject<HueLightContract>(light.Value.ToString())));
            }

            return lights;
        }

        /// <summary>
        /// Gets all lights connected to the bridge.
        /// </summary>
        /// <returns>All lights connected to the bridge.</returns>
        public IEnumerable<IHueLight> GetAllLights()
            => this.GetAllLightsAsync().Result;

        /// <summary>
        /// Selects the light with the specified name.
        /// </summary>
        /// <param name="name">The name of the light to select.</param>
        /// <returns>The light with the specified name.</returns>
        public async Task<IHueLight> SelectLightAsync(string name)
        {
            var lights = await this.GetAllLightsAsync().ConfigureAwait(false);
            return lights.First(l => l.Name == name);
        }

        /// <summary>
        /// Selects the light with the specified name.
        /// </summary>
        /// <param name="name">The name of the light to select.</param>
        /// <returns>The light with the specified name.</returns>
        public IHueLight SelectLight(string name)
            => this.SelectLightAsync(name).Result;
    }
}