using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using C4I;
using Caliburn.Micro;
using Marvin.ClientFramework.Base;
using Marvin.ClientFramework.Dialog;
using Marvin.Container;
using Marvin.Tools;

namespace Marvin.Products.UI.Interaction
{
    /// <summary>
    /// Interface for the Show-Revision dialog
    /// </summary>
    public interface IRevisionsViewModel : IDialogScreen
    {
        /// <summary>
        /// Product id of the selected revision
        /// </summary>
        long? SelectedRevision { get; }
    }

    [Plugin(LifeCycle.Transient, typeof(IRevisionsViewModel))]
    internal class RevisionsViewModel : DialogScreen, IRevisionsViewModel
    {
        #region Depedencies

        public IProductsController Controller { get; set; }

        #endregion

        #region Fields and Properties

        private readonly string _identifier;

        public ObservableCollection<ProductRevisionViewModel> Revisions { get; } = new ObservableCollection<ProductRevisionViewModel>();

        public RelayCommand OpenCmd { get; private set; }

        public RelayCommand CloseCmd { get; private set; }

        public TaskNotifier CurrentTask { get; set; }

        public ProductRevisionViewModel SelectedRevision { get; set; }

        long? IRevisionsViewModel.SelectedRevision => SelectedRevision?.Model.ProductId;

        #endregion

        public RevisionsViewModel(string identifier)
        {
            _identifier = identifier;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            OpenCmd = new RelayCommand(o => TryClose(true), o => SelectedRevision != null);
            CloseCmd = new RelayCommand(o => TryClose(false));

            DisplayName = "Revisions";
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var loadingTask = Task.Run(async delegate
            {
                var revisions = await Controller.GetProductRevisions(_identifier)
                    .ConfigureAwait(false);

                var vms = revisions.Select(r => new ProductRevisionViewModel(r)).ToArray();
                await Execute.OnUIThreadAsync(() => Revisions.AddRange(vms));
            });

            CurrentTask = new TaskNotifier(loadingTask);
        }
    }
}
