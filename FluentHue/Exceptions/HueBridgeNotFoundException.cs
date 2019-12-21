namespace FluentHue.Exceptions
{
    using System;

    /// <summary>
    /// An exception to be thrown when no Hue bridges were found on the local network.
    /// </summary>
    public sealed class HueBridgeNotFoundException : Exception
    {
        /// <summary>
        /// Gets a message that describes the current exception.
        /// </summary>
        public override string Message => "Could not find a Hue bridge on the local network.";
    }
}