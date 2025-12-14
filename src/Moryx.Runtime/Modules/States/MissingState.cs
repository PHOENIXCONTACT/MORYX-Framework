// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules.States
{
    /// <summary>
    /// Missing state , indicating that the server is missing.
    /// Useful for modules that are missing.
    /// </summary>
    internal class MissingState : ServerModuleStateBase
    {
        public override ServerModuleState Classification => ServerModuleState.Missing;

        public MissingState(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }

        public override Task Initialize(CancellationToken cancellationToken)
        {
            // Nothing to do here
            return Task.CompletedTask;
        }

        public override Task Start(CancellationToken cancellationToken)
        {
            // Nothing to do here
            return Task.CompletedTask;
        }

        public override Task Stop(CancellationToken cancellationToken)
        {
            // Nothing to do here
            return Task.CompletedTask;
        }
    }
}

