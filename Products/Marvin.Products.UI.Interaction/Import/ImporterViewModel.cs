using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Marvin.Controls;
using Marvin.Products.UI.Interaction.InteractionSvc;
using Marvin.Serialization;

namespace Marvin.Products.UI.Interaction
{
    internal class ImporterViewModel : PropertyChangedBase
    {
        private readonly ProductImporter _importer;
        private readonly IProductsController _productsController;
        
        public ImporterViewModel(ProductImporter importer, IProductsController productsController)
        {
            _importer = importer;
            _productsController = productsController;

            // Create fake root
            CreateParameterViewModel(_importer.Parameters);
        }

        private void CreateParameterViewModel(ImportParameter[] parameters)
        {
            if (Parameters != null)
            {
                foreach (var entry in Parameters.SubEntries.Cast<ImportParameterViewModel>())
                {
                    entry.ValueChanged -= OnUpdateTriggerChanged;
                }
            }

            Parameters = new EntryViewModel(new Entry { Key = new EntryKey { Name = "Root" } });
            foreach (var parameter in parameters)
            {
                var viewModel = new ImportParameterViewModel(new ImportParameter
                {   // Create clone to clear after every import
                    TriggersUpdate = parameter.TriggersUpdate,
                    Key = parameter.Key,
                    Validation = parameter.Validation,
                    Description = parameter.Description,
                    Value = parameter.Value.Clone(false),
                    Prototypes = parameter.Prototypes,
                    SubEntries = parameter.SubEntries.Select(se => se.Clone(true)).ToList()
                });
                viewModel.ValueChanged += OnUpdateTriggerChanged;
                Parameters.SubEntries.Add(viewModel);
            }
        }

        /// <summary>
        /// Update parameters if a <see cref="ImportParameter.TriggersUpdate"/> was modified
        /// </summary>
        private async void OnUpdateTriggerChanged(object sender, ImportParameter importParameter)
        {
            var parameters = Parameters.SubEntries.Cast<ImportParameterViewModel>().Select(ip => ip.Model).ToArray();
            parameters = await _productsController.UpdateParameters(_importer.Name, parameters);
            CreateParameterViewModel(parameters);
        }

        /// <summary>
        /// Name of this importer
        /// </summary>
        public string Name => _importer.Name;

        private EntryViewModel _parameters;
        /// <summary>
        /// Parameters of this importer
        /// </summary>
        public EntryViewModel Parameters
        {
            get { return _parameters; }
            set
            {
                if (Equals(value, _parameters))
                    return;
                _parameters = value;
                NotifyOfPropertyChange();
            }
        }

        public bool ValidateInput()
        {
            return Parameters.SubEntries.All(se => !se.Entry.Validation.IsRequired || !string.IsNullOrWhiteSpace(se.Value));
        }

        /// <summary>
        /// Import product using the current values
        /// </summary>
        /// <returns></returns>
        public Task<ProductModel> Import()
        {
            var parameters = Parameters.SubEntries.Cast<ImportParameterViewModel>().Select(ip => ip.Model).ToArray();
            return _productsController.ImportProduct(_importer.Name, parameters);
        }
    }
}