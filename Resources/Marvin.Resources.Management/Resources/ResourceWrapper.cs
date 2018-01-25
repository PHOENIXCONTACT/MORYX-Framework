using Marvin.AbstractionLayer.Resources;
using Marvin.StateMachines;

namespace Marvin.Resources.Management
{
    internal sealed class ResourceWrapper : IStateContext
    {
        internal Resource Target { get; }
        
        internal ResourceStateBase State { get; private set; }

        void IStateContext.SetState(IState state)
        {
            State = (ResourceStateBase)state;
        }

        internal ResourceWrapper(Resource target)
        {
            Target = target;
            StateMachine.Initialize(this).With<ResourceStateBase>();
        }

        internal void Initialize()
        {
            State.Initialize();
        }

        internal void HandleInitialize()
        {
            Target.Initialize();
        }

        internal void Start()
        {
            State.Start();
        }

        internal void HandleStart()
        {
            Target.Start();
        }

        internal void ErrorOccured()
        {
            State.ErrorOccured();
        }
        
        internal void Stop()
        {
            State.Stop();
        }

        internal void HandleStop()
        {
            Target.Stop();
        }
    }
}