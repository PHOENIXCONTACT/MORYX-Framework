namespace Marvin.Workflows
{
    internal class PausedState : EngineState
    {
        private readonly WorkflowSnapshot _snapshot;

        public PausedState(WorkflowEngine engine, WorkflowSnapshot snapshot) : base(engine)
        {
            _snapshot = snapshot;
        }

        /// <summary>
        /// Pause exeuction on the engine
        /// </summary>
        internal override WorkflowSnapshot Pause()
        {
            return _snapshot;
        }

        internal override void Start()
        {
            Engine.State = new RunningState(Engine);
            Engine.ExecuteResume();
        }

        internal override void Destroy()
        {
            ExecuteDestroy();
        }
    }
}