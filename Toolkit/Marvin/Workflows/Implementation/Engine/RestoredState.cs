namespace Marvin.Workflows
{
    internal class RestoredState : EngineState
    {
        private readonly WorkflowSnapshot _snapshot;

        public RestoredState(WorkflowEngine engine, WorkflowSnapshot snapshot) : base(engine)
        {
            _snapshot = snapshot;
        }

        /// <summary>
        /// Start the engine
        /// </summary>
        internal override void Start()
        {
            Engine.State = new RunningState(Engine);
            Engine.ExecuteResume();
        }

        /// <summary>
        /// Return the snapshot that was used to restore the engine
        /// </summary>
        internal override WorkflowSnapshot Pause()
        {
            return _snapshot;
        }

        internal override void Destroy()
        {
            ExecuteDestroy();
        }
    }
}