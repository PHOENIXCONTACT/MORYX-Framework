using Caliburn.Micro;
using Marvin.ClientFramework.Dialog;
using Marvin.Container;

namespace Marvin.Resources.UI
{
    /// <summary>
    /// Factory to create a resource type view model
    /// </summary>
    [PluginFactory]
    public interface IResourceDialogFactory
    {
        /// <summary>
        /// Creates a new resource type view model
        /// </summary>
        ITypeSelectorViewModel CreateTypeSelector();

        /// <summary>
        /// Create a method invocation dialog screen for a given method view model
        /// </summary>
        IMethodInvocationViewModel CreateMethodInvocation(IResourceMethodViewModel methodViewModel);

        /// <summary>
        /// Destroys the specified type view model.
        /// </summary>
        void Destroy(IDialogScreen dialog);
    }
}