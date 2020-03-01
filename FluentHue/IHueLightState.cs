namespace FluentHue
{
    using System.Threading.Tasks;
    
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
        /// Gets a value representing the color's x coordinate (using CIE xy color).
        /// </summary>
        float ColorX { get; }

        /// <summary>
        /// Gets a value representing the color's y coordinate (using CIE xy color).
        /// </summary>
        float ColorY { get; }

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
        /// Asynchronously sets the state of light (i.e. on/off) to the specified value.
        /// </summary>
        /// <param name="isOn">Whether to set the state of the light to on, off otherwise.</param>
        /// <returns>The current light state.</returns>
        Task<IHueLightState> SetStateAsync(bool isOn);

        /// <summary>
        /// Sets the state of light (i.e. on/off) to the specified value.
        /// </summary>
        /// <param name="isOn">Whether to set the state of the light to on, off otherwise.</param>
        /// <returns>The current light state.</returns>
        IHueLightState SetState(bool isOn);

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
        /// Asynchronously sets the color of the light to specified color (using CIE xy color).
        /// </summary>
        /// <param name="x">The color's x coordinate. Must be between 0 and 1.</param>
        /// <param name="y">The color's y coordinate. Must be between 0 and 1.</param>
        /// <returns>The current light state.</returns>
        Task<IHueLightState> SetColorAsync(float x, float y);

        /// <summary>
        /// Sets the color of the light to specified color (using CIE xy color).
        /// </summary>
        /// <param name="x">The color's x coordinate. Must be between 0 and 1.</param>
        /// <param name="y">The color's y coordinate. Must be between 0 and 1.</param>
        /// <returns>The current light state.</returns>
        IHueLightState SetColor(float x, float y);

        /// <summary>
        /// Returns to the light.
        /// </summary>
        /// <returns>The light.</returns>
        IHueLight End();
    }
}