namespace FluentHue
{
    using AutoMapper;
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
        private readonly IMapper _mapper = Container.Instance.GetInstance<IMapper>();
        private readonly HueBridge _bridge;
        private readonly HueLight _light;

        internal HueLightState(HueBridge bridge, HueLight light, HueLightStateContract contract)
        {
            Requires.NotNull(bridge, nameof(bridge));
            Requires.NotNull(light, nameof(light));
            Requires.NotNull(contract, nameof(contract));

            this._bridge = bridge;
            this._light = light;

            _mapper.Map(contract, this, typeof(HueLightStateContract), typeof(HueLightState));
        }

        /// <summary>
        /// Gets a value indicating whether the light is on.
        /// </summary>
        public bool IsOn { get; private set; }

        /// <summary>
        /// Gets a value representing the brightness of the light on a scale of 1-254.
        /// </summary>
        public byte Brightness { get; private set; }

        /// <summary>
        /// Gets a value representing the color's x coordinate (using CIE xy color).
        /// </summary>
        public float ColorX { get; private set; }

        /// <summary>
        /// Gets a value representing the color's y coordinate (using CIE xy color).
        /// </summary>
        public float ColorY { get; private set; }

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
        /// Asynchronously sets the state of light (i.e. on/off) to the specified value.
        /// </summary>
        /// <param name="isOn">Whether to set the state of the light to on, off otherwise.</param>
        /// <returns>The current light state.</returns>
        public Task<IHueLightState> SetStateAsync(bool isOn)
        {
            if (this.IsOn != isOn)
            {
                return this.ToggleAsync();
            }
            return Task.FromResult(this as IHueLightState);
        }

        /// <summary>
        /// Sets the state of light (i.e. on/off) to the specified value.
        /// </summary>
        /// <param name="isOn">Whether to set the state of the light to on, off otherwise.</param>
        /// <returns>The current light state.</returns>
        public IHueLightState SetState(bool isOn)
            => this.SetStateAsync(isOn).Result;

        /// <summary>
        /// Asynchronously sets the brightness of the light to the specified level.
        /// </summary>
        /// <param name="brightness">The level of brightness to set. This must be within the
        /// inclusive range of 1-254.</param>
        /// <returns>The current light state.</returns>
        public async Task<IHueLightState> SetBrightnessAsync(byte brightness)
        {
            Requires.Range(
                brightness > 0 && brightness < byte.MaxValue,
                nameof(brightness),
                "The brightness must be within the inclusive range of 1 to 254.");

            this.Brightness = brightness;
            await this.SetStateAsync().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Sets the brightness of the light to the specified level.
        /// </summary>
        /// <param name="brightness">The level of brightness to set. This must be within the
        /// inclusive range of 1-254.</param>
        /// <returns>The current light state.</returns>
        public IHueLightState SetBrightness(byte brightness)
            => this.SetBrightnessAsync(brightness).Result;

        /// <summary>
        /// Asynchronously sets the color of the light to specified color (using CIE xy color).
        /// </summary>
        /// <param name="x">The color's x coordinate. Must be between 0 and 1.</param>
        /// <param name="y">The color's y coordinate. Must be between 0 and 1.</param>
        /// <returns>The current light state.</returns>
        public async Task<IHueLightState> SetColorAsync(float x, float y)
        {
            Requires.Range(
                x >= 0 && x <= 1,
                nameof(x),
                "The color's x-coordinate must be withing the inclusive range of 0 to 1");
            Requires.Range(
                y >= 0 && y <= 1,
                nameof(y),
                "The color's y-coordinate must be withing the inclusive range of 0 to 1");

            this.ColorX = x;
            this.ColorY = y;
            await this.SetStateAsync().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Sets the color of the light to specified color (using CIE xy color).
        /// </summary>
        /// <param name="x">The color's x coordinate. Must be between 0 and 1.</param>
        /// <param name="y">The color's y coordinate. Must be between 0 and 1.</param>
        /// <returns>The current light state.</returns>
        public IHueLightState SetColor(float x, float y)
            => this.SetColorAsync(x, y).Result;

        /// <summary>
        /// Returns to the light.
        /// </summary>
        /// <returns>The light.</returns>
        public IHueLight End() => this._light;

        private async Task SetStateAsync()
        {
            var request = new RestRequest($"lights/{this._light.Id}/state");
            request.AddJsonBody(_mapper.Map<HueLightStateContract>(this));

            var response = await Client.CreateRestClientForBridge(this._bridge)
                .ExecuteAsync(request, Method.PUT)
                .ConfigureAwait(false);

            if (response.IsSuccessful == false)
            {
                throw new Exception("There was an error setting the state of a light.");
            }
        }
    }
}