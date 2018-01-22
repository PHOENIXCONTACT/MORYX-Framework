using Caliburn.Micro;
using Marvin.Products.UI.Interaction.InteractionSvc;

namespace Marvin.Products.UI.Interaction
{
    internal interface IImportViewModel : IScreen
    {
        ProductModel ImportedProduct { get; }
    }
}