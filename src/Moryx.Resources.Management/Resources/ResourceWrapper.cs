// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Modules;
using Moryx.StateMachines;

namespace Moryx.Resources.Management
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
            ((IInitializable)Target).Initialize();
        }

        internal void Start()
        {
            State.Start();
        }

        internal void HandleStart()
        {
            ((IPlugin)Target).Start();
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
            ((IPlugin)Target).Stop();
        }
    }
}
