using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moryx.Launcher
{
    /// <summary>
    /// Component to determine navigation items for the shell
    /// </summary>
    public interface IShellNavigator
    {
        /// <summary>
        /// Gathers the available Web Modules to display in the shell
        /// </summary>
        Task<IReadOnlyList<WebModuleItem>> GetWebModuleItems(HttpContext context);
    }

}