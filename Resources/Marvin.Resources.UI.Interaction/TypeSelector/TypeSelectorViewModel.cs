using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using C4I;
using Marvin.ClientFramework.Base;
using Marvin.Container;

namespace Marvin.Resources.UI.Interaction
{
    [Plugin(LifeCycle.Transient, typeof(ITypeSelectorViewModel))]
    internal class TypeSelectorViewModel : DialogScreen, ITypeSelectorViewModel
    {
        private ResourceTypeViewModel _selectedType;

        #region Fields and properties

        public ICommand CancelCmd { get; private set; }

        public DelegateCommand CreateCmd { get; private set; }

        /// <summary>
        /// Type tree is set depending on selected node
        /// </summary>
        public IEnumerable<IResourceTypeViewModel> TypeTree { get; set; }

        public ResourceTypeViewModel SelectedType
        {
            get { return _selectedType; }
            private set
            {
                if (Equals(value, _selectedType)) return;
                _selectedType = value;
                NotifyOfPropertyChange();
                CreateCmd.RaiseCanExecuteChanged();
            }
        }

        ///
        public string SelectedTypeName => SelectedType != null ? SelectedType.Name : string.Empty;

        public IConstructorViewModel Constructor => SelectedType?.Constructors.FirstOrDefault(c => c.IsSelected);

        #endregion

        ///
        protected override void OnInitialize()
        {
            base.OnInitialize();

            CreateCmd = new DelegateCommand(Create, CanCreate);
            CancelCmd = new DelegateCommand(Cancel);

            DisplayName = "Resource type selection:";
        }

        /// <summary>
        /// Checks if the resource type can be created <see cref="CreateCmd"/>
        /// </summary>
        private bool CanCreate(object obj)
        {
            return SelectedType != null && SelectedType.Creatable;
        }

        /// <summary>
        /// Will be called by <see cref="CreateCmd"/> and will return a true result
        /// </summary>
        private void Create(object obj)
        {
            TryClose(true);
        }

        /// <summary>
        /// Will be called by <see cref="CancelCmd"/> and will return a false result
        /// </summary>
        private void Cancel(object obj)
        {
            TryClose(false);
        }
        
        public void OnTreeItemChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
        {
            var selected = (ResourceTypeViewModel) args.NewValue;
            SelectedType = selected;
        }
    }
}