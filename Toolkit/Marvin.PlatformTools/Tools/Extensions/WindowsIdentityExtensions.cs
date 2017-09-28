using System.Security.Principal;

namespace Marvin.Tools
{
    /// <summary>
    /// Extensions for the <see cref="WindowsIdentity"/>
    /// </summary>
    public static class WindowsIdentityExtensions
    {
        /// <summary>
        /// Will return the UserName without the domain.
        /// </summary>
        public static string NameWithoutDomain(this WindowsIdentity identity)
        {
            var parts = identity.Name.Split('\\');
            return parts[1];
        }
    }
}