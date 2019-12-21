namespace FluentHue
{
    public interface IHueBridge
    {
        /// <summary>
        /// The id of the bridge (e.g. 001788fffe4c2912).
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The private IP address of the bridge on its local network.
        /// </summary>
        string LocalIpAddress { get; }
    }
}