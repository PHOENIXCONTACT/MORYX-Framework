using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marvin.AbstractionLayer.UI;
using Marvin.Logging;
using Marvin.Resources.UI.Interaction.ResourceInteraction;
using Marvin.Serialization;

namespace Marvin.Resources.UI.Interaction
{
    /// <summary>
    /// Base class for resource details view model
    /// </summary>
    public abstract class ResourceDetailsViewModelBase : EditModeViewModelBase<ResourceViewModel>, IResourceDetails
    {
        #region Dependencies

        /// <summary>
        /// Internal resource controller
        /// </summary>
        internal IResourceController ResourceController { get; private set; }

        /// <summary>
        /// Dialog factory for type selectors and method invocation
        /// </summary>
        public IResourceDialogFactory DialogFactory { get; set; }

        #endregion

        #region Fields and Properties

        ///
        public long CurrentResourceId { get; private set; }

        /// <summary>
        /// Current config entries
        /// </summary>
        protected List<Entry> ConfigEntries
        {
            get { return EditableObject.Model.Properties; }
            set { EditableObject.Model.Properties = value; }
        }


        /// <summary>
        /// All methods of this type of resource
        /// </summary>
        public ResourceMethodViewModel[] Methods { get; private set; }

        /// <summary>
        /// All references of the resource
        /// </summary>
        public ReferenceViewModel[] References { get; private set; }

        /// <summary>
        /// Depth of the resource tree for GetDetails
        /// </summary>
        protected virtual int DetailsDepth => 1;

        #endregion

        ///
        public void Initialize(IInteractionController controller, string typeName)
        {
            base.Initialize();

            ResourceController = (IResourceController)controller;
            Logger = Logger.GetChild(typeName, GetType());

            Methods = new ResourceMethodViewModel[0];
            References = new ReferenceViewModel[0];
        }

        /// 
        public virtual async Task Load(long resourceId)
        {
            CurrentResourceId = resourceId;

            // Load resource
            var resource = await ResourceController.GetDetails(resourceId, DetailsDepth);

            await AssignLoadedResource(resource);
        }

        ///
        public virtual async Task Create(string resourceTypeName, long parentResourceId, object constructorModel)
        {
            var resource = await ResourceController.CreateResource(resourceTypeName, parentResourceId, constructorModel as MethodEntry);
            CurrentResourceId = resource.Id;

            await AssignLoadedResource(resource);
        }

        /// <summary>
        /// If an resource was loaded (new or extisting), the resource 
        /// can be assigned to this view model
        /// </summary>
        private async Task AssignLoadedResource(ResourceModel resource)
        {
            EditableObject = new ResourceViewModel(resource);
            NotifyOfPropertyChange(() => EditableObject);

            Methods = resource.Methods.Select(method => new ResourceMethodViewModel(method, this)).ToArray();
            References = resource.References.OrderBy(r => r.IsCollection)
                .Where(r => r.RelationType != ResourceRelationType.ParentChild) // Filter parent child relationship
                .Select(ReferenceViewModel.Create).ToArray();

            await OnConfigLoaded();

            await OnResourceLoaded();
            Logger.LogEntry(LogLevel.Trace, "Loaded resource with id {0}.", EditableObject.Id);
        }

        /// <summary>
        /// Method will be called if the resource was successfully loaded
        /// </summary>
        protected virtual Task OnResourceLoaded()
        {
            return SuccessTask;
        }

        /// <summary>
        /// Method will be called if the config was loaded successfully
        /// </summary>
        protected virtual Task OnConfigLoaded()
        {
            return SuccessTask;
        }

        /// <summary>
        /// Reload config of the object from the server
        /// </summary>
        /// <returns></returns>
        public async Task UpdateConfig()
        {
            var model = await ResourceController.GetDetails(CurrentResourceId, DetailsDepth);

            ConfigEntries = model.Properties;
        }

        /// <summary>
        /// Open the invocation dialog to execute a resource method
        /// </summary>
        protected internal async Task<Entry> OpenMethodInvocationDialog(ResourceMethodViewModel method)
        {
            var dialog = DialogFactory.CreateMethodInvocation(method);

            await DialogManager.ShowDialogAsync(dialog).ConfigureAwait(false);

            DialogFactory.Destroy(dialog);

            return dialog.ResultEntry;
        }

        /// <summary>
        /// Invoke a resource method directly
        /// </summary>
        protected async Task<Entry> InvokeResourceMethod(ResourceMethodViewModel method)
        {
            return await ResourceController.InvokeMethod(CurrentResourceId, method.Model);
        }
        
        ///
        protected override bool CanSave(object parameters)
        {
            return base.CanSave(parameters) && EditableObject.IsValid;
        }

        ///
        protected override async Task OnSave(object parameters)
        {
            await ResourceController.SaveResource(EditableObject.Model);
            await base.OnSave(parameters);
        }
    }


    /// <summary>
    /// Typed base class of <see cref="ResourceDetailsViewModelBase"/>
    /// </summary>
    public class ResourceDetailsViewModelBase<T> : ResourceDetailsViewModelBase where T : INotifyPropertyChanged, new()
    {
        /// <summary>
        /// Dedicated converter for this type of details view model
        /// </summary>
        public static readonly EntryToModelConverter ConfigConverter = EntryToModelConverter.Create<T>();

        /// <summary>
        /// Typed view model for the config
        /// </summary>
        public T ConfigViewModel { get; private set; }

        ///
        protected override async Task OnConfigLoaded()
        {
            await base.OnConfigLoaded();

            ConfigViewModel = new T();
            ConfigConverter.FromConfig(ConfigEntries, ConfigViewModel);
        }

        ///
        public override void CancelEdit()
        {
            ConfigConverter.FromConfig(ConfigEntries, ConfigViewModel);

            base.CancelEdit();
        }

        ///
        public override void EndEdit()
        {
            ConfigConverter.ToConfig(ConfigViewModel, ConfigEntries);

            base.EndEdit();
        }
    }
}