using System;
using System.ComponentModel;
using System.Windows.Input;
using Caliburn.Micro;
using Marvin.ClientFramework.Commands;

namespace Marvin.AbstractionLayer.UI
{
    /// <summary>
    /// Interface for a viewmodel with an edit mode
    /// </summary>
    public interface IEditModeViewModel : IScreen, IEditableObject
    {
        /// <summary>
        /// Will trigger the edit mode
        /// </summary>
        void EnterEditMode();

        /// <summary>
        /// <c>true</c> if view model is in edit mode
        /// </summary>
        bool IsEditMode { get; }

        /// <summary>
        /// Command to enter the edit mode for this current view
        /// </summary>
        ICommand EditModeCmd { get; }

        /// <summary>
        /// Command to cancel the edit mode for the current view
        /// </summary>
        ICommand CancelEditCmd { get; }

        /// <summary>
        /// Command to save the current work and exit the edit mode
        /// </summary>
        IAsyncCommand SaveCmd { get; }

        /// <summary>
        /// Occurs when resource saved
        /// </summary>
        event EventHandler Saved;

        /// <summary>
        /// Will be raised when the edit mode of this view model changed
        /// </summary>
        event EventHandler<EditModeChange> EditModeChanged;

        /// <summary>
        /// Will be called if the busy mode changed
        /// </summary>
        event EventHandler<bool> BusyChanged;
    }
}
