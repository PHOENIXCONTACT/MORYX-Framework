using Moryx.AbstractionLayer.Drivers;
using System.Threading.Tasks;

namespace Moryx.Drivers.Camera.Interfaces
{
    /// <summary>
    /// Interface for objects that register as listeners to camera drivers
    /// </summary>
    public interface ICameraDriverListener<T> where T : class
    {
        /// <summary>
        /// Invoked, when a new image is received by the camera
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        Task OnImage(T image);
    }
}
