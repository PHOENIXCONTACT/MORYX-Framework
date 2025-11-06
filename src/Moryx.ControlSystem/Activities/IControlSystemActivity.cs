// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Activities;

namespace Moryx.ControlSystem.Activities
{
    /// <summary>
    /// Base interface for all control system activities
    /// </summary>
    public interface IControlSystemActivity : IActivity
    {
        /// <summary>
        /// The classification of this activity
        /// </summary>
        ActivityClassification Classification { get; }
    }
}
