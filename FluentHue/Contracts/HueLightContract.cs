namespace FluentHue.Contracts
{
    /// <summary>
    /// Model containing metadata for a Philips Hue light bulb.
    /// </summary>
    internal sealed class HueLightContract
    {
        /// <summary>
        /// Gets the name of the light.
        /// </summary>
        public string Name { get; set; }
    }
}