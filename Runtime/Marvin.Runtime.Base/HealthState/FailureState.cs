using System;
using Marvin.Modules.Server;
using Marvin.Testing;

namespace Marvin.Runtime.Base.HealthState
{
    internal class FailureState : HealthState
    {
        /// <summary>
        /// Represents the failure state of a module. This will be reached when an error occured with the module.
        /// Initialize will try to get out of the error state.
        /// </summary>
        /// <param name="target">The target for which this state will be handeld.</param>
        /// <param name="context">The HealthStateController.</param>
        public FailureState(IStateBasedTransitions target, HealthStateController context) 
            : base(target, context, ServerModuleState.Failure)
        {
            StopExecution();
        }

        /// <summary>
        /// Stop plugin execution to avoid further errors
        /// </summary>
        private void StopExecution()
        {
            try
            {
                Target.Stop();
            }
            catch (Exception ex)
            {
                Context.LogNotification(Context, new FailureNotification(ex, "Failed to stop faulty module!"));
            }      
        }

        /// <summary>
        /// Does nothing when an error occured. The module is already in an error state so it will not change its state.
        /// </summary>
        /// <param name="criticalError">Flag for critical or not critical error.</param>
        [OpenCoverIgnore]
        public override void ErrorOccured(bool criticalError)
        {
        }

        /// <summary>
        /// Try to initialize the module.
        /// </summary>
        public override void Initialize()
        {
            try
            {
                Context.Current = ServerModuleState.Initializing;;
                Target.Initialize();
                Context.UpdateState(new ReadyState(Target, Context));
            }
            catch (Exception ex)
            {
                Context.LogNotification(Context, new FailureNotification(ex, "Failed to recover from failure"));
                Context.Current = ServerModuleState.Failure;
            }
        }

        /// <summary>
        /// Stop the module and set the stop state.
        /// </summary>
        public override void Stop()
        {
            Context.UpdateState(new StoppedState(Target, Context));
        }
    }
}
