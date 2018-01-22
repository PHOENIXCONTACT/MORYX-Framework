using System.Collections.Generic;
using Caliburn.Micro;
using Marvin.Resources.UI.Interaction.ResourceInteraction;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// View model to handling the states of an resources
    /// </summary>
    public class ResourceStateEntryViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStateEntryViewModel"/> class.
        /// </summary>
        public ResourceStateEntryViewModel(string name, bool isCurrent)
        {
            Name = name;
            IsCurrent = isCurrent;
        }

        /// <summary>
        /// The name of the state
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Determines weather the state is the current state of the resource
        /// </summary>
        public bool IsCurrent { get; set; }
    }

    /// <summary>
    /// View model for the resource state
    /// </summary>
    public class ResourceStateViewModel : PropertyChangedBase
    {
        //private readonly ResourceState _model;
        private ResourceStateEntryViewModel _selectedEntry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStateViewModel"/> class.
        /// </summary>
        public ResourceStateViewModel() //: this(null)
        {
            
        }

        ///// <summary>
        ///// Initializes a new instance of the <see cref="ResourceStateViewModel"/> class.
        ///// </summary>
        //internal ResourceStateViewModel(ResourceState model)
        //{
        //    Entries = new List<ResourceStateEntryViewModel>();
        //    _model = model;

        //    if (_model == null) 
        //        return;

        //    foreach (var possibleState in _model.PossibleStates)
        //    {
        //        bool isCurrent = possibleState == _model.CurrentState;
        //        var entry = new ResourceStateEntryViewModel(possibleState, isCurrent);

        //        if (entry.IsCurrent)
        //            SelectedEntry = entry;

        //        Entries.Add(entry);
        //    }
        //}

        /// <summary>
        /// Represents the state entries
        /// </summary>
        public List<ResourceStateEntryViewModel> Entries { get; private set; }

        /// <summary>
        /// Gets or sets the selected entry.
        /// </summary>
        public ResourceStateEntryViewModel SelectedEntry
        {
            get { return _selectedEntry; }
            set
            {
                if (Equals(SelectedEntry, value))
                    return;

                _selectedEntry = value;
                NotifyOfPropertyChange(() => SelectedEntry);
                NotifyOfPropertyChange(() => WasChanged);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [was changed].
        /// </summary>
        public bool WasChanged
        {
            get { return SelectedEntry != null && SelectedEntry.IsCurrent == false; }
        }
    }
}