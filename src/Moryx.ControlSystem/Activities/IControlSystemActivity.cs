// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
