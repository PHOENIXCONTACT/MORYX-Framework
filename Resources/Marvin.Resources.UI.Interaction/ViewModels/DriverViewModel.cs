using C4I;
using Marvin.Resources.UI.Interaction.ResourceInteraction;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// ViewModel to represent Drivers
    /// </summary>
    public class DriverViewModel : ValidationViewModelBase
    {
        private long _id;
        private string _name;

        #region To & From Model
        /// <summary>
        /// Copy the model data to viewmodel
        /// </summary>
        internal void CopyFromModel(Driver model)
        {
            Id = model.Id;
            Name = model.Name;
        }

        /// <summary>
        /// Write the data from the viewmodel back to the model
        /// </summary>
        internal Driver CopyToModel()
        {
            var result = new Driver {Id = _id, Name = _name};
            return result;
        }
        #endregion

        /// <summary>
        /// Database Id of the driver. Doesn't need to be shown on the UI though...
        /// </summary>
        public long Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                NotifyOfPropertyChange();
            }
            
        }

        /// <summary>
        /// Name of the driver
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Command that can be executed to remove this driver from the list of selected drivers
        /// </summary>
        public RelayCommand UnSelectDriverCmd { get; set; }
    }
}
