using System;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Marvin.ClientFramework.Base;
using Marvin.ClientFramework.Dialog;
using Marvin.Logging;

namespace Marvin.AbstractionLayer.UI
{
    /// <summary>
    /// Base workspace for master details view with edit mode
    /// </summary>
    /// <typeparam name="TDetailsType">The type of the details.</typeparam>
    /// <typeparam name="TDetailsFactory">The type of the details factory.</typeparam>
    /// <typeparam name="TEmptyDetails">The type of the empty details</typeparam>
    public abstract class MasterDetailsWorkspace<TDetailsType, TDetailsFactory, TEmptyDetails> : ModuleWorkspace<IScreen>.OneActive
        where TDetailsType : class, IEditModeViewModel 
        where TDetailsFactory : IDetailsFactory<TDetailsType>
        where TEmptyDetails : EmptyDetailsViewModelBase, new()
    {
        #region Dependencies

        /// <summary>
        /// Factory to create or destory detail view models
        /// </summary>
        public TDetailsFactory DetailsFactory { get; set; }

        /// <summary>
        /// Default dependency to show dialogs and messageboxes
        /// </summary>
        public IDialogManager DialogManager { get; set; }

        /// <summary>
        /// Its only a logger ;-)
        /// </summary>
        public IModuleLogger Logger { get; set; }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Will represent the <see cref="ConductorBaseWithActiveItem{T}.ActiveItem"/> but in the detail type
        /// </summary>
        public TDetailsType CurrentDetails
        {
            get { return (TDetailsType)ActiveItem; }
        }

        private bool _isDetailsInEditMode;

        /// <summary>
        /// Flag indicating whether the details is in edit mode
        /// </summary>
        public bool IsDetailsInEditMode
        {
            get { return _isDetailsInEditMode; }
            protected set  
            {
                _isDetailsInEditMode = value;
                NotifyOfPropertyChange();
            }
        }

        private bool _isMasterBusy;

        /// <summary>
        /// Should be set to true if the master will load some  information
        /// </summary>
        public bool IsMasterBusy
        {
            get { return _isMasterBusy; }
            protected set
            {
                _isMasterBusy = value;
                NotifyOfPropertyChange();
            }
        }

        private bool _isDetailsInBusyMode;

        /// <summary>
        /// Indicating if details view model is in busy mode
        /// </summary>
        public bool IsDetailsInBusyMode
        {
            get { return _isDetailsInBusyMode; }
            set
            {
                _isDetailsInBusyMode = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Empty details
        /// </summary>
        protected TEmptyDetails EmptyDetails;

        #endregion

        /// 
        protected override void OnInitialize()
        {
            base.OnInitialize();

            // Show busy indicator on master because we are not sure if the master is currently loaded
            IsMasterBusy = true;

            Logger = Logger.GetChild(typeof(TDetailsType).Name, GetType());

            // Create empty details here
            EmptyDetails = DetailsFactory.Create(DetailsConstants.EmptyType) as TEmptyDetails;
            ShowEmpty();
        }

        public virtual Task OnMasterItemChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
        {
            return Task.FromResult(true);
        }

        public async Task LoadDetails(Func<Task> loaderAction)
        {
            IsDetailsInBusyMode = true;

            try
            {
                await loaderAction();
            }
            catch (Exception e)
            {
                const string errorMessage = "Cannot load details because an error occured!";
                Logger.LogException(LogLevel.Error, e, errorMessage);

                EmptyDetails.Display(MessageSeverity.Error, errorMessage);
                ActivateItem(EmptyDetails);
                throw;
            }
            finally
            {
                IsDetailsInBusyMode = false;
            }
        }

        ///
        public override void ActivateItem(IScreen item)
        {
            if (item == ActiveItem)
                return;

            if (ActiveItem != null)
            {
                var detailItem = (TDetailsType)ActiveItem;
                detailItem.EditModeChanged -= OnDetailsEditModeChanged;
                detailItem.BusyChanged -= OnDetailsBusyModeChanged;
                detailItem.Saved -= OnDetailsSaved;

                DetailsFactory.Destroy(detailItem);
            }

            var newItem = (TDetailsType) item;
            newItem.EditModeChanged += OnDetailsEditModeChanged;
            newItem.BusyChanged += OnDetailsBusyModeChanged;
            newItem.Saved += OnDetailsSaved;

            base.ActivateItem(item);
            NotifyOfPropertyChange(() => CurrentDetails);
        }

        /// <summary>
        /// Called if the specialized details view model was saved
        /// </summary>
        protected virtual void OnDetailsSaved(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Called if the specialized details view model changes the edit mode
        /// </summary>
        protected virtual void OnDetailsEditModeChanged(object sender, EditModeChange changeMode)
        {
            IsDetailsInEditMode = changeMode == EditModeChange.Enabled;
        }

        /// <summary>
        /// Called if the specialized details view model changes the busy mode
        /// </summary>
        private void OnDetailsBusyModeChanged(object sender, bool busy)
        {
            IsDetailsInBusyMode = busy;
        }

        /// <summary>
        /// Shows the empty view model with the message to select a product from the tree.
        /// </summary>
        protected virtual void ShowEmpty()
        {
            ActivateItem(EmptyDetails);
        }
    }
}