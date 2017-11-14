namespace Marvin.Workflows
{
    /// <summary>
    /// State pattern for the engine
    /// </summary>
    internal abstract class EngineState
    {
        protected WorkflowEngine Engine { get; private set; }

        protected EngineState(WorkflowEngine engine)
        {
            Engine = engine;
        }

        /// <summary>
        /// Initialize the engine
        /// </summary>
        internal virtual void Initialize(IWorkflow workflow)
        {
        }

        /// <summary>
        /// Start the engine
        /// </summary>
        internal virtual void Start()
        {
        }

        /// <summary>
        /// Transition called when engine was completed
        /// </summary>
        internal virtual void Completed()
        {
        }

        /// <summary>
        /// Pause exeuction on the engine
        /// </summary>
        internal virtual WorkflowSnapshot Pause()
        {
            return null;
        }

        /// <summary>
        /// Restore the workflow from a snapshot
        /// </summary>
        internal virtual void Restore(WorkflowSnapshot snapshot)
        {
        }

        /// <summary>
        /// Destroy the engine instance
        /// </summary>
        internal virtual void Destroy()
        {
        }
        protected void ExecuteDestroy()
        {
            Engine.State = new IdleState(Engine);
            Engine.ExecuteDispose();
        }

        /// <summary>
        /// Create new instance of the engine state
        /// </summary>
        internal static EngineState Create(WorkflowEngine engine)
        {
            return new IdleState(engine);
        }
    }
}