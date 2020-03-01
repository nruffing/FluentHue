namespace FluentHue.Contracts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Model containing the contract for initiating a search for new Hue lights.
    /// </summary>
    internal sealed class DiscoverNewHueLightsContract
    {
        [JsonProperty("deviceid")]
        public string[] SerialNumbers { get; set; }
    }
}