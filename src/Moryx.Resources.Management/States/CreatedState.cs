// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Resources.Management
{
    /// <summary>
    /// State of a newly created <see cref="ResourceWrapper"/>
    /// </summary>
    internal class CreatedState : ResourceStateBase
    {
        /// <summary>
        /// constructor
        /// </summary>
        public CreatedState(ResourceWrapper context, StateMap stateMap) : base(context, stateMap)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            Context.HandleInitialize();
            NextState(StateInitialized);
        }
    }
}
