﻿namespace FluentHue
{
    using FluentHue.Contracts;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Validation;

    /// <summary>
    /// Represents a Philips Hue bridge.
    /// </summary>
    public sealed class HueBridge : IHueBridge
    {
        /// <summary>
        /// Initializes a new <see cref="HueBridge"/>.
        /// </summary>
        /// <param name="metadata">The metadata found during bridge discovery.</param>
        internal HueBridge(HueBridgeMetadata metadata)
        {
            Requires.NotNull(metadata, nameof(metadata));
            Requires.NotNullOrWhiteSpace(metadata.Id, nameof(metadata.Id));
            Requires.NotNullOrWhiteSpace(metadata.InternalIpAddress, nameof(metadata.InternalIpAddress));

            this.Id = metadata.Id;
            this.LocalIpAddress = metadata.InternalIpAddress;
        }

        /// <summary>
        /// Initializes a new <see cref="HueBridge"/>.
        /// </summary>
        /// <param name="localIpAddress">The private IP address of the bridge on its local network.</param>
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
        /// Asynchronously gets all lights connected to the bridge.
        /// </summary>
        /// <returns>All lights connected to the bridge.</returns>
        public async Task<IEnumerable<IHueLight>> GetAllLightsAsync()
        {
            var response = await Client.CreateRestClientForBridge(this)
                .ExecuteAsync(new RestRequest("lights"), Method.GET);

            var jObject = JsonConvert.DeserializeObject<JObject>(response.Content);
            var lights = new List<HueLight>();
            foreach (JProperty light in (JToken)jObject)
            {
                lights.Add(new HueLight(this, light.Name, JsonConvert.DeserializeObject<HueLightMetadata>(light.Value.ToString())));
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
        public IHueLight SelectLight(string name)
        {
            throw new NotImplementedException();
        }
    }
}