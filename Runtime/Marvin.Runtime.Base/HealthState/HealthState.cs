using Marvin.Modules.Server;

namespace Marvin.Runtime.Base.HealthState
{
    /// <summary>
    /// This makes use of the state pattern described by the GoF
    /// </summary>
    internal abstract class HealthState
    {
        /// <summary>
        /// The health state of the current module.
        /// </summary>
        /// <param name="target">The target for which this state will be handeld.</param>
        /// <param name="context">The HealthStateController.</param>
        /// <param name="state">Current state of the target.</param>
        protected HealthState(IStateBasedTransitions target, HealthStateController context, ServerModuleState state) 
        {
            Target = target;
            Context = context;
            Context.Current = state;
        }

        /// <summary>
        /// <see cref="HealthStateController"/> instances managing this lifecylce
        /// </summary>
        protected HealthStateController Context { get; set; }

        /// <summary>
        /// Server module performing valid transitions
        /// </summary>
        internal IStateBasedTransitions Target { get; set; }

        #region Transitions
        /// <summary>
        /// Handle an occured error.
        /// </summary>
        /// <param name="criticalError">flag to set critical or not.</param>
        public abstract void ErrorOccured(bool criticalError);

        /// <summary>
        /// Initialize phase of the module.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Try to start a module.
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// Stops a module.
        /// </summary>
        public virtual void Stop()
        {
        }
        #endregion

    }
}
