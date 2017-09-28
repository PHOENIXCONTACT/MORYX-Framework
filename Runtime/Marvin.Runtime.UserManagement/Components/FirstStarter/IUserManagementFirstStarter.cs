using Marvin.Configuration;

namespace Marvin.Runtime.UserManagement.FirstStarter
{
    /// <summary>
    /// This interface is designed to enable the initialization of the user management
    /// after deployment, because possible configuration front ends might depend on a
    /// running user management.
    /// </summary>
    public interface IUserManagementFirstStarter
    {
        /// <summary>
        /// Initialize the user management configuration
        /// </summary>
        void InitializeUserManagement(ModuleConfig config, IConfigManager configManager);
    }
}
