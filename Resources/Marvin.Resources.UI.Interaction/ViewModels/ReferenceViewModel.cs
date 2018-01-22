using System.Collections.ObjectModel;
using System.Linq;
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
                NotifyOfPropertyChange();
            }
        }
    }

    internal class SingleReferenceViewModel : ReferenceViewModel
    {
        public SingleReferenceViewModel(ResourceReferenceModel model) : base(model)
        {
            if (model.Targets.Length == 1)
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
                if (Model.Targets.Length == 0)
                    Model.Targets = new[] { value.Model };
                else
                    Model.Targets[0] = value.Model;
                NotifyOfPropertyChange();
            }
        }
    }

    internal class MultiReferenceViewModel : ReferenceViewModel
    {
        public MultiReferenceViewModel(ResourceReferenceModel model) : base(model)
        {
            foreach (var target in Model.Targets)
            {
                var drvVm = new ResourceViewModel(target);
                SelectedTargets.Add(drvVm);
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
    }
}