using System.Threading.Tasks;
using Marvin.AbstractionLayer.UI;
using Marvin.ClientFramework.Base;

namespace Marvin.Products.UI.Interaction
{
    [ProductDetailsRegistration(DetailsConstants.EmptyType)]
    internal class EmptyDetailsViewModel : EmptyDetailsViewModelBase, IProductDetails
    {
        public Task Load(long productId)
        {
            return SuccessTask;
        }

        public DelegateCommand CreateRevisionCmd { get; }

        public long ProductId { get; } = 0;

        public EmptyDetailsViewModel()
        {
            CreateRevisionCmd = new DelegateCommand(o => {}, o => false);
        }

        public void CreateRevision()
        {
            throw new System.InvalidOperationException("Cannot create a revision on empty details");
        }
    }
}
