using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.Maintenance.Web
{
    /// <summary>
    /// Service contract for the maintenance file system.
    /// </summary>
    [ServiceContract]
    [ServiceVersion(ServerVersion = "1.0.0.0", MinClientVersion = "1.0.0.0")]
    internal interface IWebServerFileSystem
    {
        [OperationContract]
        [WebGet(UriTemplate = "*")]
        Stream Html();

        [OperationContract]
        [WebGet(UriTemplate = "bundle.js")]
        Stream BundleJs();

        [OperationContract]
        [WebGet(UriTemplate = "favicon.ico")]
        Stream FavIcon();
    }
}
