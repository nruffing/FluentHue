using System.Collections.Generic;
using System.Threading.Tasks;

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

        /// <summary>
        /// Sets the user to use when communicating with the bridge. This is required to be called
        /// before any further communication with the bridge.
        /// </summary>
        /// <param name="user">The Hue user that has permission to communicate with this bridge.</param>
        /// <returns>The current bridge.</returns>
        IHueBridge WithUser(string user);

        /// <summary>
        /// Asynchronously gets all lights connected to the bridge.
        /// </summary>
        /// <returns>All lights connected to the bridge.</returns>
        Task<IEnumerable<IHueLight>> GetAllLightsAsync();
        
        /// <summary>
        /// Gets all lights connected to the bridge.
        /// </summary>
        /// <returns>All lights connected to the bridge.</returns>
        IEnumerable<IHueLight> GetAllLights();

        /// <summary>
        /// Selects the light with the specified name.
        /// </summary>
        /// <param name="name">The name of the light to select.</param>
        /// <returns>The light with the specified name.</returns>
        IHueLight SelectLight(string name);
    }
}