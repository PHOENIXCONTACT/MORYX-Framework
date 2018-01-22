using System.ComponentModel;

namespace Marvin.Resources.UI
{
    /// <summary>
    /// Interface for the constructor view model because of reasons...
    /// </summary>
    public interface IConstructorViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// At least some property
        /// </summary>
        string DisplayName { get; }
    }
}