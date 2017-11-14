namespace Marvin.Workflows
{
    internal class RunningState : EngineState
    {
        public RunningState(WorkflowEngine engine) : base(engine)
        {
        }

        internal override WorkflowSnapshot Pause()
        {
            var snapShot = Engine.ExecutePause();
            Engine.State = new PausedState(Engine, snapShot);
            return snapShot;
        }

        internal override void Completed()
        {
            Engine.State = new IdleState(Engine);
        }

        internal override void Destroy()
        {
            Engine.ExecutePause();
            ExecuteDestroy();
        }
    }
}