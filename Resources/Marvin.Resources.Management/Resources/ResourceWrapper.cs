using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marvin.AbstractionLayer.Resources;
using Marvin.StateMachines;

namespace Marvin.Resources.Management
{
    internal sealed class ResourceWrapper : IStateContext
    {
        internal Resource Target { get; }
        
        internal ResourceStateBase State { get; private set; }

        public void SetState(IState state)
        {
            State = (ResourceStateBase)state;
        }

        internal ResourceWrapper(Resource target)
        {
            Target = target;
            StateMachine.Initialize<ResourceStateBase>(this);
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
