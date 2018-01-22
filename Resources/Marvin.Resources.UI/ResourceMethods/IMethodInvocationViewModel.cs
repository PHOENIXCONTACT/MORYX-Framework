using Caliburn.Micro;
using Marvin.ClientFramework.Dialog;
using Marvin.Controls;
using Marvin.Serialization;

namespace Marvin.Resources.UI
{
    /// <summary>
    /// Interface for the dialog screen to invoke methods
    /// </summary>
    public interface IMethodInvocationViewModel : IDialogScreen
    {
        /// <summary>
        /// Return value of the method invocation
        /// </summary>
        EntryViewModel InvocationResult { get; }
    }
}