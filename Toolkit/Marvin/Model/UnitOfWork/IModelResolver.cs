using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marvin.Model
{
    /// <summary>
    /// Factory capable of creating any 
    /// </summary>
    public interface IModelResolver
    {
        /// <summary>
        /// Create an open context using the model namespace
        /// </summary>
        IUnitOfWorkFactory GetByNamespace(string modelNamespace);
    }
}
