using Newtonsoft.Json;

namespace FluentHue.Contracts
{
    /// <summary>
    /// Model containing metadata for a Philips Hue light bulb's state.
    /// </summary>
    internal sealed class HueLightStateMetadata
    {
        /// <summary>
        /// Gets or sets a value indicating whether the light is on.
        /// </summary>
        [JsonProperty("on")]
        public bool On { get; set; }
    }
}