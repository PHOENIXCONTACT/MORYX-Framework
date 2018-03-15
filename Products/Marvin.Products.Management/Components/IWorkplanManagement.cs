using System.Collections.Generic;
using Marvin.Workflows;

namespace Marvin.Products.Management
{
    /// <summary>
    /// Component to handle workplans
    /// </summary>
    internal interface IWorkplanManagement : IWorkplans
    {
        /// <summary>
        /// Creates the workplan.
        /// </summary>
        IWorkplan Create(string name);
    }
}