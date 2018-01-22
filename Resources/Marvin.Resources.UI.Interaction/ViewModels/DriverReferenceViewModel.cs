using System.Collections.ObjectModel;
using System.Windows;
using C4I;
using Marvin.ClientFramework.Base;
using Marvin.Resources.UI.Interaction.ResourceInteraction;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// ViewModel to represent Driver references
    /// </summary>
    public class DriverReferenceViewModel : ValidationViewModelBase
    {
        #region Fields

        private ObservableCollection<DriverViewModel> _possibleDrivers = new ObservableCollection<DriverViewModel>();
        private ObservableCollection<DriverViewModel> _selectedDrivers = new ObservableCollection<DriverViewModel>();
        private DriverReferenceType _referenceType;
        private DriverViewModel _selectedPossibleDriver;
        private DriverViewModel _selectedFromSelectedDriversList;
        private DriverReference _model;

        /// <summary>
        /// Command to add a selected driver to list of selected drivers
        /// </summary>
        public DelegateCommand SelectDriverCmd { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverReferenceViewModel"/> class.
        /// </summary>
        public DriverReferenceViewModel()
        {
            SelectDriverCmd = new DelegateCommand(SelectDriver);
        }

        private void SelectDriver(object parameter)
        {
            if (_referenceType == DriverReferenceType.FixedSize && SelectedDrivers.Count >= _model.Size)
                return;

            var driver = ((DriverViewModel)parameter).CopyToModel();
            var selDrvVm = new DriverViewModel();
            selDrvVm.CopyFromModel(driver);
            selDrvVm.UnSelectDriverCmd = new RelayCommand(unSelDrv =>
            {
                SelectedDrivers.Remove((DriverViewModel)unSelDrv);
                NotifyOfPropertyChange(() => SelectDriverActionEnabled);
            });
            SelectedDrivers.Add(selDrvVm);
            NotifyOfPropertyChange(() => SelectDriverActionEnabled);
        }

        
        #region To & From Model

        /// <summary>
        /// Copy the model data to viewmodels
        /// </summary>
        internal void CopyFromModel(DriverReference model)
        {
            _model = model;
            _referenceType = model.ReferenceType;
            
            if (model.PossibleDrivers != null)
            {
                foreach (Driver drv in model.PossibleDrivers)
                {
                    var drvVm = new DriverViewModel();
                    drvVm.CopyFromModel(drv);
                    PossibleDrivers.Add(drvVm);
                }
            }

            if (model.SelectedDrivers != null)
            {
                foreach (Driver drv in model.SelectedDrivers)
                {
                    var drvVm = new DriverViewModel();
                    drvVm.CopyFromModel(drv);
                    SelectedDrivers.Add(drvVm);
                }
            }
        }

        /// <summary>
        /// Write the data from the viewmodel back to the model
        /// </summary>
        internal DriverReference CopyToModel()
        {
            var result = new DriverReference
            {
                Size = _model.Size,
                ReferenceType = _referenceType,
                PossibleDrivers = new Driver[_possibleDrivers.Count],
                SelectedDrivers = new Driver[_selectedDrivers.Count]
            };

            for (int i = 0; i<_possibleDrivers.Count ; i++)
            {
                result.PossibleDrivers[i] = _possibleDrivers[i].CopyToModel();
            }

            for (int i = 0; i<_selectedDrivers.Count ; i++)
            {
                result.SelectedDrivers[i] = _selectedDrivers[i].CopyToModel();
            }
            return result;
        }

        #endregion

        /// <summary>
        /// List of drivers the user can choose from to use for this reference
        /// </summary>
        public ObservableCollection<DriverViewModel> PossibleDrivers 
        {
            get { return _possibleDrivers; }
            set 
            {
                _possibleDrivers = value;
                NotifyOfPropertyChange();
            }
        }
        
        /// <summary>
        /// List of drivers that have been selected to be used by this driver reference
        /// </summary>
        public ObservableCollection<DriverViewModel> SelectedDrivers 
        {
            get { return _selectedDrivers; }
            set 
            {
                _selectedDrivers = value;
                NotifyOfPropertyChange();
            }
        }
        /// <summary>
        /// Currently selected Driver from the PossibleDrivers list
        /// </summary>
        public DriverViewModel SelectedPossibleDriver
        {
            get { return _selectedPossibleDriver; }
            set
            {
                _selectedPossibleDriver = value; 
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => SelectDriverActionEnabled);
            }
        }

        /// <summary>
        /// Visibility of the driver detail area
        /// </summary>
        public Visibility DriverDetailsVisibility
        {
            get { return SelectedFromSelectedDriversList != null ? Visibility.Visible : Visibility.Collapsed; }
        }

        /// <summary>
        /// Currently selected Driver from the SelectedDrivers list
        /// </summary>
        public DriverViewModel SelectedFromSelectedDriversList
        {
            get { return _selectedFromSelectedDriversList; }
            set
            {
                _selectedFromSelectedDriversList = value; 
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => DriverDetailsVisibility);
            }
        }
        
        /// <summary>
        /// Make the Text for FixedSize visible
        /// </summary>
        public Visibility FixedSizeVisibility
        {
            get  { return _referenceType == DriverReferenceType.FixedSize ? Visibility.Visible : Visibility.Collapsed; }
        }

        /// <summary>
        /// Text that will be shown for References of type: <see cref="DriverReferenceType"/>.FixedType
        /// </summary>
        public string SizeText
        {
            get { return "This reference has a fixed number of drivers: " + _model.Size; }
        }

        /// <summary>
        /// Check if the Add-To-SelectedDrivers-Button may be enabled
        /// </summary>
        public bool SelectDriverActionEnabled
        {
            get { return SelectedPossibleDriver != null && (_referenceType != DriverReferenceType.FixedSize || _model.Size > SelectedDrivers.Count); }
        }
    }
}