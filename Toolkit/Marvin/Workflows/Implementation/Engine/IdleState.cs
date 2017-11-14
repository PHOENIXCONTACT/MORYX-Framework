namespace Marvin.Workflows
{
    internal class IdleState : EngineState
    {
        public IdleState(WorkflowEngine engine) : base(engine)
        {
        }

        /// <summary>
        /// Initialize the engine
        /// </summary>
        internal override void Initialize(IWorkflow workflow)
        {
            Engine.State = new ReadyState(Engine);
            Engine.ExecuteInitialize(workflow);
        }
    }
}