using System.Collections.Generic;
using System.ServiceModel;
using Marvin.Container;
using Marvin.Runtime.UserManagement.ClientConfigStore;
using Marvin.Runtime.UserManagement.UserAuthentication;

namespace Marvin.Runtime.UserManagement.Wcf
{
    [Plugin(LifeCycle.Transient, typeof(IUserManagementService))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, IncludeExceptionDetailInFaults = true)]
    internal class UserManagementService : IUserManagementService
    {
        public IUserAuthentication UserAuthentication { get; set; }
        public IClientConfigStore ClientConfigStore { get; set; }

        public UserModel GetUserInfos()
        {
            return UserAuthentication.GetUserInfos();
        }

        public ApplicationConfiguration GetApplication(string applicationName)
        {
            return UserAuthentication.GetApplication(applicationName);
        }

        public Dictionary<string, OperationAccess> GetOperationAccesses(ModuleConfiguration module)
        {
            return UserAuthentication.GetOperationAccesses(module);
        }

        public ClientConfigModel GetConfiguration(string clientId, string typeName)
        {
            return ClientConfigStore.GetConfiguration(clientId, typeName);
        }

        public bool SaveConfiguration(string clientId, ClientConfigModel configModel)
        {
            return ClientConfigStore.SaveConfiguration(clientId, configModel);
        }
    }
}
