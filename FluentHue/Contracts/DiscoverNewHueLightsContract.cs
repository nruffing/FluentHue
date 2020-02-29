using Newtonsoft.Json;

namespace FluentHue.Contracts
{
    /// <summary>
    /// Model containing the contract for initiating a search for new Hue lights.
    /// </summary>
    internal sealed class DiscoverNewHueLightsContract
    {
        [JsonProperty("deviceid")]
        public string[] SerialNumbers { get; set; }
    }
}