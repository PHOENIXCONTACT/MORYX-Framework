// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules.States
{
    /// <summary>
    /// Missing state , indicating that the server is missing.
    /// Usefull for modules that are missing.
    /// </summary>
    internal class MissingState : ServerModuleStateBase
    {
        public override ServerModuleState Classification => ServerModuleState.Missing;

        public MissingState(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }

        public override void Initialize()
        {
            // Nothing to do here
        }

        public override void Start()
        {
            // Nothing to do here
        }

        public override void Stop()
        {
            // Nothing to do here
        }
    }
}

