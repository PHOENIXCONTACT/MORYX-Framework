using Marvin.Container;

namespace Marvin.Runtime.UserManagement.ClientConfigStore
{
    /// <summary>
    /// Factory to create config store implementation
    /// </summary>
    [PluginFactory(typeof(IConfigBasedComponentSelector))]
    internal interface IClientConfigStoreFactory
    {
        IClientConfigStore CreateStore(ClientConfigStoreConfigBase config);

        void Destroy(IClientConfigStore store);
    }
}