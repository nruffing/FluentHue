namespace FluentHue
{
    using FluentHue.Contracts;
    using System;
    using Validation;

    /// <summary>
    /// Represents a Philips Hue light bulb.
    /// </summary>
    public sealed class HueLight : IHueLight
    {
        /// <summary>
        /// The bridge this light is connected to.
        /// </summary>
        private IHueBridge _bridge;

        /// <summary>
        /// The id that will be used to identify this light bulb when communicating with the bridge.
        /// </summary>
        private string _id;

        /// <summary>
        /// Initializes a new <see cref="HueLight"/>.
        /// </summary>
        /// <param name="bridge">The bridge the light is connected to.</param>
        /// <param name="id">The id that the bridge identifies this light as.</param>
        /// <param name="metadata">The light's metadata.</param>
        public HueLight(IHueBridge bridge, string id, HueLightMetadata metadata)
        {
            Requires.NotNull(bridge, nameof(bridge));
            Requires.NotNullOrWhiteSpace(id, nameof(id));
            Requires.NotNull(metadata, nameof(metadata));

            this._bridge = bridge;
            this._id = id;

            this.Name = metadata.Name;
        }

        /// <summary>
        /// Return to the bridge.
        /// </summary>
        /// <returns>The bridge.</returns>
        public IHueBridge End() => this._bridge;

        /// <summary>
        /// Gets the name of the light.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the current state of the light.
        /// </summary>
        /// <returns>The current state of the light.</returns>
        public IHueLightState GetCurrentState()
        {
            throw new NotImplementedException();
        }
    }
}