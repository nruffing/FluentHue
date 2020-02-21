using Newtonsoft.Json;

namespace FluentHue.Contracts
{
    /// <summary>
    /// Model containing metadata for a Philips Hue light bulb's state.
    /// </summary>
    internal sealed class HueLightStateMetadata
    {
        [JsonProperty("on")]
        public bool On { get; set; }

        [JsonProperty("bri")]
        public byte Brightness { get; set; }
    }
}