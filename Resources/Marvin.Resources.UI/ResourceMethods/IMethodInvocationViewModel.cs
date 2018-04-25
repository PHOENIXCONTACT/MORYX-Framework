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
        /// Result returned from the web-service
        /// </summary>
        Entry ResultEntry { get; }
    }
}