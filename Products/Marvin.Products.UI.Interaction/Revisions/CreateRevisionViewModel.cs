using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using C4I;
using Marvin.ClientFramework.Base;
using Marvin.ClientFramework.Dialog;
using Marvin.Container;
using Marvin.Products.UI.Interaction.InteractionSvc;

namespace Marvin.Products.UI.Interaction
{
    public interface ICreateRevisionViewModel : IDialogScreen
    {
        long CreatedProductRevision { get; set; }
    }

    [Plugin(LifeCycle.Transient, typeof(ICreateRevisionViewModel))]
    internal class CreateRevisionViewModel : DialogScreen, ICreateRevisionViewModel
    {
        #region Fields and Properties

        private string _numberErrorMessage;
        private string _revisionNumber;

        /// <summary>
        ///     Display name for Create Revision popup
        /// </summary>
        public override string DisplayName => "Create new revision";

        public AsyncCommand CreateRevisionCmd { get; private set; }

        public RelayCommand CloseCmd { get; private set; }

        public string Comment { get; set; }

        private ProductRevisionEntry[] _productRevsions;

        public string RevisionNumber
        {
            get { return _revisionNumber; }
            set
            {
                if (StringHasOnlyDigits(value))
                {
                    _revisionNumber = value;
                    CreateRevisionCmd.RaiseCanExecuteChanged();
                    NotifyOfPropertyChange();
                }
            }
        }

        public string ProductName => _structureEntry.Name;

        /// <summary>
        ///     Displays error messege from revision number validation
        /// </summary>
        public string NumberErrorMessage
        {
            get { return _numberErrorMessage; }
            set
            {
                _numberErrorMessage = value;
                NotifyOfPropertyChange();
            }
        }

        private string _generalErrorMessage;
        private readonly StructureEntryViewModel _structureEntry;

        /// <summary>
        /// Displays general error messages if new revision creation is failed
        /// </summary>
        public string GeneralErrorMessage
        {
            get { return _generalErrorMessage; }
            set
            {
                _generalErrorMessage = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        ///     Product revision which should be created after succed OnCreateClick() call
        /// </summary>
        public long CreatedProductRevision { get; set; }

        #endregion

        #region Dependecies

        /// <summary>
        ///     Injected products controller
        /// </summary>
        public IProductsController ProductsController { get; set; }

        #endregion

        /// <summary>
        ///     Constructor
        /// </summary>
        public CreateRevisionViewModel(StructureEntryViewModel structureEntry)
        {
            _structureEntry = structureEntry;
        }

        protected override async void OnInitialize()
        {
            base.OnInitialize();

            CreateRevisionCmd = new AsyncCommand(CreateRevision, CanCreateRevision);
            CloseCmd = new RelayCommand(Close, CanClose);

            _productRevsions = await ProductsController.GetProductRevisions(_structureEntry.MaterialNumber);

            RevisionNumber = GetNextRevisionNumber().ToString();
        }

        private short GetNextRevisionNumber()
        {
            return (short)(_productRevsions.Max(pr => pr.Revision) + 1);
        }

        private bool CanClose(object obj)
        {
            return !CreateRevisionCmd.IsExecuting;
        }

        private bool CanCreateRevision(object arg)
        {
            NumberErrorMessage = string.Empty;

            short result;
            if (!short.TryParse(RevisionNumber, out result))
            {
                NumberErrorMessage = "Revision number has no valid format.";
                return false;
            }

            var isRevisionNumberFree = _productRevsions.All(pr => pr.Revision != short.Parse(RevisionNumber));

            if (!isRevisionNumberFree)
            {
                NumberErrorMessage = "Revision not available.";
                return false;
            }

            return true;
        }

        private Task CreateRevision(object arg)
        {
            GeneralErrorMessage = string.Empty;
            return ProductsController.CreateRevision(_structureEntry.Id, short.Parse(RevisionNumber), Comment).ContinueWith(
                delegate(Task<ProductModel> t)
                {
                    switch (t.Status)
                    {
                        case TaskStatus.Faulted:
                            GeneralErrorMessage = "Revision cannot be created.";
                            break;
                        case TaskStatus.RanToCompletion:
                            CreatedProductRevision = t.Result.Id;
                            TryClose(true);
                            break;
                    }
                });
        }

        /// <summary>
        ///     Method call on Cancel
        /// </summary>
        private void Close(object parameters)
        {
            TryClose(false);
        }

        private static bool StringHasOnlyDigits(string text)
        {
            var regex = new Regex("^[0-9]?[0-9]?$");
            var result = regex.IsMatch(text);
            return result;
        }
    }
}