using System.Threading.Tasks;
using Marvin.AbstractionLayer.UI;

namespace Marvin.Resources.UI.Interaction
{
    [ResourceDetailsRegistration(DetailsConstants.EmptyType)]
    internal class EmptyDetailsViewModel : EmptyDetailsViewModelBase, IResourceDetails
    {
        public long CurrentResourceId
        {
            get { return 0; }
        }

        public Task Load(long resourceId)
        {
            return SuccessTask;
        }

        public Task Create(string resourceTypeName, long parentResourceId, object unused)
        {
            return SuccessTask;
        }
    }
}