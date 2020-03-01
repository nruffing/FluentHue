namespace FluentHue.Contracts
{
    using Newtonsoft.Json;
    
    /// <summary>
    /// Model containing the state of a Philips Hue light bulb.
    /// </summary>
    internal sealed class HueLightStateContract
    {
        [JsonProperty("on")]
        public bool IsOn { get; set; }

        [JsonProperty("bri")]
        public byte Brightness { get; set; }

        [JsonProperty("xy")]
        public float[] Color { get; set; }
    }
}