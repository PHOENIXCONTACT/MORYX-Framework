namespace Marvin.StateMachines
{
    /// <summary>
    /// Represents the context of a state machine including a typed state property
    /// </summary>
    public interface IStateContext
    {
        /// <summary>
        /// Update state on context
        /// </summary>
        void SetState(IState state);
    }
}