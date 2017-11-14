namespace Marvin.Workflows
{
    internal class ReadyState : EngineState
    {
        public ReadyState(WorkflowEngine engine) : base(engine)
        {
        }

        /// <summary>
        /// Start the engine
        /// </summary>
        internal override void Start()
        {
            Engine.State = new RunningState(Engine);
            Engine.ExecuteStart();
        }

        internal override void Restore(WorkflowSnapshot snapshot)
        {
            Engine.State = new RestoredState(Engine, snapshot);
            Engine.ExecuteRestore(snapshot);
        }

        internal override void Destroy()
        {
            ExecuteDestroy();
        }
    }
}