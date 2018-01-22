using System;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Marvin.ClientFramework.Base;
using Marvin.Controls;
using Marvin.Resources.UI.Interaction.ResourceInteraction;
using Marvin.Serialization;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// View model that represents a EditorVisible method of a resource
    /// </summary>
    public class ResourceMethodViewModel : PropertyChangedBase, IResourceMethodViewModel
    {
        private readonly ResourceDetailsViewModelBase _parent;

        internal MethodEntry Model { get; }

        /// <summary>
        /// Create view model without invocation
        /// </summary>
        internal ResourceMethodViewModel(MethodEntry model) : this(model, null)
        {
        }

        /// <summary>
        /// Create view model from model
        /// </summary>
        internal ResourceMethodViewModel(MethodEntry model, ResourceDetailsViewModelBase parent)
        {
            _parent = parent;
            Model = model;
            ExecuteCommand = new AsyncCommand(InvokeMethod);
        }

        /// <summary>
        /// Id of the resource offering this method
        /// </summary>
        public long ResourceId => _parent.CurrentResourceId;

        /// <summary>
        /// Name of the method on the class
        /// </summary>
        public string Name => Model.Name;

        /// <summary>
        /// Name the button should have
        /// </summary>
        public string DisplayName => Model.DisplayName;

        /// <summary>
        /// Optional description of the method for button-mouseover
        /// </summary>
        public string Description => Model.Description;

        /// <summary>
        /// Parameters of the method
        /// </summary>
        public Entry[] Parameters => Model.Parameters;

        /// <summary>
        /// ViewModel for the ConfigEditor
        /// </summary>
        public EntryViewModel EditorViewModel { get; set; }

        /// <summary>
        /// Command to execute this method
        /// </summary>
        public AsyncCommand ExecuteCommand { get; set; }

        private Task InvokeMethod(object parameter)
        {
            return _parent.InvokeResourceMethod(this);
        }
    }
}