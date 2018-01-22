namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Interface for handler classes used to distribute mixed type messages to specialized handler methods
    /// </summary>
    public interface IHandlerMap<in T>
    {
        /// <summary>
        /// Distribution message handler
        /// </summary>
        void Handle(T message);
    }
}