using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Marvin.ClientFramework.Base;
using Marvin.Resources.UI.Interaction.ResourceInteraction;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// View model representing a reference property on a resource
    /// </summary>
    public abstract class ReferenceViewModel : ValidationViewModelBase
    {
        #region Fields

        internal ResourceReferenceModel Model { get; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceViewModel"/> class.
        /// </summary>
        internal ReferenceViewModel(ResourceReferenceModel reference)
        {
            Model = reference;

            foreach (var target in Model.PossibleTargets)
            {
                var drvVm = new ResourceViewModel(target);
                PossibleTargets.Add(drvVm);
            }
        }

        /// <summary>
        /// Create a reference view model
        /// </summary>
        internal static ReferenceViewModel Create(ResourceReferenceModel reference)
        {
            return reference.IsCollection
                ? (ReferenceViewModel)new MultiReferenceViewModel(reference)
                : new SingleReferenceViewModel(reference);
        }

        /// <summary>
        /// Name of the reference
        /// </summary>
        public string Name => Model.Name;

        /// <summary>
        /// This reference is a collection
        /// </summary>
        public bool IsCollection => Model.IsCollection;

        private ObservableCollection<ResourceViewModel> _possibleTargets = new ObservableCollection<ResourceViewModel>();
        /// <summary>
        /// List of drivers the user can choose from to use for this reference
        /// </summary>
        public ObservableCollection<ResourceViewModel> PossibleTargets
        {
            get { return _possibleTargets; }
            set
            {
                _possibleTargets = value;
                NotifyOfPropertyChange();
            }
        }
    }

    /// <summary>
    /// View model for resource references that only reference a single target
    /// </summary>
    public class SingleReferenceViewModel : ReferenceViewModel
    {
        internal SingleReferenceViewModel(ResourceReferenceModel model) : base(model)
        {
            if (model.Targets.Count == 1)
                SelectedTarget = PossibleTargets.First(p => p.Name == model.Targets[0].Name);
        }

        private ResourceViewModel _selectedTarget;
        /// <summary>
        /// Currently selected Driver from the PossibleDrivers list
        /// </summary>
        public ResourceViewModel SelectedTarget
        {
            get { return _selectedTarget; }
            set
            {
                _selectedTarget = value;
                if (value == null)
                    Model.Targets.Clear();
                else if (Model.Targets.Count == 0)
                    Model.Targets.Add(value.Model);
                else
                    Model.Targets[0] = value.Model;
                NotifyOfPropertyChange();
            }
        }
    }

    /// <summary>
    /// View model for resource reference that have multiple targets
    /// </summary>
    public class MultiReferenceViewModel : ReferenceViewModel
    {
        internal MultiReferenceViewModel(ResourceReferenceModel model) : base(model)
        {
            AddTarget = new DelegateCommand(AddToTargets, CanAddToTargets);
            RemoveTarget = new DelegateCommand(RemoveFromTargets, CanRemoveFromTargets);

            foreach (var target in Model.Targets)
            {
                var drvVm = new ResourceViewModel(target);
                SelectedTargets.Add(drvVm);
            }
        }

        /// <summary>
        /// Delegate to add targets to the reference
        /// </summary>
        public DelegateCommand AddTarget { get; set; }

        /// <summary>
        /// Delegate to remove targets from the reference
        /// </summary>
        public DelegateCommand RemoveTarget { get; set; }


        private ResourceViewModel _selectedPossibleTarget;
        /// <summary>
        /// Currently selected Driver from the PossibleDrivers list
        /// </summary>
        public ResourceViewModel SelectedPossibleTarget
        {
            get { return _selectedPossibleTarget; }
            set
            {
                _selectedPossibleTarget = value;
                AddTarget.RaiseCanExecuteChanged();
                NotifyOfPropertyChange();
            }
        }

        private ObservableCollection<ResourceViewModel> _selectedTargets = new ObservableCollection<ResourceViewModel>();
        /// <summary>
        /// List of drivers that have been selected to be used by this driver reference
        /// </summary>
        public ObservableCollection<ResourceViewModel> SelectedTargets
        {
            get { return _selectedTargets; }
            set
            {
                _selectedTargets = value;
                NotifyOfPropertyChange();
            }
        }

        private ResourceViewModel _selectedTarget;
        /// <summary>
        /// Currently selected Driver from the PossibleDrivers list
        /// </summary>
        public ResourceViewModel SelectedTarget
        {
            get { return _selectedTarget; }
            set
            {
                _selectedTarget = value;
                RemoveTarget.RaiseCanExecuteChanged();
                NotifyOfPropertyChange();
            }
        }

        private bool CanAddToTargets(object unused) => SelectedPossibleTarget != null;

        private void AddToTargets(object unused)
        {
            // Update underlying model
            Model.Targets.Add(SelectedPossibleTarget.Model);
            // Update UI
            SelectedTargets.Add(SelectedPossibleTarget);
        }

        private bool CanRemoveFromTargets(object unused) => SelectedTarget != null;

        private void RemoveFromTargets(object unused)
        {
            // Remove from Model
            Model.Targets.Remove(SelectedTarget.Model);
            // Update UI
            SelectedTargets.Remove(SelectedTarget);
        }
    }
}