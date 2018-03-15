using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using Marvin.ClientFramework.Commands;
using Marvin.ClientFramework.Dialog;
using Marvin.Logging;

namespace Marvin.AbstractionLayer.UI
{
    /// <summary>
    /// Base class for viewmodels with an edit mode
    /// </summary>
    public abstract class EditModeViewModelBase : Screen, IEditModeViewModel
    {
        #region Dependencies

        /// <summary>
        /// Logger for this details view model
        /// </summary>
        public IModuleLogger Logger { get; set; }

        /// <summary>
        /// Dialog manager do show dialogs and messageboxes
        /// </summary>
        public IDialogManager DialogManager { get; set; }

        #endregion

        #region Fields and Properties

        private bool _isEditMode;
        private bool _isBusy;

        /// <summary>
        /// Static representation of a successfull task
        /// </summary>
        protected static Task<bool> SuccessTask = Task.FromResult(true);

        ///
        public ICommand EditModeCmd { get; private set; }

        ///
        public ICommand CancelEditCmd { get; private set; }

        /// 
        public IAsyncCommand SaveCmd { get; private set; }

        #endregion

        /// 
        protected override void OnInitialize()
        {
            base.OnInitialize();
            Initialize();
        }

        /// <summary>
        /// Initializes this instance of the edit mode with all needed information
        /// </summary>
        protected virtual void Initialize()
        {
            EditModeCmd = new DelegateCommand(EnterEditMode, CanEnterEditMode);
            CancelEditCmd = new DelegateCommand(CancelEditMode, CanCancelEditMode);
            SaveCmd = new AsyncCommand(Save, CanSave, true);
        }

        /// <inheritdoc />
        public void EnterEditMode()
        {
            EnterEditMode(null);
        }

        /// <inheritdoc />
        public bool IsEditMode
        {
            get { return _isEditMode; }
            private set
            {
                _isEditMode = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Indicates if the view is busy like during the save process
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }
            protected set
            {
                _isBusy = value;
                RaiseBusyChanged(_isBusy);
                NotifyOfPropertyChange();
            }
        }

        ///
        public event EventHandler<bool> BusyChanged;
        private void RaiseBusyChanged(bool busyMode)
        {
            BusyChanged?.Invoke(this, busyMode);
        }

        /// <summary>
        /// Method will be called by the <see cref="EditModeCmd"/> to check if the 
        /// Editmode can be entered or not
        /// </summary>
        protected virtual bool CanEnterEditMode(object parameters)
        {
            return IsEditMode == false;
        }

        /// <summary>
        /// Command execution handler for the <see cref="EditModeCmd"/>
        /// </summary>
        private void EnterEditMode(object parameters)
        {
            BeginEdit();

            IsEditMode = true;
            RaiseEditModeChanged(EditModeChange.Enabled);
        }

        /// <summary>
        /// Method will be called by the <see cref="EditModeCmd"/> to check if the 
        /// Editmode can be canceled or not
        /// </summary>
        protected virtual bool CanCancelEditMode(object parameters)
        {
            return !CanEnterEditMode(parameters);
        }

        /// <summary>
        /// Command execution handler for the <see cref="CancelEditCmd"/>
        /// </summary>
        private void CancelEditMode(object parameters)
        {
            CancelEdit();

            IsEditMode = false;
            RaiseEditModeChanged(EditModeChange.Disabled);
        }

        /// <summary>
        /// Method will be called by the <see cref="EditModeCmd"/> to check if the current viewmodel can be saved
        /// </summary>
        protected virtual bool CanSave(object parameters)
        {
            return IsEditMode;
        }

        /// <summary>
        /// Command execution handler for the <see cref="SaveCmd"/>
        /// </summary>
        protected async Task Save(object parameters)
        {
            IsBusy = true;

            //Copy view model properties to model.
            EndEdit();

            try
            {
                await OnSave(parameters);

                IsEditMode = false;
                RaiseEditModeChanged(EditModeChange.Disabled);

                RaiseSaved();
            }
            catch (Exception)
            {
                DialogManager.ShowMessageBox("Cannot save the current product.", "Error while saving!");
                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Will be called if save was executed to add on save logic
        /// </summary>
        protected virtual Task OnSave(object parameters)
        {
            return SuccessTask;
        }

        ///
        public virtual void BeginEdit()
        {
            Logger.LogEntry(LogLevel.Trace, "Will beginn edit mode.");
        }

        ///
        public virtual void EndEdit()
        {
            Logger.LogEntry(LogLevel.Trace, "Will end edit mode.");
        }

        ///
        public virtual void CancelEdit()
        {
            Logger.LogEntry(LogLevel.Trace, "Will cancel edit mode.");
        }

        ///
        public event EventHandler<EditModeChange> EditModeChanged;
        private void RaiseEditModeChanged(EditModeChange changeMode)
        {
            ((DelegateCommand)EditModeCmd).RaiseCanExecuteChanged();
            ((DelegateCommand)CancelEditCmd).RaiseCanExecuteChanged();

            EditModeChanged?.Invoke(this, changeMode);
        }


        /// <inheritdoc />
        public event EventHandler Saved;

        protected void RaiseSaved()
        {
            Saved?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Base class for viewmodels with an edit mode
    /// </summary>
    public abstract class EditModeViewModelBase<T> : EditModeViewModelBase
        where T : IEditableObject
    {
        private T _editableObject;

        /// <summary>
        /// Represents the editable object
        /// </summary>
        public T EditableObject
        {
            get { return _editableObject; }
            protected set
            {
                _editableObject = value;
                NotifyOfPropertyChange();
            }
        }

        ///
        public override void BeginEdit()
        {
            base.BeginEdit();

            EditableObject.BeginEdit();
        }

        ///
        public override void EndEdit()
        {
            base.EndEdit();

            EditableObject.EndEdit();
        }

        ///
        public override void CancelEdit()
        {
            EditableObject.CancelEdit();

            base.CancelEdit();
        }
    }
}