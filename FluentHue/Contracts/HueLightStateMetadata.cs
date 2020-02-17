using Newtonsoft.Json;

namespace FluentHue.Contracts
{
    /// <summary>
    /// Model containing metadata for a Philips Hue light bulb's state.
    /// </summary>
    public sealed class HueLightStateMetadata
    {
        /// <summary>
        /// Gets or sets a value indicating whether the light is on.
        /// </summary>
        public bool On { get; set; }
    }
}