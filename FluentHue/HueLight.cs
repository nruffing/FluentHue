namespace FluentHue
{
    using FluentHue.Contracts;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using RestSharp;
    using System;
    using System.Threading.Tasks;
    using Validation;

    /// <summary>
    /// Represents a Philips Hue light bulb.
    /// </summary>
    public sealed class HueLight : IHueLight
    {
        /// <summary>
        /// The bridge this light is connected to.
        /// </summary>
        private HueBridge _bridge;

        /// <summary>
        /// Initializes a new <see cref="HueLight"/>.
        /// </summary>
        /// <param name="bridge">The bridge the light is connected to.</param>
        /// <param name="id">The id that the bridge identifies this light as.</param>
        /// <param name="metadata">The light's metadata.</param>
        internal HueLight(HueBridge bridge, string id, HueLightMetadata metadata)
        {
            Requires.NotNull(bridge, nameof(bridge));
            Requires.NotNullOrWhiteSpace(id, nameof(id));
            Requires.NotNull(metadata, nameof(metadata));

            this._bridge = bridge;
            this.Id = id;

            this.Name = metadata.Name;
        }

        /// <summary>
        /// The id that will be used to identify this light bulb when communicating with the bridge.
        /// </summary>
        internal string Id { get; }
        
        /// <summary>
        /// Gets the name of the light.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Return to the bridge.
        /// </summary>
        /// <returns>The bridge.</returns>
        public IHueBridge End() => this._bridge;

        /// <summary>
        /// Asynchronously gets the current state of the light.
        /// </summary>
        /// <returns>The current state of the light.</returns>
        public async Task<IHueLightState> GetCurrentStateAsync()
        {
            var response = await Client.CreateRestClientForBridge(this._bridge)
                .ExecuteAsync(new RestRequest($"lights/{this.Id}"), Method.GET)
                .ConfigureAwait(false);

            if (response.IsSuccessful == false)
            {
                throw new Exception("There was an error retrieving the current state of a light.");
            }

            var jObject = JsonConvert.DeserializeObject<JObject>(response.Content);
            return new HueLightState(
                this._bridge,
                this,
                JsonConvert.DeserializeObject<HueLightStateMetadata>(jObject.GetValue("state").ToString()));
        }

        /// <summary>
        /// Gets the current state of the light.
        /// </summary>
        /// <returns>The current state of the light.</returns>
        public IHueLightState GetCurrentState()
            => this.GetCurrentStateAsync().Result;
    }
}