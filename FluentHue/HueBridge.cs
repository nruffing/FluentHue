namespace FluentHue
{
    using FluentHue.Contracts;
    using Validation;

    /// <summary>
    /// Represents a Philips Hue bridge.
    /// </summary>
    public sealed class HueBridge : IHueBridge
    {
        /// <summary>
        /// Initializes a new <see cref="HueBridge"/>.
        /// </summary>
        /// <param name="metadata">The metadata found during bridge discovery.</param>
        internal HueBridge(HueBridgeMetadata metadata)
        {
            Requires.NotNull(metadata, nameof(metadata));
            Requires.NotNullOrWhiteSpace(metadata.Id, nameof(metadata.Id));
            Requires.NotNullOrWhiteSpace(metadata.InternalIpAddress, nameof(metadata.InternalIpAddress));

            this.Id = metadata.Id;
            this.LocalIpAddress = metadata.InternalIpAddress;
        }

        /// <summary>
        /// Initializes a new <see cref="HueBridge"/>.
        /// </summary>
        /// <param name="localIpAddress">The private IP address of the bridge on its local network.</param>
        internal HueBridge(string localIpAddress)
        {
            Requires.NotNullOrWhiteSpace(localIpAddress, nameof(localIpAddress));

            this.LocalIpAddress = localIpAddress;
        }

        /// <summary>
        /// The id of the bridge (e.g. 001788fffe4c2912). This is not when explicitly providing
        /// the internal IP address (e.g. HueBrigeLocator.SelectWithLocalIp).
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The private IP address of the bridge on its local network.
        /// </summary>
        public string LocalIpAddress { get; }
    }
}