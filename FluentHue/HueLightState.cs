namespace FluentHue
{
    using FluentHue.Contracts;
    using RestSharp;
    using System;
    using System.Threading.Tasks;
    using Validation;

    /// <summary>
    /// Represents the state of a Philips Hue light bulb.
    /// </summary>
    public sealed class HueLightState : IHueLightState
    {
        private readonly HueBridge _bridge;
        private readonly HueLight _light;

        internal HueLightState(HueBridge bridge, HueLight light, HueLightStateMetadata metadata)
        {
            Requires.NotNull(bridge, nameof(bridge));
            Requires.NotNull(light, nameof(light));
            Requires.NotNull(metadata, nameof(metadata));

            this._bridge = bridge;
            this._light = light;

            this.IsOn = metadata.On;
        }

        /// <summary>
        /// Gets a value indicating whether the light is on.
        /// </summary>
        public bool IsOn { get; private set; }

        /// <summary>
        /// Asynchronously toggles the current state of the light (e.g. off to on).
        /// </summary>
        /// <returns>The current light state.</returns>
        public async Task<IHueLightState> ToggleAsync()
        {
            this.IsOn = !this.IsOn;

            await this.SetStateAsync().ConfigureAwait(false);

            return this;
        }

        /// <summary>
        /// Toggles the current state of the light (e.g. off to on).
        /// </summary>
        /// <returns>The current light state.</returns>
        public IHueLightState Toggle()
            => this.ToggleAsync().Result;

        /// <summary>
        /// Returns to the light.
        /// </summary>
        /// <returns>The light.</returns>
        public IHueLight End() => this._light;

        private async Task SetStateAsync()
        {
            var request = new RestRequest($"lights/{this._light.Id}/state");
            request.AddJsonBody(this.ToContract());

            var response = await Client.CreateRestClientForBridge(this._bridge)
                .ExecuteAsync(request, Method.PUT)
                .ConfigureAwait(false);

            if (response.IsSuccessful == false)
            {
                throw new Exception("There was an error setting the state of a light.");
            }
        }

        private HueLightStateMetadata ToContract()
            => new HueLightStateMetadata()
            {
                On = this.IsOn,
            };
    }
}