using Caliburn.Micro;
using Marvin.Controls;
using Marvin.Resources.UI.Interaction.ResourceInteraction;
using Marvin.Serialization;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// View model that represents a constructor
    /// </summary>
    public class ConstructorViewModel : PropertyChangedBase, IConstructorViewModel
    {
        internal MethodEntry Constructor { get; }

        /// <summary>
        /// Create new constructor view model from constructor method view model
        /// </summary>
        internal ConstructorViewModel(MethodEntry constructor)
        {
            Constructor = constructor;
            Parameters = new EntryViewModel(constructor.Parameters);
        }

        /// <summary>
        /// Display name of this constructor
        /// </summary>
        public string DisplayName => Constructor.DisplayName;

        private bool _isSelected;
        /// <summary>
        /// Flag if this constructor was selected
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value == _isSelected)
                    return;
                _isSelected = value;
                NotifyOfPropertyChange();
            }
        }

        private EntryViewModel _parameters;
        /// <summary>
        /// View model of the parameters for the config editor
        /// </summary>
        public EntryViewModel Parameters
        {
            get { return _parameters; }
            set
            {
                if (Equals(value, _parameters))
                    return;
                _parameters = value;
                NotifyOfPropertyChange();
            }
        }
    }
}