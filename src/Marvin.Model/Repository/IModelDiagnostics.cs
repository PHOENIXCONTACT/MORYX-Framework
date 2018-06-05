using System;

namespace Marvin.Model
{
    /// <summary>
    /// Interface for UnitOfWork diagnostics
    /// </summary>
    public interface IModelDiagnostics
    {
        /// <summary>
        /// Will be executed if the database logs
        /// </summary>
        Action<string> Log { get; set; }
    }
}