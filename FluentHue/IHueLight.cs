namespace FluentHue
{
    using System.Threading.Tasks;
    
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
        /// Asynchronously renames the current light.
        /// </summary>
        /// <param name="name">The new name.</param>
        /// <returns>The current light.</returns>
        Task<IHueLight> RenameAsync(string name);

        /// <summary>
        /// Renames the current light.
        /// </summary>
        /// <param name="name">The new name.</param>
        /// <returns>The current light.</returns>
        IHueLight Rename(string name); 

        /// <summary>
        /// Asynchronously gets the current state of the light.
        /// </summary>
        /// <returns>The current state of the light.</returns>
        Task<IHueLightState> GetCurrentStateAsync();

        /// <summary>
        /// Gets the current state of the light.
        /// </summary>
        /// <returns>The current state of the light.</returns>
        IHueLightState GetCurrentState();
    }
}