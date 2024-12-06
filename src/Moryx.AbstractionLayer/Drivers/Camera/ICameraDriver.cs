using Moryx.AbstractionLayer.Drivers;
using System.Threading.Tasks;

namespace Moryx.Drivers.Camera.Interfaces
{
    /// <summary>
    /// Interface for camera devices, that provide image data
    /// </summary>
    public interface ICameraDriver<TImage> : IDriver where TImage : class
    {
        /// <summary>
        /// Registers an ICameraDriverListener that should be provided
        /// with images.
        /// </summary>
        void Register(ICameraDriverListener<TImage> listener);

        /// <summary>
        /// Unregisters an ICameraDriverListener
        /// </summary>
        void Unregister(ICameraDriverListener<TImage> listener);

        /// <summary>
        /// Capture a single image from the camera
        /// </summary>
        /// <returns>
        ///     The image that was captured or null in case no image 
        ///     could be retrieved
        /// </returns>
        Task<TImage?> CaptureImage();
    }
}
