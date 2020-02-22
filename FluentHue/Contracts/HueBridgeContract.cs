namespace FluentHue.Contracts
{
    /// <summary>
    /// Model containing metadata for a Phillips Hue bridge.
    /// </summary>
    internal sealed class HueBridgeContract
    {
        /// <summary>
        /// The id of the bridge (e.g. 001788fffe4c2912).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The private IP address of the bridge on its local network.
        /// </summary>
        public string InternalIpAddress { get; set; }
    }
}