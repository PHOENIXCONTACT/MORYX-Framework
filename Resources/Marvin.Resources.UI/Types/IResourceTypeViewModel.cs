using System.Collections.Generic;

namespace Marvin.Resources.UI
{
    /// <summary>
    /// Interface for the type viewmodel
    /// </summary>
    public interface IResourceTypeViewModel
    {
        /// <summary>
        /// The name of the resource 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Flag if this type can be instantiated
        /// </summary>
        bool Creatable { get; }

        /// <summary>
        /// The children of the current tree item
        /// </summary>
        IEnumerable<IResourceTypeViewModel> DerivedTypes { get; }
    }
}