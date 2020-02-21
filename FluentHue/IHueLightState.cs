using System.Threading.Tasks;

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

        /// <summary>
        /// Asynchronously toggles the current state of the light (e.g. off to on).
        /// </summary>
        /// <returns>The current light state.</returns>
        Task<IHueLightState> ToggleAsync();

        /// <summary>
        /// Toggles the current state of the light (e.g. off to on).
        /// </summary>
        /// <returns>The current light state.</returns>
        IHueLightState Toggle();

        /// <summary>
        /// Returns to the light.
        /// </summary>
        /// <returns>The light.</returns>
        IHueLight End();
    }
}