// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.StateMachines;

namespace Moryx.Runtime.Modules
{
    internal abstract class ServerModuleStateBase : StateBase<IServerModuleStateContext>
    {
        public abstract ServerModuleState Classification { get; }

        protected ServerModuleStateBase(IServerModuleStateContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        public virtual void Initialize()
        {
            InvalidState();
        }

        public virtual void Start()
        {
            InvalidState();
        }

        public virtual void Stop()
        {
            InvalidState();
        }

        public virtual void ValidateHealthState()
        {
            throw new HealthStateException(Classification);
        }

        [StateDefinition(typeof(StoppedState), IsInitial = true)]
        protected const int StateStopped = 0;

        [StateDefinition(typeof(InitializingState))]
        protected const int StateInitializing = 10;

        [StateDefinition(typeof(ReadyState))]
        protected const int StateReady = 20;

        [StateDefinition(typeof(StartingState))]
        protected const int StateStarting = 30;

        [StateDefinition(typeof(RunningState))]
        protected const int StateRunning = 40;

        [StateDefinition(typeof(ReadyStoppingState))]
        protected const int StateReadyStopping = 50;

        [StateDefinition(typeof(RunningStoppingState))]
        protected const int StateRunningStopping = 55;

        [StateDefinition(typeof(RunningFailureState))]
        protected const int StateRunningFailure = 60;

        [StateDefinition(typeof(InitializedFailureState))]
        protected const int StateInitializedFailure = 65;
    }
}
