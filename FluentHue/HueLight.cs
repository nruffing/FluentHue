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
        private HueBridge _bridge;

        internal HueLight(HueBridge bridge, string id, HueLightContract contract)
        {
            Requires.NotNull(bridge, nameof(bridge));
            Requires.NotNullOrWhiteSpace(id, nameof(id));
            Requires.NotNull(contract, nameof(contract));
            Requires.NotNullOrWhiteSpace(contract.Name, nameof(contract.Name));

            this._bridge = bridge;
            this.Id = id;

            this.Name = contract.Name;
        }

        /// <summary>
        /// The id that will be used to identify this light bulb when communicating with the bridge.
        /// </summary>
        internal string Id { get; }
        
        /// <summary>
        /// Gets the name of the light.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Return to the bridge.
        /// </summary>
        /// <returns>The bridge.</returns>
        public IHueBridge End() => this._bridge;

        /// <summary>
        /// Asynchronously renames the current light.
        /// </summary>
        /// <param name="name">The new name.</param>
        /// <returns>The current light.</returns>
        public async Task<IHueLight> RenameAsync(string name)
        {
            Requires.NotNullOrWhiteSpace(name, nameof(name));

            var request = new RestRequest($"lights/{this.Id}");
            request.AddJsonBody(new HueLightContract()
            {
                Name = name,
            });

            var response = await Client.CreateRestClientForBridge(this._bridge)
                .ExecuteAsync(request, Method.PUT)
                .ConfigureAwait(false);

            if (response.IsSuccessful == false)
            {
                throw new Exception($"There was an error renaming the light from {this.Name} to {name}.");
            }

            this.Name = name;
            return this;
        }

        /// <summary>
        /// Renames the current light.
        /// </summary>
        /// <param name="name">The new name.</param>
        /// <returns>The current light.</returns>
        public IHueLight Rename(string name)
            => this.RenameAsync(name).Result;

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
                JsonConvert.DeserializeObject<HueLightStateContract>(jObject.GetValue("state").ToString()));
        }

        /// <summary>
        /// Gets the current state of the light.
        /// </summary>
        /// <returns>The current state of the light.</returns>
        public IHueLightState GetCurrentState()
            => this.GetCurrentStateAsync().Result;
    }
}