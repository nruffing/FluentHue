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
        /// Gets a value representing the brightness of the light on a scale of 1-254.
        /// </summary>
        byte Brightness { get; }

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
        /// Asynchronously sets the brightness of the light to the specified level.
        /// </summary>
        /// <param name="brightness">The level of brightness to set. This must be within the
        /// inclusive range of 1-254.</param>
        /// <returns>The current light state.</returns>
        Task<IHueLightState> SetBrightnessAsync(byte brightness);

        /// <summary>
        /// Sets the brightness of the light to the specified level.
        /// </summary>
        /// <param name="brightness">The level of brightness to set. This must be within the
        /// inclusive range of 1-254.</param>
        /// <returns>The current light state.</returns>
        IHueLightState SetBrightness(byte brightness);

        /// <summary>
        /// Returns to the light.
        /// </summary>
        /// <returns>The light.</returns>
        IHueLight End();
    }
}