namespace Marvin.Threading
{
    /// <summary>
    /// Extension of <see cref="IParallelOperations"/> to register an <see cref="IParallelObserver"/>
    /// for diagnostic purposes.
    /// </summary>
    public interface IParallelObservation : IParallelOperations
    {
        /// <summary>
        /// Register an observer
        /// </summary>
        void Register(IParallelObserver observer);

        /// <summary>
        /// Unregister an observer
        /// </summary>
        void Unregister(IParallelObserver observer);
    }
}