namespace Marvin.Workflows
{
    /// <summary>
    /// Interface for a transition handling wrapper
    /// </summary>
    public interface IAttemptInvokation
    {
        /// <summary>
        /// Tries to map the transition to a certain type. If successful it invokes
        /// the target and returns true, otherwise it returns false.
        /// </summary>
        bool TryInvoke(ITransition transition);
    }
}