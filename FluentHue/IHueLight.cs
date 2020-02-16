namespace FluentHue
{
    /// <summary>
    /// Represents a Philips Hue light bulb.
    /// </summary>
    public interface IHueLight
    {
        /// <summary>
        /// Gets the name of the light.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Return to the bridge.
        /// </summary>
        /// <returns>The bridge.</returns>
        IHueBridge End();

        /// <summary>
        /// Gets the current state of the light.
        /// </summary>
        /// <returns>The current state of the light.</returns>
        IHueLightState GetCurrentState();
    }
}