using System.Collections.Generic;
using System.ServiceModel;
using Marvin.Tools.Wcf;

namespace Marvin.Runtime.UserManagement.Wcf
{
    /// <summary>
    /// Interface for the usermanagementservice wcf contract. 
    /// </summary>
    [ServiceContract]
    [ServiceVersion(ServerVersion = "1.1.0.0", MinClientVersion = "1.0.0.0")]
    public interface IUserManagementService
    {
        /// <summary>
        /// Get User information.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        UserModel GetUserInfos();

        /// <summary>
        /// Get an application configuration from an application name.
        /// </summary>
        /// <param name="application">The application name for which the configuration should be fetched.</param>
        /// <returns>delivers an <see cref="ApplicationConfiguration"/>.</returns>
        [OperationContract]
        ApplicationConfiguration GetApplication(string application);

        /// <summary>
        /// Get the operation access for specified module configuration.
        /// </summary>
        /// <param name="module">The module configuration for which the operation access should be fetched.</param>
        /// <returns>Dictionary with the operation access of the module.</returns>
        [OperationContract]
        Dictionary<string, OperationAccess> GetOperationAccesses(ModuleConfiguration module);

        /// <summary>
        /// Get a <see cref="ClientConfigModel"/> from a specified library.
        /// </summary>
        [OperationContract]
        ClientConfigModel GetConfiguration(string clientId, string typeName);

        /// <summary>
        /// Save the given <see cref="ClientConfigModel"/>.
        /// </summary>
        [OperationContract]
        bool SaveConfiguration(string clientId, ClientConfigModel configModel);
    }
}
