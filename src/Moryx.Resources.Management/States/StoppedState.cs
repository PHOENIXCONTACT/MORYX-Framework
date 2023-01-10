// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Resources.Management
{
    /// <summary>
    /// State of the <see cref="ResourceWrapper"/> when the wrappd resource was stopped
    /// </summary>
    internal class StoppedState : ResourceStateBase
    {
        /// <summary>
        /// constructor
        /// </summary>
        public StoppedState(ResourceWrapper context, StateMap stateMap) : base(context, stateMap)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            NextState(StateInitialized);
        }
    }
}
