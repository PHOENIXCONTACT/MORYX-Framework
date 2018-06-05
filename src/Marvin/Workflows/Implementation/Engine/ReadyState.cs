namespace Marvin.Workflows
{
    internal class ReadyState : EngineState
    {
        public ReadyState(WorkflowEngine context, StateMap stateMap) : base(context, stateMap)
        {
        }

        internal override void Initialize(IWorkflow workflow)
        {
            // Nothing happens
        }

        internal override void Start()
        {
            NextState(StateRunning);
            Context.ExecuteStart();
        }

        internal override void Restore(WorkflowSnapshot snapshot)
        {
            NextState(StateRestored);
            Context.CurrentSnapshot = snapshot;
            Context.ExecuteRestore();
        }

        internal override void Destroy()
        {
            ExecuteDestroy();
        }
    }
}