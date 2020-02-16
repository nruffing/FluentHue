namespace FluentHue
{
    /// <summary>
    /// Represents the state of a Philips Hue light bulb.
    /// </summary>
    public interface IHueLightState
    {
        /// <summary>
        /// Gets a value indicating whether the light is on.
        /// </summary>
        bool IsOn { get; }
    }
}