namespace FluentHue.Contracts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Model containing metadata for a Philips Hue light bulb.
    /// </summary>
    internal sealed class HueLightContract
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}