using Newtonsoft.Json;

namespace FluentHue.Contracts
{
    /// <summary>
    /// Model containing metadata for a Philips Hue light bulb.
    /// </summary>
    internal sealed class HueLightContract
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}