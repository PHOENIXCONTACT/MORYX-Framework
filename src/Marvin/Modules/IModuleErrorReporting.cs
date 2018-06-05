using System;

namespace Marvin.Modules
{
    /// <summary>
    /// Event args passed to the ErrorOccured event
    /// </summary>
    public class ModuleErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Error that occured
        /// </summary>
        public Exception OccuredError { get; set; }
        
        /// <summary>
        /// Flag if this error is critical
        /// </summary>
        public bool IsCriticalError { get; set; }
    }

    /// <summary>
    /// Central component to report errors to the containing module
    /// </summary>
    public interface IModuleErrorReporting
    {
        /// <summary>
        /// Report internal failure to parent module
        /// </summary>
        void ReportFailure(object sender, Exception exception);
        /// <summary>
        /// Report an error to be treated as a warning
        /// </summary>
        void ReportWarning(object sender, Exception exception);
    }
}
