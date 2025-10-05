// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;

namespace Moryx.ControlSystem.Setups
{
    /// <summary>
    /// Additional interface for setup steps to declare setup classification
    /// </summary>
    public interface ISetupStep
    {
        /// <summary>
        /// Classification of the step
        /// </summary>
        SetupClassification Classification { get; }
    }
}