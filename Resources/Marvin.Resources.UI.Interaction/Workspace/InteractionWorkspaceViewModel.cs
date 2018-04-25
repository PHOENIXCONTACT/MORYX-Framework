using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Marvin.AbstractionLayer.UI;
using Marvin.ClientFramework.Base;
using Marvin.Container;
using Marvin.Modules.Client;
using Marvin.Serialization;
using Marvin.Tools;
using MessageBoxImage = Marvin.ClientFramework.Dialog.MessageBoxImage;
using MessageBoxOptions = Marvin.ClientFramework.Dialog.MessageBoxOptions;

namespace Marvin.Resources.UI.Interaction
{
    [Plugin(LifeCycle.Singleton, typeof(IModuleWorkspace), Name = WorkspaceName)]
    internal class InteractionWorkspaceViewModel : MasterDetailsWorkspace<IResourceDetails, IResourceDetailsFactory, EmptyDetailsViewModel>
    {
        internal const string WorkspaceName = "InteractionWorkspaceViewModel";

        #region Dependencies

        public IResourceController ResourceController { get; set; }

        public IResourceDialogFactory TypeSelectorFactory { get; set; }

        #endregion

        #region Fields and Properties

        public DelegateCommand AddResourceCmd { get; private set; }
        public AsyncCommand RemoveResourceCmd { get; private set; }

        public ResourceTreeItemViewModel[] ResourceTree { get; private set; }

        private ResourceTreeItemViewModel _selectedResource;
        public ResourceTreeItemViewModel SelectedResource
        {
            get { return _selectedResource; }
            set
            {
                if (Equals(value, _selectedResource))
                    return;
                _selectedResource = value;
                NotifyOfPropertyChange();
                RemoveResourceCmd.RaiseCanExecuteChanged();
                AddResourceCmd.RaiseCanExecuteChanged();
            }
        }

        #endregion

        ///
        protected override void OnInitialize()
        {
            base.OnInitialize();

            AddResourceCmd = new DelegateCommand(AddResource, CanAddResource);
            RemoveResourceCmd = new AsyncCommand(RemoveResource, CanRemoveResource);

            // register at controller events
            ResourceController.ResourceTreeUpdated += OnResourceTreeUpdated;

            //refresh resource tree
            RefreshResourceTree();
        }

        /// <summary>
        /// Called when the resource tree was updated updated
        /// </summary>
        private void OnResourceTreeUpdated(object sender, EventArgs eventArgs)
        {
            RefreshResourceTree();
        }

        /// <summary>
        /// Refreshes the resource tree and loads all data from the controller.
        /// </summary>
        private void RefreshResourceTree()
        {
            var oldTree = ResourceTree;

            IsMasterBusy = false;
            var refreshedTree = ResourceController.ResourceTree.Select(r => new ResourceTreeItemViewModel(r)).ToArray();

            //Merge old tree to the new loaded tree
            if (oldTree != null)
            {
                var oldFlatTree = oldTree.Flatten(t => t.Children).ToList();
                var newFlatTree = refreshedTree.Flatten(t => t.Children).ToList();

                foreach (var newTreeItem in newFlatTree)
                {
                    var oldTreeItem = oldFlatTree.FirstOrDefault(t => t.Id == newTreeItem.Id);
                    if (oldTreeItem == null)
                        continue;

                    newTreeItem.IsExpanded = oldTreeItem.IsExpanded;
                    newTreeItem.IsSelected = oldTreeItem.IsSelected;
                }
            }

            ResourceTree = refreshedTree;
            NotifyOfPropertyChange(() => ResourceTree);
        }

        /// <summary>
        /// Called from the resource tree if an selected item changed
        /// </summary>
        public override Task OnMasterItemChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
        {
            return SelectResource((ResourceTreeItemViewModel)args.NewValue);
        }

        /// <summary>
        /// Select <see cref="ResourceTreeItemViewModel"/> and load the details with <see cref="LoadDetails"/>
        /// </summary>
        private async Task SelectResource(ResourceTreeItemViewModel resource)
        {
            if (resource == null)
            {
                SelectedResource = null;
                ShowEmpty();
                return;
            }

            SelectedResource = resource;

            //Select view model for the right resource type
            var detailsVm = DetailsFactory.Create(SelectedResource.Type);
            await LoadDetails(async delegate { await detailsVm.Load(_selectedResource.Id); });

            ActivateItem(detailsVm);
        }

        /// <summary>
        /// Determines whethera new resource can be created as child of the selected resource
        /// </summary>
        private bool CanAddResource(object parameters)
        {
            return IsDetailsInEditMode == false;
        }

        /// <summary>
        /// Will add a resource by type wich will be selected via a dialog
        /// </summary>
        private void AddResource(object parameters)
        {
            var typeDialog = TypeSelectorFactory.CreateTypeSelector();

            typeDialog.TypeTree = SelectedResource == null 
                ? ResourceController.TypeTree.Select(type => new ResourceTypeViewModel(type)) 
                : SelectedResource.PossibleChildren;

            DialogManager.ShowDialog(typeDialog, delegate
            {
                if (typeDialog.Result == false || string.IsNullOrEmpty(typeDialog.SelectedTypeName))
                    return;

                var resourceTypeName = typeDialog.SelectedTypeName;
                var constructor = typeDialog.Constructor as ConstructorViewModel;
                TypeSelectorFactory.Destroy(typeDialog);

                CreateResource(resourceTypeName, constructor?.Constructor);
            });
        }

        /// <summary>
        /// Will create a resource by the given type name and will show the details view
        /// </summary>
        private void CreateResource(string resourceTypeName, MethodEntry constructorModel)
        {
            long parentId = 0;

            if (_selectedResource != null)
                parentId = _selectedResource.Id;

            var detailsVm = DetailsFactory.Create(resourceTypeName);
            
            Task.Run(async delegate
            {
                await LoadDetails(async delegate
                {
                    await detailsVm.Create(resourceTypeName, parentId, constructorModel);
                });
                detailsVm.EnterEditMode();

                ActivateItem(detailsVm);
            });
        }

        /// <summary>
        /// Determines weather new resource can be deleted or not
        /// </summary>
        private bool CanRemoveResource(object parameters)
        {
            return IsDetailsInEditMode == false && SelectedResource != null;
        }

        /// <summary>
        /// Removes the given resource.
        /// </summary>
        private async Task RemoveResource(object parameters)
        {
            var result = await ResourceController.RemoveResource(SelectedResource.Id);
            if (result == false)
            {
                DialogManager.ShowMessageBox("There was an error occured on the server, while removing the resource.",
                    "Error while removing resource", MessageBoxOptions.Ok, MessageBoxImage.Error);
            }
            ResourceController.UpdateTree();
        }

        /// 
        protected override void OnDetailsEditModeChanged(object sender, EditModeChange changeMode)
        {
            base.OnDetailsEditModeChanged(sender, changeMode);

            RemoveResourceCmd.RaiseCanExecuteChanged();
            AddResourceCmd.RaiseCanExecuteChanged();

            if (changeMode != EditModeChange.Chanceled)
                return;

            if (CurrentDetails.CurrentResourceId == 0)
                Task.Run(() => SelectResource(_selectedResource));
        }

        /// 
        protected override void OnDetailsSaved(object sender, EventArgs eventArgs)
        {
            ResourceController.UpdateTree();
        }

        /// 
        protected override void ShowEmpty()
        {
            EmptyDetails.Display(MessageSeverity.Info, "Please select a resource from the tree on the left side.");
            base.ShowEmpty();
        }
    }
}