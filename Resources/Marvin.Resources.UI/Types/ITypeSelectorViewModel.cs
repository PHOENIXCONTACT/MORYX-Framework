using System.Collections.Generic;
using Marvin.ClientFramework.Dialog;

namespace Marvin.Resources.UI
{
    /// <summary>
    /// View model interface for the resource type selection
    /// </summary>
    public interface ITypeSelectorViewModel : IDialogScreen
    {
        /// <summary>
        /// The selected resource type
        /// </summary>
        string SelectedTypeName { get; }

        /// <summary>
        /// Constructor that was selected
        /// </summary>
        IConstructorViewModel Constructor { get; }

        /// <summary>
        /// Tree of resource types
        /// </summary>
        IEnumerable<IResourceTypeViewModel> TypeTree { get; set; }
    }
}