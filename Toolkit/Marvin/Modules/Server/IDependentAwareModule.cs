namespace Marvin.Modules.Server
{
    /// <summary>
    /// Interface for all modules that are aware of their dependends and want to modify their behavior accordingly.
    /// For example a module might postpone its events till a listener is registered
    /// </summary>
    public interface IDependentAwareModule : IServerModule
    {
        /// <summary>
        /// Called when all dependent modules are running and wired
        /// </summary>
        void DependentsWired();

        /// <summary>
        /// Called when one or more dependends were unwired
        /// </summary>
        void DependentUnwired();
    }
}