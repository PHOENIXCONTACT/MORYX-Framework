using System.ServiceModel;
using Marvin.Tools.Wcf;

namespace Marvin.Resources.UI
{
    /// <summary>
    /// Base class for resource interaction controllers
    /// </summary>
    public abstract class ResourceInteractionControllerBase<TWcfClient, TWcfInterface>
        : HttpServiceConnectorBase<TWcfClient, TWcfInterface>, IResourceInteractionController
        where TWcfClient : ClientBase<TWcfInterface>, TWcfInterface
        where TWcfInterface : class
    {
    }
}