namespace Marvin.StateMachines
{
    /// <summary>
    /// Interface representing an state machine state
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Will be called while entering the next state
        /// </summary>
        void OnEnter();

        /// <summary>
        /// Will be called while exiting the current state
        /// </summary>
        void OnExit();
    }
}