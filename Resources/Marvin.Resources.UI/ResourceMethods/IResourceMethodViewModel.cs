using System.ComponentModel;
using Marvin.Serialization;

namespace Marvin.Resources.UI
{
    /// <summary>
    /// Interface for view models representing a resource method
    /// </summary>
    public interface IResourceMethodViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Id of the resource offering this method
        /// </summary>
        long ResourceId { get; }

        /// <summary>
        /// Name of the method on the class
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Name the button should have
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Optional description of the method for button-mouseover
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Parameters of the method
        /// </summary>
        Entry[] Parameters { get; }
    }
}