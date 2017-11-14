namespace Marvin.Runtime.Modules
{
    /// <summary>
    /// Interface for server module base to access state based transitions
    /// </summary>
    public interface IStateBasedTransitions
    {
        /// <summary>
        /// Initialize the module
        /// </summary>
        void Initialize();

        /// <summary>
        /// Start the module
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the module
        /// </summary>
        void Stop();
    }
}
