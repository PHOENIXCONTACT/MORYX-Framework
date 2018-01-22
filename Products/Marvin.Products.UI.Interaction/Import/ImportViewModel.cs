using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using C4I;
using Caliburn.Micro;
using Marvin.ClientFramework.Base;
using Marvin.Container;
using Marvin.Logging;
using Marvin.Products.UI.Interaction.InteractionSvc;

namespace Marvin.Products.UI.Interaction
{
    [Plugin(LifeCycle.Transient, typeof(IImportViewModel))]
    internal class ImportViewModel : Screen, IImportViewModel
    {
        #region Dependencies

        /// <summary>
        /// Injected products controller
        /// </summary>
        public IProductsController ProductsController { get; set; }

        public IModuleLogger Logger { get; set; }


        #endregion

        #region Fields and Properties

        public AsyncCommand OkCmd { get; private set; }

        public ICommand CancelCmd { get; private set; }

        #endregion

        private string _errorText;
        /// <summary>
        /// Error text
        /// </summary>
        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                _errorText = value;
                NotifyOfPropertyChange();
            }
        }

        private BindableCollection<ImporterViewModel> _importers;
        public BindableCollection<ImporterViewModel> Importers
        {
            get { return _importers; }
            set
            {
                _importers = value;
                NotifyOfPropertyChange();
            }
        }
        private ImporterViewModel _selectedImporter;
        /// <summary>
        /// Selected importer
        /// </summary>
        public ImporterViewModel SelectedImporter
        {
            get { return _selectedImporter; }
            set
            {
                _selectedImporter = value;
                NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Imported product
        /// </summary>
        public ProductModel ImportedProduct { get; private set; }


        protected override void OnInitialize()
        {
            DisplayName = "Product Importer";

            OkCmd = new AsyncCommand(Ok);
            CancelCmd = new RelayCommand(Cancel);

            var importers = ProductsController.Customization.Importers.Select(i => new ImporterViewModel(i, ProductsController));
            Importers = new BindableCollection<ImporterViewModel>(importers);
            if (Importers.Count > 0)
                SelectedImporter = Importers[0];
        }

        /// <summary>
        /// Imports product
        /// </summary>
        private async Task Ok(object obj)
        {
            if (!SelectedImporter.ValidateInput())
            {
                ErrorText = "Please fill all required fields!";
                return;
            }
            try
            {
                ImportedProduct = await SelectedImporter.Import();
            }
            catch (Exception ex)
            {
                ErrorText = (string.IsNullOrEmpty(ex.Message) ? "An error occured while creating the product." : ex.Message) + "\nPlease check the current input.";
                Logger.LogException(LogLevel.Error, ex, ex.Message);
                return;
            }
            
            TryClose(true);
        }

        /// <summary>
        ///     Method call on Cancel
        /// </summary>
        private void Cancel(object obj)
        {
            TryClose(false);
        }
    }
}