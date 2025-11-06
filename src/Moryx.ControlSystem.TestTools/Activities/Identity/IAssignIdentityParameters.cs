// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.ControlSystem.Capabilities;

namespace Moryx.ControlSystem.TestTools.Activities
{
    /// <summary>
    /// Parameters used for identity assignment
    /// </summary>
    public interface IAssignIdentityParameters : IParameters
    {
        /// <summary>
        /// Identity type
        /// </summary>
        int Type { get; set; }

        /// <summary>
        /// Supported sources for identities
        /// </summary>
        IdentitySource Source { get; set; }

        /// <summary>
        /// Amount of identities to assign
        /// </summary>
        int Amount { get; set; }

        /// <summary>
        /// Target the Identity should be assigned to.
        /// </summary>
        object Target { get; }
    }
}
